namespace MediumHardMod;
using System.Text.Json;
using static ReplaceNotes;

public static class Program
{
    private static bool _overwriteExistingCharts;
    public static void Main(string[] args)
    {
        if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]) || !Directory.Exists(args[0]))
        {
            Output.WriteLine(true, """ 
                Please supply a valid input directory as the first argument.
            
                Example:
                .\MediumHardMod C:\\BandBoomboxSongs
            """);
            return;
        }

        if (args.Contains("-Overwrite")) _overwriteExistingCharts = true;

        ProcessFolder(args[0]);
    }

    private static void ProcessFolder(string folder)
    {
        foreach (var subFolder in Directory.GetDirectories(folder))
            ProcessFolder(subFolder);

        foreach (var inputFile in new DirectoryInfo(folder).GetFiles("*.sjson"))
        {
            var (success, message) = ProcessFile(inputFile);
            Output.WriteLine(!success, message);
        }
    }

    private static (bool, string) ProcessFile(FileInfo inputFile)
    {
        var songText = File.ReadAllText(inputFile.FullName);
        var song = JsonSerializer.Deserialize(songText, SongJsonContext.Default.Song);

        if (song is null)
            return (false, $"{inputFile} is not a valid song file");

        if (song.SongCharts.Count == 0)
            return (false, $"{inputFile} does not contain any charts");

        var mediumChart = song.SongCharts.FirstOrDefault(x => x.Difficulty == (int) Difficulty.Medium);

        if (mediumChart is null)
            return (false, $"No medium song chart found in file \"{inputFile.Name}\"");

        if (mediumChart.Notes.Length == 0)
            return (false, $"No medium notes found in file \"{inputFile.Name}\"");

        if (song.HasChart(Difficulty.Extra) && !_overwriteExistingCharts)
            return (false, $"Extra chart already exists in file \"{inputFile.Name}\"");

        AddExtraDifficulty(song, mediumChart);

        var result = JsonSerializer.Serialize(song, typeof(Song), SongJsonContext.Default);
        File.WriteAllText(inputFile.FullName, result);

        return (true, $"Extra chart successfully created in \"{inputFile}\"");
    }
}