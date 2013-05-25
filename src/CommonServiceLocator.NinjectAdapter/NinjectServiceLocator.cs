#region License
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
using Microsoft.Practices.ServiceLocation;
using Ninject;
#endregion

namespace CommonServiceLocator.NinjectAdapter
{
    public class NinjectServiceLocator : ServiceLocatorImplBase
    {
        public IKernel Kernel { get; private set; }

        public NinjectServiceLocator(IKernel kernel)
        {
            Kernel = kernel;
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            // key == null must be specifically handled as not asking for a specific keyed instance
            // http://commonservicelocator.codeplex.com/wikipage?title=API%20Reference&referringTitle=Home
            //     The implementation should be designed to expect a null for the string key parameter, 
            //     and MUST interpret this as a request to get the "default" instance for the requested 
            //     type. This meaning of default varies from locator to locator.
            if (key == null)
            {
                return Kernel.Get(serviceType);
            }
            return Kernel.Get(serviceType, key);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return Kernel.GetAll(serviceType);
        }
    }
}
