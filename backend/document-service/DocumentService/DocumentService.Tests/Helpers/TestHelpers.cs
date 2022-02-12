using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Tests.Helpers
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
            Console.WriteLine($"GetFilePath: fileName={fileName} fileSize={fileSize}");

            var filePath = Path.Combine(_testDataDirectory, fileName);

            Console.WriteLine($"FilePath: {filePath}");

            using (StreamWriter sw = new StreamWriter(new FileStream(filePath, FileMode.Create)))
            {
                sw.Write(new char[fileSize]);
            }

            return filePath;
        }

        private static string GetTestDataDirectoryPath()
        {
            string workingDirectory = Environment.CurrentDirectory;
            Console.WriteLine($"CurrentDirectory: {workingDirectory}");

            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            Console.WriteLine($"projectDirectory: {projectDirectory}");

return projectDirectory;
           // return Path.Combine(projectDirectory, "TestData");
        }
    }
}
