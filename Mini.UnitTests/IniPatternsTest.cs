using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mini.UnitTests
{
    [TestClass]
    public class IniPatternsTest
    {
        [TestMethod]
        public void IniSettingKeyWhitespaceParseTest()
        {
            var document = new IniDocument("IniSettingKeyWhitespaceParseTestDocument.ini");
            var section = document["Section"];
            Assert.AreEqual(section["FirstKey"].Value,  "FirstValue");
            Assert.AreEqual(section["SecondKey"].Value, "SecondValue");
            Assert.AreEqual(section["ThirdKey"].Value,  "ThirdValue");
            Assert.AreEqual(section["FourthKey"].Value, "FourthValue");
            Assert.AreEqual(section["FifthKey"].Value,  "FifthValue");
            Assert.AreEqual(section["Sixth Key"].Value, "Sixth Value");
        }
    }
}
