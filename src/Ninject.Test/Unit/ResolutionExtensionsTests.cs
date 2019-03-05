using Moq;
using Ninject.Activation;
using Ninject.Parameters;
using Ninject.Planning.Bindings;
using Ninject.Syntax;
using Ninject.Tests.Fakes;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Ninject.Test.Unit
{
    public class ResolutionExtensionsTests
    {
        private Mock<IResolutionRoot> _rootMock;
        private Mock<IRequest> _requestMock;
        private MockSequence _sequence;

        public ResolutionExtensionsTests()
        {
            _rootMock = new Mock<IResolutionRoot>(MockBehavior.Strict);
            _requestMock = new Mock<IRequest>(MockBehavior.Strict);
            _sequence = new MockSequence();
        }

        [Fact]
        public void CanResolve_RootAndParameters_ShouldThrowArgumentNullExceptionWhenRootIsNull()
        {
            const IResolutionRoot root = null;
            var parameters = new IParameter[0];

            var actual = Assert.Throws<ArgumentNullException>(
                () => ResolutionExtensions.CanResolve<string>(root, parameters));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(root), actual.ParamName);
        }

        [Fact]
        public void CanResolve_RootAndParameters_ParametersIsNull()
        {
            var root = _rootMock.Object;
            IParameter[] parameters = null;

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(string), null, parameters, false, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CanResolve(_requestMock.Object))
                     .Returns(true);

            Assert.True(ResolutionExtensions.CanResolve<string>(root, parameters));

            _rootMock.Reset();
            _requestMock.Reset();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(string), null, parameters, false, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CanResolve(_requestMock.Object))
                     .Returns(false);

            Assert.False(ResolutionExtensions.CanResolve<string>(root, parameters));
        }

        [Fact]
        public void CanResolve_RootAndNameAndParameters_ShouldThrowArgumentNullExceptionWhenRootIsNull()
        {
            const IResolutionRoot root = null;
            var name = "NAME";
            var parameters = new IParameter[0];

            var actual = Assert.Throws<ArgumentNullException>(
                () => ResolutionExtensions.CanResolve<string>(root, name, parameters));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(root), actual.ParamName);
        }

        [Fact]
        public void CanResolve_RootAndNameAndParameters_ParametersIsNull()
        {
            var root = _rootMock.Object;
            var name = "NAME";
            IParameter[] parameters = null;

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(string), It.IsNotNull<Func<IBindingMetadata, bool>>(), parameters, false, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CanResolve(_requestMock.Object))
                     .Returns(true);

            Assert.True(ResolutionExtensions.CanResolve<string>(root, name, parameters));

            _rootMock.Reset();
            _requestMock.Reset();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(string), It.IsNotNull<Func<IBindingMetadata, bool>>(), parameters, false, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CanResolve(_requestMock.Object))
                     .Returns(false);

            Assert.False(ResolutionExtensions.CanResolve<string>(root, name, parameters));
        }

        [Fact]
        public void CanResolve_RootAndConstraintAndParameters_ShouldThrowArgumentNullExceptionWhenRootIsNull()
        {
            const IResolutionRoot root = null;
            Func<IBindingMetadata, bool> constraint = (_) => true;
            var parameters = new IParameter[0];

            var actual = Assert.Throws<ArgumentNullException>(
                () => ResolutionExtensions.CanResolve<string>(root, constraint, parameters));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(root), actual.ParamName);

        }

        [Fact]
        public void CanResolve_RootAndConstraintAndParameters_ParametersIsNull()
        {
            Func<IBindingMetadata, bool> constraint = (_) => true;
            IParameter[] parameters = null;

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(string), constraint, parameters, false, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CanResolve(_requestMock.Object))
                     .Returns(true);

            Assert.True(ResolutionExtensions.CanResolve<string>(_rootMock.Object, constraint, parameters));

            _rootMock.Reset();
            _requestMock.Reset();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(string), constraint, parameters, false, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CanResolve(_requestMock.Object))
                     .Returns(false);

            Assert.False(ResolutionExtensions.CanResolve<string>(_rootMock.Object, constraint, parameters));
        }

        [Fact]
        public void CanResolve_RootAndServiceAndParameters_ShouldThrowArgumentNullExceptionWhenRootIsNull()
        {
            const IResolutionRoot root = null;
            var service = typeof(string);
            var parameters = new IParameter[0];

            var actual = Assert.Throws<ArgumentNullException>(
                () => ResolutionExtensions.CanResolve(root, service, parameters));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(root), actual.ParamName);
        }

        [Fact]
        public void CanResolve_RootAndServiceAndParameters_ServiceIsNull()
        {
            var root = _rootMock.Object;
            const Type service = null;
            var parameters = new IParameter[0];

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(service, (Func<IBindingMetadata, bool>) null, parameters, false, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CanResolve(_requestMock.Object))
                     .Returns(true);

            Assert.True(ResolutionExtensions.CanResolve(root, service, parameters));

            _rootMock.Reset();
            _requestMock.Reset();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(service, (Func<IBindingMetadata, bool>) null, parameters, false, true)).Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CanResolve(_requestMock.Object))
                     .Returns(false);

            Assert.False(ResolutionExtensions.CanResolve(root, service, parameters));
        }

        [Fact]
        public void CanResolve_RootAndServiceAndParameters_ParametersIsNull()
        {
            var root = _rootMock.Object;
            var service = typeof(string);
            IParameter[] parameters = null;

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(service, (Func<IBindingMetadata, bool>) null, parameters, false, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CanResolve(_requestMock.Object))
                     .Returns(true);

            Assert.True(ResolutionExtensions.CanResolve(root, service, parameters));

            _rootMock.Reset();
            _requestMock.Reset();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(service, (Func<IBindingMetadata, bool>) null, parameters, false, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CanResolve(_requestMock.Object))
                     .Returns(false);

            Assert.False(ResolutionExtensions.CanResolve(root, service, parameters));
        }

        [Fact]
        public void CanResolve_RootAndServiceAndNameAndParameters_ShouldThrowArgumentNullExceptionWhenRootIsNull()
        {
            const IResolutionRoot root = null;
            var service = typeof(string);
            var name = "NAME";
            var parameters = new IParameter[0];

            var actual = Assert.Throws<ArgumentNullException>(
                () => ResolutionExtensions.CanResolve(root, service, name, parameters));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(root), actual.ParamName);
        }

        [Fact]
        public void CanResolve_RootAndServiceAndNameAndParameters_ServiceIsNull()
        {
            var root = _rootMock.Object;
            const Type service = null;
            var name = "NAME";
            var parameters = new IParameter[0];

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(service, It.IsNotNull<Func<IBindingMetadata, bool>>(), parameters, false, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CanResolve(_requestMock.Object))
                     .Returns(true);

            Assert.True(ResolutionExtensions.CanResolve(root, service, name, parameters));

            _rootMock.Reset();
            _requestMock.Reset();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(service, It.IsNotNull<Func<IBindingMetadata, bool>>(), parameters, false, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CanResolve(_requestMock.Object))
                     .Returns(false);

            Assert.False(ResolutionExtensions.CanResolve(root, service, name, parameters));
        }

        [Fact]
        public void CanResolve_RootAndServiceAndNameAndParameters_ParametersIsNull()
        {
            var root = _rootMock.Object;
            var service = typeof(string);
            var name = "NAME";
            IParameter[] parameters = null;

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(service, It.IsNotNull<Func<IBindingMetadata, bool>>(), parameters, false, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CanResolve(_requestMock.Object))
                     .Returns(true);

            Assert.True(ResolutionExtensions.CanResolve(root, service, name, parameters));

            _rootMock.Reset();
            _requestMock.Reset();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(service, It.IsNotNull<Func<IBindingMetadata, bool>>(), parameters, false, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CanResolve(_requestMock.Object))
                     .Returns(false);

            Assert.False(ResolutionExtensions.CanResolve(root, service, name, parameters));
        }

        [Fact]
        public void CanResolve_RootAndServiceAndConstraintAndParameters_ShouldThrowArgumentNullExceptionWhenRootIsNull()
        {
            const IResolutionRoot root = null;
            var service = typeof(string);
            Func<IBindingMetadata, bool> constraint = (_) => true;
            var parameters = new IParameter[0];

            var actual = Assert.Throws<ArgumentNullException>(
                () => ResolutionExtensions.CanResolve(root, service, constraint, parameters));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(root), actual.ParamName);
        }

        [Fact]
        public void CanResolve_RootAndServiceAndConstraintAndParameters_ServiceIsNull()
        {
            var root = _rootMock.Object;
            const Type service = null;
            Func<IBindingMetadata, bool> constraint = (_) => true;
            var parameters = new IParameter[0];

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(service, constraint, parameters, false, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CanResolve(_requestMock.Object))
                     .Returns(true);

            Assert.True(ResolutionExtensions.CanResolve(root, service, constraint, parameters));

            _rootMock.Reset();
            _requestMock.Reset();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(service, constraint, parameters, false, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CanResolve(_requestMock.Object))
                     .Returns(false);

            Assert.False(ResolutionExtensions.CanResolve(root, service, constraint, parameters));
        }

        [Fact]
        public void CanResolve_RootAndServiceAndConstraintAndParameters_ParametersIsNull()
        {
            var root = _rootMock.Object;
            var service = typeof(string);
            Func<IBindingMetadata, bool> constraint = (_) => true;
            IParameter[] parameters = null;

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(string), constraint, parameters, false, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CanResolve(_requestMock.Object))
                     .Returns(true);

            Assert.True(ResolutionExtensions.CanResolve(root, service, constraint, parameters));

            _rootMock.Reset();
            _requestMock.Reset();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(string), constraint, parameters, false, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CanResolve(_requestMock.Object))
                     .Returns(false);

            Assert.False(ResolutionExtensions.CanResolve(root, service, constraint, parameters));
        }

        [Fact]
        public void Get_RootAndParameters_ShouldThrowArgumentNullExceptionWhenRootIsNull()
        {
            const IResolutionRoot root = null;
            var parameters = new IParameter[0];

            var actual = Assert.Throws<ArgumentNullException>(
                () => ResolutionExtensions.Get<string>(root, parameters));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(root), actual.ParamName);
        }

        [Fact]
        public void Get_RootAndParameters_ResolveReturnsNull()
        {
            var root = _rootMock.Object;
            var parameters = new IParameter[0];

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), (Func<IBindingMetadata, bool>)null, parameters, false, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Returns(null);

            var actual = ResolutionExtensions.Get<IWeapon>(root, parameters);

            Assert.Null(actual);
        }

        [Fact]
        public void Get_RootAndParameters_ResolvesToInstanceOfService()
        {
            var root = _rootMock.Object;
            var parameters = new IParameter[0];
            var resolvedInstance = new Dagger();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), (Func<IBindingMetadata, bool>) null, parameters, false, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Returns(resolvedInstance);

            var actual = ResolutionExtensions.Get<IWeapon>(root, parameters);

            Assert.NotNull(actual);
            Assert.Same(resolvedInstance, actual);
        }

        [Fact]
        public void Get_RootAndParameters_ResolvesToInstanceNotOfService()
        {
            var root = _rootMock.Object;
            var parameters = new IParameter[0];
            var resolvedInstance = 5L;

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWarrior), (Func<IBindingMetadata, bool>)null, parameters, false, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Returns(resolvedInstance);

            var actual = Assert.Throws<InvalidCastException>(() => ResolutionExtensions.Get<IWarrior>(root, parameters));

            Assert.Null(actual.InnerException);
            Assert.Equal($"Unable to cast object of type '{resolvedInstance.GetType().FullName}' to type '{typeof(IWarrior).FullName}'.", actual.Message);
        }

        [Fact]
        public void Get_RootAndParameters_ResolveThrowsActivationException()
        {
            var root = _rootMock.Object;
            var parameters = new IParameter[0];
            var activationException = new ActivationException();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), (Func<IBindingMetadata, bool>) null, parameters, false, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Throws(activationException);

            var actual = Assert.Throws<ActivationException>(() => ResolutionExtensions.Get<IWeapon>(root, parameters));

            Assert.Same(activationException, actual);
        }

        [Fact]
        public void Get_RootAndConstraintAndParameters_ShouldThrowArgumentNullExceptionWhenRootIsNull()
        {
            const IResolutionRoot root = null;
            Func<IBindingMetadata, bool> constraint = (_) => true;
            var parameters = new IParameter[0];

            var actual = Assert.Throws<ArgumentNullException>(
                () => ResolutionExtensions.Get<string>(root, constraint, parameters));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(root), actual.ParamName);
        }

        [Fact]
        public void Get_RootAndConstraintAndParameters_ResolveReturnsNull()
        {
            var root = _rootMock.Object;
            Func<IBindingMetadata, bool> constraint = (_) => true;
            var parameters = new IParameter[0];
            var resolvedInstances = Enumerable.Empty<object>();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), constraint, parameters, false, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Returns(null);

            var actual = ResolutionExtensions.Get<IWeapon>(root, constraint, parameters);

            Assert.Null(actual);
        }

        [Fact]
        public void Get_RootAndConstraintAndParameters_ResolvesToInstanceOfService()
        {
            var root = _rootMock.Object;
            Func<IBindingMetadata, bool> constraint = (_) => true;
            var parameters = new IParameter[0];
            var resolvedInstance = new Dagger();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), constraint, parameters, false, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Returns(resolvedInstance);

            var actual = ResolutionExtensions.Get<IWeapon>(root, constraint, parameters);

            Assert.NotNull(actual);
            Assert.Same(resolvedInstance, actual);
        }

        [Fact]
        public void Get_RootAndConstraintAndParameters_ResolvesToInstanceNotOfService()
        {
            var root = _rootMock.Object;
            Func<IBindingMetadata, bool> constraint = (_) => true;
            var parameters = new IParameter[0];
            var resolvedInstance = 5L;

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWarrior), constraint, parameters, false, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Returns(resolvedInstance);

            var actual = Assert.Throws<InvalidCastException>(() => ResolutionExtensions.Get<IWarrior>(root, constraint, parameters));

            Assert.Null(actual.InnerException);
            Assert.Equal($"Unable to cast object of type '{resolvedInstance.GetType().FullName}' to type '{typeof(IWarrior).FullName}'.", actual.Message);
        }

        [Fact]
        public void Get_RootAndConstraintAndParameters_ResolveThrowsActivationException()
        {
            var root = _rootMock.Object;
            Func<IBindingMetadata, bool> constraint = (_) => true;
            var parameters = new IParameter[0];
            var activationException = new ActivationException();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), constraint, parameters, false, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Throws(activationException);

            var actual = Assert.Throws<ActivationException>(() => ResolutionExtensions.Get<IWeapon>(root, constraint, parameters));

            Assert.Same(activationException, actual);
        }

        [Fact]
        public void Get_RootAndNameAndParameters_ShouldThrowArgumentNullExceptionWhenRootIsNull()
        {
            const IResolutionRoot root = null;
            var name = "NAME";
            var parameters = new IParameter[0];

            var actual = Assert.Throws<ArgumentNullException>(
                () => ResolutionExtensions.Get<string>(root, name, parameters));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(root), actual.ParamName);
        }

        [Fact]
        public void Get_RootAndNameAndParameter_ResolveReturnsNull()
        {
            var root = _rootMock.Object;
            var name = "NAME";
            var parameters = new IParameter[0];

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), It.IsNotNull<Func<IBindingMetadata, bool>>(), parameters, false, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Returns(null);

            var actual = ResolutionExtensions.Get<IWeapon>(root, name, parameters);

            Assert.Null(actual);
        }

        [Fact]
        public void Get_RootAndNameAndParameters_ResolvesToInstanceOfService()
        {
            var root = _rootMock.Object;
            var name = "NAME";
            var parameters = new IParameter[0];
            var resolvedInstance = new Dagger();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), It.IsNotNull<Func<IBindingMetadata, bool>>(), parameters, false, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Returns(resolvedInstance);

            var actual = ResolutionExtensions.Get<IWeapon>(root, name, parameters);

            Assert.NotNull(actual);
            Assert.Same(resolvedInstance, actual);
        }

        [Fact]
        public void Get_RootAndNameAndParameters_ResolvesToInstanceNotOfService()
        {
            var root = _rootMock.Object;
            var name = "NAME";
            var parameters = new IParameter[0];
            var resolvedInstance = 5L;

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(string), It.IsNotNull<Func<IBindingMetadata, bool>>(), parameters, false, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Returns(resolvedInstance);

            var actual = Assert.Throws<InvalidCastException>(() => ResolutionExtensions.Get<string>(root, name, parameters));

            Assert.Null(actual.InnerException);
            Assert.Equal($"Unable to cast object of type '{resolvedInstance.GetType().FullName}' to type '{typeof(string).FullName}'.", actual.Message);
        }

        [Fact]
        public void Get_RootAndNameAndParameters_ResolveThrowsActivationException()
        {
            var root = _rootMock.Object;
            var name = "NAME";
            var parameters = new IParameter[0];
            var activationException = new ActivationException();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), It.IsNotNull<Func<IBindingMetadata, bool>>(), parameters, false, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Throws(activationException);

            var actual = Assert.Throws<ActivationException>(() => ResolutionExtensions.Get<IWeapon>(root, name, parameters));

            Assert.Same(activationException, actual);
        }

        [Fact]
        public void TryGet_RootAndParameters_ShouldThrowArgumentNullExceptionWhenRootIsNull()
        {
            const IResolutionRoot root = null;
            var parameters = new IParameter[0];

            var actual = Assert.Throws<ArgumentNullException>(
                () => ResolutionExtensions.TryGet<string>(root, parameters));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(root), actual.ParamName);
        }

        [Fact]
        public void TryGet_RootAndParameters_ResolvesToNull()
        {
            var root = _rootMock.Object;
            var parameters = new IParameter[0];

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), (Func<IBindingMetadata, bool>) null, parameters, true, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Returns(null);

            var actual = ResolutionExtensions.TryGet<IWeapon>(root, parameters);

            Assert.Null(actual);
        }

        [Fact]
        public void TryGet_RootAndParameters_ResolvesToInstanceOfService()
        {
            var root = _rootMock.Object;
            var parameters = new IParameter[0];
            var resolvedInstance = new Dagger();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), (Func<IBindingMetadata, bool>) null, parameters, true, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Returns(resolvedInstance);

            var actual = ResolutionExtensions.TryGet<IWeapon>(root, parameters);

            Assert.NotNull(actual);
            Assert.Same(resolvedInstance, actual);
        }

        [Fact]
        public void TryGet_RootAndParameters_ResolvesToInstanceNotOfService()
        {
            var root = _rootMock.Object;
            var parameters = new IParameter[0];
            var resolvedInstance = 5L;

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWarrior), (Func<IBindingMetadata, bool>) null, parameters, true, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Returns(resolvedInstance);

            var actual = Assert.Throws<InvalidCastException>(() => ResolutionExtensions.TryGet<IWarrior>(root, parameters));

            Assert.Null(actual.InnerException);
            Assert.Equal($"Unable to cast object of type '{resolvedInstance.GetType().FullName}' to type '{typeof(IWarrior).FullName}'.", actual.Message);
        }

        [Fact]
        public void TryGet_RootAndParameters_ResolveThrowsActivationException()
        {
            var root = _rootMock.Object;
            var parameters = new IParameter[0];
            var activationException = new ActivationException();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), (Func<IBindingMetadata, bool>) null, parameters, true, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Throws(activationException);

            var actual = ResolutionExtensions.TryGet<IWeapon>(root, parameters);

            Assert.Null(actual);
        }

        [Fact]
        public void TryGet_RootAndConstraintAndParameters_ShouldThrowArgumentNullExceptionWhenRootIsNull()
        {
            const IResolutionRoot root = null;
            Func<IBindingMetadata, bool> constraint = (_) => true;
            var parameters = new IParameter[0];

            var actual = Assert.Throws<ArgumentNullException>(
                () => ResolutionExtensions.TryGet<string>(root, constraint, parameters));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(root), actual.ParamName);
        }

        [Fact]
        public void TryGet_RootAndConstraintAndParameters_ResolvesToNull()
        {
            var root = _rootMock.Object;
            Func<IBindingMetadata, bool> constraint = (_) => true;
            var parameters = new IParameter[0];

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), constraint, parameters, true, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Returns(null);

            var actual = ResolutionExtensions.TryGet<IWeapon>(root, constraint, parameters);

            Assert.Null(actual);
        }

        [Fact]
        public void TryGet_RootAndConstraintAndParameters_ResolvesToInstanceOfService()
        {
            var root = _rootMock.Object;
            Func<IBindingMetadata, bool> constraint = (_) => true;
            var parameters = new IParameter[0];
            var resolvedInstance = new Dagger();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), constraint, parameters, true, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Returns(resolvedInstance);

            var actual = ResolutionExtensions.TryGet<IWeapon>(root, constraint, parameters);

            Assert.NotNull(actual);
            Assert.Same(resolvedInstance, actual);
        }

        [Fact]
        public void TryGet_RootAndConstraintAndParameters_ResolvesToInstanceNotOfService()
        {
            var root = _rootMock.Object;
            Func<IBindingMetadata, bool> constraint = (_) => true;
            var parameters = new IParameter[0];
            var resolvedInstance = 5L;

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWarrior), constraint, parameters, true, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Returns(resolvedInstance);

            var actual = Assert.Throws<InvalidCastException>(() => ResolutionExtensions.TryGet<IWarrior>(root, constraint, parameters));

            Assert.Null(actual.InnerException);
            Assert.Equal($"Unable to cast object of type '{resolvedInstance.GetType().FullName}' to type '{typeof(IWarrior).FullName}'.", actual.Message);
        }

        [Fact]
        public void TryGet_RootAndConstraintAndParameters_ResolveThrowsActivationException()
        {
            var root = _rootMock.Object;
            Func<IBindingMetadata, bool> constraint = (_) => true;
            var parameters = new IParameter[0];
            var activationException = new ActivationException();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), constraint, parameters, true, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Throws(activationException);

            var actual = ResolutionExtensions.TryGet<IWeapon>(root, constraint, parameters);

            Assert.Null(actual);
        }

        [Fact]
        public void TryGet_RootAndNameAndParameters_ShouldThrowArgumentNullExceptionWhenRootIsNull()
        {
            const IResolutionRoot root = null;
            var name = "NAME";
            var parameters = new IParameter[0];

            var actual = Assert.Throws<ArgumentNullException>(
                () => ResolutionExtensions.TryGet<string>(root, name, parameters));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(root), actual.ParamName);
        }

        [Fact]
        public void TryGet_RootAndNameAndParameters_ResolvesToNull()
        {
            var root = _rootMock.Object;
            var name = "NAME";
            var parameters = new IParameter[0];

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), It.IsNotNull<Func<IBindingMetadata, bool>>(), parameters, true, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Returns(null);

            var actual = ResolutionExtensions.TryGet<IWeapon>(root, name, parameters);

            Assert.Null(actual);
        }

        [Fact]
        public void TryGet_RootAndNameAndParameters_ResolvesToInstanceOfService()
        {
            var root = _rootMock.Object;
            var name = "NAME";
            var parameters = new IParameter[0];
            var resolvedInstance = new Dagger();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), It.IsNotNull<Func<IBindingMetadata, bool>>(), parameters, true, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Returns(resolvedInstance);

            var actual = ResolutionExtensions.TryGet<IWeapon>(root, name, parameters);

            Assert.NotNull(actual);
            Assert.Same(resolvedInstance, actual);
        }

        [Fact]
        public void TryGet_RootAndNameAndParameters_ResolvesToInstanceNotOfService()
        {
            var root = _rootMock.Object;
            var name = "NAME";
            var parameters = new IParameter[0];
            var resolvedInstance = 5L;

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWarrior), It.IsNotNull<Func<IBindingMetadata, bool>>(), parameters, true, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Returns(resolvedInstance);

            var actual = Assert.Throws<InvalidCastException>(() => ResolutionExtensions.TryGet<IWarrior>(root, name, parameters));

            Assert.Null(actual.InnerException);
            Assert.Equal($"Unable to cast object of type '{resolvedInstance.GetType().FullName}' to type '{typeof(IWarrior).FullName}'.", actual.Message);
        }

        [Fact]
        public void TryGet_RootAndNameAndParameters_ResolveThrowsActivationException()
        {
            var root = _rootMock.Object;
            var name = "NAME";
            var parameters = new IParameter[0];
            var activationException = new ActivationException();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), It.IsNotNull<Func<IBindingMetadata, bool>>(), parameters, true, true))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Throws(activationException);

            var actual = ResolutionExtensions.TryGet<IWeapon>(root, name, parameters);

            Assert.Null(actual);
        }

        [Fact]
        public void TryGetAndThrowOnInvalidBinding_RootAndParameters_ShouldThrowArgumentNullExceptionWhenRootIsNull()
        {
            const IResolutionRoot root = null;
            var parameters = new IParameter[0];

            var actual = Assert.Throws<ArgumentNullException>(
                () => ResolutionExtensions.TryGetAndThrowOnInvalidBinding<string>(root, parameters));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(root), actual.ParamName);
        }

        [Fact]
        public void TryGetAndThrowOnInvalidBinding_RootAndParameters_ResolveReturnsNull()
        {
            var root = _rootMock.Object;
            var parameters = new IParameter[0];

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), (Func<IBindingMetadata, bool>) null, parameters, true, true))
                     .Returns(_requestMock.Object);
            _requestMock.InSequence(_sequence)
                        .SetupSet(p => p.ForceUnique = true);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Returns(null);

            var actual = ResolutionExtensions.TryGetAndThrowOnInvalidBinding<IWeapon>(root, parameters);

            Assert.Null(actual);
        }

        [Fact]
        public void TryGetAndThrowOnInvalidBinding_RootAndParameters_ResolvesToInstanceOfService()
        {
            var root = _rootMock.Object;
            var parameters = new IParameter[0];
            var resolvedInstance = new Dagger();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), (Func<IBindingMetadata, bool>) null, parameters, true, true))
                     .Returns(_requestMock.Object);
            _requestMock.InSequence(_sequence)
                        .SetupSet(p => p.ForceUnique = true);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Returns(resolvedInstance);

            var actual = ResolutionExtensions.TryGetAndThrowOnInvalidBinding<IWeapon>(root, parameters);

            Assert.NotNull(actual);
            Assert.Same(resolvedInstance, actual);
        }

        [Fact]
        public void TryGetAndThrowOnInvalidBinding_RootAndParameters_ResolvesToInstanceNotOfService()
        {
            var root = _rootMock.Object;
            var parameters = new IParameter[0];
            var resolvedInstance = 5L;

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWarrior), (Func<IBindingMetadata, bool>) null, parameters, true, true))
                     .Returns(_requestMock.Object);
            _requestMock.InSequence(_sequence)
                        .SetupSet(p => p.ForceUnique = true);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Returns(resolvedInstance);

            var actual = Assert.Throws<InvalidCastException>(() => ResolutionExtensions.TryGetAndThrowOnInvalidBinding<IWarrior>(root, parameters));

            Assert.Null(actual.InnerException);
            Assert.Equal($"Unable to cast object of type '{resolvedInstance.GetType().FullName}' to type '{typeof(IWarrior).FullName}'.", actual.Message);
        }

        [Fact]
        public void TryGetAndThrowOnInvalidBinding_RootAndParameters_ResolveThrowsActivationException()
        {
            var root = _rootMock.Object;
            var parameters = new IParameter[0];
            var activationException = new ActivationException();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), (Func<IBindingMetadata, bool>)null, parameters, true, true))
                     .Returns(_requestMock.Object);
            _requestMock.InSequence(_sequence)
                        .SetupSet(p => p.ForceUnique = true);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Throws(activationException);

            var actual = Assert.Throws<ActivationException>(() => ResolutionExtensions.TryGetAndThrowOnInvalidBinding<IWeapon>(root, parameters));

            Assert.Same(activationException, actual);
        }

        [Fact]
        public void TryGetAndThrowOnInvalidBinding_RootAndConstraintAndParameters_ShouldThrowArgumentNullExceptionWhenRootIsNull()
        {
            const IResolutionRoot root = null;
            Func<IBindingMetadata, bool> constraint = (_) => true;
            var parameters = new IParameter[0];

            var actual = Assert.Throws<ArgumentNullException>(
                () => ResolutionExtensions.TryGetAndThrowOnInvalidBinding<string>(root, constraint, parameters));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(root), actual.ParamName);
        }

        [Fact]
        public void TryGetAndThrowOnInvalidBinding_RootAndConstraintAndParameters_ResolveReturnsNull()
        {
            var root = _rootMock.Object;
            Func<IBindingMetadata, bool> constraint = (_) => true;
            var parameters = new IParameter[0];

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), constraint, parameters, true, true))
                     .Returns(_requestMock.Object);
            _requestMock.InSequence(_sequence)
                        .SetupSet(p => p.ForceUnique = true);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Returns(null);

            var actual = ResolutionExtensions.TryGetAndThrowOnInvalidBinding<IWeapon>(root, constraint, parameters);

            Assert.Null(actual);
        }

        [Fact]
        public void TryGetAndThrowOnInvalidBinding_RootAndConstraintAndParameters_ResolvesToInstanceOfService()
        {
            var root = _rootMock.Object;
            Func<IBindingMetadata, bool> constraint = (_) => true;
            var parameters = new IParameter[0];
            var resolvedInstance = new Dagger();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), constraint, parameters, true, true))
                     .Returns(_requestMock.Object);
            _requestMock.InSequence(_sequence)
                        .SetupSet(p => p.ForceUnique = true);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Returns(resolvedInstance);

            var actual = ResolutionExtensions.TryGetAndThrowOnInvalidBinding<IWeapon>(root, constraint, parameters);

            Assert.NotNull(actual);
            Assert.Same(resolvedInstance, actual);
        }

        [Fact]
        public void TryGetAndThrowOnInvalidBinding_RootAndConstraintAndParameters_ResolvesToInstanceNotOfService()
        {
            var root = _rootMock.Object;
            Func<IBindingMetadata, bool> constraint = (_) => true;
            var parameters = new IParameter[0];
            var resolvedInstance = 5L;

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWarrior), constraint, parameters, true, true))
                     .Returns(_requestMock.Object);
            _requestMock.InSequence(_sequence)
                        .SetupSet(p => p.ForceUnique = true);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Returns(resolvedInstance);

            var actual = Assert.Throws<InvalidCastException>(() => ResolutionExtensions.TryGetAndThrowOnInvalidBinding<IWarrior>(root, constraint, parameters));

            Assert.Null(actual.InnerException);
            Assert.Equal($"Unable to cast object of type '{resolvedInstance.GetType().FullName}' to type '{typeof(IWarrior).FullName}'.", actual.Message);
        }

        [Fact]
        public void TryGetAndThrowOnInvalidBinding_RootAndConstraintAndParameters_ResolvesToInstanceNotOfService_ShouldThrowInvalidCastException()
        {
            var root = _rootMock.Object;
            Func<IBindingMetadata, bool> constraint = (_) => true;
            var parameters = new IParameter[0];
            var resolvedInstance = new Monk();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), constraint, parameters, true, true))
                     .Returns(_requestMock.Object);
            _requestMock.InSequence(_sequence)
                        .SetupSet(p => p.ForceUnique = true);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Returns(resolvedInstance);

            var actual = Assert.Throws<InvalidCastException>(() => ResolutionExtensions.TryGetAndThrowOnInvalidBinding<IWeapon>(root, constraint, parameters));

            Assert.Null(actual.InnerException);
            Assert.Equal($"Unable to cast object of type '{resolvedInstance.GetType().FullName}' to type '{typeof(IWeapon).FullName}'.", actual.Message);
        }

        [Fact]
        public void TryGetAndThrowOnInvalidBinding_RootAndConstraintAndParameters_ResolveThrowsActivationException()
        {
            var root = _rootMock.Object;
            Func<IBindingMetadata, bool> constraint = (_) => true;
            var parameters = new IParameter[0];
            var activationException = new ActivationException();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), constraint, parameters, true, true))
                     .Returns(_requestMock.Object);
            _requestMock.InSequence(_sequence)
                        .SetupSet(p => p.ForceUnique = true);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Throws(activationException);

            var actual = Assert.Throws<ActivationException>(() => ResolutionExtensions.TryGetAndThrowOnInvalidBinding<IWeapon>(root, constraint, parameters));

            Assert.Same(activationException, actual);
        }

        [Fact]
        public void TryGetAndThrowOnInvalidBinding_RootAndNameAndParameters_ShouldThrowArgumentNullExceptionWhenRootIsNull()
        {
            const IResolutionRoot root = null;
            var name = "NAME";
            var parameters = new IParameter[0];

            var actual = Assert.Throws<ArgumentNullException>(
                () => ResolutionExtensions.TryGetAndThrowOnInvalidBinding<string>(root, name, parameters));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(root), actual.ParamName);
        }

        [Fact]
        public void TryGetAndThrowOnInvalidBinding_RootAndNameAndParameters_ResolveReturnsNull()
        {
            var root = _rootMock.Object;
            var name = "NAME";
            var parameters = new IParameter[0];
            var resolvedInstances = Enumerable.Empty<object>();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), It.IsNotNull<Func<IBindingMetadata, bool>>(), parameters, true, true))
                     .Returns(_requestMock.Object);
            _requestMock.InSequence(_sequence)
                        .SetupSet(p => p.ForceUnique = true);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Returns(null);

            var actual = ResolutionExtensions.TryGetAndThrowOnInvalidBinding<IWeapon>(root, name, parameters);

            Assert.Null(actual);
        }

        [Fact]
        public void TryGetAndThrowOnInvalidBinding_RootAndNameAndParameters_ResolveReturnsInstanceOfService()
        {
            var root = _rootMock.Object;
            var name = "NAME";
            var parameters = new IParameter[0];
            var resolvedInstance = new Dagger();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), It.IsNotNull<Func<IBindingMetadata, bool>>(), parameters, true, true))
                     .Returns(_requestMock.Object);
            _requestMock.InSequence(_sequence)
                        .SetupSet(p => p.ForceUnique = true);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Returns(resolvedInstance);

            var actual = ResolutionExtensions.TryGetAndThrowOnInvalidBinding<IWeapon>(root, name, parameters);

            Assert.NotNull(actual);
            Assert.Same(resolvedInstance, actual);
        }

        [Fact]
        public void TryGetAndThrowOnInvalidBinding_RootAndNameAndParameters_ResolveReturnsInstanceNotOfService()
        {
            var root = _rootMock.Object;
            var name = "NAME";
            var parameters = new IParameter[0];
            var resolvedInstance = 5L;

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWarrior), It.IsNotNull<Func<IBindingMetadata, bool>>(), parameters, true, true))
                     .Returns(_requestMock.Object);
            _requestMock.InSequence(_sequence)
                        .SetupSet(p => p.ForceUnique = true);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Returns(resolvedInstance);

            var actual = Assert.Throws<InvalidCastException>(() => ResolutionExtensions.TryGetAndThrowOnInvalidBinding<IWarrior>(root, name, parameters));

            Assert.Null(actual.InnerException);
            Assert.Equal($"Unable to cast object of type '{resolvedInstance.GetType().FullName}' to type '{typeof(IWarrior).FullName}'.", actual.Message);
        }

        [Fact]
        public void TryGetAndThrowOnInvalidBinding_RootAndNameAndParameters_ResolveThrowsActivationException()
        {
            var root = _rootMock.Object;
            var name = "NAME";
            var parameters = new IParameter[0];
            var activationException = new ActivationException();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), It.IsNotNull<Func<IBindingMetadata, bool>>(), parameters, true, true))
                     .Returns(_requestMock.Object);
            _requestMock.InSequence(_sequence)
                        .SetupSet(p => p.ForceUnique = true);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.ResolveSingle(_requestMock.Object))
                     .Throws(activationException);

            var actual = Assert.Throws<ActivationException>(() => ResolutionExtensions.TryGetAndThrowOnInvalidBinding<IWeapon>(root, name, parameters));

            Assert.Same(activationException, actual);
        }

        [Fact]
        public void GetAll_RootAndParameters_ShouldThrowArgumentNullExceptionWhenRootIsNull()
        {
            const IResolutionRoot root = null;
            var parameters = new IParameter[0];

            var actual = Assert.Throws<ArgumentNullException>(
                () => ResolutionExtensions.GetAll<string>(root, parameters));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(root), actual.ParamName);
        }

        [Fact]
        public void GetAll_RootAndParameters_NoInstancesOfService()
        {
            var root = _rootMock.Object;
            var parameters = new IParameter[0];
            var resolvedInstances = Enumerable.Empty<object>();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), (Func<IBindingMetadata, bool>) null, parameters, true, false))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.Resolve(_requestMock.Object))
                     .Returns(resolvedInstances);

            var actual = ResolutionExtensions.GetAll<IWeapon>(root, parameters);

            Assert.NotNull(actual);
            Assert.Empty(actual);
        }

        [Fact]
        public void GetAll_RootAndParameters_SingleInstanceOfService()
        {
            var root = _rootMock.Object;
            var parameters = new IParameter[0];
            var resolvedInstances = new object[] { new Dagger() };

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), (Func<IBindingMetadata, bool>) null, parameters, true, false))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.Resolve(_requestMock.Object))
                     .Returns((IEnumerable<object>) resolvedInstances);

            var actual = ResolutionExtensions.GetAll<IWeapon>(root, parameters);

            Assert.NotNull(actual);

            using (var enumerator = actual.GetEnumerator())
            {
                Assert.True(enumerator.MoveNext());
                Assert.Same(resolvedInstances[0], enumerator.Current);
                Assert.False(enumerator.MoveNext());
            }
        }

        [Fact]
        public void GetAll_RootAndParameters_SingleInstanceNotOfService()
        {
            var root = _rootMock.Object;
            var parameters = new IParameter[0];
            var resolvedInstances = new object[] { 5L };

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWarrior), (Func<IBindingMetadata, bool>) null, parameters, true, false))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.Resolve(_requestMock.Object))
                     .Returns((IEnumerable<object>) resolvedInstances);

            var actual = ResolutionExtensions.GetAll<IWarrior>(root, parameters);

            Assert.NotNull(actual);

            using (var enumerator = actual.GetEnumerator())
            {
                var actualException = Assert.Throws<InvalidCastException>(() => enumerator.MoveNext());

                Assert.Null(actualException.InnerException);
                Assert.Equal($"Unable to cast object of type '{resolvedInstances[0].GetType().FullName}' to type '{typeof(IWarrior).FullName}'.", actualException.Message);
            }
        }

        [Fact]
        public void GetAll_RootAndParameters_MultipleInstancesOfService()
        {
            var root = _rootMock.Object;
            var parameters = new IParameter[0];
            var resolvedInstances = new object[] { new Dagger(), new Sword() };

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), (Func<IBindingMetadata, bool>) null, parameters, true, false))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.Resolve(_requestMock.Object))
                     .Returns((IEnumerable<object>) resolvedInstances);

            var actual = ResolutionExtensions.GetAll<IWeapon>(root, parameters);

            Assert.NotNull(actual);
            Assert.Equal(resolvedInstances, actual);
        }

        [Fact]
        public void GetAll_RootAndParameters_MultipleInstancesNotOfService_ShouldThrowInvalidCastException()
        {
            var root = _rootMock.Object;
            var parameters = new IParameter[0];
            var resolvedInstances = new object[] { new Dagger(), new Monk() };

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), (Func<IBindingMetadata, bool>) null, parameters, true, false))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.Resolve(_requestMock.Object))
                     .Returns((IEnumerable<object>) resolvedInstances);

            var actual = ResolutionExtensions.GetAll<IWeapon>(root, parameters);

            Assert.NotNull(actual);

            using (var enumerator = actual.GetEnumerator())
            {
                Assert.True(enumerator.MoveNext());
                Assert.Same(resolvedInstances[0], enumerator.Current);

                var actualException = Assert.Throws<InvalidCastException>(() => enumerator.MoveNext());

                Assert.Null(actualException.InnerException);
                Assert.Equal($"Unable to cast object of type '{resolvedInstances[1].GetType().FullName}' to type '{typeof(IWeapon).FullName}'.", actualException.Message);
            }
        }

        [Fact]
        public void GetAll_RootAndParameters_ResolveThrowsActivationException()
        {
            var root = _rootMock.Object;
            var parameters = new IParameter[0];
            var activationException = new ActivationException();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), (Func<IBindingMetadata, bool>) null, parameters, true, false))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.Resolve(_requestMock.Object))
                     .Throws(activationException);

            var actual = Assert.Throws<ActivationException>(() => ResolutionExtensions.GetAll<IWeapon>(root, parameters));

            Assert.Same(activationException, actual);
        }

        [Fact]
        public void GetAll_RootAndConstraintAndParameters_ShouldThrowArgumentNullExceptionWhenRootIsNull()
        {
            const IResolutionRoot root = null;
            Func<IBindingMetadata, bool> constraint = (_) => true;
            var parameters = new IParameter[0];

            var actual = Assert.Throws<ArgumentNullException>(
                () => ResolutionExtensions.GetAll<string>(root, constraint, parameters));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(root), actual.ParamName);
        }

        [Fact]
        public void GetAll_RootAndConstraintAndParameters_NoInstancesOfService()
        {
            var root = _rootMock.Object;
            Func<IBindingMetadata, bool> constraint = (_) => true;
            var parameters = new IParameter[0];
            var resolvedInstances = Enumerable.Empty<object>();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), constraint, parameters, true, false))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.Resolve(_requestMock.Object))
                     .Returns(resolvedInstances);

            var actual = ResolutionExtensions.GetAll<IWeapon>(root, constraint, parameters);

            Assert.NotNull(actual);
            Assert.Empty(actual);
        }

        [Fact]
        public void GetAll_RootAndConstraintAndParameters_SingleInstanceOfService()
        {
            var root = _rootMock.Object;
            Func<IBindingMetadata, bool> constraint = (_) => true;
            var parameters = new IParameter[0];
            var resolvedInstances = new object[] { new Dagger() };

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), constraint, parameters, true, false))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.Resolve(_requestMock.Object))
                     .Returns((IEnumerable<object>) resolvedInstances);

            var actual = ResolutionExtensions.GetAll<IWeapon>(root, constraint, parameters);

            Assert.NotNull(actual);

            using (var enumerator = actual.GetEnumerator())
            {
                Assert.True(enumerator.MoveNext());
                Assert.Same(resolvedInstances[0], enumerator.Current);
                Assert.False(enumerator.MoveNext());
            }
        }

        [Fact]
        public void GetAll_RootAndConstraintAndParameters_SingleInstanceNotOfService()
        {
            var root = _rootMock.Object;
            Func<IBindingMetadata, bool> constraint = (_) => true;
            var parameters = new IParameter[0];
            var resolvedInstances = new object[] { 5L };

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWarrior), constraint, parameters, true, false))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.Resolve(_requestMock.Object))
                     .Returns((IEnumerable<object>) resolvedInstances);

            var actual = ResolutionExtensions.GetAll<IWarrior>(root, constraint, parameters);

            Assert.NotNull(actual);

            using (var enumerator = actual.GetEnumerator())
            {
                var actualException = Assert.Throws<InvalidCastException>(() => enumerator.MoveNext());

                Assert.Null(actualException.InnerException);
                Assert.Equal($"Unable to cast object of type '{resolvedInstances[0].GetType().FullName}' to type '{typeof(IWarrior).FullName}'.", actualException.Message);
            }
        }

        [Fact]
        public void GetAll_RootAndConstraintAndParameters_MultipleInstancesOfService()
        {
            var root = _rootMock.Object;
            Func<IBindingMetadata, bool> constraint = (_) => true;
            var parameters = new IParameter[0];
            var resolvedInstances = new object[] { new Dagger(), new Sword() };

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), constraint, parameters, true, false))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.Resolve(_requestMock.Object))
                     .Returns((IEnumerable<object>) resolvedInstances);

            var actual = ResolutionExtensions.GetAll<IWeapon>(root, constraint, parameters);

            Assert.Equal(resolvedInstances, actual);
        }

        [Fact]
        public void GetAll_RootAndConstraintAndParameters_MultipleInstancesNotOfService_ShouldThrowInvalidCastException()
        {
            var root = _rootMock.Object;
            Func<IBindingMetadata, bool> constraint = (_) => true;
            var parameters = new IParameter[0];
            var resolvedInstances = new object[] { new Dagger(), new Monk() };

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), constraint, parameters, true, false))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.Resolve(_requestMock.Object))
                     .Returns((IEnumerable<object>) resolvedInstances);

            var actual = ResolutionExtensions.GetAll<IWeapon>(root, constraint, parameters);

            using (var enumerator = actual.GetEnumerator())
            {
                Assert.True(enumerator.MoveNext());
                Assert.Same(resolvedInstances[0], enumerator.Current);

                var actualException = Assert.Throws<InvalidCastException>(() => enumerator.MoveNext());

                Assert.Null(actualException.InnerException);
                Assert.Equal($"Unable to cast object of type '{resolvedInstances[1].GetType().FullName}' to type '{typeof(IWeapon).FullName}'.", actualException.Message);
            }
        }

        [Fact]
        public void GetAll_RootAndConstraintAndParameters_ResolveThrowsActivationException()
        {
            var root = _rootMock.Object;
            Func<IBindingMetadata, bool> constraint = (_) => true;
            var parameters = new IParameter[0];
            var activationException = new ActivationException();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), constraint, parameters, true, false))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.Resolve(_requestMock.Object))
                     .Throws(activationException);

            var actual = Assert.Throws<ActivationException>(() => ResolutionExtensions.GetAll<IWeapon>(root, constraint, parameters));

            Assert.Same(activationException, actual);
        }

        [Fact]
        public void GetAll_RootAndNameAndParameters_ShouldThrowArgumentNullExceptionWhenRootIsNull()
        {
            const IResolutionRoot root = null;
            var name = "NAME";
            var parameters = new IParameter[0];

            var actual = Assert.Throws<ArgumentNullException>(
                () => ResolutionExtensions.GetAll<string>(root, name, parameters));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(root), actual.ParamName);
        }

        [Fact]
        public void GetAll_RootAndNameAndParameters_NoInstancesOfService()
        {
            var root = _rootMock.Object;
            var name = "NAME";
            var parameters = new IParameter[0];
            var resolvedInstances = Enumerable.Empty<object>();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), It.IsNotNull<Func<IBindingMetadata, bool>>(), parameters, true, false))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.Resolve(_requestMock.Object))
                     .Returns(resolvedInstances);

            var actual = ResolutionExtensions.GetAll<IWeapon>(root, name, parameters);

            Assert.NotNull(actual);
            Assert.Empty(actual);
        }

        [Fact]
        public void GetAll_RootAndNameAndParameters_SingleInstanceOfService()
        {
            var root = _rootMock.Object;
            var name = "NAME";
            var parameters = new IParameter[0];
            var resolvedInstances = new object[] { new Dagger() };

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), It.IsNotNull<Func<IBindingMetadata, bool>>(), parameters, true, false))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.Resolve(_requestMock.Object))
                     .Returns((IEnumerable<object>) resolvedInstances);

            var actual = ResolutionExtensions.GetAll<IWeapon>(root, name, parameters);

            Assert.NotNull(actual);
            Assert.Equal(resolvedInstances, actual);
        }

        [Fact]
        public void GetAll_RootAndNameAndParameters_SingleInstanceNotOfService()
        {
            var root = _rootMock.Object;
            var name = "NAME";
            var parameters = new IParameter[0];
            var resolvedInstances = new object[] { 5L };

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(string), It.IsNotNull<Func<IBindingMetadata, bool>>(), parameters, true, false))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.Resolve(_requestMock.Object))
                     .Returns((IEnumerable<object>) resolvedInstances);

            var actual = ResolutionExtensions.GetAll<string>(root, name, parameters);

            using (var enumerator = actual.GetEnumerator())
            {
                var actualException = Assert.Throws<InvalidCastException>(() => enumerator.MoveNext());

                Assert.Null(actualException.InnerException);
                Assert.Equal($"Unable to cast object of type '{resolvedInstances[0].GetType().FullName}' to type '{typeof(string).FullName}'.", actualException.Message);
            }
        }

        [Fact]
        public void GetAll_RootAndNameAndParameters_MultipleInstancesOfService()
        {
            var root = _rootMock.Object;
            var name = "NAME";
            var parameters = new IParameter[0];
            var resolvedInstances = new object[] { new Dagger(), new Sword() };

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), It.IsNotNull<Func<IBindingMetadata, bool>>(), parameters, true, false))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.Resolve(_requestMock.Object))
                     .Returns((IEnumerable<object>) resolvedInstances);

            var actual = ResolutionExtensions.GetAll<IWeapon>(root, name, parameters);

            Assert.Equal(resolvedInstances, actual);
        }

        [Fact]
        public void GetAll_RootAndNameAndParameters_MultipleInstancesNotOfService_ShouldThrowInvalidCastException()
        {
            var root = _rootMock.Object;
            var name = "NAME";
            var parameters = new IParameter[0];
            var resolvedInstances = new object[] { new Dagger(), new Monk() };

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), It.IsNotNull<Func<IBindingMetadata, bool>>(), parameters, true, false))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.Resolve(_requestMock.Object))
                     .Returns((IEnumerable<object>) resolvedInstances);

            var actual = ResolutionExtensions.GetAll<IWeapon>(root, name, parameters);

            Assert.NotNull(actual);

            using (var enumerator = actual.GetEnumerator())
            {
                Assert.True(enumerator.MoveNext());
                Assert.Same(resolvedInstances[0], enumerator.Current);

                var actualException = Assert.Throws<InvalidCastException>(() => enumerator.MoveNext());

                Assert.Null(actualException.InnerException);
                Assert.Equal($"Unable to cast object of type '{resolvedInstances[1].GetType().FullName}' to type '{typeof(IWeapon).FullName}'.", actualException.Message);
            }
        }

        [Fact]
        public void GetAll_RootAndNameAndParameters_ResolveThrowsActivationException()
        {
            var root = _rootMock.Object;
            var name = "NAME";
            var parameters = new IParameter[0];
            var activationException = new ActivationException();

            _rootMock.InSequence(_sequence)
                     .Setup(p => p.CreateRequest(typeof(IWeapon), It.IsNotNull<Func<IBindingMetadata, bool>>(), parameters, true, false))
                     .Returns(_requestMock.Object);
            _rootMock.InSequence(_sequence)
                     .Setup(p => p.Resolve(_requestMock.Object))
                     .Throws(activationException);

            var actual = Assert.Throws<ActivationException>(() => ResolutionExtensions.GetAll<IWeapon>(root, name, parameters));

            Assert.Same(activationException, actual);
        }

    }
}
