using System;
using System.Reflection;

namespace Ninject.Messaging
{
	public interface IPublication : IDisposable
	{
		object Publisher { get; }
		EventInfo Event { get; }
	}
}