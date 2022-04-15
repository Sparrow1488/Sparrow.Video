using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using Sparrow.Video.Entities;

namespace Sparrow.Video.Tests
{
    [TestClass]
    public class ConfigurationTests
    {
        [TestMethod]
        public void AddSrc_ValidVideoPath_SuccessAddedInCollection()
        {
            var config = new Configuration();
            var filePath = @"C:\Users\aleks\Videos\Desktop\Desktop 2021.12.08 - 16.38.02.03.mp4";
            string expectedFileName = "Desktop 2021.12.08 - 16.38.02.03";
            string expectedFileExtension = "mp4";
            string expectedRootPath = @"C:\Users\aleks\Videos\Desktop";

            config.AddSrc(filePath);
            var addedFile = config.Sources.FirstOrDefault();

            Assert.AreEqual(expectedFileName, addedFile.Name);
            Assert.AreEqual(expectedFileExtension, addedFile.Extension);
            Assert.AreEqual(expectedRootPath.Replace("/", "\\"), addedFile.RootPath);
        }

        [TestMethod]
        public void AddSrc_NotExistsFile_FileNotFoundException()
        {
            var config = new Configuration();
            Assert.ThrowsException<FileNotFoundException>(() => config.AddSrc(@"D:\games\ffmpg\video.vi"));
        }

        [TestMethod]
        public void SaveTo_NotDirectoryExistsPath_DirectoryNotFoundException()
        {
            var config = new Configuration();
            Assert.ThrowsException<DirectoryNotFoundException>(() => config.SaveTo(@"D:\gam42s", "name"));
        }

        [TestMethod]
        public void SaveTo_DirPathOrNameIsNullOrEmpty_ArgumentException()
        {
            var config = new Configuration();
            Assert.ThrowsException<ArgumentException>(() => config.SaveTo(null, ""));
        }

        [TestMethod]
        public void SaveTo_FilePath_ValidMetaFile()
        {
            var config = new Configuration();
            var expectedName = "[TEST]-Output-Video_File";
            var expectedDirPath = @"D:\games\ffmpeg";

            config.SaveTo(expectedName, expectedDirPath);
            var @out = config.OutputFile;

            Assert.AreEqual(expectedDirPath, @out.RootPath);
            Assert.AreEqual(expectedName, @out.Name);
        }

        [TestMethod]
        public void Quality_Null_ArgumentException()
        {
            var config = new Configuration();
            Assert.ThrowsException<ArgumentException>(() => config.Quality(null));
        }

        [TestMethod]
        public void SaveAs_Null_ArgumentException()
        {
            var config = new Configuration();
            Assert.ThrowsException<ArgumentException>(() => config.SaveAs(null));
        }

    }
}