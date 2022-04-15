using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Sparrow.Video.Tests
{
    [TestClass]
    public class FFMpegEditorTests
    {
        [TestMethod]
        public void Configure_NullObject_ArgumentNullException()
        {
            var editor = new FFMpegEditor(@"D:\games\ffmpeg\ffmpeg.exe");
            Assert.ThrowsException<ArgumentNullException>(() => editor.Configure(null));
        }

    }
}