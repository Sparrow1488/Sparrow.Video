using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sparrow.Video.Entities;
using Sparrow.Video.Enums;

namespace Sparrow.Video.Tests
{
    [TestClass]
    public class ScriptBuilderTests
    {
        public ScriptBuilderTests()
        {
            _builder = new ScriptBuilder();
            _config = new Configuration()
                          .AddSrc(_rootPath + "1.mp4")
                          .AddSrc(_rootPath + "2.mp4")
                          .SaveTo("result");
        }

        private string _rootPath = @"C:\Users\aleks\Downloads\test-videos-2\";
        private ScriptBuilder _builder;
        private Configuration _config;

        [TestMethod]
        public void Build_InpAndOutConfigurs_ValidScript()
        {
            var expected = $"-i \"{_rootPath}1.mp4\" -i \"{_rootPath}2.mp4\" \"{Paths.OutputFiles}/result.mp4\"".Replace('/', '\\');
            var result = _builder.Build(_config).Replace('/', '\\');

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Build_InpAndOutConfigursAndFormatter_ValidScript()
        {
            var expected = $"-i \"{Paths.Meta.Path}/combined-files".Replace('/', '\\');
            var result = _builder.Build(_config, format => 
                                format.CombineSourcesInTxt(_config.Sources)).Replace('/', '\\');

            StringAssert.Contains(result, expected);
            StringAssert.Contains(result, ".txt");
            StringAssert.Contains(result, _config.OutputFile.ToString());
        }

        [TestMethod]
        public void ConfigureInputs_InpAndOutConfigureAndChangedFormatter_ValidScript()
        {
            var builder = new ScriptBuilder();
            builder.ConfigureInputs(commands =>
            {
                commands.Add("-f concat");
                commands.Add("-safe 0");
            });
            var expected = $"-f concat -safe 0 -i \"{_rootPath}1.mp4\" -i \"{_rootPath}2.mp4\" \"{Paths.OutputFiles}/result.mp4\"".Replace('/', '\\');

            var result = builder.Build(_config).Replace('/', '\\');

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ConfigureOutputs_InpAndOutConfigureAndChangedFormatter_ValidScript()
        {
            var builder = new ScriptBuilder();
            builder.ConfigureOutputs(commands => {
                commands.Add("-c copy");
            });
            var expected = $"-i \"{_rootPath}1.mp4\" -i \"{_rootPath}2.mp4\" -c copy \"{Paths.OutputFiles}/result.mp4\"".Replace('/', '\\');

            var result = builder.Build(_config).Replace('/', '\\');

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void MoreMethods_NullValues_Exception()
        {
            var builder = new ScriptBuilder();
            Assert.ThrowsException<ArgumentNullException>(() => builder.Build(null, null));
            Assert.ThrowsException<ArgumentNullException>(() => builder.ChangeFormat(null));
            Assert.ThrowsException<ArgumentNullException>(() => builder.ConfigureInputs(null));
            Assert.ThrowsException<ArgumentNullException>(() => builder.ConfigureOutputs(null));
            Assert.ThrowsException<ArgumentNullException>(() => builder.ConfigureOutputs(null));
        }
    }
}
