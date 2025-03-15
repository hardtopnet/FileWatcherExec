namespace HardTopNet.FileWatcherExec
{
    internal class Program
    {
        // TODO
        // add file filter
        // implement recursive watch

        static void Main(string[] args)
        {
            string directoryPath = null;
            string programPath = null;
            bool recursive = false;
            string programArgumentFormat = null;
            bool closePreviousInstance = true;
            string[] fileFilters = null;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "/d":
                        if (i + 1 < args.Length)
                        {
                            directoryPath = args[++i].TrimEnd('.').TrimEnd('\\');
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
                    case "/f":
                        if (i + 1 < args.Length)
                        {
                            fileFilters = args[++i].Split(';');
                        }
                        break;
                }
            }

            if (string.IsNullOrEmpty(directoryPath))
            {
                Console.WriteLine("Usage: FileWatcherExec /d <directoryPath> [/p <programPath>] [/r] [/c] [/a <arg format>] [/f <file filters>]");
                Console.WriteLine("/d directory to watch");
                Console.WriteLine("/p path to the program to execute. Omit to use default Windows associations");
                Console.WriteLine("/r recursive watch");
                Console.WriteLine("/c close previous running instance of program");
                Console.WriteLine("/a format of the argument to execute program. Omit to use file name as argument");
                Console.WriteLine("\tenter the format between quotes. The filename placeholder is $");
                Console.WriteLine("\tdon't add quotes around $, and if you need quotes in the argument, double them");
                Console.WriteLine("/f file filters to watch, separated by ;. Supports ? and * wildcards");
                return;
            }

            var watcher = new FileWatcher(directoryPath)
            {
                programPath = programPath,
                recursive = recursive,
                format = programArgumentFormat,
                closePreviousInstance = closePreviousInstance,
                fileFilters = fileFilters,
            };
            watcher.Start();
        }
    }
}
