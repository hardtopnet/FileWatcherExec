# FileWatcherExec

This program watches a folder for new files, then for every file found it starts a program with the new file path as argument.

I'm using this to watch Steam screenshots folders on a remote machine (either through mounted network folder or sync'ed folder with SyncThing).
For every new screenshot I'm triggering the default Windows viewer to display the screenshot.
This allows me to review every new screenshot to ensure I framed it correctly or there aren't any unwanted elements.

## Features
- Watch specified folder with recursive option.
- Specify filters to check for specific files.
- Specify a format to call the program with a custom argument. By default it uses the new file path as argument.
- Option to close the program's previous running instance before launching a new one.

## Usage

`FileWatcherExec /d <directoryPath> [/p <programPath>] [/r] [/c] [/a <arg format>] [/f <file filters>]`

- `/d` is the path to the folder to watch.
- `/p` is the path to the executable to run for every new file detected. If none specified, uses the default one in Windows file associations.
- `/r` recursively watch.
- `/c` close previous instance before launching executable.
- `/a` executable arguments format. If none specified, uses the new file full path as argument.
	- use `$` as a placeholder for the new file full path.
	- e.g. if the executable requires a `/file` switch to load a file, you can specify `/file=$`.
- `/f` is the file filter, only the files matching the filters will trigger the executable
	- the filter can contain wildcards `\*` and `?`, if specifying multiple filters use `;` as separator

> don't forget to use double quotes if any parameter contains spaces.

## Building

This program uses .NET 8

Simply use MSBuild with default parameters
`MSBuild.exe /t:rebuild /property:Configuration=Release`

To publish a single executable use the following
`dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true -o bin\Publish`

