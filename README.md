# Sparrow.Video

## Warning

**NOT SUPPORT YET**. Use [current](https://github.com/Sparrow1488/AutoShortcut) version.

## Introduction

Проект написан на основе открытой библиотеки FFMpeg, которая предоставляет возможности различной обработки видео и аудио. Данная библиотека решает задачу автоматического склеивания и предварительной обработки коротких видеороликов с последующим объединением в одну склейку.

## Simple to use

```C#
string rootDirectory = "./";
var files = Directory.GetFiles("./").ToList();
Log.Information($"New files from directory \"{rootDirectory}\" ({files.Count})");
var editor = new FFMpegEditor().Configure(config => 
                 config.RestoreSrc()
                 .AddDistinctSrcRange(files)
                 .SaveTo("Compilation")
                 .Loop(file => file.Analyse.GetVideo().Duration <= 13,2)
                 .Quality(VideoQuality.FHD));

await editor.ConcatSourcesAsync(ConcatType.ReencodingConcatConvertedViaTransportStream);
```

## Dependencies

* .NET 6
* Newtonsoft.Json → v13.0.1

## Roadmap

Решить задачу - хорошо. Но решить задачу чисто - еще лучше. Поэтому немного отдохнув от написания тонны ~~говно-кода~~ плохого кода, можно приступить к исправлению косяков и навести чистоту в коде.

* Refactoring
* Детальная настройка конфигурации редактора
* Конвейерная обработка файлов внутри редактора
