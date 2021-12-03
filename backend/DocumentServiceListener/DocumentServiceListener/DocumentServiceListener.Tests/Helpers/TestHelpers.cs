using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DocumentServiceListener.Tests.Helpers
{
    public static class TestHelpers
    {
        private static readonly string _testDataDirectory;

        static TestHelpers()
        {
            _testDataDirectory = GetTestDataDirectoryPath();
        }

        public static string GetFilePath(string fileName, int fileSize)
        {
            var filePath = Path.Combine(_testDataDirectory, fileName);

            using (StreamWriter sw = new StreamWriter(new FileStream(filePath, FileMode.Create)))
            {
                sw.Write(new char[fileSize]);
            }

            return filePath;
        }

        private static string GetTestDataDirectoryPath()
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;

            return Path.Combine(projectDirectory, "TestData");
        }
    }
}
