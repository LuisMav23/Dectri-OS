using Cosmos.System.FileSystem.Listing;
using System;
using System.Collections.Generic;
using Sys = Cosmos.System;

namespace Test_OS
{
    public class Kernel : Sys.Kernel
    {
        DirectoryEntry CurrentDirectory;
        Sys.FileSystem.CosmosVFS VFS;
        List<String> User = null;
        bool isLoggedon = false;

        protected override void BeforeRun()
        {
            Console.Clear();
            VFS = new Sys.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(VFS);
            CurrentDirectory = VFS.GetDirectory(@"0:\Root");
        }

        protected override void Run()
        {
            while (!isLoggedon)
            {
                Console.WriteLine("________ ___________________________________ ________   _______ ________");
                Console.WriteLine("___  __ \\___  ____/__  ____/___  __/___  __ \\____  _/   __  __ \\__  ___/");
                Console.WriteLine("__  / / /__  __/   _  /     __  /   __  /_/ / __  /     _  / / /_____ \\ ");
                Console.WriteLine("_  /_/ / _  /___   / /___   _  /    _  _, _/ __/ /      / /_/ / ____/ / ");
                Console.WriteLine("/_____/  /_____/   \\____/   /_/     /_/ |_|  /___/      \\____/  /____/  ");
                Console.WriteLine("\nUSER LOGIN");
                Console.Write("Username: ");
                var Name = Console.ReadLine();
                Console.Write("Password: ");
                var Password = Console.ReadLine();
                User = Security.User.userAuth(Name, Password);
                if (User != null)
                {
                    isLoggedon = true;
                    Console.Clear();
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("User does not exist.");
                }
                
            }
            Console.Write("< (" + User[0] + ") "+CurrentDirectory.mFullPath);
            Console.Write(">$ ");
            String input = Console.ReadLine();
            var inputStream = input.Split(" ");
            switch (inputStream[0])
            {
                case "echo":
                    TestOS_CLI.OS.echo(inputStream);
                    break;
                case "dir": TestOS_CLI.FileSystem.getDirList(CurrentDirectory, VFS);
                    break;
                case "mkdir":
                    TestOS_CLI.FileSystem.makeDir(CurrentDirectory, inputStream[1], VFS);
                    break;
                case "rmdir":
                    TestOS_CLI.FileSystem.deleteDir(CurrentDirectory, inputStream[1], VFS);
                    break;
                case "cd":
                    CurrentDirectory = TestOS_CLI.FileSystem.changeDir(CurrentDirectory, inputStream[1], VFS);
                    break;
                case "touch":
                    TestOS_CLI.FileSystem.makeFile(CurrentDirectory, inputStream[1], VFS);
                    break;
                case "shutdown":
                    TestOS_CLI.OS.shutdown();
                    break;
                case "reboot":
                    TestOS_CLI.OS.reboot();
                    break;
                case "cls":
                    Console.Clear();
                    break;
                case "color":
                    TestOS_CLI.OS.ChangeForegroundColor(inputStream[1]);
                    break;
                case "background":
                    TestOS_CLI.OS.ChangeBackgroundColor(inputStream[1]);
                    break;
                case "sysinfo":
                    TestOS_CLI.OS.displaySystem(VFS);
                    break;
                case "datetime":
                    TestOS_CLI.OS.displayDateTime();
                    break;
                default:
                    Console.WriteLine($"Cannot recognize {inputStream[0]} as a system command.");
                    break;
            }
        }
    }
}

namespace Security
{

    public static class User
    {
        private static List<List<String>> users = new List<List<String>>
        {
            new List<String> {"admin", "admin", "5"},
            new List<String> {"user", "12345", "3"}
        };
        public static List<String> userAuth(string name, string pass)
        {
            foreach (var user in users)
            {
                if (user[0] == name && user[1] == pass)
                {
                    return user;
                }
            }
            return null;

        }
    }
}

namespace TestOS_CLI
{
    public static class FileSystem
    {
        public static void getDirList(DirectoryEntry directory, Sys.FileSystem.CosmosVFS fs)
        {
            var dirList = fs.GetDirectoryListing(directory);
            Console.WriteLine("\nDirectory: " + directory.mName);
            Console.WriteLine("======================================");
            foreach (var dir in dirList)
            {
                Console.WriteLine("\t" + dir.mName);
            }
            Console.WriteLine("======================================\n");
        }

