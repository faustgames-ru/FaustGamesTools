using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ContentBuilder
{
    class Program
    {
        private const string SubfoldersFlag = "-subfolders";
        private static string[] HelpMessage =
        {
            "",
            $"Hi!",
            "",
            $"to convert single file:",
            $"   ConentBuilder ../source_folder/single_file.fbx ../target_folder",
            "",
            $"to convert all *.fbx files in folder:",
            $"   ConentBuilder ../source_folder/*.fbx ../target_folder",
            "",
            $"to include subfolders:",
            $"   ConentBuilder ../source_folder/*.fbx ../target_folder {SubfoldersFlag}",
            "",
            $"Good Luck Dude!",
            "",
        };

        static void DisplayHelp()
        {
            foreach (var line in HelpMessage)
            {
                Console.WriteLine(line);
            }
        }

        static bool ValidateArgs(string[] args)
        {
            if (args == null)
                return false;
            if (args.Length < 2)
                return false;
            if (args.Length > 3)
                return false;
            if (args.Length == 3)
            {
                if (args[2].ToLower() != SubfoldersFlag)
                    return false;
            }
            return true;
        }

        static void CreateTargetFolder(string targetFolder)
        {
            Directory.CreateDirectory(targetFolder);
        }

        static string GetTaregetFile(string file, string targetFolder, string newExt)
        {
            return Path.Combine(targetFolder, Path.GetFileNameWithoutExtension(file) + newExt);
        }

        static void ReportFile(string file, string targetFolder, string newExt)
        {
            var shortSource = file.Remove(0, BaseDir.Length);
            var target = GetTaregetFile(file, targetFolder, newExt);
            var shortTarget = target.Remove(0, BaseDir.Length);
            Console.WriteLine($"{Path.GetFileName(file)}->{Path.GetFileNameWithoutExtension(file) + newExt}");
        }

        static void ProcessFbxFile(string file, string targetFolder)
        {
            var ext = ".mesh";
            ReportFile(file, targetFolder, ext);
            ConvertFbxFile(file, GetTaregetFile(file, targetFolder, ext));
        }

        static void ConvertFbxFile(string file, string target)
        {
            var converter = new ConverterFbx();
            converter.Convert(file, target);
        }

        static void ProcessFile(string file, string targetFolder)
        {
            var ext = Path.GetExtension(file).ToLower();
            switch (ext)
            {
                case ".fbx":
                    ProcessFbxFile(file, targetFolder);
                    break;
            }
        }

        static void Process(string sourcePattern, string targetFolder, bool subfolders)
        {
            var directory = Path.GetDirectoryName(sourcePattern);
            if (!Directory.Exists(directory))
            {
                Console.WriteLine($"unable to find directory: \"{directory}\"");
                return;
            }
            CreateTargetFolder(targetFolder);

            var filePattern = Path.GetFileName(sourcePattern);
            var files = Directory.GetFiles(directory, filePattern);
            foreach (var file in files)
            {
                ProcessFile(file, targetFolder);
            }
            if (subfolders)
            {
                var sourceDirectories = Directory.GetDirectories(directory);
                foreach (var dir in sourceDirectories)
                {
                    var directoryName = Path.GetFileName(dir);
                    Process(Path.Combine(dir, filePattern), Path.Combine(targetFolder, directoryName), subfolders);
                }
            }
        }

        [DllImport("shlwapi", EntryPoint = "PathCanonicalize")]
        private static extern bool PathCanonicalize(StringBuilder lpszDst, string lpszSrc);

        private static string BaseDir;

        static void Main(string[] args)
        {
            if (!ValidateArgs(args))
            {
                DisplayHelp();
                Console.ReadKey();
                return;
            }

            BaseDir = AppDomain.CurrentDomain.BaseDirectory;
            bool subfolders = args.Length == 3;
            var targetFolder = Path.Combine(BaseDir, args[1].Replace('/', '\\'));
            Process(Path.Combine(BaseDir, args[0].Replace('/', '\\')), targetFolder, subfolders);
            Console.ReadKey();
        }
    }
}
