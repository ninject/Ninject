#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using IronRuby;
using IronRuby.Builtins;
using IronRuby.Runtime;
using Microsoft.Scripting.Hosting;
using Ninject.Components;
using Ninject.Dynamic.Extensions;

#endregion

namespace Ninject.Dynamic
{
    /// <summary>
    /// A wrapper for ScriptEngine, Runtime and Context
    /// This class handles all the interaction with IronRuby
    /// </summary>
    public class RubyEngine : IRubyEngine
    {

        private static ScriptRuntime _scriptRuntime;
        private readonly bool _shouldShutdown;

        /// <summary>
        /// Initializes a new instance of the <see cref="RubyEngine"/> class.
        /// </summary>
        public RubyEngine()
        {
            if (_scriptRuntime.IsNull())
            {
                _scriptRuntime = InitializeIronRuby();
                _shouldShutdown = true;
            }

            Initialize();
        }

        /// <summary>
        /// Gets the runtime.
        /// </summary>
        /// <value>The runtime.</value>
        internal static ScriptRuntime Runtime { get { return _scriptRuntime;  } }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        internal RubyContext Context { get; private set; }

        /// <summary>
        /// Gets the engine.
        /// </summary>
        /// <value>The engine.</value>
        internal ScriptEngine Engine { get; private set; }

        /// <summary>
        /// Gets the current scope.
        /// </summary>
        /// <value>The current scope.</value>
        internal ScriptScope CurrentScope { get; private set; }

        /// <summary>
        /// Gets the operations.
        /// </summary>
        /// <value>The operations.</value>
        internal ObjectOperations Operations { get; private set; }

        #region IRubyEngine Members

        /// <summary>
        /// Calls the method.
        /// </summary>
        /// <param name="receiver">The receiver.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public object CallMethod(object receiver, string message, params object[] args)
        {
            return Operations.InvokeMember(receiver, GetMethodName(receiver, message), args);
        }

        /// <summary>
        /// Loads the assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public void LoadAssembly(Assembly assembly)
        {
            Runtime.LoadAssembly(assembly);
        }

        /// <summary>
        /// Executes the script.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <returns></returns>
        public object ExecuteScript(string script)
        {
            return Engine.Execute(script, CurrentScope);
        }

        /// <summary>
        /// Defines the read only global variable.
        /// </summary>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="value">The value.</param>
        public void DefineReadOnlyGlobalVariable(string variableName, object value)
        {
            Context.DefineReadOnlyGlobalVariable(variableName, value);
        }

        /// <summary>
        /// Gets the ruby class.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <returns></returns>
        public RubyClass GetRubyClass(string className)
        {
            var klass = GetGlobalVariable<RubyClass>(className);
            return klass;
        }

        /// <summary>
        /// Gets the global variable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public T GetGlobalVariable<T>(string name)
        {
            return Runtime.Globals.GetVariable<T>(name);
        }

        /// <summary>
        /// Loads the assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        public void LoadAssemblies(params Type[] assemblies)
        {
            assemblies.ForEach(type => LoadAssembly(type.Assembly));
        }

        #endregion

        private string GetMethodName(object receiver, string message)
        {
            var methodNames = Operations.GetMemberNames(receiver);

            if (methodNames.Contains(message.Pascalize())) return message.Pascalize();
            if (methodNames.Contains(message.Underscore())) return message.Underscore();

            // really? we got here.. that must be some pretty funky naming.
            return message;
        }

        private void Initialize()
        {
            Engine = Ruby.GetEngine(Runtime);
            Context = Ruby.GetExecutionContext(Engine);
            CurrentScope = Engine.CreateScope();
            Operations = Engine.CreateOperations();
            LoadAssemblies(typeof (object), typeof (Uri), typeof (StandardKernel), typeof (IRubyEngine));
            RequireRubyFile("Ninject.Dynamic.initializer.rb", ReaderType.Assembly);
        }


        /// <summary>
        /// Requires the ruby file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="readerType">Type of the reader.</param>
        internal object RequireRubyFile(string path, ReaderType readerType)
        {
            if (readerType == ReaderType.Assembly)
                return Engine.CreateScriptSource(new AssemblyStreamContentProvider(path, typeof (IRubyEngine).Assembly), null, Encoding.UTF8).Execute();
            
            return Engine.CreateScriptSourceFromFile(path, Encoding.UTF8).Execute();
        }

        internal object ExecuteFile(string path)
        {
            return Engine.CreateScriptSourceFromFile(path, Encoding.UTF8).Execute(CurrentScope);
        }

        internal T ExecuteFile<T>(string path)
        {
            return (T) ExecuteFile(path);
        }


        private static ScriptRuntime InitializeIronRuby()
        {
            var rubySetup = Ruby.CreateRubySetup();

            var runtimeSetup = new ScriptRuntimeSetup();
            runtimeSetup.LanguageSetups.Add(rubySetup);
            runtimeSetup.DebugMode = true;

            return Ruby.CreateRuntime(runtimeSetup);
        }

        #region Implementation of IDisposable

        /// <summary>
        ///  Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if(_shouldShutdown) Runtime.Shutdown();
        }

        #endregion

        #region Implementation of INinjectComponent

        /// <summary>
        /// Gets or sets the settings that are being used.
        /// </summary>
        public INinjectSettings Settings { get; set; }

        #endregion
    }
}