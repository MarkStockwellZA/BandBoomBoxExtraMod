namespace MediumHardMod;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;
using static ReplaceNotes;

public static class Program
{
    public static async Task Main(string[] args)
    {
         BenchmarkRunner.Run<TheApp>();

         //var a = new TheApp();
         //a.RunIt();
    }
}

[MemoryDiagnoser]
public class TheApp
{
    private bool _overwriteExistingCharts;

    [Benchmark]
    public void RunIt()
    {
        var before = DateTime.Now;
        _overwriteExistingCharts = true;
        ProcessFolder("/home/mark/example_input");
        var after = DateTime.Now;
        var duration = after - before;
        Console.WriteLine($"Finished processing in {duration.TotalMilliseconds} milliseconds");
    }

    private void ProcessFolder(string folder)
    {
        foreach (var subFolder in Directory.GetDirectories(folder))
            ProcessFolder(subFolder);

        Parallel.ForEach(new DirectoryInfo(folder).GetFiles("*.sjson"), inputFile =>
        {
            _ = ProcessFile(inputFile);
            //Output.WriteLine(!success, message);
        });

        // foreach (var inputFile in new DirectoryInfo(folder).GetFiles("*.sjson"))
        // {
        //     _ = ProcessFile(inputFile);
        //     //Output.WriteLine(!success, message);
        // }
    }

    private bool ProcessFile(FileInfo inputFile)
    {
        var songText = File.ReadAllText(inputFile.FullName);
        var song = JsonSerializer.Deserialize<Song>(songText); //SongJsonContext.Default.Song

        if (song is null)
            return false;

        if (song.SongCharts.Count == 0)
            return false;

        var mediumChart = song.SongCharts.FirstOrDefault(x => x.Difficulty == (int) Difficulty.Medium);

        if (mediumChart is null)
            return false;

        if (mediumChart.Notes is not { Length: > 0 })
            return false;

        if (song.HasChart(Difficulty.Extra) && !_overwriteExistingCharts)
            return false;

        AddExtraDifficulty(song, mediumChart);

        var result = JsonSerializer.Serialize(song, typeof(Song));//SongJsonContext.Default
        File.WriteAllText(inputFile.FullName, result);

        return true;
    }
}