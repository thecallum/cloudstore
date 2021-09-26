using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Tests.Helpers
{
    public static class TestHelpers
    {
        public static string CreateTestFile(string fileName, int fileSize)
        {
            var filePath = Path.Combine(GetTestDataDirectoryPath(), fileName);

            using (StreamWriter sw = new StreamWriter(new FileStream(filePath, FileMode.OpenOrCreate)))
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
