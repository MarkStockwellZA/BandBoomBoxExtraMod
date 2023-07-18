﻿namespace MediumHardMod;

internal class Song
{
    public string? ID { get; set; }
    public string? Title { get; set; }
    public string? Subtitle { get; set; }
    public string? Artist { get; set; }
    public string? ChartAuthor { get; set; }
    public string? Issues { get; set; }
    public string? AudioFile { get; set; }
    public int Version { get; set; }
    public decimal Bpm { get; set; }
    public decimal AudioStart { get; set; }
    public decimal Offset { get; set; }
    public decimal Length { get; set; }
    public int BeatsPerMeasure { get; set; }
    public Dictionary<int, string>? Sections { get; set; }
    public List<SongChart>? SongCharts { get; set; }
}