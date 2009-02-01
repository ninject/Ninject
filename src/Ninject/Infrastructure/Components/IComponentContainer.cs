using System;
using System.Collections.Generic;

namespace Ninject.Infrastructure.Components
{
	public interface IComponentContainer : IDisposable
	{
		INinjectSettings Settings { get; set; }

		void Add<TService, TImplementation>()
			where TService : INinjectComponent
			where TImplementation : TService, INinjectComponent;

		void RemoveAll<T>() where T : INinjectComponent;

		T Get<T>() where T : INinjectComponent;
		IEnumerable<T> GetAll<T>() where T : INinjectComponent;

		object Get(Type service);
		IEnumerable<object> GetAll(Type service);
	}
}