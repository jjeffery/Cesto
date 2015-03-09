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

namespace Cesto.Config
{
	/// <summary>
	/// Interface which provides all information required to persist a configuration
	/// parameter.
	/// </summary>
	/// <remarks>
	/// This interface is implemented explicitly by <see cref="ConfigParameter"/>. The
	/// reason for this is that the end user of <see cref="ConfigParameter"/> (and its
	/// derived types) does not typically have use for the methods in this interface,
	/// and explicit iterface implementation means that these methods do not show up
	/// in Visual Studio's auto-completion list.
	/// </remarks>
	public interface IConfigParameter
	{
		string Name { get; }
		string ParameterType { get; }
		string Description { get; }
		string Summary { get; }

		/// <summary>
		/// Is this configuration parameter read only. A parameter is read only
		/// if its underlying configuration storage mechanism is read only, or 
		/// it can also be read only if it is a derived value.
		/// </summary>
		bool IsReadOnly { get; }

		/// <summary>
		/// A derived configuration paramter never access the configuration storage,
		/// it derives its value from another source (often other configuration paramters).
		/// </summary>
		bool IsDerived { get; }

		/// <summary>
		/// Perform validation on a proposed string value.
		/// </summary>
		/// <returns>
		/// Returns <c>null</c> if value is valid for this parameter type, an error message otherwise.
		/// </returns>
		string ValidateText(string proposedValue);

		/// <summary>
		/// Update the value of the parameter.
		/// </summary>
		/// <param name="value">
		/// New value. This should be a valid text value for this type of parameter, otherwise an exception will be thrown.
		/// Call <see cref="ValidateText"/> before calling this function, as this function does not perform any validation.
		/// </param>
		void SetValueText(string value);

		/// <summary>
		/// Gets the value of this parameter as a text string.
		/// </summary>
		string GetValueText();

		/// <summary>
		/// Gets the default value of this parameter as a text string.
		/// </summary>
		string GetDefaultValueText();

        /// <summary>
        /// Gets the value of this parameter as a text string suitable for display
        /// in a list. Most parameter types return the same value as <see cref="GetValueText"/>,
        /// but passwords, for example, return elided text.
        /// </summary>
	    string GetDisplayText();
	}

	/// <summary>
	/// Generic typed configuration parameter interface.
	/// </summary>
	/// <typeparam name="T">Parameter type</typeparam>
	/// <remarks>
	/// This interface is implemented explicitly by <see cref="ConfigParameter{T, TParameter}"/>, and 
	/// all of its derived types. The reason for implementing explicitly is that the calling program will
	/// usually be accessing config parameters in a read-only fashion, and it would clutter the auto-completion
	/// to have methods for default value and changing the value. When a program does want extra control, it
	/// just has to explicitly cast to <see cref="IConfigParameter{T}"/>. Because this is not entirely obvious,
	/// the <see cref="ConfigParameter{T, TParameter}"/> class has a property which returns itself cast to
	/// this interface.
	/// </remarks>
	public interface IConfigParameter<T> : IConfigParameter
	{
		/// <summary>
		/// The current value for this parameter.
		/// </summary>
		T Value { get; }

		/// <summary>
		/// The default value for this parameter.
		/// </summary>
		T DefaultValue { get; }

		/// <summary>
		/// Saves the new value. No validation is performed, it is assumed that the
		/// caller has called <see cref="Validate"/> prior to calling this method.
		/// </summary>
		/// <param name="value"></param>
		void SetValue(T value);

		/// <summary>
		/// Check if the proposed value is valid for this configuration parameter.
		/// </summary>
		/// <param name="value">Proposed value</param>
		/// <returns>Returns <c>null</c> if the value is valid, otherwise returns an error message.</returns>
		string Validate(T value);
	}
}