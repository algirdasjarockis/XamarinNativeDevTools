using System.Xml.Linq;

namespace XamarinNativeDevTools
{
    public class FileUsageService
    {
        //
        // Scans for given usages and returns items which don't have any usage anywhere
        //
        // @param string[] needles      - phrases we looking for, example: "icon1" (formatted and prepared name, not full resource name)
        // @param string[] searchDirs   - directories where to look at, example: ["/full/path/to/Views", "/full/path/to/Viewmodels" ...]
        //
        // @return string[] - zero usage items
        //
        public List<string> ScanForZeroUsages(string[] needles, string[] searchDirs, string[] fileExtensions, string[] skipFilePatterns)
        {
            var filesMatched = new List<string>();
            var aggregated = needles.ToDictionary<string, string, int>(needle => needle, needle => 0);

            foreach (var dir in searchDirs) 
            {
                //Console.WriteLine($"Scanning dir: {dir} ...");
                var files = Directory
                    .EnumerateFiles(dir, "*", SearchOption.AllDirectories)
                    .Where(file => fileExtensions.Any(file.ToLower().EndsWith));

                if (skipFilePatterns.Length > 0)
                    files = files.Where(file => !skipFilePatterns.Any(file.Contains));

                foreach (var file in files)
                    foreach (var line in File.ReadLines(file))
                        foreach (var foundNeedle in needles.Where(needle => line.Contains(needle)))
                            aggregated[foundNeedle]++;
            }

            return aggregated.Where(pair => pair.Value == 0).Select(pair => pair.Key).ToList();
        }
    }
}