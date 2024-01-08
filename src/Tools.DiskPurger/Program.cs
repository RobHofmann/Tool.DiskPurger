using Dasync.Collections;

public class Program
{
    static async Task Main(string[] args)
    {
        var program = new Program();
        try
        {
            await program.RunAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.ToString());
        }
    }

    public async Task RunAsync()
    {
        int appThreads = 0;
        if (!int.TryParse(Environment.GetEnvironmentVariable("APP_THREADS"), out appThreads))
            throw new ArgumentException("Please define the environment variable APP_THREADS to define the number of file deletion threads.");
        int gracePeriodBeforeDeletingInSeconds = 0;
        if (!int.TryParse(Environment.GetEnvironmentVariable("GRACE_PERIOD_BEFORE_DELETING_IN_SECONDS"), out gracePeriodBeforeDeletingInSeconds))
            throw new ArgumentException("Please define the environment variable GRACE_PERIOD_BEFORE_DELETING_IN_SECONDS to define the number of days to keep as retention for your blobs.");
        string fileExclusionListString = Environment.GetEnvironmentVariable("EXCLUDE_FILES");
        string[] fileExclusionList = null;
        if (!string.IsNullOrWhiteSpace(fileExclusionListString))
            fileExclusionList = fileExclusionListString.Split(',');
        string baseDir = Environment.GetEnvironmentVariable("BASE_DIR");

        Console.WriteLine($"Hello, World! Tools.DiskPurger starting to delete files from {baseDir}.");
        Console.WriteLine($"Starting deletion...");
        await RunDeleteAsync(baseDir, appThreads, gracePeriodBeforeDeletingInSeconds, fileExclusionList);
        Console.WriteLine($"Finished deletion...");
    }

    public async Task RunDeleteAsync(string baseDir, int appThreads, int gracePeriodBeforeDeletingInSeconds, string[] fileExclusionList)
    {
        await Directory.EnumerateFiles(baseDir, "", SearchOption.AllDirectories).Select(x => new FileInfo(x)).Where(x => x.CreationTime < DateTime.Now.AddSeconds(gracePeriodBeforeDeletingInSeconds * -1)).ParallelForEachAsync(async file =>
        {
            if (fileExclusionList.Contains(file.FullName))
                return;

            Console.WriteLine($"Deleting {file.FullName}");
            var deleteResult = await DeleteFileAsync(baseDir, file);
            if (deleteResult)
                Console.WriteLine($"Succesfully deleted {file.FullName}");
            else
                Console.WriteLine($"Failed deleting {file.FullName}");
        }, maxDegreeOfParallelism: appThreads);
    }

    public async Task<bool> DeleteFileAsync(string baseDir, FileInfo file)
    {
        var fileName = file.FullName.Replace(baseDir + Path.DirectorySeparatorChar, "");
        Console.WriteLine($"Deleting {file.FullName}...");
        file.Delete();
        Console.WriteLine($"Deleted {file.FullName}...");
        return true;
    }
}