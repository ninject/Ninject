#region Usings

using System;
using System.Reflection;
using IronRuby.Builtins;
using Ninject.Components;

#endregion

namespace Ninject.Dynamic
{
    public interface IRubyEngine : INinjectComponent
    {
        /// <summary>
        /// Calls the method.
        /// </summary>
        /// <param name="receiver">The receiver.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        object CallMethod(object receiver, string message, params object[] args);

        /// <summary>
        /// Loads the assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        void LoadAssembly(Assembly assembly);

        /// <summary>
        /// Executes the script.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <returns></returns>
        object ExecuteScript(string script);

        /// <summary>
        /// Defines the read only global variable.
        /// </summary>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="value">The value.</param>
        void DefineReadOnlyGlobalVariable(string variableName, object value);

        /// <summary>
        /// Gets the ruby class.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <returns></returns>
        RubyClass GetRubyClass(string className);

        /// <summary>
        /// Gets the global variable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        T GetGlobalVariable<T>(string name);

        /// <summary>
        /// Loads the assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        void LoadAssemblies(params Type[] assemblies);
    }
}