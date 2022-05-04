using BenchmarkDotNet.Attributes;
using Ninject.Selection;
using Ninject.Selection.Heuristics;
using Ninject.Tests.Fakes;

namespace Ninject.Benchmarks.Selection
{
    [MemoryDiagnoser]
    public class SelectorBenchmark
    {
        private NinjectSettings _settingsInjectNonPublicIsTrueAndInjectParentPrivatePropertiesIsTrue;
        private Selector _selectorInjectNonPublicIsTrueAndInjectParentPrivatePropertiesIsTrue;
        private NinjectSettings _settingsInjectNonPublicIsTrueAndInjectParentPrivatePropertiesIsFalse;
        private Selector _selectorInjectNonPublicIsTrueAndInjectParentPrivatePropertiesIsFalse;
        private NinjectSettings _settingsInjectNonPublicIsFalseAndInjectParentPrivatePropertiesIsTrue;
        private Selector _selectorInjectNonPublicIsFalseAndInjectParentPrivatePropertiesIsTrue;
        private NinjectSettings _settingsInjectNonPublicIsFalseAndInjectParentPrivatePropertiesIsFalse;
        private Selector _selectorInjectNonPublicIsFalseAndInjectParentPrivatePropertiesIsFalse;

        public SelectorBenchmark()
        {
        }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _settingsInjectNonPublicIsTrueAndInjectParentPrivatePropertiesIsTrue = new NinjectSettings
            {
                InjectNonPublic = true,
                InjectParentPrivateProperties = true
            };
            _selectorInjectNonPublicIsTrueAndInjectParentPrivatePropertiesIsTrue = new Selector(
                new[]
                {
                    new StandardInjectionHeuristic
                    {
                        Settings = _settingsInjectNonPublicIsTrueAndInjectParentPrivatePropertiesIsTrue
                    }
                })
            {
                Settings = _settingsInjectNonPublicIsTrueAndInjectParentPrivatePropertiesIsTrue
            };

            _settingsInjectNonPublicIsTrueAndInjectParentPrivatePropertiesIsFalse = new NinjectSettings
            {
                InjectNonPublic = true,
                InjectParentPrivateProperties = false
            };
            _selectorInjectNonPublicIsTrueAndInjectParentPrivatePropertiesIsFalse = new Selector(
                new[]
                {
                    new StandardInjectionHeuristic
                    {
                        Settings = _settingsInjectNonPublicIsTrueAndInjectParentPrivatePropertiesIsFalse
                    }
                })
            {
                Settings = _settingsInjectNonPublicIsTrueAndInjectParentPrivatePropertiesIsFalse
            };

            _settingsInjectNonPublicIsFalseAndInjectParentPrivatePropertiesIsTrue = new NinjectSettings
            {
                InjectNonPublic = false,
                InjectParentPrivateProperties = true
            };
            _selectorInjectNonPublicIsFalseAndInjectParentPrivatePropertiesIsTrue = new Selector(
                new[]
                {
                    new StandardInjectionHeuristic
                    {
                        Settings =_settingsInjectNonPublicIsFalseAndInjectParentPrivatePropertiesIsTrue
                    }
                })
            {
                Settings = _settingsInjectNonPublicIsFalseAndInjectParentPrivatePropertiesIsTrue
            };

            _settingsInjectNonPublicIsFalseAndInjectParentPrivatePropertiesIsFalse = new NinjectSettings
            {
                InjectNonPublic = false,
                InjectParentPrivateProperties = false
            };
            _selectorInjectNonPublicIsFalseAndInjectParentPrivatePropertiesIsFalse = new Selector(
                new[]
                {
                    new StandardInjectionHeuristic
                    {
                        Settings=_settingsInjectNonPublicIsFalseAndInjectParentPrivatePropertiesIsFalse
                    }
                })
            {
                Settings = _settingsInjectNonPublicIsFalseAndInjectParentPrivatePropertiesIsFalse
            };
        }

