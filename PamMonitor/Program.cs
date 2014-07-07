using System;
using System.IO;
using System.Security.Permissions;


namespace PamMonitor
{
    class Program
    {
        System.Collections.Hashtable map = new System.Collections.Hashtable();

        static void Main(string[] args)
        {
            Run();
        }
        // See http://msdn.microsoft.com/en-us/library/system.io.filesystemwatcher.changed(v=VS.90).aspx?appId=Dev10IDEF1&l=EN-US&k=k(FILESYSTEMWATCHER1_CHANGED);k(FILESYSTEMWATCHER);k(TargetFrameworkMoniker-".NETFRAMEWORK&k=VERSION=V3.5");k(DevLang-CSHARP)&rd=true
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void Run()
        {
            string[] args = System.Environment.GetCommandLineArgs();

            // If a directory is not specified, exit program.
            if (args.Length != 2)
            {
                // Display the proper way to call the program.
                Console.WriteLine("Usage: PamMonitor.exe (directory)");
                Console.WriteLine(" Monitora fino a 15000 files multi-livello.");
                return;
            }

            
            // Recursive find all directories
            string path = args[1];
            Program p = (new Program());

            Console.WriteLine("Analyzing...");
            p.recursiveWatch(path);
            p.enableAll();
            
            // Wait for the user to quit the program.
            Console.WriteLine(p.map.Count+" dirs monitored. Press \'q\' to quit the sample.");
            while (Console.Read() != 'q') ;
            p = null;
        }

        private  void enableAll()
        {
            lock (this)
            {
                Console.WriteLine("Warm up..." + map.Count);
                foreach (FileSystemWatcher watcher in this.map.Values)
                {
                    try
                    {
                        if (!watcher.EnableRaisingEvents)
                        {
                            watcher.EnableRaisingEvents = true;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("enabled failed:" + e.ToString());
                    }
                }
            }
        }

        public FileSystemWatcher recursiveWatch(string path)
        {
            var w = watchDirectory(path);
            map.Add(path, w);
            try
            {
                var subdirectories = Directory.GetDirectories(path);
            
                foreach (var d in subdirectories)
                {

                    if (map.Count > 150000)
                    {
                        Console.WriteLine("Limit Reached Dir monitored:"+map.Count);
                        return w;
                    }
                    else
                    {
                        recursiveWatch(d);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error on:" + path + " " + e.ToString());
                
            }
            return w;
        }

        private  FileSystemWatcher watchDirectory(string path)
        {
            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path;
            /* Watch for changes in LastAccess and LastWrite times, and
               the renaming of files or directories. */
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName
                /* GG-ADDED:*/
               | NotifyFilters.Size | NotifyFilters.CreationTime;
            // Watch Everything
            watcher.Filter = "*.*";

            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);

            // Begin watching.
            //watcher.EnableRaisingEvents = true;
            //Console.WriteLine("Monitoring:" + path);
            return watcher;
        }

        // Define the event handlers.
        private  void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            Console.WriteLine(e.ChangeType+";" + e.FullPath );
            checkDir(e.FullPath);
        }

        private  void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            //Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
            Console.WriteLine("MV;{0};{1}", e.OldFullPath, e.FullPath);
            checkDir(e.FullPath);
        }

        private void checkDir(string path)
        {
            if (Directory.Exists(path))
            {
                if (!map.ContainsKey(path))
                {
                    lock (this)
                    {
                        Console.WriteLine("New dir..." + path + " updating...");
                        this.recursiveWatch(path);
                    }
                    enableAll();
                }
            }
        }

    }

}
