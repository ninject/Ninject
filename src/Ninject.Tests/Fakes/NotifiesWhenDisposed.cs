using System;
using Ninject.Infrastructure.Disposal;

namespace Ninject.Tests.Fakes
{
    public class NotifiesWhenDisposed : DisposableObject, INotifyWhenDisposed
    {
    }
}