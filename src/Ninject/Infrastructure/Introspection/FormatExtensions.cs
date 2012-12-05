#region License
// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2010, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 
#endregion
#region Using Directives
using System;
using System.IO;
using System.Reflection;
using System.Text;
using Ninject.Activation;
using Ninject.Planning.Bindings;
using Ninject.Planning.Targets;
#endregion

namespace Ninject.Infrastructure.Introspection
{
    using System.Globalization;

    /// <summary>
    /// Provides extension methods for string formatting
    /// </summary>
    public static class FormatExtensions
    {
        /// <summary>
        /// Formats the activation path into a meaningful string representation.
        /// </summary>
        /// <param name="request">The request to be formatted.</param>
        /// <returns>The activation path formatted as string.</returns>
        public static string FormatActivationPath(this IRequest request)
        {
            using (var sw = new StringWriter())
            {
                IRequest current = request;

                while (current != null)
                {
                    sw.WriteLine("{0,3}) {1}", current.Depth + 1, current.Format());
                    current = current.ParentRequest;
                }

                return sw.ToString();
            }
        }

        /// <summary>
        /// Formats the given binding into a meaningful string representation. 
        /// </summary>
        /// <param name="binding">The binding to be formatted.</param>
        /// <param name="context">The context.</param>
        /// <returns>The binding formatted as string</returns>
        public static string Format(this IBinding binding, IContext context)
        {
            using (var sw = new StringWriter())
            {
                if (binding.Condition != null)
                    sw.Write("conditional ");

                if (binding.IsImplicit)
                    sw.Write("implicit ");

                IProvider provider = binding.GetProvider(context);

                switch (binding.Target)
                {
                    case BindingTarget.Self:
                        sw.Write("self-binding of {0}", binding.Service.Format());
                        break;

                    case BindingTarget.Type:
                        sw.Write("binding from {0} to {1}", binding.Service.Format(), provider.Type.Format());
                        break;

                    case BindingTarget.Provider:
                        sw.Write("provider binding from {0} to {1} (via {2})", binding.Service.Format(),
                            provider.Type.Format(), provider.GetType().Format());
                        break;

                    case BindingTarget.Method:
                        sw.Write("binding from {0} to method", binding.Service.Format());
                        break;

                    case BindingTarget.Constant:
                        sw.Write("binding from {0} to constant value", binding.Service.Format());
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return sw.ToString();
            }
        }

        /// <summary>
        /// Formats the specified request into a meaningful string representation.
        /// </summary>
        /// <param name="request">The request to be formatted.</param>
        /// <returns>The request formatted as string.</returns>
        public static string Format(this IRequest request)
        {
            using (var sw = new StringWriter())
            {
                if (request.Target == null)
                    sw.Write("Request for {0}", request.Service.Format());
                else
                    sw.Write("Injection of dependency {0} into {1}", request.Service.Format(), request.Target.Format());

                return sw.ToString();
            }
        }

        /// <summary>
        /// Formats the specified target into a meaningful string representation..
        /// </summary>
        /// <param name="target">The target to be formatted.</param>
        /// <returns>The target formatted as string.</returns>
        public static string Format(this ITarget target)
        {
            using (var sw = new StringWriter())
            {
                switch (target.Member.MemberType)
                {
                    case MemberTypes.Constructor:
                        sw.Write("parameter {0} of constructor", target.Name);
                        break;

                    case MemberTypes.Method:
                        sw.Write("parameter {0} of method {1}", target.Name, target.Member.Name);
                        break;

                    case MemberTypes.Property:
                        sw.Write("property {0}", target.Name);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                sw.Write(" of type {0}", target.Member.ReflectedType.Format());

                return sw.ToString();
            }
        }

        /// <summary>
        /// Formats the specified type into a meaningful string representation..
        /// </summary>
        /// <param name="type">The type to be formatted.</param>
        /// <returns>The type formatted as string.</returns>
        public static string Format(this Type type)
        {
            var friendlyName = GetFriendlyName(type);

#if !MONO
            if (friendlyName.Contains("AnonymousType"))
                return "AnonymousType";
#else

            if (friendlyName.Contains("__AnonType"))
                return "AnonymousType";
#endif

            switch (friendlyName.ToLower(CultureInfo.InvariantCulture))
            {
                case "int16": return "short";
                case "int32": return "int";
                case "int64": return "long";
                case "string": return "string";
                case "object": return "object";
                case "boolean": return "bool";
                case "void": return "void";
                case "char": return "char";
                case "byte": return "byte";
                case "uint16": return "ushort";
                case "uint32": return "uint";
                case "uint64": return "ulong";
                case "sbyte": return "sbyte";
                case "single": return "float";
                case "double": return "double";
                case "decimal": return "decimal";
            }

            var genericArguments = type.GetGenericArguments();
            if(genericArguments.Length > 0)
                return FormatGenericType(friendlyName, genericArguments);
            
            return friendlyName;
        }

        private static string GetFriendlyName(Type type)
        {
            var friendlyName = type.FullName ?? type.Name;

            // remove generic arguments
            var firstBracket = friendlyName.IndexOf('[');
            if (firstBracket > 0)
                friendlyName = friendlyName.Substring(0, firstBracket);

            // remove assembly info
            var firstComma = friendlyName.IndexOf(',');
            if (firstComma > 0)
                friendlyName = friendlyName.Substring(0, firstComma);

            // remove namespace
            var lastPeriod = friendlyName.LastIndexOf('.');
            if (lastPeriod >= 0)
                friendlyName = friendlyName.Substring(lastPeriod + 1);

            return friendlyName;
        }

        private static string FormatGenericType(string friendlyName, Type[] genericArguments)
        {
            //var genericTag = "`" + genericArguments.Length;
            //var genericArgumentNames = new string[genericArguments.Length];
            //for (int i = 0; i < genericArguments.Length; i++)
            //    genericArgumentNames[i] = genericArguments[i].Format();

            //return friendlyName.Replace(genericTag, string.Join(", ", genericArgumentNames));

            var sb = new StringBuilder(friendlyName.Length + 10);

            var genericArgumentIndex = 0;
            var startIndex = 0;
            for (var index = 0; index < friendlyName.Length; index++)
            {
                if (friendlyName[index] == '`')
                {
                    var numArguments = friendlyName[index+1] - 48;
                    
                    sb.Append(friendlyName.Substring(startIndex, index - startIndex));
                    AppendGenericArguments(sb, genericArguments, genericArgumentIndex, numArguments);
                    genericArgumentIndex += numArguments;

                    startIndex = index + 2;
                }
            }
            if (startIndex < friendlyName.Length)
                sb.Append(friendlyName.Substring(startIndex));

            return sb.ToString();
        }

        private static void AppendGenericArguments(StringBuilder sb, Type[] genericArguments, int start, int count)
        {
            sb.Append("{");

            for(int i = 0; i < count; i++)
            {
                if (i != 0)
                    sb.Append(", ");

                sb.Append(genericArguments[start + i].Format());
            }
            
            sb.Append("}");
        }
    }
}