#region Usings

using System;
using System.Collections;

#endregion

namespace Ninject.Dynamic.Extensions
{
    internal static class IDictionaryExtensions
    {
        public static void ForEach(this IDictionary dictionary, Action<object, object> iterator)
        {
            foreach (var key in dictionary.Keys)
            {
                iterator(key, dictionary[key]);
            }
        }
    }
}