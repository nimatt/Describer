using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Assert.AreEqual(nameof(obj.FirstProp) + ": first", DescriptionBuilder.Describe(obj,
                        o => o.FirstProp
                    ));
        }

        [Test]
        public void Method()
        {
            var obj = new TestClass();
            Assert.AreEqual(nameof(obj.MyMethod) + "(): " + obj.MyMethod(), DescriptionBuilder.Describe(obj,
                        o => o.MyMethod()
                    ));
        }

        [Test]
        public void MethodOnPropertyOnProperty()
        {
            var obj = new TestClass();
            Assert.AreEqual(nameof(obj.FirstProp) + ": 5", DescriptionBuilder.Describe(obj,
                        o => o.FirstProp.Length.ToString()
                    ));
        }

        [Test]
        public void MethodOnPropertyOnMethodOnPropertyOnProperty()
        {
            var obj = new TestClass();
            Assert.AreEqual(nameof(obj.FirstProp) + ": 1", DescriptionBuilder.Describe(obj,
                        o => o.FirstProp.Length.ToString().Length.ToString()
                    ));
        }

        [Test]
        public void MethodWithParam()
        {
            var obj = new TestClass();
            Assert.AreEqual(nameof(obj.MethodWithParam) + "(): " + obj.MethodWithParam("foo"), DescriptionBuilder.Describe(obj,
                        o => o.MethodWithParam("foo")
                    ));
        }

        [Test]
        public void OuterFunction()
        {
            var obj = new TestClass();
            Assert.AreEqual(nameof(obj.NumProp) + ": " + obj.NumProp, DescriptionBuilder.Describe(obj,
                        o => NumToString(o.NumProp)
                    ));
        }

        private string NumToString(int val)
        {
            return val.ToString();
        }

        private class TestClass
        {
            public string FirstProp => "first";

            public int NumProp => 5;

            public string MyMethod()
            {
                return "Foo";
            }

            public string MethodWithParam(string val)
            {
                return val;
            }
        }
    }
}
