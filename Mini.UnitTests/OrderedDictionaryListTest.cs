using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Mini.UnitTests
{
    [TestClass]
    public class OrderedDictionaryListTest
    {
        [TestMethod]
        public void CollectionTValueTest()
        {
            var odl1 = new OrderedDictionaryList<string, int>();

            odl1["one"] = 1;
            odl1["two"] = 2;
            odl1.AddUnkeyed(3);
            Assert.IsTrue(((ICollection<int>)odl1).Contains(1));
            Assert.IsTrue(((ICollection<int>)odl1).Contains(2));
            Assert.IsTrue(((ICollection<int>)odl1).Contains(3));

            odl1["two2"] = 2;
            Assert.IsTrue(((ICollection<int>)odl1).Remove(2));
            Assert.IsFalse(((ICollection<int>)odl1).Contains(2));
        }
    }
}