        [Benchmark]
        public void Constructor()
        {
            new Selector(new[]
            {
                new StandardInjectionHeuristic
                {
                    Settings = _settingsInjectNonPublicIsFalseAndInjectParentPrivatePropertiesIsFalse
                }
            })
            {
                Settings = _settingsInjectNonPublicIsFalseAndInjectParentPrivatePropertiesIsFalse
            };
        }

        [Benchmark]
        public void SelectConstructorsForInjection_InjectNonPublicIsTrue()
        {
            var constructors = _selectorInjectNonPublicIsTrueAndInjectParentPrivatePropertiesIsFalse.SelectConstructorsForInjection(typeof(MyService));
            foreach (var constructor in constructors)
            {
                if (constructor == null)
                    break;
            }
        }

        [Benchmark]
        public void SelectConstructorsForInjection_InjectNonPublicIsFalse()
        {
            var constructors = _selectorInjectNonPublicIsFalseAndInjectParentPrivatePropertiesIsFalse.SelectConstructorsForInjection(typeof(MyService));
            foreach (var constructor in constructors)
            {
                if (constructor == null)
                    break;
            }
        }

        [Benchmark]
        public void SelectMethodsForInjection_InjectNonPublicIsTrue()
        {
            var methods = _selectorInjectNonPublicIsTrueAndInjectParentPrivatePropertiesIsFalse.SelectMethodsForInjection(typeof(MyService));
            foreach (var method in methods)
            {
                if (method == null)
                    break;
            }
        }

        [Benchmark]
        public void SelectMethodsForInjection_InjectNonPublicIsFalse()
        {
            var methods = _selectorInjectNonPublicIsFalseAndInjectParentPrivatePropertiesIsFalse.SelectMethodsForInjection(typeof(MyService));
            foreach (var method in methods)
            {
                if (method == null)
                    break;
            }
        }

        [Benchmark]
        public void SelectPropertiesForInjection_InjectNonPublicIsTrueAndInjectParentPrivatePropertiesIsTrue()
        {
            var properties = _selectorInjectNonPublicIsTrueAndInjectParentPrivatePropertiesIsTrue.SelectPropertiesForInjection(typeof(MyService));
            foreach (var property in properties)
            {
                if (property == null)
                    break;
            }
        }

        [Benchmark]
        public void SelectPropertiesForInjection_InjectNonPublicIsTrueAndInjectParentPrivatePropertiesIsFalse()
        {
            var properties = _selectorInjectNonPublicIsTrueAndInjectParentPrivatePropertiesIsFalse.SelectPropertiesForInjection(typeof(MyService));
            foreach (var property in properties)
            {
                if (property == null)
                    break;
            }
        }

        [Benchmark]
        public void SelectPropertiesForInjection_InjectNonPublicIsFalseAndInjectParentPrivatePropertiesIsTrue()
        {
            var properties = _selectorInjectNonPublicIsFalseAndInjectParentPrivatePropertiesIsTrue.SelectPropertiesForInjection(typeof(MyService));
            foreach (var property in properties)
            {
                if (property == null)
                    break;
            }
        }

        [Benchmark]
        public void SelectPropertiesForInjection_InjectNonPublicIsFalseAndInjectParentPrivatePropertiesIsFalse()
        {
            var properties = _selectorInjectNonPublicIsFalseAndInjectParentPrivatePropertiesIsFalse.SelectPropertiesForInjection(typeof(MyService));
            foreach (var property in properties)
            {
                if (property == null)
                    break;
            }
        }

        public class MyService : MyServiceBase
        {
            public MyService(IWarrior warrior) : base(1)
            {
            }

            public MyService(IWeapon weapon) : base(1)
            {
            }

            private MyService(string name) : base(1)
            {
            }

            [Inject]
            public string Name { get; set; }

            [Inject]
            private IWarrior Warrior { get; set; }

            [Inject]
            public void AddWeapon(IWeapon weapon)
            {
            }

            [Inject]
            public void AddHuman(IHuman human)
            {
            }
        }

        public class MyServiceBase
        {
            protected MyServiceBase(int id)
            {
            }

            [Inject]
            private int Id { get; set; }

            [Inject]
            public void AddWarrior(IWarrior warrior)
            {
            }
        }

    }
}
