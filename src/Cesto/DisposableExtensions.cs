#region License

// Copyright 2004-2015 John Jeffery <john@jeffery.id.au>
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

namespace Cesto
{
	/// <summary>
	///     Extension methods for <see cref="IDisposable" /> objects.
	/// </summary>
	public static class DisposableExtensions
	{
	    /// <summary>
	    /// Add the <see cref="IDisposable"/> object to the <see cref="DisposableCollection"/>.
	    /// </summary>
	    /// <param name="disposable">The <see cref="IDisposable"/> to add to the collection.</param>
	    /// <param name="collection">The <see cref="DisposableCollection"/> to add the disposable to. Must not be null.</param>
	    /// <returns>
	    /// Returns <paramref name="disposable"/>.
	    /// </returns>
	    public static T AddTo<T>(this T disposable, DisposableCollection collection) where T : IDisposable
	    {
            Verify.ArgumentNotNull(collection, "collection");
            if (disposable != null)
	        {
                collection.Add(disposable);
	        }
	        return disposable;
	    }
	}
}