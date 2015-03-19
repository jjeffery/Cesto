#region License

// Copyright 2004-2013 John Jeffery <john@jeffery.id.au>
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.IO;
using Cesto.Config.Storage;

namespace Cesto.Config
{
	public abstract class ConfigParameter : IConfigParameter
	{
		private string _summary;

		/// <summary>
		/// The name of the parameter.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// String representation of the parameter type (eg "String", "Int32", "Boolean").
		/// </summary>
		public string ParameterType { get; private set; }

		/// <summary>
		/// Brief description of the parameter.
		/// </summary>
		public string Description { get; protected set; }

		public string Summary
		{
			get
			{
				if (_summary == null)
				{
					_summary = GetSummary();
				}
				return _summary;
			}
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			var other = obj as ConfigParameter;
			if (other == null)
			{
				return false;
			}

			return Name == other.Name && ParameterType == other.ParameterType;
		}

		public override string ToString()
		{
			return ((IConfigParameter) this).GetValueText();
		}

		bool IConfigParameter.IsReadOnly
		{
			get { throw new NotImplementedException(); }
		}

		bool IConfigParameter.IsDerived
		{
			get { throw new NotImplementedException(); }
		}

		string IConfigParameter.ValidateText(string proposedValue)
		{
			throw new NotImplementedException();
		}

		void IConfigParameter.SetValueText(string value)
		{
			throw new NotImplementedException();
		}

		string IConfigParameter.GetValueText()
		{
			throw new NotImplementedException();
		}

	    string IConfigParameter.GetDisplayText()
	    {
	        throw new NotImplementedException();
	    }

		string IConfigParameter.GetDefaultValueText()
		{
			throw new NotImplementedException();
		}

		protected ConfigParameter(string paramName, string paramType)
		{
			Name = Verify.ArgumentNotNull(paramName, "paramName");
			ParameterType = Verify.ArgumentNotNull(paramType, "paramType");
			Description = string.Empty;
		}

		private string GetSummary()
		{
			if (Description.Contains("\n"))
			{
				// Multi-line description, use the first line.
				using (var reader = new StringReader(Description))
				{
					return (reader.ReadLine() ?? string.Empty).Trim();
				}
			}

			// Single line description is the same as the summary.
			return Description;
		}

		/// <summary>
		/// The persistant storage used for configuration values.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This value is initially set to a memory only storage. This is useful for unit testing.
		/// </para>
		/// <para>
		/// If the <see cref="ConfigFacility"/> is registered with the Castle Windsor container,
		/// then this storage will be automatically set to the component registered with the container.
		/// </para>
		/// </remarks>
		public static IConfigStorage Storage = new MemoryStorage();

		public static readonly ConfigParameterCollection All = new ConfigParameterCollection();
	}

	public interface IConfigParameterBuilder<T>
	{
		IConfigParameterBuilder<T> ChangeAction(Action callback);
		IConfigParameterBuilder<T> Description(string description);
		IConfigParameterBuilder<T> DefaultValue(T defaultValue);
		IConfigParameterBuilder<T> DefaultValue(Func<T> callback);
		IConfigParameterBuilder<T> DerivedValue(Func<T> callback);
		IConfigParameterBuilder<T> Validation(Func<T, string> callback);
	}

