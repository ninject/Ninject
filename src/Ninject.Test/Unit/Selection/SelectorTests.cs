using Moq;
using Ninject.Selection;
using Ninject.Selection.Heuristics;
using Ninject.Tests.Fakes;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Ninject.Test.Unit.Selection
{
    public class SelectorTests
    {
        private Mock<IInjectionHeuristic> _injectionHeuristicMock1;
        private Mock<IInjectionHeuristic> _injectionHeuristicMock2;
        private Mock<INinjectSettings> _settingsMock;
        private MockSequence _sequence;
        private IEnumerable<IInjectionHeuristic> _injectionHeuristics;
        private Selector _selector;

        public SelectorTests()
        {
            _injectionHeuristicMock1 = new Mock<IInjectionHeuristic>(MockBehavior.Strict);
            _injectionHeuristicMock2 = new Mock<IInjectionHeuristic>(MockBehavior.Strict);
            _settingsMock = new Mock<INinjectSettings>(MockBehavior.Strict);
            _sequence = new MockSequence();

            _injectionHeuristics = new IInjectionHeuristic[]
                {
                    _injectionHeuristicMock1.Object,
                    _injectionHeuristicMock2.Object
                };

            _selector = new Selector(_injectionHeuristics, _settingsMock.Object);
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullExceptionWhenInjectionHeuristicsIsNull()
        {
            IInjectionHeuristic[] injectionHeuristics = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new Selector(injectionHeuristics, _settingsMock.Object));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(injectionHeuristics), actual.ParamName);
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullExceptionWhenSettingsIsNull()
        {
            const INinjectSettings settings = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new Selector(_injectionHeuristics, settings));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(settings), actual.ParamName);
        }

        [Fact]
        public void SelectConstructorsForInjection_ShouldThrowArgumentNullExceptionWhenTypeIsNull()
        {
            const Type type = null;

            var actual = Assert.Throws<ArgumentNullException>(() => _selector.SelectConstructorsForInjection(type));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(type), actual.ParamName);

        }

        [Fact]
        public void SelectConstructorsForInjection_InjectNonPublicIsTrue_ShouldReturnEmptyResultWhenTypeIsSubclassOfMulticastDelegate()
        {
            MyDelegate delegate1 = () => { };
            MyDelegate delegate2 = () => { };
            MyDelegate multiDelegate = delegate1 + delegate2;

            _settingsMock.Setup(p => p.InjectNonPublic).Returns(true);

            Assert.Empty(_selector.SelectConstructorsForInjection(delegate1.GetType()));
            Assert.Empty(_selector.SelectConstructorsForInjection(multiDelegate.GetType()));
        }

        [Fact]
        public void SelectConstructorsForInjection_InjectNonPublicIsFalse_ShouldReturnEmptyResultWhenTypeIsSubclassOfMulticastDelegate()
        {
            MyDelegate delegate1 = () => { };
            MyDelegate delegate2 = () => { };
            MyDelegate multiDelegate = delegate1 + delegate2;

            _settingsMock.Setup(p => p.InjectNonPublic).Returns(false);

            Assert.Empty(_selector.SelectConstructorsForInjection(delegate1.GetType()));
            Assert.Empty(_selector.SelectConstructorsForInjection(multiDelegate.GetType()));
        }

        [Fact]
        public void SelectConstructorsForInjection_InjectNonPublicIsTrue_ShouldReturnPublicAndNonPublicInstanceConstructors()
        {
            var type = typeof(MyService);

            _settingsMock.Setup(p => p.InjectNonPublic).Returns(true);

            var actual = _selector.SelectConstructorsForInjection(typeof(MyService));

            Assert.Equal(new [] 
                            {
                                type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new [] { typeof(string) }, Array.Empty<ParameterModifier>()),
                                type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new [] { typeof(int) }, Array.Empty<ParameterModifier>()),
                                type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new [] { typeof(int), typeof(string) }, Array.Empty<ParameterModifier>()),
                                type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new [] { typeof(string), typeof(int) }, Array.Empty<ParameterModifier>()),
                                type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new [] { typeof(Type) }, Array.Empty<ParameterModifier>()),
                                type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new [] { typeof(Type), typeof(string) }, Array.Empty<ParameterModifier>())
                            },
                        actual);
        }

        [Fact]
        public void SelectConstructorsForInjection_InjectNonPublicIsFalse_ShouldOnlyReturnPublicConstructors()
        {
            var type = typeof(MyService);

            _settingsMock.Setup(p => p.InjectNonPublic).Returns(false);

            var actual = _selector.SelectConstructorsForInjection(type);

            Assert.Equal(new []
                            {
                                type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new [] { typeof(Type) }, Array.Empty<ParameterModifier>()),
                                type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new [] { typeof(Type), typeof(string) }, Array.Empty<ParameterModifier>())
                            },
                        actual);
        }

        [Fact]
        public void SelectConstructorsForInjection_InjectNonPublicIsTrue_ShouldReturnEmptyResultWhenTypeIsStaticClass()
        {
            _settingsMock.Setup(p => p.InjectNonPublic).Returns(true);

            var actual = _selector.SelectConstructorsForInjection(typeof(MyFactory));

            Assert.Empty(actual);
        }

        [Fact]
        public void SelectConstructorsForInjection_InjectNonPublicIsFalse_ShouldReturnEmptyResultWhenTypeIsStaticClass()
        {
            _settingsMock.Setup(p => p.InjectNonPublic).Returns(false);

            var actual = _selector.SelectConstructorsForInjection(typeof(MyFactory));

            Assert.Empty(actual);
        }

        [Fact]
        public void SelectMethodsForInjection_ShouldThrowArgumentNullExceptionWhenTypeIsNull()
        {
            const Type type = null;

            var actual = Assert.Throws<ArgumentNullException>(() => _selector.SelectMethodsForInjection(type));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(type), actual.ParamName);
        }

        [Fact]
        public void SelectMethodsForInjection_InjectNonPublicIsTrue_ShouldReturnPublicAndNonPublicInstanceMethodsThatAreEligibleForInjection()
        {
            #region Arrange

            var type = typeof(MyService);
            var getWeaponMethod = type.GetMethod("get_Weapon", BindingFlags.NonPublic | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var setWeaponMethod = type.GetMethod("set_Weapon", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(IWeapon) }, Array.Empty<ParameterModifier>());
            var setIdMethod = type.GetMethod("set_Id", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(int) }, Array.Empty<ParameterModifier>());
            var getNameMethod = type.GetMethod("get_Name", BindingFlags.NonPublic | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var getEnabledMethod = type.GetMethod("get_Enabled", BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var setStopMethod = type.GetMethod("set_Stop", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(bool) }, Array.Empty<ParameterModifier>());
            var getVisibleMethod = type.GetMethod("get_Visible", BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var setVisibleMethod = type.GetMethod("set_Visible", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(bool) }, Array.Empty<ParameterModifier>());
            var doMethodNoArgs = type.GetMethod("Do", BindingFlags.NonPublic | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var doMethodStringArg = type.GetMethod("Do", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(string) }, Array.Empty<ParameterModifier>());
            var doMethodStringAndInt32Args = type.GetMethod("Do", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(string), typeof(int) }, Array.Empty<ParameterModifier>());
            var executeMethodNoArgs = type.GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var executeMethodStringArg = type.GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(string) }, Array.Empty<ParameterModifier>());
            var executeMethodStringAndInt32Arg = type.GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(string), typeof(int) }, Array.Empty<ParameterModifier>());
            var toStringMethod = type.GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var equalsMethod = type.GetMethod("Equals", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(object) }, Array.Empty<ParameterModifier>());
            var getHashCodeMethod = type.GetMethod("GetHashCode", BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var getTypeMethod = type.GetMethod("GetType", BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var finalizeMethod = type.GetMethod("Finalize", BindingFlags.NonPublic | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var memberwiseCloneMethod = type.GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());

            _settingsMock.Setup(p => p.InjectNonPublic).Returns(true);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(getWeaponMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(getWeaponMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(setWeaponMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(setWeaponMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(setIdMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(setIdMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(getNameMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(getNameMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(getEnabledMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(getEnabledMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(setStopMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(setStopMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(getVisibleMethod)).Returns(true);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(setVisibleMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(setVisibleMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(doMethodNoArgs)).Returns(true);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(doMethodStringArg)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(doMethodStringArg)).Returns(true);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(doMethodStringAndInt32Args)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(doMethodStringAndInt32Args)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(executeMethodNoArgs)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(executeMethodNoArgs)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(executeMethodStringArg)).Returns(true);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(executeMethodStringAndInt32Arg)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(executeMethodStringAndInt32Arg)).Returns(true);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(toStringMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(toStringMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(equalsMethod)).Returns(true);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(getHashCodeMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(getHashCodeMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(getTypeMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(getTypeMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(finalizeMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(finalizeMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(memberwiseCloneMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(memberwiseCloneMethod)).Returns(false);

            #endregion Arrange

            var actual = _selector.SelectMethodsForInjection(type);

            Assert.Equal(new[]
                            {
                                getVisibleMethod,
                                doMethodNoArgs,
                                doMethodStringArg,
                                executeMethodStringArg,
                                executeMethodStringAndInt32Arg,
                                equalsMethod
                            },
                        actual);
        }

        [Fact]
        public void SelectMethodsForInjection_InjectNonPublicIsFalse_ShouldReturnPublicAndNonPublicInstanceMethodsThatAreEligibleForInjection()
        {
            #region Arrange

            var type = typeof(MyService);
            var getEnabledMethod = type.GetMethod("get_Enabled", BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var setStopMethod = type.GetMethod("set_Stop", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(bool) }, Array.Empty<ParameterModifier>());
            var getVisibleMethod = type.GetMethod("get_Visible", BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var setVisibleMethod = type.GetMethod("set_Visible", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(bool) }, Array.Empty<ParameterModifier>());
            var executeMethodNoArgs = type.GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var executeMethodStringArg = type.GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(string) }, Array.Empty<ParameterModifier>());
            var executeMethodStringAndInt32Arg = type.GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(string), typeof(int) }, Array.Empty<ParameterModifier>());
            var toStringMethod = type.GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var equalsMethod = type.GetMethod("Equals", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(object) }, Array.Empty<ParameterModifier>());
            var getHashCodeMethod = type.GetMethod("GetHashCode", BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var getTypeMethod = type.GetMethod("GetType", BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var memberwiseCloneMethod = type.GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());

            _settingsMock.Setup(p => p.InjectNonPublic).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(getEnabledMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(getEnabledMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(setStopMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(setStopMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(getVisibleMethod)).Returns(true);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(setVisibleMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(setVisibleMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(executeMethodNoArgs)).Returns(true);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(executeMethodStringArg)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(executeMethodStringArg)).Returns(true);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(executeMethodStringAndInt32Arg)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(executeMethodStringAndInt32Arg)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(toStringMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(toStringMethod)).Returns(true);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(equalsMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(equalsMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(getHashCodeMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(getHashCodeMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(getTypeMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(getTypeMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(memberwiseCloneMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(memberwiseCloneMethod)).Returns(false);

            #endregion Arrange

            var actual = _selector.SelectMethodsForInjection(type);

            Assert.Equal(new[]
                            {
                                getVisibleMethod,
                                executeMethodNoArgs,
                                executeMethodStringArg,
                                toStringMethod
                            },
                        actual);
        }

        [Fact]
        public void SelectPropertiesForInjection_ShouldThrowArgumentNullExceptionWhenTypeIsNull()
        {
            const Type type = null;

            var actual = Assert.Throws<ArgumentNullException>(() => _selector.SelectPropertiesForInjection(type));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(type), actual.ParamName);
        }

        [Fact]
        public void SelectPropertiesForInjection_InjectNonPublicIsTrueAndInjectParentPrivatePropertiesIsFalse_ShouldReturnPublicAndNonPublicInstanceMethodsThatAreEligibleForInjection()
        {
            #region Arrange

            var type = typeof(MyService);
            var weaponProperty = type.GetProperty("Weapon", BindingFlags.NonPublic | BindingFlags.Instance);
            var idProperty = type.GetProperty("Id", BindingFlags.NonPublic | BindingFlags.Instance);
            var nameProperty = type.GetProperty("Name", BindingFlags.NonPublic | BindingFlags.Instance);
            var activeProperty = type.GetProperty("Active", BindingFlags.Public | BindingFlags.Instance);
            var enabledProperty = type.GetProperty("Enabled", BindingFlags.Public | BindingFlags.Instance);
            var visibleProperty = type.GetProperty("Visible", BindingFlags.Public | BindingFlags.Instance);
            var stopProperty = type.GetProperty("Stop", BindingFlags.Public | BindingFlags.Instance);

            _settingsMock.InSequence(_sequence)
                         .Setup(p => p.InjectNonPublic)
                         .Returns(true);
            _injectionHeuristicMock1.InSequence(_sequence)
                                    .Setup(p => p.ShouldInject(weaponProperty))
                                    .Returns(true);
            _injectionHeuristicMock1.InSequence(_sequence)
                                    .Setup(p => p.ShouldInject(idProperty))
                                    .Returns(false);
            _injectionHeuristicMock2.InSequence(_sequence)
                                    .Setup(p => p.ShouldInject(idProperty))
                                    .Returns(false);
            _injectionHeuristicMock1.InSequence(_sequence)
                                    .Setup(p => p.ShouldInject(nameProperty))
                                    .Returns(false);
            _injectionHeuristicMock2.InSequence(_sequence)
                                    .Setup(p => p.ShouldInject(nameProperty))
                                    .Returns(false);
            _injectionHeuristicMock1.InSequence(_sequence)
                                    .Setup(p => p.ShouldInject(enabledProperty))
                                    .Returns(false);
            _injectionHeuristicMock2.InSequence(_sequence)
                                    .Setup(p => p.ShouldInject(enabledProperty))
                                    .Returns(false);
            _injectionHeuristicMock1.InSequence(_sequence)
                                    .Setup(p => p.ShouldInject(stopProperty))
                                    .Returns(true);
            _injectionHeuristicMock1.InSequence(_sequence)
                                    .Setup(p => p.ShouldInject(visibleProperty))
                                    .Returns(false);
            _injectionHeuristicMock2.InSequence(_sequence)
                                    .Setup(p => p.ShouldInject(visibleProperty))
                                    .Returns(false);
            _settingsMock.InSequence(_sequence)
                         .Setup(p => p.InjectParentPrivateProperties)
                         .Returns(false);

            #endregion Arrange

            var actual = _selector.SelectPropertiesForInjection(type);

            Assert.Equal(new[]
                            {
                                weaponProperty,
                                stopProperty
                            },
                        actual);
        }

        [Fact]
        public void SelectPropertiesForInjection_InjectNonPublicIsFalseAndInjectParentPrivatePropertiesIsFalse_ShouldReturnPublicPropertiesDeclaradOnSpecifiedTypeThatAreEligibleForInjection()
        {
            #region Arrange

            var type = typeof(MyService);
            var enabledProperty = type.GetProperty("Enabled", BindingFlags.Public | BindingFlags.Instance);
            var visibleProperty = type.GetProperty("Visible", BindingFlags.Public | BindingFlags.Instance);
            var stopProperty = type.GetProperty("Stop", BindingFlags.Public | BindingFlags.Instance);

            _settingsMock.InSequence(_sequence)
                         .Setup(p => p.InjectNonPublic)
                         .Returns(false);
            _injectionHeuristicMock1.InSequence(_sequence)
                                    .Setup(p => p.ShouldInject(enabledProperty))
                                    .Returns(false);
            _injectionHeuristicMock2.InSequence(_sequence)
                                    .Setup(p => p.ShouldInject(enabledProperty))
                                    .Returns(false);
            _injectionHeuristicMock1.InSequence(_sequence)
                                    .Setup(p => p.ShouldInject(visibleProperty))
                                    .Returns(false);
            _injectionHeuristicMock2.InSequence(_sequence)
                                    .Setup(p => p.ShouldInject(visibleProperty))
                                    .Returns(true);
            _injectionHeuristicMock1.InSequence(_sequence)
                                    .Setup(p => p.ShouldInject(stopProperty))
                                    .Returns(true);
            _settingsMock.InSequence(_sequence)
                         .Setup(p => p.InjectParentPrivateProperties)
                         .Returns(false);

            #endregion Arrange

            var actual = _selector.SelectPropertiesForInjection(type);

            Assert.Equal(new[]
                            {
                                visibleProperty,
                                stopProperty
                            },
                        actual);
        }

        public class MyService
        {
            static MyService()
            {
            }

            private MyService(string name)
            {
            }

            protected MyService(int iterations)
            {
            }

            protected internal MyService(int iterations, string name)
            {
            }

            internal MyService(string name, int iterations)
            {
            }

            public MyService(Type type)
            {
            }

            public MyService(Type type, string name)
            {
            }

            private IWeapon Weapon { get; set; }

            protected int Id
            {
                set { }
            }

            internal string Name { get; }

            public bool Enabled { get; }

            public bool Stop
            {
                set { }
            }

            public bool Visible { get; set; }

            public static string ExecuteCount { get; set; }

            private void Do()
            {
            }

            protected void Do(string action)
            {
            }

            internal void Do(string action, int iterations)
            {
            }

            public void Execute()
            {
            }

            public void Execute(string name)
            {
            }

            public void Execute(string name, int iterations)
            {
            }

            public static MyService Create()
            {
                return new MyService(5);
            }

            private static MyService Create(int iterations)
            {
                return new MyService(iterations);
            }
        }

        public abstract class ServiceBase
        {
            public bool Active { get; set; }
        }


        public static class MyFactory
        {
            public static MyService Create()
            {
                return new MyService(typeof(string));
            }
        }

        private delegate void MyDelegate();
    }
}
