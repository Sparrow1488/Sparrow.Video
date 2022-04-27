﻿using Serilog;
using Sparrow.Video;
using Sparrow.Video.Abstractions;
using Sparrow.Video.Enums;

internal class Program
{
    private static async Task Main()
    {
        Log.Information("Sparrow.Video started");

        var editor = CreateVideoEditor();

        Log.Information("Mode: Concationates videos");
        await editor.ConcatSourcesAsync(ConcatType.ReencodingConcatConvertedViaTransportStream);
        Log.Information("Completed");
    }

    private static IVideoEditor CreateVideoEditor()
    {
        string rootDirectory = "./";
        var files = Directory.GetFiles("./").ToList();
        Log.Information($"New files from directory \"{rootDirectory}\" ({files.Count})");
        var editor = new FFMpegEditor().Configure(config =>
                                    config.RestoreSrc()
                                          .AddDistinctSrcRange(files)
                                          .SaveTo("Compilation")
                                          .Loop(file => file.Analyse.GetVideo().Duration <= 13, 2)
                                          .Quality(VideoQuality.FHD));
        return editor;
    }

    private static void ConfigureLogger()
    {
        Log.Logger = new LoggerConfiguration()
              .MinimumLevel
              .Debug()
              .WriteTo.Console()
              .CreateLogger();
    }
}



