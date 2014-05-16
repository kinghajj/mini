namespace Mini.UnitTests
{
    using System;
    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    public class IniPatternsTest
    {
        private const string IniDocument = @"[Section]
FirstKey=FirstValue
SecondKey= SecondValue
ThirdKey =ThirdValue
FourthKey = FourthValue
 FifthKey=FifthValue";

        //private IniPatterns _iniPatterns;

        [SetUp]
        public void Setup()
        {
            var memoryStream = CreateIniMemoryStream();
            //_iniPatterns = new IniPatterns(new StreamReader(memoryStream));
        }

        [TearDown]
        public void Teardown()
        {
            //_iniPatterns = null;
        }

        private MemoryStream CreateIniMemoryStream()
        {
            var memStream = new MemoryStream();
            var stringBytes = System.Text.Encoding.UTF8.GetBytes(IniDocument);
            memStream.Write(stringBytes, 0, stringBytes.Length);
            // Set the stream position to the beginning so we can read from it.
            memStream.Seek(0, SeekOrigin.Begin);

            return memStream;
        }

        [Test]
        public void Test()
        {
            //foreach (var result in _iniPatterns)
            //{
            //    Assert.That(result.Name, Is.Not.Null);
            //}
        }
    }
}