        public static void makeDir(DirectoryEntry directory, string input, Sys.FileSystem.CosmosVFS fs)
        {
            var dir = directory.mFullPath + "\\" + input;
            fs.CreateDirectory(dir);
            Console.WriteLine("Created " + fs.GetDirectory(dir).mName + ".");
        }

        public static void deleteDir(DirectoryEntry directory, string input, Sys.FileSystem.CosmosVFS fs)
        {
            var dirList = fs.GetDirectoryListing(directory);
            foreach (var dir in dirList)
            {
                if (dir.mName == input)
                {
                    var direc = fs.GetDirectory(directory + input);
                    fs.DeleteDirectory(dir);
                    Console.WriteLine("Destroyed " + dir.mName + ".");
                    return;
                }
            }
            Console.WriteLine("Directory does not exists.");
            return;

        }

        public static DirectoryEntry changeDir(DirectoryEntry directory, string input, Sys.FileSystem.CosmosVFS fs)
        {
            
            if (input == "..")
            {
                if (directory.mParent == null)
                {
                    Console.WriteLine("You are at the root directory.");
                    return directory;
                }
                else
                {
                    return directory.mParent;
                }
            }
            else
            {
                var dirList = fs.GetDirectoryListing(directory);
                foreach (var dir in dirList)
                {
                    if (dir.mName == input)
                    {
                        return fs.GetDirectory(directory.mFullPath + input);
                    }
                }
                Console.WriteLine("Directory does not exists.");    
                return directory;
            }
        }

        public static void makeFile(DirectoryEntry directory, string input, Sys.FileSystem.CosmosVFS fs)
        {
            var dir = directory.mFullPath + "\\" + input;
            fs.CreateFile(dir);
            Console.WriteLine("Created " + fs.GetFile(dir).mName + ".");
        }
    }

    public static class OS
    {
        public static void displaySystem(Sys.FileSystem.CosmosVFS fs)
        {
            Console.Clear();
            Console.WriteLine("============================================================================");
            Console.WriteLine("________ ___________________________________ ________   _______ ________");
            Console.WriteLine("___  __ \\___  ____/__  ____/___  __/___  __ \\____  _/   __  __ \\__  ___/");
            Console.WriteLine("__  / / /__  __/   _  /     __  /   __  /_/ / __  /     _  / / /_____ \\ ");
            Console.WriteLine("_  /_/ / _  /___   / /___   _  /    _  _, _/ __/ /      / /_/ / ____/ / ");
            Console.WriteLine("/_____/  /_____/   \\____/   /_/     /_/ |_|  /___/      \\____/  /____/  ");
            Console.WriteLine("============================================================================");
            Console.WriteLine("Kernel Version: DecTri 1.0.0");
            Console.WriteLine($"VFS Type:{fs.GetFileSystemType(@"0:\")}");
            Console.WriteLine($"Free Space:{fs.GetAvailableFreeSpace(@"0:\")}");
            Console.WriteLine("DecTri OS Team:");
            Console.WriteLine("- Calisay, Anne Krishane");
            Console.WriteLine("- Castro, Martin Vince");
            Console.WriteLine("- Eslabra, Jan Kristian");
            Console.WriteLine("- Gabriel, Luis Maverick");
            Console.WriteLine("- Labrador, Lain Ysabel");
            Console.WriteLine("- Nono, Ricardo");
            Console.WriteLine("- Santos, Cedrick");
            Console.WriteLine("============================================================================");
            Console.WriteLine("Press any key to go back...");
            Console.ReadKey();
            Console.Clear();
            return;

        }

        public static void echo(String[] stream)
        {
            for (int i = 1; i < stream.Length; i++)
            {
                Console.Write(stream[i]);
            }
        }

        public static void shutdown()
        {
            Console.WriteLine("Are you sure you want to shutdown? (Y/n)");
            String input = Console.ReadLine();
            if (input.ToLower() == "y")
            {
                Sys.Power.Shutdown();
                return;
            }
            return;
        }

        public static void reboot()
        {
            Console.WriteLine("Are you sure you want to reboot? (Y/n)");
            String input = Console.ReadLine();
            if (input.ToLower() == "y")
            {
                Sys.Power.Reboot();
                return;
            }
            return;
        }

