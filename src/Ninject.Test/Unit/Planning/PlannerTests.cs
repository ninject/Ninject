//-------------------------------------------------------------------------------
// <copyright file="ConstructorArgumentInBindingConfigurationBuilderTest.cs" company="Ninject Project Contributors">
//   Copyright (c) 2009-2013 Ninject Project Contributors
//   Authors: Ivan Appert (iappert@gmail.com)
//           
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   you may not use this file except in compliance with one of the Licenses.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//   or
//       http://www.microsoft.com/opensource/licenses.mspx
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//-------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Ninject.Planning;
using Ninject.Planning.Strategies;
using Xunit;

namespace Ninject.Test.Unit.Planning
{
    public class PlannerTests
    {
        private Mock<IPlanningStrategy> _strategyOneMock;
        private Mock<IPlanningStrategy> _strategyTwoMock;
        private IEnumerable<IPlanningStrategy> _strategies;
        private Planner _target;

        public PlannerTests()
        {
            _strategyOneMock = new Mock<IPlanningStrategy>(MockBehavior.Loose);
            _strategyTwoMock = new Mock<IPlanningStrategy>(MockBehavior.Loose);

            _strategies = new List<IPlanningStrategy>
                {
                    _strategyOneMock.Object,
                    _strategyTwoMock.Object,
                };
            _target = new Planner(_strategies);
        }

        [Fact]
        public void Ctor_ShouldThrowArgumentNullExceptionWhenStrategiesIsNull()
        {
            IEnumerable<IPlanningStrategy> strategies = null;

            var actualException = Assert.Throws<ArgumentNullException>(() => new Planner(strategies));

            Assert.Null(actualException.InnerException);
            Assert.Equal(nameof(strategies), actualException.ParamName);
        }

        [Fact]
        public void Ctor_ShouldCloneStrategies()
        {
            var actualStrategies = _target.Strategies;

            Assert.NotNull(actualStrategies);
            Assert.Equal(_strategies, actualStrategies);
            Assert.NotSame(_strategies, actualStrategies);
        }

        [Fact]
        public void GetPlan_ShouldThrowArgumentNullExceptionWhenTypeIsNull()
        {
            const Type type = null;

            var actualException = Assert.Throws<ArgumentNullException>(() => _target.GetPlan(type));

            Assert.Null(actualException.InnerException);
            Assert.Equal(nameof(type), actualException.ParamName);
        }

        /// <summary>
        /// We do not use a factory to create plans, so we can only verify whether a plan for a given type is created only
        /// once by checking whether the strategies have only been executed once for a plan.
        /// </summary>
        [Fact]
        public void GetPlan_ShouldCreatePlanOnceForTypeWhenNoPlanExistsForType()
        {
            const int threads = 25;
            const int iterationsPerThread = 50;
            var type = GetType();
            var startSignaled = new ManualResetEvent(false);

            var tasks = Enumerable.Range(1, threads).Select((_) =>
                {
                    // Wait for signal; this is done to increase likelyhood that more than one thread will
                    // attempt to create the signal concurrently
                    startSignaled.WaitOne();

                    return Task.Factory.StartNew(() =>
                        {
                            for (var i = 0; i < iterationsPerThread; i++)
                            {
                                _target.GetPlan(type);
                            }
                        });
                });

            startSignaled.Set();
            Task.WaitAll(tasks.ToArray());

            var actualPlan = _target.GetPlan(type);

            Assert.NotNull(actualPlan);
            Assert.Same(type, actualPlan.Type);
            Assert.Same(actualPlan, _target.GetPlan(type));

            _strategyOneMock.Verify(p => p.Execute(It.IsAny<Plan>()), Times.Once());
            _strategyOneMock.Verify(p => p.Execute(It.Is<Plan>((plan) => ReferenceEquals(plan, actualPlan))), Times.Once());
            _strategyTwoMock.Verify(p => p.Execute(It.IsAny<Plan>()), Times.Once());
            _strategyTwoMock.Verify(p => p.Execute(It.Is<Plan>((plan) => ReferenceEquals(plan, actualPlan))), Times.Once());
        }

        [Fact]
        public void GetPlan_ShouldCreatePlanForEachType()
        {
            var typeString = typeof(string);
            var typeInt32 = typeof(int);

            var planString = _target.GetPlan(typeString);

            _strategyOneMock.Verify(p => p.Execute(It.Is<Plan>((plan) => ReferenceEquals(plan, planString))), Times.Once());
            _strategyTwoMock.Verify(p => p.Execute(It.Is<Plan>((plan) => ReferenceEquals(plan, planString))), Times.Once());

            var planInt32 = _target.GetPlan(typeInt32);

            _strategyOneMock.Verify(p => p.Execute(It.Is<Plan>((plan) => ReferenceEquals(plan, planInt32))), Times.Once());
            _strategyTwoMock.Verify(p => p.Execute(It.Is<Plan>((plan) => ReferenceEquals(plan, planInt32))), Times.Once());

            _strategyOneMock.Verify(p => p.Execute(It.IsAny<Plan>()), Times.Exactly(2));
            _strategyTwoMock.Verify(p => p.Execute(It.IsAny<Plan>()), Times.Exactly(2));
        }

        [Fact]
        public void CreateEmptyPlan_ShouldThrowArgumentNullExceptionWhenTypeIsNull()
        {
            const Type type = null;
            var planner = new MyPlanner(_strategies);

            var actualException = Assert.Throws<ArgumentNullException>(() => planner.CreateEmptyPlan(type));

            Assert.Null(actualException.InnerException);
            Assert.Equal(nameof(type), actualException.ParamName);
        }

        public class MyPlanner : Planner
        {
            public MyPlanner(IEnumerable<IPlanningStrategy> strategies) : base(strategies)
            {
            }

            public new IPlan CreateEmptyPlan(Type type)
            {
                return base.CreateEmptyPlan(type);
            }
        }

    }
}
