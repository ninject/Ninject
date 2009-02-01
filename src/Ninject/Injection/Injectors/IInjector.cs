using System;
using System.Reflection;

namespace Ninject.Injection.Injectors
{
	public interface IInjector<T>
		where T : MemberInfo
	{
		T Member { get; }
	}
}