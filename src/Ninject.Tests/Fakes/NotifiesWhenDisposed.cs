namespace Ninject.Tests.Fakes
{
    using System;
    using Ninject.Infrastructure.Disposal;

    public class NotifiesWhenDisposed : DisposableObject, INotifyWhenDisposed
    {
    }
}