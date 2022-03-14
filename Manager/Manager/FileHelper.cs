using System;
using System.IO;
using System.Text;

namespace FileMenedger
{
    public static class FileHelper
    {
        
        public static void CreateNewTextFile(string file_1, string file_2, Encoding encoding, string input, bool flag)
        {
            if (!flag) 
            {
                string newFile = file_1 + "+" + file_2;
                using (FileStream file = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), newFile),
                           FileMode.CreateNew))
                {
                }

                File.AppendAllLines(Path.Combine(Directory.GetCurrentDirectory(), newFile),
                    File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), file_1)), encoding);
                File.AppendAllLines(Path.Combine(Directory.GetCurrentDirectory(), newFile),
                    File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), file_2)), encoding);
            }
            else
            {
                string fileName = input;
                try
                {
                    // Creates file and read text from console.
                    using (FileStream file = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), fileName),
                               FileMode.CreateNew))
                    {
                    }

                    using (StreamWriter sr = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), fileName),
                               true, encoding))
                    {
                        string s = null;
                        Console.WriteLine("Введите содержимое файла:");
                        while (true)
                        {
                            s = Console.ReadLine();
                            if (s == "stop")
                            {
                                break;
                            }

                            sr.WriteLine(s);
                        }
                    }

                    Console.WriteLine("Запись в файл выполнена.");
                    Console.WriteLine($"Кодировка файла - {GetEncoding(fileName)}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        
        // Concat two text files.
        public static void ConcatTwoFiles(string file_1, string file_2)
        {
            ReadTextFile(file_1);
            ReadTextFile(file_2);
            Encoding encoding = Encoding.UTF8;
            string fileName = file_1 + "+" + file_2;
            FileHelper.CreateNewTextFile(file_1, file_2, encoding, fileName, false);
        }

        // Reads text from file.
        public static void ReadTextFile(string fileName)
        {
            FileInfo file = new FileInfo(fileName);
            if (file.Extension == ".txt")
            {
                DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());
                bool flag = false;
                string FullFilePath = "";

                // If the file exists.
                foreach (var item in dir.GetFiles())
                {
                    if (fileName == item.Name)
                    {
                        flag = true;
                        FullFilePath = item.FullName;
                        break;
                    }
                }

                if (flag)
                {
                    // If everything is ok, reading text.
                    Encoding encoding = GetEncoding(FullFilePath);
                    using (StreamReader sr = new StreamReader(FullFilePath, encoding))
                    {
                        string str = "";
                        while (!sr.EndOfStream)
                        {
                            str = sr.ReadLine();
                            Console.WriteLine(str);
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"В данном контексте такого файла {fileName} не существует!");
                }
            }
            else
                Console.WriteLine("Проверьте расширение файла.");
        }

        // The following method returns encoding of text file
        // which I use in such methods where I need to know encoding of current file.
        private static Encoding GetEncoding(string fileName)
        {
            try
            {
                // I read first bytes of the file and detect the encode.
                byte[] buf = new byte[12000];
                int length;
                using (FileStream fs = File.OpenRead(fileName))
                {
                    length = fs.Read(buf, 0, buf.Length);
                }

                Ude.CharsetDetector det = new Ude.CharsetDetector();
                det.Feed(buf, 0, length);
                det.DataEnd();
                return Encoding.GetEncoding(det.Charset);
            }
            catch
            {
                // If I have en exception, I try to get an encoding by standart tools of language.
                Encoding encoding = null;
                using (StreamReader sr = new StreamReader(fileName))
                {
                    encoding = sr.CurrentEncoding;
                }

                return encoding;
            }
        }
        
        // Reading file with encoding which user wants.
        public static void ReadFileInEncoding(string file, Encoding encode)
        {
            FileInfo file1 = new FileInfo(file);
            if (file1.Extension == ".txt")
            {
                using (StreamReader sr = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), file), encode))
                {
                    Console.WriteLine(sr.ReadToEnd());
                }
            }
            else
                Console.WriteLine("Проверьте расширение файла.");
        }
        
        // Choosing encode which user wants.
        public static Encoding GetEncodeToCreateFile(string input)
        {
            switch (input)
            {
                case "utf8":
                    return Encoding.UTF8;
                case "utf32":
                    return Encoding.UTF32;
                case "unicode":
                    return Encoding.Unicode;
                case "ascii":
                    return Encoding.ASCII;
                default:
                    return Encoding.UTF8;
            }
        }
        
        // One more parser to parse commands with 
        public static string[] ParseTwoFiles(string names)
        {
            string[] arr = new string[2];
            string[] temp_arr = names.Split();
            string name_1 = "";
            string name_2 = "";
            int count = 0;
            for (int i = 0; i < temp_arr.Length; ++i)
            {
                if (Convert.ToString(temp_arr[i]) == Convert.ToString('|'))
                {
                    count = i + 1;
                    break;
                }
                else
                    name_1 += Convert.ToString(temp_arr[i]);
            }

            while (Convert.ToString(temp_arr[count]) == Convert.ToString(' '))
            {
                count++;
            }

            for (int i = count; i < temp_arr.Length; ++i)
            {
                name_2 += Convert.ToString(temp_arr[i]);
            }

            arr[0] = name_1;
            arr[1] = name_2;

            return arr;
        }
        
        // Method allows you to delete file.
        public static void DeleteFile(string nameFile)
        {
            try
            {
                FileInfo file = new FileInfo(nameFile);
                if (file.Exists)
                {
                    file.Delete();
                    Console.WriteLine("Файл удалён.");
                }
                else
                {
                    Console.WriteLine(
                        "Ошибка при удалении файла. Проверьте существование файла или корректность ввода");
                }
            }
            catch
            {
                Console.WriteLine("Произошла ошибка при удалении файла.");
            }
        }
    }
}