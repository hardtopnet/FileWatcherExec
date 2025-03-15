namespace FileWatcherExec
{
    internal class Program
    {
        // TODO
        // add file filter
        // implement argument format

        static void Main(string[] args)
        {
            string directoryPath = null;
            string programPath = null;
            bool recursive = false;
            string programArgumentFormat = null;
            bool closePreviousInstance = true;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "/d":
                        if (i + 1 < args.Length)
                        {
                            directoryPath = args[++i];
                        }
                        break;
                    case "/p":
                        if (i + 1 < args.Length)
                        {
                            programPath = args[++i];
                        }
                        break;
                    case "/r":
                        recursive = true;
                        break;
                    case "/c":
                        closePreviousInstance = true;
                        break;
                    case "/a":
                        if (i + 1 < args.Length)
                        {
                            programArgumentFormat = args[++i];
                        }
                        break;
                }
            }

            if (string.IsNullOrEmpty(directoryPath))
            {
                Console.WriteLine("Usage: FileWatcherExec /d <directoryPath> [/p <programPath>] [/r] [/c] [/a <arg format>]");
                Console.WriteLine("/d : directory to watch");
                Console.WriteLine("/p : path to the program to execute. Omit to use default Windows associations");
                Console.WriteLine("/r : recursive watch");
                Console.WriteLine("/c : close previous running instance of program");
                Console.WriteLine("/a : format of the argument to execute program. Omit to use file name as argument");
                return;
            }

            var watcher = new FileWatcher(directoryPath)
            {
                programPath = programPath,
                recursive = recursive,
                format = programArgumentFormat,
                closePreviousInstance = closePreviousInstance,
            };
            watcher.Start();
        }
    }
}
