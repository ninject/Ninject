#region License
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2009, Enkari, Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
#region Using Directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Ninject.Activation;
using Ninject.Activation.Providers;
using Ninject.Infrastructure.Language;
using Ninject.Planning.Bindings;
using Ninject.Planning.Targets;
#endregion

namespace Ninject.Infrastructure.Introspection
{
	internal static class FormatExtensions
	{
		public static string FormatActivationPath(this IRequest request)
		{
			using (var sw = new StringWriter())
			{
				IRequest current = request;

				while (current != null)
				{
					sw.WriteLine("{0,3}) {1}", current.Depth + 1, current.Format());
					current = current.Parent;
				}

				return sw.ToString();
			}
		}

		public static string Format(this IBinding binding, IContext context)
		{
			using (var sw = new StringWriter())
			{
				if (binding.Condition != null)
					sw.Write("conditional ");

				if (binding.Metadata.IsImplicit)
					sw.Write("implicit ");

				IProvider provider = binding.GetProvider(context);

				if (binding.Service == provider.Type)
					sw.Write("self-binding of {0}", binding.Service.Format());
				else
					sw.Write("binding from {0} to {1}", binding.Service.Format(), provider.Type.Format());

				Type providerType = provider.GetType();

				if (providerType != typeof(StandardProvider))
					sw.Write(" (via {0})", providerType.Format());

				return sw.ToString();
			}
		}

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

		public static string Format(this Type type)
		{
			if (type.IsGenericType)
			{
				var sb = new StringBuilder();

				sb.Append(type.Name.Substring(0, type.Name.LastIndexOf('`')));
				sb.Append("[");

				foreach (Type genericArgument in type.GetGenericArguments())
				{
					sb.Append(genericArgument.Format());
					sb.Append(", ");
				}

				sb.Remove(sb.Length - 2, 2);
				sb.Append("]");

				return sb.ToString();
			}
			else
			{
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
}