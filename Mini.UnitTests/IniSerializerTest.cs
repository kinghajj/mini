using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mini.UnitTests
{
    [TestClass]
    public class IniSerializerTest
    {
        [IniValueContainer]
        private class Example
        {
            [IniValue("Woot", "Foo", "Bar")]
            public string WootFoo { get; set; }
        }

        private readonly IniSerializer<Example> _exampleSerializer;

        public IniSerializerTest()
        {
            _exampleSerializer = IniSerializer<Example>.New();
        }

        [TestMethod]
        public void TestSerialization()
        {
            var example = new Example { WootFoo = "Baz" };
            var serialized = new IniDocument();
            _exampleSerializer.Serialize(example, serialized);
            Assert.AreEqual(example.WootFoo, serialized["Woot"]["Foo"].Value);
        }

        [TestMethod]
        public void TestDeserialization()
        {
            var ini = new IniDocument();
            ini["Woot"]["Foo"].Value = "Hello";
            var deserialized = _exampleSerializer.Deserialize(ini);
            Assert.AreEqual(ini["Woot"]["Foo"].Value, deserialized.WootFoo);
        }
    }
}