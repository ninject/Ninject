using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Ninject.Infrastructure.Tracing
{
	public class TraceInfo
	{
		private readonly RuntimeTypeHandle _typeHandle;
		private readonly RuntimeMethodHandle _methodHandle;

		public Type Type
		{
			get { return Type.GetTypeFromHandle(_typeHandle); }
		}

		public MethodBase Method
		{
			get { return MethodBase.GetMethodFromHandle(_methodHandle); }
		}

		public string FileName { get; set; }
		public string FilePath { get; set; }
		public int LineNumber { get; set; }

		public bool HasFileInfo
		{
			get { return !String.IsNullOrEmpty(FileName); }
		}

		public TraceInfo(Type type, MethodBase method)
		{
			_typeHandle = type.TypeHandle;
			_methodHandle = method.MethodHandle;
		}

		public override string ToString()
		{
			string result = Type.Format() + "." + Method.Name + "()";

			if (HasFileInfo)
				result += " at " + FileName + ":" + LineNumber;

			return result;
		}

		public static TraceInfo FromStackTrace()
		{
			var trace = new StackTrace(true);

			foreach (StackFrame frame in trace.GetFrames())
			{
				MethodBase method = frame.GetMethod();
				Type type = method.DeclaringType;

				if (type.Namespace != null && !type.Namespace.StartsWith("Ninject"))
				{
					var info = new TraceInfo(type, method);

					string path = frame.GetFileName();

					if (path != null)
					{
						info.FilePath = path;
						info.FileName = Path.GetFileName(path);
					}

					info.LineNumber = frame.GetFileLineNumber();

					return info;
				}
			}

			return null;
		}
	}
}