using System;
using System.Reflection;

namespace Ninject.Infrastructure.Language
{
	internal static class ExtensionsForTargetInvocationException
	{
		public static void RethrowInnerException(this TargetInvocationException exception)
		{
			Exception innerException = exception.InnerException;

			FieldInfo stackTraceField = typeof(Exception).GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic);
			stackTraceField.SetValue(innerException, innerException.StackTrace);

			throw innerException;
		}
	}
}