        public static void ChangeForegroundColor(String color)
        {
            switch (color)
            {
                case "green":
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case "red":
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case "yellow":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case "d-green":
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
                case "d-red":
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case "blue":
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case "cyan":
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case "d-blue":
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    break;
                case "d-cyan":
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    break;
                case "d-yellow":
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case "gray":
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case "d-gray":
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                case "magenta":
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case "d-magenta":
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    break;
                case "white":
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case "default":
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case "help":
                    Console.WriteLine("To change the console text color, use this command 'color <insert color>'");
                    Console.WriteLine("Available Colors: ");
                    Console.WriteLine("-\tGREEN: green");
                    Console.WriteLine("-\tRED: red");
                    Console.WriteLine("-\tYELLOW: yellow");
                    Console.WriteLine("-\tDARK GREEN: d-green");
                    Console.WriteLine("-\tDARK RED: d-red");
                    Console.WriteLine("-\tBLUE: blue");
                    Console.WriteLine("-\tCYAN: cyan");
                    Console.WriteLine("-\tDARK BLUE: d-blue");
                    Console.WriteLine("-\tDARK CYAN: d-cyan");
                    Console.WriteLine("-\tDARK YELLOW: d-yellow");
                    Console.WriteLine("-\tGRAY: gray");
                    Console.WriteLine("-\tDARK GRAY: d-gray");
                    Console.WriteLine("-\tMAGENTA: magenta");
                    Console.WriteLine("-\tDARK MAGENTA: d-magenta");
                    Console.WriteLine("-\tWHITE: white");
                    break;
                default:
                    Console.WriteLine($"{color} is not recognized by the system. For more info type \'color help\'");
                    break;
            }
        }

        public static void ChangeBackgroundColor(string color)
        {
            switch (color)
            {
                case "green":
                    Console.BackgroundColor = ConsoleColor.Green;
                    break;
                case "red":
                    Console.BackgroundColor = ConsoleColor.Red;
                    break;
                case "yellow":
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    break;
                case "d-green":
                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                    break;
                case "d-red":
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    break;
                case "blue":
                    Console.BackgroundColor = ConsoleColor.Blue;
                    break;
                case "cyan":
                    Console.BackgroundColor = ConsoleColor.Cyan;
                    break;
                case "d-blue":
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    break;
                case "d-cyan":
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    break;
                case "d-yellow":
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    break;
                case "gray":
                    Console.BackgroundColor = ConsoleColor.Gray;
                    break;
                case "d-gray":
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    break;
                case "magenta":
                    Console.BackgroundColor = ConsoleColor.Magenta;
                    break;
                case "d-magenta":
                    Console.BackgroundColor = ConsoleColor.DarkMagenta;
                    break;
                case "white":
                    Console.BackgroundColor = ConsoleColor.White;
                    break;
                case "default":
                    Console.ResetColor();
                    break;
                case "help":
                    Console.WriteLine("To change the console background color, use this command 'bgcolor <insert color>'");
                    Console.WriteLine("Available Colors: ");
                    Console.WriteLine("-\tGREEN: green");
                    Console.WriteLine("-\tRED: red");
                    Console.WriteLine("-\tYELLOW: yellow");
                    Console.WriteLine("-\tDARK GREEN: d-green");
                    Console.WriteLine("-\tDARK RED: d-red");
                    Console.WriteLine("-\tBLUE: blue");
                    Console.WriteLine("-\tCYAN: cyan");
                    Console.WriteLine("-\tDARK BLUE: d-blue");
                    Console.WriteLine("-\tDARK CYAN: d-cyan");
                    Console.WriteLine("-\tDARK YELLOW: d-yellow");
                    Console.WriteLine("-\tGRAY: gray");
                    Console.WriteLine("-\tDARK GRAY: d-gray");
                    Console.WriteLine("-\tMAGENTA: magenta");
                    Console.WriteLine("-\tDARK MAGENTA: d-magenta");
                    Console.WriteLine("-\tWHITE: white");
                    break;
                default:
                    Console.WriteLine($"{color} is not recognized by the system. For more info type \'bgcolor help\'");
                    break;
            }
            Console.Clear();
        }

        public static void displayDateTime()
        {
            Console.WriteLine(DateTime.Now.ToLongDateString());
            Console.WriteLine(DateTime.Now.ToLongTimeString());
        }

    }
}

