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
using System.Diagnostics;
using System.IO;
using System.Reflection;
#endregion

namespace Ninject.Infrastructure.Tracing
{
	/// <summary>
	/// Contains information indicating where an object was created in code.
	/// </summary>
	public class TraceInfo
	{
		private readonly RuntimeTypeHandle _typeHandle;
		private readonly RuntimeMethodHandle _methodHandle;

		/// <summary>
		/// Gets the type that created the object.
		/// </summary>
		public Type Type
		{
			get { return Type.GetTypeFromHandle(_typeHandle); }
		}

		/// <summary>
		/// Gets the method or constructor that created the object.
		/// </summary>
		public MethodBase Method
		{
			get { return MethodBase.GetMethodFromHandle(_methodHandle); }
		}

		/// <summary>
		/// Gets the name of the file which contains the code that created the object.
		/// </summary>
		public string FileName { get; private set; }

		/// <summary>
		/// Gets the path of the file which contains the code that created the object.
		/// </summary>
		public string FilePath { get; private set; }

		/// <summary>
		/// Gets the line number of the file which contains the code that created the object.
		/// </summary>
		public int LineNumber { get; private set; }

		/// <summary>
		/// Gets a value indicating whether file information is available.
		/// </summary>
		public bool HasFileInfo
		{
			get { return !String.IsNullOrEmpty(FileName); }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TraceInfo"/> class.
		/// </summary>
		/// <param name="type">The type that created the object.</param>
		/// <param name="method">The method or constructor that created the object.</param>
		public TraceInfo(Type type, MethodBase method)
		{
			_typeHandle = type.TypeHandle;
			_methodHandle = method.MethodHandle;
		}

		/// <summary>
		/// Converts the object into its string representation.
		/// </summary>
		/// <returns>The string representation of the object.</returns>
		public override string ToString()
		{
			string result = Type + "." + Method.Name + "()";

			if (HasFileInfo)
				result += " at " + FileName + ":" + LineNumber;

			return result;
		}

		/// <summary>
		/// Creates a <see cref="TraceInfo"/> from the current stack trace.
		/// </summary>
		/// <returns>The created <see cref="TraceInfo"/> object.</returns>
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