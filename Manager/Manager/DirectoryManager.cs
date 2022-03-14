using System;
using System.IO;

namespace FileMenedger
{
    public static class DirectoryManager
    {
        // Method allows you to copy file to any directory.
        public static void CopyFileToDir(string newDir, string file)
        {
            try
            {
                File.Copy(Path.Combine(Directory.GetCurrentDirectory(), file), Path.Combine(newDir, file), false);
                Console.WriteLine($"Файл скопирован в директорию:{Path.Combine(newDir, file)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        // Method allows you to remove file to any directory.
        public static void RemoveFileToNewDir(string newDir, string file)
        {
            try
            {
                if (Directory.Exists(newDir))
                {
                    File.Copy(Path.Combine(Directory.GetCurrentDirectory(), file), Path.Combine(newDir, file), false);
                    Console.WriteLine($"Файл скопирован в директорию:{Path.Combine(newDir, file)}");
                    FileInfo temp_file = new FileInfo(file);
                    temp_file.Delete();
                }
                else
                    Console.WriteLine("Такой директории нет.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // Creation a new directory.
        public static void CreateDirectory(string NameDir)
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
                bool flag = true;
                // Checking if the same folder exists.
                foreach (var name in dirInfo.GetDirectories())
                {
                    if (name.Name == NameDir)
                    {
                        flag = false;
                        break;
                    }
                }

                if (flag)
                {
                    if (!dirInfo.Exists)
                    {
                        dirInfo.Create();
                    }

                    dirInfo.CreateSubdirectory(NameDir);
                    Console.WriteLine($"Вы успешно создали папку {NameDir}");
                }
                else
                {
                    Console.WriteLine("Директория с таким именем уже существует.");
                }
            }
            catch
            {
                // Checking for connection to the local disks.
                CheckDiskActivity();
            }
        }

        // Checking for connection to the current disk.
        private static void CheckDiskActivity()
        {
            try
            {
                Directory.GetCurrentDirectory();
            }
            catch
            {
                Console.WriteLine(
                    "Текущая директория перестала быть доступной.\nПожалуйста, попробуйте воспользоваться командой change disk, чтобы сменить диск и начать работу с директориями заново. ");
            }
        }

        // Method allows you to delete directory.
        public static void DeleteDirectory(string nameDir)
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(nameDir);
                dirInfo.Delete(true);
                Console.WriteLine("Директория со всем содержимым удалена.");
            }
            catch
            {
                Console.WriteLine("Проверьте ввод. Директории с подобным именем нет или она занята другой программой.");
            }
        }

        // The following method realize the movement to the parent directory.
        public static void GoParentDirectory(string currentDir)
        {
            try
            {
                DriveInfo[] dis = DriveInfo.GetDrives();
                bool flag = true;
                // Checking if current path is not disk.
                foreach (DriveInfo di in dis)
                {
                    if (Convert.ToString(di) == currentDir)
                    {
                        flag = false;
                    }
                }

                if (flag)
                {
                    Environment.CurrentDirectory = Directory.GetParent(currentDir).FullName;
                }
                else
                {
                    Console.WriteLine("Опускаться ниже некуда!");
                }
            }
            catch
            {
                CheckDiskActivity();
            }
        }

        // The following method realize the movement to the specified directory.
        public static void GoSpecifiedDirectory(string input)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());
                bool flag = false;

                // Checking if path exists.
                foreach (var item in dir.GetDirectories())
                {
                    if (item.Name == input)
                    {
                        Environment.CurrentDirectory = Convert.ToString(item);
                        flag = true;
                        break;
                    }
                }

                if (!flag)
                {
                    Console.WriteLine("Такой директории нет!");
                }
            }
            catch
            {
                CheckDiskActivity();
            }
        }
        
        // Checking for the correct command for cd.
        public static void RequestHandlerCd(string[] arr_input)
        {
            if (arr_input.Length == 2)
            {
                if (arr_input[1] == "..")
                {
                    GoParentDirectory(Directory.GetCurrentDirectory());
                }
                else
                {
                    GoSpecifiedDirectory(arr_input[1]);
                }
            }
            else if (arr_input.Length > 3)
            {
                string s = "";
                for (int i = 1; i < arr_input.Length; ++i)
                {
                    s += arr_input[i];
                }
            }
        }
        
        // Method allows you to change disk to another if it is ready and available.
        public static void ChangeDisk(string newDisk)
        {
            DriveInfo[] dis = DriveInfo.GetDrives();
            bool flag = false;

            foreach (var disk in dis)
            {
                // Check for availability.
                if (disk.Name == newDisk && disk.IsReady)
                {
                    Environment.CurrentDirectory = newDisk;
                    flag = true;
                    Console.WriteLine($"Вы успешно перешли на диск {newDisk}");
                    break;
                }
            }

            if (!flag)
            {
                Console.WriteLine(
                    "Диска не существует или вы ввели неправильное название! Попробуйте ввести команду снова!");
            }
        }
        
        // Method shows folders and files in current directory.
        public static void ShowFilesAndFolders()
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());
                int count = 0;
                foreach (var item in dir.GetDirectories())
                {
                    Console.WriteLine($"DIR - {item.Name}");
                    count++;
                }

                foreach (var item in dir.GetFiles())
                {
                    Console.WriteLine($"FILE - {item.Name}");
                    count++;
                }

                if (count == 0)
                {
                    Console.WriteLine("В текущей директории нет файлов и папок.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        
        // Method shows available disks.
        public static void GetDiskNames()
        {
            DriveInfo[] dis = DriveInfo.GetDrives();
            try
            {
                foreach (DriveInfo di in dis)
                {
                    if (di.IsReady)
                        Console.WriteLine("Диск {0} имеется в системе и его тип {1}.", di.Name, di.DriveType);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.GetType().Name);
            }
        }
    }
}