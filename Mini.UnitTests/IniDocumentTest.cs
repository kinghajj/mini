using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mini.UnitTests
{
    [TestClass]
    public class IniDocumentTest
    {
        [TestMethod]
        public void SimpleEqualityTest()
        {
            var ini1 = new IniDocument();
            var ini2 = new IniDocument();
            var ini3 = new IniDocument();
            // the first two are constructed in the same order, and so are equal
            ini1["Hello"]["World"].Value = "Foo";
            ini1["Second"]["Thing"].Value = "What?";
            ini2["Hello"]["World"].Value = "Foo";
            ini2["Second"]["Thing"].Value = "What?";
            // the third has a flipped order, so it is not equal to the other two
            ini3["Second"]["Thing"].Value = "What?";
            ini3["Hello"]["World"].Value = "Foo";
            Assert.IsTrue(ini1.Equals(ini2));
            Assert.IsTrue(ini2.Equals(ini1));
            Assert.IsFalse(ini1.Equals(ini3));
            Assert.IsFalse(ini3.Equals(ini1));
            Assert.IsFalse(ini2.Equals(ini3));
            Assert.IsFalse(ini3.Equals(ini2));
        }

        [TestMethod]
        public void LargeEqualityTest()
        {
            var ini1 = new IniDocument();
            var ini2 = new IniDocument();

            foreach (var ini in new[] {ini1, ini2})
            {
                for (var i = 0; i < 1000; i++)
                {
                    var section = ini[string.Format("Section{0}", i)];
                    for (var j = 0; j < 1000; j++)
                    {
                        section[string.Format("Setting{0}", j)].Value = string.Format("Value{0}", j);
                    }
                }
            }

            Assert.IsTrue(ini1.Equals(ini2));
        }
    }
}
