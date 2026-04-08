namespace MediumHardMod;
using System.Collections.Immutable;

public record SongChart
(
    string? Group,
    int Difficulty,
    int DifficultyLevel,
    ImmutableArray<string> Notes
);