	public abstract class ConfigParameter<T, TParameter> : ConfigParameter, IConfigParameter<T>
		where TParameter : ConfigParameter<T, TParameter>
	{
		private T _defaultValue;
		private bool _defaultValueSet;
		private Func<T> _defaultValueCallback;
		private Func<T> _derivedValueCallback;
		private Func<T, string> _validationCallback;
		private Action _changedCallback;

		protected ConfigParameter(string paramName, string paramType) : base(paramName, paramType)
		{
		}

		public IConfigParameter<T> Extra
		{
			get { return this; }
		}

		public TParameter With(Action<IConfigParameterBuilder<T>> callback)
		{
			if (callback != null)
			{
				callback(new Builder(this));
			}
			return (TParameter) this;
		}

		private class Builder : IConfigParameterBuilder<T>
		{
			private readonly ConfigParameter<T, TParameter> _outer;

			public Builder(ConfigParameter<T, TParameter> outer)
			{
				_outer = outer;
			}

			public IConfigParameterBuilder<T> Description(string description)
			{
				_outer.Description = (description ?? string.Empty).Trim();
				return this;
			}

			public IConfigParameterBuilder<T> DefaultValue(T defaultValue)
			{
				_outer._defaultValue = defaultValue;
				_outer._defaultValueSet = true;
				return this;
			}

			public IConfigParameterBuilder<T> DefaultValue(Func<T> callback)
			{
				_outer._defaultValueCallback = callback;
				return this;
			}

			public IConfigParameterBuilder<T> DerivedValue(Func<T> callback)
			{
				_outer._derivedValueCallback = callback;
				return this;
			}

			public IConfigParameterBuilder<T> Validation(Func<T, string> callback)
			{
				_outer._validationCallback = callback;
				return this;
			}

			public IConfigParameterBuilder<T> ChangeAction(Action callback)
			{
				_outer._changedCallback = callback;
				return this;
			}
		}

		public static implicit operator T(ConfigParameter<T, TParameter> configParam)
		{
			return configParam.Value;
		}

		public T Value
		{
			get
			{
				if (_derivedValueCallback != null)
				{
					return _derivedValueCallback();
				}
				var configValue = Storage.GetValue(this);
				if (configValue.HasValue)
				{
					return (T) ConvertFromString(configValue.Value);
				}
				return GetDefaultValue();
			}
		}

		bool IConfigParameter.IsReadOnly
		{
			get { return Storage.IsReadOnly || _derivedValueCallback != null; }
		}

		bool IConfigParameter.IsDerived
		{
			get { return _derivedValueCallback != null; }
		}

		string IConfigParameter.ValidateText(string proposedValue)
		{
			T value;
			try
			{
				value = (T) ConvertFromString(proposedValue);
			}
			catch (Exception)
			{
				return string.Format("Not a valid {0} value.", ParameterType);
			}

			return DoValidate(value);
		}

		void IConfigParameter.SetValueText(string textValue)
		{
			DoSetValue((T) ConvertFromString(textValue));
		}

		string IConfigParameter.GetValueText()
		{
			return ConvertToString(Value);
		}

		string IConfigParameter.GetDefaultValueText()
		{
			return ConvertToString(GetDefaultValue());
		}

	    string IConfigParameter.GetDisplayText()
	    {
	        return ConvertToDisplayString(Value);
	    }

		string IConfigParameter<T>.Validate(T value)
		{
			return DoValidate(value);
		}

		private string DoValidate(T value)
		{
			string result = null;
			if (_validationCallback != null)
			{
				result = _validationCallback(value);
			}
			return result;
		}

		void IConfigParameter<T>.SetValue(T value)
		{
			DoSetValue(value);
		}

		private void DoSetValue(T value)
		{
			if (Extra.IsReadOnly)
			{
				throw new ReadOnlyConfigException("Attempt to write to read only config parameter") {
					ConfigParameter = this
				};
			}
			var newValue = ConvertToString(value);
			var oldValue = Storage.GetValue(this);
			if (oldValue.HasValue && oldValue.Value == newValue)
			{
				// no change, so do nothing
				return;
			}
			Storage.SetValue(this, ConvertToString(value));
			if (_changedCallback != null)
			{
				_changedCallback();
			}
		}

		T IConfigParameter<T>.DefaultValue
		{
			get { return GetDefaultValue(); }
		}

		private T GetDefaultValue()
		{
			if (!_defaultValueSet)
			{
				if (_defaultValueCallback != null)
				{
					_defaultValue = _defaultValueCallback();
					_defaultValueSet = true;
				}
			}
			return _defaultValue;
		}

		/// <summary>
		/// Convert a string value into the type required for this configuration parameter.
		/// </summary>
		/// <exception cref="FormatException">
		/// Text is the wrong format for the configuration parameter type.
		/// </exception>
		protected abstract object ConvertFromString(string text);

		/// <summary>
		/// Convert the configuration value into a string suitable for persisting.
		/// </summary>
		protected abstract string ConvertToString(object value);

	    protected virtual string ConvertToDisplayString(object value)
	    {
	        return ConvertToString(value);
	    }
	}
}