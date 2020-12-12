using System.IO;
using System.ServiceProcess;
using System.Threading;

namespace LabDeCSharp
{

    public partial class Service1 : ServiceBase
    {
        FileWatcher watcher;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {

            watcher = new FileWatcher(new Manager().GetOptions());
            new Thread(new ThreadStart(watcher.Start)).Start();
        }

        protected override void OnStop()
        {
            watcher.Stop();
            Thread.Sleep(1000);
        }
    }

    class FileWatcher
    {
        FileSystemWatcher watcher;
        Options options;
        public string SourcePath => watcher.Path;
        private string copyPath;
        public string CopyPath 
        { 
            get => copyPath; 
            private set
            {
                if (!Directory.Exists(value))
                {
                    Directory.CreateDirectory(value);
                }
                copyPath = value;
            }
        }
        public FileWatcher(Options config)
        {
            options = config;
            if (!Directory.Exists(options.SourceDirectory))
            {
                Directory.CreateDirectory(options.SourceDirectory);
            }
            watcher = new FileSystemWatcher(options.SourceDirectory)
            {
                IncludeSubdirectories = true
            };
            watcher.Changed += FileChanged;
            watcher.Created += FileChanged;
            watcher.Deleted += FileChanged;
            watcher.Renamed += FileRenamed;
            CopyPath = options.TargetDirectory;
        }
        private string SubPath(string path) => path.Replace(SourcePath, "").Replace(CopyPath, "");
        private void Synchronize(string path)
        {
            string source = SourcePath + SubPath(path);
            string copy = CopyPath + SubPath(path);
            string archive = copy + ".gz";
            try
            {
                if (Directory.Exists(source))
                {
                    if (!Directory.Exists(copy))
                    {
                        Directory.CreateDirectory(copy);
                    }
                }
                if (!Directory.Exists(source))
                {
                    if (Directory.Exists(copy))
                    {
                        Directory.Delete(copy, true);
                    }
                }
                //if (File.Exists(copy))
                //{
                //    File.Delete(copy);
                //}
                if (File.Exists(archive))
                {
                    File.Delete(archive);
                }
                if (File.Exists(source))
                {
                    byte[] EncryptionKey = new byte[16] { 42, 42, 42, 42, 42, 42, 42, 42, 42, 42, 42, 42, 42, 42, 42, 42 };
                    byte[] EncryptionIV = new byte[16] { 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13 };
                    //File.Copy(source, copy);
                    File.WriteAllBytes(copy, Encryption.EncryptStringToBytes_Aes(
                        File.ReadAllText(source), EncryptionKey, EncryptionIV));
                    Archiving.Acrhive(copy);
                    File.Delete(copy);

                    Archiving.Unarchive(copy + ".gz");
                    Directory.CreateDirectory(Path.GetDirectoryName(options.ArchiveDirectory + SubPath(copy)));
                    File.WriteAllText(options.ArchiveDirectory + SubPath(copy),
                        Encryption.DecryptStringFromBytes_Aes(
                            File.ReadAllBytes(copy), EncryptionKey, EncryptionIV));
                    File.Delete(copy);
                }
            }
            catch { }
        }
        private void FileChanged(object sender, FileSystemEventArgs args)
        {
            Synchronize(args.FullPath);
        }
        private void FileRenamed(object sender, RenamedEventArgs args)
        {
            Synchronize(args.OldFullPath);
            Synchronize(args.FullPath);
        }
        public void CheckForNewFiles()
        {
            CheckAllNewFiles(SourcePath);
        }
        private void CheckAllNewFiles(string path)
        {
            foreach (var dir in Directory.EnumerateDirectories(path))
            {
                Synchronize(dir);
                CheckAllNewFiles(dir);
            }
            foreach (var file in Directory.EnumerateFiles(path))
            {
                Synchronize(file);
            }
        }
        public void CheckForDeletedFiles()
        {
            CheckAllDeletedFiles(CopyPath);
        }
        private void CheckAllDeletedFiles(string path)
        {
            foreach (var file in Directory.EnumerateFiles(path))
            {
                Synchronize(file);
            }
            foreach (var dir in Directory.EnumerateDirectories(path))
            {
                CheckAllDeletedFiles(dir);
                Synchronize(dir);
            }
        }

        public void Start()
        {
            CheckForDeletedFiles();
            CheckForNewFiles();
            watcher.EnableRaisingEvents = true;
        }
        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
        }
    }
}
