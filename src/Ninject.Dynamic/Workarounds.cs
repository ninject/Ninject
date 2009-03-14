extern alias clr3;

using clr3::System;
using IronRuby.Builtins;
using Ninject.Activation;

namespace Ninject.Dynamic
{
    public static class Workarounds
    {
        public static Func<IRequest, bool> ToRequestPredicate(Proc proc)
        {
            return r => (bool)proc.Call(r);
        }

    }
}