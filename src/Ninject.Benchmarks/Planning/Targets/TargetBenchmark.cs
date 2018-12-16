using BenchmarkDotNet.Attributes;
using Ninject.Planning.Bindings;
using Ninject.Planning.Targets;
using Ninject.Tests.Fakes;
using System;
using System.Reflection;

namespace Ninject.Benchmarks.Planning.Targets
{
    [MemoryDiagnoser]
    public class TargetBenchmark
    {
        private Type _type;
        private ConstructorInfo _constructor;
        private ParameterInfo[] _parameters;
        private BindingMetadata _bindingMetadata;

        public TargetBenchmark()
        {
            _type = typeof(MyService);
            _constructor = _type.GetConstructor(new[] { typeof(IWeapon), typeof(IWeapon), typeof(IWeapon), typeof(IWeapon), typeof(IWeapon) });
            _parameters = _constructor.GetParameters();
            _bindingMetadata = new BindingMetadata
                {
                    Name = "A"
                };
        }

        [Benchmark]
        public void Constructor_NoConstraintAttribute()
        {
            new MyTarget(_constructor, _parameters[0], _parameters[0].Name, _type);
        }

        [Benchmark]
        public void Constructor_OneConstraintAttribute_Match()
        {
            new MyTarget(_constructor, _parameters[1], _parameters[1].Name, _type);
        }

        [Benchmark]
        public void Constructor_OneConstraintAttribute_NoMatch()
        {
            new MyTarget(_constructor, _parameters[2], _parameters[2].Name, _type);
        }

        [Benchmark]
        public void Constructor_MoreThanOneConstraintAttribute_AllMatch()
        {
            new MyTarget(_constructor, _parameters[3], _parameters[3].Name, _type);
        }

        [Benchmark]
        public void Constructor_MoreThanOneConstraintAttribute_NoMatch()
        {
            new MyTarget(_constructor, _parameters[4], _parameters[4].Name, _type);
        }

        [Benchmark]
        public void Constraint_NoConstraintAttribute()
        {
            var targetNoConstraintAttribute = new MyTarget(_constructor, _parameters[0], _parameters[0].Name, _type);
            var constraint = targetNoConstraintAttribute.Constraint;
            if (constraint != null)
            {
                throw new Exception();
            }
        }

        [Benchmark]
        public void Constraint_OneConstraintAttribute_NoMatch()
        {
            var targetOneConstraintAttributeNoMatch = new MyTarget(_constructor, _parameters[2], _parameters[2].Name, _type);
            var constraint = targetOneConstraintAttributeNoMatch.Constraint;
            if (constraint == null || constraint(_bindingMetadata))
            {
                throw new Exception();
            }
        }

        [Benchmark]
        public void Constraint_OneConstraintAttribute_Match()
        {
            var targetOneConstraintAttributeMatch = new MyTarget(_constructor, _parameters[1], _parameters[1].Name, _type);
            var constraint = targetOneConstraintAttributeMatch.Constraint;
            if (constraint == null || !constraint(_bindingMetadata))
            {
                throw new Exception();
            }
        }

        [Benchmark]
        public void Constraint_MoreThanOneConstraintAttributes_AllMatch()
        {
            var targetMoreThanOneConstraintAttributeAllMatch = new MyTarget(_constructor, _parameters[3], _parameters[3].Name, _type);
            var constraint = targetMoreThanOneConstraintAttributeAllMatch.Constraint;
            if (constraint == null || !constraint(_bindingMetadata))
            {
                throw new Exception();
            }
        }

        [Benchmark]
        public void Constraint_MoreThanOneConstraintAttributes_NoneMatch()
        {
            var targetMoreThanOneConstraintAttributeNoneMatch = new MyTarget(_constructor, _parameters[4], _parameters[4].Name, _type);
            var constraint = targetMoreThanOneConstraintAttributeNoneMatch.Constraint;
            if (constraint == null || constraint(_bindingMetadata))
            {
                throw new Exception();
            }
        }

        public class MyTarget : Target<ParameterInfo>
        {
            private string _name;
            private Type _type;

            public MyTarget(MemberInfo member, ParameterInfo site, string name, Type type)
                : base(member, site)
            {
                _name = name;
                _type = type;
            }

            public override string Name => _name;

            public override Type Type => _type;
        }

        public class MyService
        {
            public MyService(IWeapon weapon1,
                             [Named("A")]
                             IWeapon weapon2,
                             [Named("B")]
                             IWeapon weapon3,
                             [Named("A")]
                             [Named("A")]
                             IWeapon weapon4,
                             [Named("B")]
                             [Named("B")]
                             IWeapon weapon5)
            {
            }
        }
    }
}
