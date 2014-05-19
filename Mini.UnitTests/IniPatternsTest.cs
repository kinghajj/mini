using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mini.UnitTests
{
    [TestClass]
    public class IniPatternsTest
    {
        [TestMethod]
        public void IniSettingKeyWhitespaceParseTest()
        {
            const string testDocument =
                @"[Section]
                FirstKey=FirstValue
                SecondKey= SecondValue
                ThirdKey =ThirdValue
                FourthKey = FourthValue
                 FifthKey=FifthValue
                Sixth Key = Sixth Value";

            using (var iniMemoryStream = new MemoryStream(Encoding.UTF8.GetBytes(testDocument)))
            {
                var document = new IniDocument(new StreamReader(iniMemoryStream));
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
}
