using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sparrow.Video.Entities;

namespace Sparrow.Video.Tests
{
    [TestClass]
    public class FileMetaTests
    {
        [TestMethod]
        public void From_PathToVideo_ValidMeta()
        {
            string expExtension = "mp4";
            string expName = "tHE3rrLdPmWw_stmwrlCIOxWvFNSeNevnkx7wb6lgss";
            string expRootPath = @"C:\Users\aleks\Downloads";

            var meta = FileMeta.From(@"C:\Users\aleks\Downloads\tHE3rrLdPmWw_stmwrlCIOxWvFNSeNevnkx7wb6lgss.mp4");

            Assert.AreEqual(expExtension, meta.Extension);
            Assert.AreEqual(expName, meta.Name);
            Assert.AreEqual(expRootPath, meta.RootPath);
        }

        [TestMethod]
        public void From_NullOrEmptyValue_ArgumentException()
        {
            //Assert.ThrowsException<ArgumentException>(() => FileMeta.From(null));
        }
    }
}
