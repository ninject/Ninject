using System;
using System.Collections.Generic;
using Ninject.Activation;
using Ninject.Parameters;
using Ninject.Planning.Targets;
using Ninject.Resolution;

namespace Ninject.Infrastructure
{
	public class ResolutionScope : IResolutionScope
	{
		public IKernel Kernel { get; private set; }

		public ResolutionScope(IKernel kernel)
		{
			Kernel = kernel;
		}

		public void Dispose()
		{
			Disposed(this, EventArgs.Empty);
			Disposed = null;
			GC.SuppressFinalize(this);
		}

		public event EventHandler Disposed = delegate { };

		public IEnumerable<IContext> Resolve(Type service, IEnumerable<IConstraint> constraints, IEnumerable<IParameter> parameters)
		{
			return Resolve(CreateDirectRequest(service, constraints, parameters));
		}

		public IEnumerable<IContext> Resolve(IRequest request)
		{
			return Kernel.Resolve(request);
		}

		protected virtual IRequest CreateDirectRequest(Type service, IEnumerable<IConstraint> constraints, IEnumerable<IParameter> parameters)
		{
			return new Request(service, constraints, parameters, () => this);
		}
	}
}