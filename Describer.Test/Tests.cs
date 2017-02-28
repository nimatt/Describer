using NUnit.Framework;

namespace Describer.Test
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void SimpleProperty()
        {
            var obj = new TestClass();
            Assert.AreEqual($"{{ {nameof(obj.FirstProp)}: first }}", DescriptionBuilder.Describe(obj,
                        o => o.FirstProp
                    ));
        }

        [Test]
        public void Method()
        {
            var obj = new TestClass();
            Assert.AreEqual($"{{ {nameof(obj.MyMethod)}(): {obj.MyMethod()} }}", DescriptionBuilder.Describe(obj,
                        o => o.MyMethod()
                    ));
        }

        [Test]
        public void MethodOnPropertyOnProperty()
        {
            var obj = new TestClass();
            Assert.AreEqual($"{{ {nameof(obj.FirstProp)}: 5 }}", DescriptionBuilder.Describe(obj,
                        o => o.FirstProp.Length.ToString()
                    ));
        }

        [Test]
        public void MethodOnPropertyOnMethodOnPropertyOnProperty()
        {
            var obj = new TestClass();
            Assert.AreEqual($"{{ {nameof(obj.FirstProp)}: 1 }}", DescriptionBuilder.Describe(obj,
                        o => o.FirstProp.Length.ToString().Length.ToString()
                    ));
        }

        [Test]
        public void MethodWithParam()
        {
            var obj = new TestClass();
            Assert.AreEqual($"{{ {nameof(obj.MethodWithParam)}(): {obj.MethodWithParam("foo")} }}", DescriptionBuilder.Describe(obj,
                        o => o.MethodWithParam("foo")
                    ));
        }

        [Test]
        public void OuterFunction()
        {
            var obj = new TestClass();
            Assert.AreEqual($"{{ {nameof(obj.NumProp)}: {obj.NumProp} }}", DescriptionBuilder.Describe(obj,
                        o => NumToString(o.NumProp)
                    ));
        }

        [Test]
        public void ObjectProperty()
        {
            var obj = new TestClass();
            var obj2 = new SecondTestClass();
            obj.ObjProp = obj2;
            Assert.AreEqual($"{{ {nameof(obj.ObjProp)}: {{ {nameof(obj2.SecNumProp)}: {obj2.SecNumProp} }} }}",
                DescriptionBuilder.Describe(obj,
                        o => DescriptionBuilder.Describe(o.ObjProp,
                                o2 => o2.SecNumProp.ToString()
                            )
                    ));
        }

        [Test]
        public void Addition()
        {
            var obj = new TestClass();
            Assert.AreEqual($"{{ {nameof(obj.NumProp)}: {obj.NumProp + obj.NumProp} }}",
                DescriptionBuilder.Describe(obj,
                        o => (o.NumProp + o.NumProp).ToString()
                    ));
        }

        [Test]
        public void AdditionDifferentConstant()
        {
            var obj = new TestClass();
            Assert.AreEqual($"{{ {nameof(obj.NumProp)}: {obj.NumProp + 2} }}",
                DescriptionBuilder.Describe(obj,
                        o => (o.NumProp + 2).ToString()
                    ));
        }

        [Test]
        public void AdditionDifferentProps()
        {
            var obj = new TestClass();
            Assert.AreEqual($"{{ {nameof(obj.NumProp)}, {nameof(obj.NumProp2)}: {obj.NumProp + obj.NumProp2} }}",
                DescriptionBuilder.Describe(obj,
                        o => (o.NumProp + o.NumProp2).ToString()
                    ));
        }

        [Test]
        public void MultiParameterMethod()
        {
            var obj = new TestClass();
            Assert.AreEqual($"{{ {nameof(obj.NumProp)}: {NumsToString(obj.NumProp, obj.NumProp)} }}",
                DescriptionBuilder.Describe(obj,
                        o => NumsToString(o.NumProp, o.NumProp)
                    ));
        }

        [Test]
        public void MultiParameterMethodDifferentProps()
        {
            var obj = new TestClass();
            Assert.AreEqual($"{{ {nameof(obj.NumProp)}, {nameof(obj.NumProp2)}: {NumsToString(obj.NumProp, obj.NumProp2)} }}",
                DescriptionBuilder.Describe(obj,
                        o => NumsToString(o.NumProp, o.NumProp2)
                    ));
        }

        [Test]
        public void ObjectMethodWithParam()
        {
            var obj = new TestClass();
            Assert.AreEqual($"{{ {nameof(obj.MethodWithParam)}(), {nameof(obj.NumProp)}: {obj.MethodWithParam(obj.NumProp.ToString())} }}",
                DescriptionBuilder.Describe(obj,
                        o => o.MethodWithParam(o.NumProp.ToString())
                    ));
        }

        [Test]
        public void ObjectMethodWithTwoParams()
        {
            var obj = new TestClass();
            Assert.AreEqual($"{{ {nameof(obj.MethodWithParam)}(), {nameof(obj.NumProp)}, {nameof(obj.NumProp2)}: {obj.MethodWithParam(obj.NumProp.ToString(), obj.NumProp2)} }}",
                DescriptionBuilder.Describe(obj,
                        o => o.MethodWithParam(o.NumProp.ToString(), o.NumProp2)
                    ));
        }

        [Test]
        public void ObjectMethodWithDifferentObjectParam()
        {
            var obj = new TestClass();
            var obj2 = new TestClass();
            Assert.AreEqual($"{{ {nameof(obj.MethodWithParam)}(): {obj.MethodWithParam(obj2.NumProp.ToString())} }}",
                DescriptionBuilder.Describe(obj,
                        o => o.MethodWithParam(obj2.NumProp.ToString())
                    ));
        }

        [Test]
        public void TwoProperties()
        {
            var obj = new TestClass();
            Assert.AreEqual($"{{ {nameof(obj.NumProp)}: {obj.NumProp}; {nameof(obj.NumProp2)}: {obj.NumProp2} }}",
                DescriptionBuilder.Describe(obj,
                        o => o.NumProp.ToString(),
                        o => o.NumProp2.ToString()
                    ));
        }

        private string NumToString(int val)
        {
            return val.ToString();
        }

        private string NumsToString(int x, int y)
        {
            return (x + y).ToString();
        }

        private class TestClass
        {
            public string FirstProp => "first";
            public int NumProp => 5;
            public int NumProp2 => 7;

            public SecondTestClass ObjProp = new SecondTestClass();

            public string MyMethod()
            {
                return "Foo";
            }

            public string MethodWithParam(string val)
            {
                return val;
            }

            public string MethodWithParam(string val, int num)
            {
                return val + num;
            }
        }

        private class SecondTestClass
        {
            public int SecNumProp = 2;
        }
    }
}
