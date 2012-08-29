﻿#region License
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
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ninject.Activation;
using Ninject.Planning.Bindings;
#endregion

namespace Ninject.Infrastructure.Introspection
{
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
                    sw.WriteLine("{0,3}) {1}", current.Depth + 1, current);
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
        /// Formats the specified type into a meaningful string representation..
        /// </summary>
        /// <param name="type">The type to be formatted.</param>
        /// <returns>The type formatted as string.</returns>
        public static string Format(this Type type)
        {
            if (type.IsGenericType)
            {
                var sb = new StringBuilder();

                sb.Append(type.Name.Substring(0, type.Name.LastIndexOf('`')));
                sb.Append("{");

                var args = type.GetGenericArguments();

                foreach (Type genericArgument in args)
                {
                    sb.Append(genericArgument.Format());
                    sb.Append(", ");
                }

                sb.Remove(sb.Length - 2, 2);
                sb.Append("}");

                return sb.ToString();
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean: return "bool";
                case TypeCode.Char: return "char";
                case TypeCode.SByte: return "sbyte";
                case TypeCode.Byte: return "byte";
                case TypeCode.Int16: return "short";
                case TypeCode.UInt16: return "ushort";
                case TypeCode.Int32: return "int";
                case TypeCode.UInt32: return "uint";
                case TypeCode.Int64: return "long";
                case TypeCode.UInt64: return "ulong";
                case TypeCode.Single: return "float";
                case TypeCode.Double: return "double";
                case TypeCode.Decimal: return "decimal";
                case TypeCode.DateTime: return "DateTime";
                case TypeCode.String: return "string";
                default: return type.Name;
            }

        }
    }
}