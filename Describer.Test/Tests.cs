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
        public void First()
        {
            Assert.IsNotNull(new TestClass().ToString());
        }

        private class TestClass
        {
            public string FirstProp { get; set; }

            public TestClass()
            {
                FirstProp = "first";
            }

            public override string ToString()
            {
                return DescriptionBuilder.Describe(this,
                        o => o.FirstProp
                    );
            }
        }
    }
}
