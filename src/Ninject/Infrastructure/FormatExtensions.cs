using System;
using System.Text;

namespace Ninject.Infrastructure
{
	public static class FormatExtensions
	{
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
					sb.Append(",");
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