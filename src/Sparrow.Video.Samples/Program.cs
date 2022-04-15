using Serilog;
using Sparrow.Video;
using Sparrow.Video.Abstractions;
using Sparrow.Video.Entities;
using Sparrow.Video.Enums;

Log.Logger = new LoggerConfiguration()
              .MinimumLevel
              .Debug()
              .WriteTo.Console()
              .CreateLogger();

Log.Information("Sparrow.Video started");
args = new[] { @"E:\Йога\Source-Films-Makers\отдельно sfm\52\4_compilation", "new" };

string rootVideos = "";
PrepareEnvironment();
var editor = CreateVideoEditor(args);

Log.Information("Mode: Concationates videos");
await editor.ConcatSourcesAsync(ConcatType.ReencodingConcatConvertedViaTransportStream);
Log.Information("Completed");

IVideoEditor CreateVideoEditor(string[] args)
{
    var files = Directory.GetFiles(rootVideos).ToList();
    Log.Information($"New files from directory \"{rootVideos}\" ({files.Count})");
    var editor = new FFMpegEditor().Configure(config =>
                                config.RestoreSrc()
                                      .AddDistinctSrcRange(files)
                                      .SaveTo("Compilation")
                                      .Loop(file => file.Analyse.GetVideo().Duration <= 11, 2)
                                      .Quality(VideoQuality.FHD));
    return editor;
}

void PrepareEnvironment()
{
    string commands = string.Empty;
    foreach (var arg in args)
    {
        commands += arg + "___";
    }
    Log.Information("Input commands: " + commands);

    if (args.Length == 0)
    {
        Log.Information("Input args is empty");
    }
    else
    {
        rootVideos = args[0];
        Log.Information($"Processing directory path: \"{rootVideos}\"");
    }
    if (args.Length == 2)
    {
        if (args[1] == "new")
        {
            Project.CreateProject();
            Log.Information("Create new project");
        }
        else
        {
            Project.UseExistsProject(args[1]);
            Log.Information("Using exists project " + args[1]);
        }
    }
    else
    {
        Project.CreateProject();
        Log.Information("(NOT Specify) Create new project");
    }
}