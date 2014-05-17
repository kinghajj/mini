namespace Mini.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class IniPatternsTest
    {
        private const string IniDocument = 
@"[Section]
FirstKey=FirstValue
SecondKey= SecondValue
ThirdKey =ThirdValue
FourthKey = FourthValue
 FifthKey=FifthValue
Sixth Key = Sixth Value";

        private static MemoryStream _iniMemoryStream;
        private static IList<IniPattern> _patterns;

        /// <summary>
        /// Only set up the memory stream once per test class.
        /// </summary>
        [ClassInitialize]
        public static void Setup(TestContext testContext)
        {
            _iniMemoryStream = CreateIniMemoryStream();
            _patterns = new IniPatterns(new StreamReader(_iniMemoryStream)).ToList();
        }

        [ClassCleanup]
        public static void Teardown()
        {
            _patterns = null;

            if (_iniMemoryStream != null)
            {
                _iniMemoryStream.Close();
                _iniMemoryStream = null;
            }
        }

        private static MemoryStream CreateIniMemoryStream()
        {
            var memStream = new MemoryStream();
            var stringBytes = System.Text.Encoding.UTF8.GetBytes(IniDocument);
            memStream.Write(stringBytes, 0, stringBytes.Length);
            // Set the stream position to the beginning so we can read from it.
            memStream.Seek(0, SeekOrigin.Begin);

            return memStream;
        }

        [TestMethod]
        public void FirstSetting_NoWhiteSpace_ExpectResultNotNull()
        {
            // FirstKey=FirstValue
            Assert.IsNotNull(_patterns[0]);
        }

        [TestMethod]
        public void SecondSetting_ValueLeadingWhiteSpace_ExpectResultNotNull()
        {
            // SecondKey= SecondValue
            Assert.IsNotNull(_patterns[1]);
        }

        [TestMethod]
        public void ThirdSetting_KeyTrailingWhiteSpace_ExpectResultNotNull()
        {
            // ThirdKey =ThirdValue
            Assert.IsNotNull(_patterns[2]);
        }

        [TestMethod]
        public void FourthSetting_WhiteSpaceAroundEqualSign_ExpectResultNotNull()
        {
            // FourthKey = FourthValue
            Assert.IsNotNull(_patterns[3]);
        }

        [TestMethod]
        public void FifthSetting_KeyLeadingWhiteSpace_ExpectResultNotNull()
        {
            //  FifthKey=FifthValue
            Assert.IsNotNull(_patterns[4]);
        }

        [TestMethod]
        public void SixthSetting_WhiteSpaceEverywhere_ExpectResultNotNull()
        {
            // Sixth Key = Sixth Value
            Assert.IsNotNull(_patterns[5]);
        }
    }
}