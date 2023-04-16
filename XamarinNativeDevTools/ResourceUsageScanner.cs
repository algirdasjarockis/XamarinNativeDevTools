using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XamarinNativeDevTools
{
    public class ResourceUsageScanner
    {
        private const string AndroidEntryFile = "MainApplication.cs";
        private const string IosEntryFile = "AppDelegate.cs";

        public ResourceUsageScanner() { }

        public void ScanSolutionDirectory(string rootDir)
        {
            var includes = new string[] { AndroidEntryFile, IosEntryFile };

            var files = Directory.EnumerateFiles(rootDir, "", SearchOption.AllDirectories)
                .Where(file => includes.Any(file.Contains))
                .ToArray();

            string? androidProjectDir = null;
            string? iosProjectDir = null;

            try
            {
                androidProjectDir = Path.GetDirectoryName(files[0].EndsWith(AndroidEntryFile) ? files[0] : files[1]);
                iosProjectDir = Path.GetDirectoryName(files[0].EndsWith(IosEntryFile) ? files[0] : files[1]);
            }
            catch (IndexOutOfRangeException) 
            {
                Console.WriteLine($"[Warning] Not both platform projects exist. Android: {androidProjectDir}, iOS: {iosProjectDir}");
            }
            finally
            {
                if (!string.IsNullOrEmpty(androidProjectDir))
                    ;// ScanAndroidProject(androidProjectDir);

                if (!string.IsNullOrEmpty(iosProjectDir))
                    ScanIosProject(iosProjectDir);
            }
        }

        private void ScanAndroidProject(string dir)
        {
            var skipResources = new string[] { "colors", "dimens", "strings", "styles" };
            var noUsages = new FileUsageService().ScanForZeroUsages(
                ScanForAndroidResources(dir)
                    .Select(path => Path.GetFileNameWithoutExtension(path))
                    .Where(name => !skipResources.Any(name.Equals))
                    .ToHashSet()
                    .ToArray(),
                ScanForAndroidDirectories(dir).ToArray(),
                new string[] { ".xml", ".cs", ".axml" }
            );

            if (noUsages.Any())
            {
                Console.WriteLine("Elements with no usage were found in Android project:");
                foreach (var item in noUsages)
                    Console.WriteLine($" - {item}");
            }
         }

        private void ScanIosProject(string dir)
        {
            var noUsages = new FileUsageService().ScanForZeroUsages(
                ScanForIosResources(dir)
                    .Select(path => Path.GetFileNameWithoutExtension(path))
                    .Select(path => path.Replace("@2x", "").Replace("@3x", ""))
                    .ToHashSet()
                    .ToArray(),
                ScanForIosDirectories(dir).ToArray(),
                new string[] { ".xml", ".cs", ".xib" }
            );

            if (noUsages.Any())
            {
                Console.WriteLine("Elements with no usage were found in iOS project:");
                foreach (var item in noUsages)
                    Console.WriteLine($" - {item}");
            }
        }

        private IEnumerable<string> ScanForAndroidResources(string directory)
        {
            var includes = new string[] { ".axml", ".xml", ".png" };
            var files = Directory.EnumerateFiles($"{directory}/Resources", "", SearchOption.AllDirectories)
                .Where(file => !file.Contains(".idea"))
                .Where(file => includes.Any(file.EndsWith));

            return files;
        }

        private IEnumerable<string> ScanForAndroidDirectories(string directory)
        {
            var excludes = new string[] { @"\obj", @"/obj", @"\bin", @"/bin" };

            var files = Directory.EnumerateDirectories($"{directory}", "", SearchOption.AllDirectories)
                .Where(file => !excludes.Any(file.Contains))
                .Where(file => !file.Contains(".idea"));

            return files;
        }

        private IEnumerable<string> ScanForIosResources(string directory)
        {
            var includes = new string[] { ".png" };
            var files = Directory.EnumerateFiles($"{directory}/Resources", "", SearchOption.AllDirectories)
                .Where(file => includes.Any(file.EndsWith));

            return files;
        }

        private IEnumerable<string> ScanForIosDirectories(string directory)
        {
            var excludes = new string[] { @"\obj", @"/obj", @"\bin", @"/bin" };

            var files = Directory.EnumerateDirectories($"{directory}", "", SearchOption.AllDirectories)
                .Where(file => !excludes.Any(file.Contains))
                .Where(file => !file.Contains(".idea"));

            return files;
        }
    }
}
