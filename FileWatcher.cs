
using System.Diagnostics;

namespace FileWatcherExec
{
    internal class FileWatcher
    {
        internal bool recursive;
        internal string? format;
        internal string directoryPath;
        internal string programPath;
        internal bool closePreviousInstance;

        private static Dictionary<string, string> extensionProgramCache = new Dictionary<string, string>();

        public FileWatcher(string directoryPath)
        {
            this.directoryPath = directoryPath;
        }

        internal void Start()
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} Initialization");
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} Path to check: {directoryPath}");
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} Program to execute: {programPath}");
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} Recursive? {recursive}");
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} Close previous instance? {closePreviousInstance}");
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} Program argument format: {format}");

            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [FATAL] Directory does not exist: {directoryPath}");
                return;
            }

            if (string.IsNullOrEmpty(programPath))
            {
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} No program provided, using Windows default associations");
            }
            else if (!File.Exists(programPath))
            {
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [FATAL] Program does not exist: {programPath}");
                return;
            }

            Console.WriteLine($"{DateTime.Now:HH:mm:ss} Watching folder... Press 'q' to quit");

            var watcher = new FileSystemWatcher(directoryPath);
            watcher.EnableRaisingEvents = true;
            watcher.Created += OnCreated;

            while (Console.ReadKey(true).Key != ConsoleKey.Q) { }

            // exiting
            watcher.Created -= OnCreated;
            watcher.Dispose();

            Console.WriteLine($"{DateTime.Now:HH:mm:ss} Exiting");
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            var programToExecute = programPath;

            Console.WriteLine($"{DateTime.Now:HH:mm:ss} New file found: {e.Name}");

            if (programPath == null)
            {
                string extension = Path.GetExtension(e.FullPath);
                programToExecute = GetDefaultProgramForExtension(extension);

                if (programToExecute == null)
                {
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss} [ERROR] No default program found for extension: {extension}");
                    return;
                }
            }

            // Check if the program is already running and kill it if that's the case
            if (closePreviousInstance)
            {
                var runningProcesses = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(programToExecute));
                foreach (var runningProcess in runningProcesses)
                {
                    try
                    {
                        runningProcess.Kill();
                        runningProcess.WaitForExit();
                        Console.WriteLine($"{DateTime.Now:HH:mm:ss} Killed running instance of the program: {runningProcess.ProcessName}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{DateTime.Now:HH:mm:ss} [ERROR] Failed to kill process: {ex.Message}");
                    }
                }
            }

            string arguments = BuildArgs(e.FullPath);
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} Opening file with: {Path.GetFileName(programToExecute)} {arguments}");

            // Executing the program with the new file as an argument
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = programToExecute,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.Exited += (s, ev) =>
            {
                process.Dispose();
            };
        }

        private string BuildArgs(string fullPath)
        {
            if(fullPath.Contains(" "))
            {
                fullPath = $"\"{fullPath}\"";
            }

            if (string.IsNullOrEmpty(format))
            {
                return fullPath;
            }
            return format.Replace("$", fullPath);
        }

        private string? GetDefaultProgramForExtension(string extension)
        {
            if (extensionProgramCache.TryGetValue(extension, out var cachedProgram))
            {
                return cachedProgram;
            }

            string? defaultProgram = null;
            try
            {
                var progId = Microsoft.Win32.Registry.GetValue($@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts\{extension}\UserChoice", "Progid", null) as string;
                if (progId != null)
                {
                    defaultProgram = Microsoft.Win32.Registry.GetValue($@"HKEY_CLASSES_ROOT\{progId}\shell\open\command", "", null) as string;
                    if (defaultProgram != null)
                    {
                        if (defaultProgram.StartsWith("\""))
                        {
                            var parts = defaultProgram.Split('"');
                            if (parts.Length > 1)
                            {
                                defaultProgram = parts[1];
                            }
                        }
                        else
                        {
                            var parts = defaultProgram.Split(' ');
                            if (parts.Length > 0)
                            {
                                defaultProgram = parts[0];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [ERROR] Failed to get default program for extension {extension}: {ex.Message}");
            }

            extensionProgramCache[extension] = defaultProgram;
            return defaultProgram;
        }
    }
}