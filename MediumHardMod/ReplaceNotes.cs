using System.Text;
namespace MediumHardMod;

internal static class ReplaceNotes
{
    private static readonly Dictionary<char, char> _replacements = new()
    {
        ['4'] = 'D',
        ['5'] = 'E',
        ['7'] = 'A',
        ['8'] = 'B',
        ['D'] = '4',
        ['E'] = '5',
        ['A'] = '7',
        ['B'] = '8'
    };

    private static char ReplaceChar(char x) => _replacements.GetValueOrDefault(x, x);

    private static string? ReplaceSubNote(string subNote) => 
        $"{subNote[0]}{ReplaceChar(subNote[1])}{ReplaceChar(subNote[2])}{subNote[3]}";

    private static string ReplaceAllNotes(string note, ref bool replace)
    {
        var result = new StringBuilder();

        foreach (var subNote in note.Split(' ').AsSpan())
        {
            if (result.Length != 0) result.Append(' ');

            result.Append(replace ? ReplaceSubNote(subNote) : subNote);

            if (subNote != "0000") replace = !replace;
        }

        return result.ToString();
    }

    public static void AddExtraDifficulty(Song song, SongChart mediumChart)
    {
        ArgumentNullException.ThrowIfNull(song);
        ArgumentNullException.ThrowIfNull(mediumChart);

        SongChart extraChart = new
        (
            mediumChart.Group,
            (int) Difficulty.Extra,
            mediumChart.DifficultyLevel + 1,
            new string[mediumChart.Notes?.Length ?? 0]
        );

        // Remove any existing extra charts
        var existingCharts = song.SongCharts
            .Where(x => x.Difficulty == 10 && x.Group == extraChart.Group);
        
        foreach (var chart in existingCharts)
        {
            song.SongCharts.Remove(chart);
        }

        bool replace = true;

        for (int i = 0; i < mediumChart.Notes?.Length; i++)
        {
            var newNote = ReplaceAllNotes(mediumChart.Notes[i], ref replace);
            extraChart.Notes[i] = newNote;
        }

        song.SongCharts.Add(extraChart);
        song.SongCharts = song.SongCharts.OrderBy(x => x.DifficultyLevel).ToList();
    }
}
