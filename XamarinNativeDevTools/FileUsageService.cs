namespace XamarinNativeDevTools
{
    public class FileUsageService
    {
        //
        // Scans for given usages and returns items which don't have any usage anywhere
        //
        // @param string[] needles      - phrases we looking for, example: "icon1" (formatted and prepared name, not full resource name)
        // @param string[] haystacks    - directories where to look at, example: ["/full/path/to/Views", "/full/path/to/Viewmodels" ...]
        //
        // @return string[] - zero usage items
        //
        public List<string> ScanForZeroUsages(IEnumerable<string> needles, string[] haystacks, string[] fileExtensions)
        {
            var filesMatched = new List<string>();

            var aggregated = needles.ToDictionary<string, string, int>(needle => needle, needle => 0);

            foreach (var dir in haystacks) 
            {
                var files = Directory
                    .EnumerateFiles(dir)
                    .Where(file => fileExtensions.Any(file.ToLower().EndsWith))
                    .ToList();

                foreach (var file in files)
                    foreach (var line in File.ReadLines(file))
                        foreach (var foundNeedle in needles.Where(needle => line.Contains(needle)))
                            aggregated[foundNeedle]++;
            }

            return aggregated.Where(pair => pair.Value == 0).Select(pair => pair.Key).ToList();
        }
    }
}