using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;

namespace FileMenedger
{
    class Program
    {
        // The following method shows commands which user can use.
        private static void Help()
        {
            Console.WriteLine("Введите <help> для того, чтобы увидеть подсказки о командах.\n" +
                              "Введите <current> для того, чтобы узнать текущую директорию.\n" +
                              "Введите <dir> для того, чтобы увидеть папки и файлы, которые находятся в текущей директории.\n" +
                              "Введите <cd ..> для того, чтобы опуститься вниз на директорию.\n" +
                              "Введите <cd название_директории> для того, чтобы перейти в эту директорию.\n" +
                              "Введите <disk info> для того, чтобы посмотреть информацию о дисках на вашем компьютере.\n" +
                              "Введите <change disk>, а затем на новой строке введите название диска, для того, чтобы сменить диск на другой при необходимости.\n" +
                              "Введите <concat файл_1 | файл_2> (<|> - разделитель между файлами), для того, чтобы выполнить конкатенацию двух текстовых файлов и сохранить его в новый файл. " +
                              "\nБудет созданы в текущей директории.\n" +
                              "Введите <cat файл.txt> для того, вывести содержимое текстового файла в консоль.\n" +
                              "Введите <NewDir название_папки> для того, чтобы создать новую папку в текущей директории.(В случае наличия табуляции в названии, она будет заменена на пробелы) (доп. функционал)\n" +
                              "Введите <deleteDir название_папки> для того, чтобы удалить папку со всем содержимым. Не удаляйте системные данные или файлы важные для работы приложения.(доп. функционал)\n" +
                              "Введите <deleteFile название_файла> для того, чтобы удалить выбранный файл. Не удаляйте системные данные или файлы важные для работы приложения.\n" +
                              "Введите <copy имя_файла | полное_имя_директории_куда_скопировать_файл>, чтобы скопировать файл в другую директорию.\n" +
                              "Введите <remove имя_файла | полное_имя_директории_куда_скопировать_файл>, чтобы скопировать файл в другую директорию.\n" +
                              "                          в случае, если папка, куда вы хотите скопировать или переместить файл находится в текущей директории, то полный путь можно не вводить.\n" +
                              "Введите <CreateFile имя_файла> для создания нового файла и записи в него текста с нужной кодировкой. " +
                              "В конце названия файла писать <.txt>  (В случае наличия табуляции в названии, она будет заменена на пробелы.\n" +
                              "Введите <EncodeHelp> для того, чтобы посмотреть доступные кодировки для создания и чтения файлов.\n" +
                              "Введите <ReadFile имя_файла> для того, чтобы считать текстовый файл в выбранной кодировке.\n" +
                              "Введите <clear> для очищения консоли.  (доп. функционал)\n\n" +
                              "Для завершения напишите exit.");
        }

        // Parsing input string.
        private static string[] ParseInput(string input)
        {
            input = input.Replace("\t", " ");
            string command = String.Empty;
            string fileDir = String.Empty;
            int count = 0;
            string[] arrOfCommands = new string[2];
            arrOfCommands[0] = String.Empty;
            arrOfCommands[1] = String.Empty;

            // Cutting whitespaces before input if they are.
            if (input.Length > 0 && !String.IsNullOrWhiteSpace(input))
            {
                while (input[count] == ' ')
                    ++count;

                // Connection first part of the input.
                int temp_c = count;
                for (int i = count; i < input.Length; ++i)
                {
                    if (Convert.ToString(input[i]) == Convert.ToString(' '))
                    {
                        count = i;
                        break;
                    }

                    command += Convert.ToString(input[i]);
                }

                if (count == temp_c)
                    count = input.Length;

                string dir = String.Empty;
                for (int i = count; i < input.Length; ++i)
                    dir += Convert.ToString(input[i]);

                if (!string.IsNullOrWhiteSpace(dir))
                {
                    while (input[count] == ' ')
                        ++count;

                    int count_last = input.Length;

                    // Cutting whitespaces from the end.
                    if (input[input.Length - 1] == ' ')
                        for (int i = input.Length - 1; i > 0; --i)
                            if (String.IsNullOrWhiteSpace(Convert.ToString(input[i])))
                                count_last--;
                            else
                                break;

                    // Connect second part of the input.
                    for (int i = count; i < count_last; ++i)
                        fileDir += Convert.ToString(input[i]);
                }

                arrOfCommands[0] = Convert.ToString(command);
                arrOfCommands[1] = Convert.ToString(fileDir);
            }

            return arrOfCommands;
        }

        // Shows user which encodings is available.
        private static void GetCodeEncoding() =>
            Console.WriteLine("Available encodings: <utf8>, <ascii>, <unicode>, <utf32>");

        // Method which calls all methods.
        private static void Navigation()
        {
            while (true)
            {
                try
                {
                    string input;
                    if (Directory.Exists(Directory.GetCurrentDirectory()))
                    {
                        input = Console.ReadLine();
                        string[] arr_input = ParseInput(input);
                        if (arr_input[0] == "cd")
                        {
                            DirectoryManager.RequestHandlerCd(arr_input);
                            Console.WriteLine(Directory.GetCurrentDirectory());
                        }
                        else if (arr_input[0] == "cat")
                        {
                            if (arr_input[1].Length < 1 || String.IsNullOrWhiteSpace(arr_input[1]))
                                throw new ArgumentException("Incorrect file name. Please, try again.");

                            FileHelper.ReadTextFile(arr_input[1]);
                        }
                        else if (arr_input[0] == "help")
                        {
                            if (!String.IsNullOrWhiteSpace(arr_input[1]))
                                throw new ArgumentException("Incorrect input. Please, try again.");

                            Help();
                        }
                        else if (arr_input[0] == "disk" && arr_input[1] == "info")
                        {
                            DirectoryManager.GetDiskNames();
                        }
                        else if (arr_input[0] == "current")
                        {
                            if (String.IsNullOrWhiteSpace(arr_input[1]))
                                throw new ArgumentException("Incorrect command. Please, try again.");

                            Console.WriteLine(Directory.GetCurrentDirectory());
                        }
                        else if (arr_input[0] == "dir")
                        {
                            if (!String.IsNullOrWhiteSpace(arr_input[1]))
                                throw new ArgumentException("Incorrect command. Please, try again.");

                            DirectoryManager.ShowFilesAndFolders();
                        }
                        else if (arr_input[0] == "change" && arr_input[1] == "disk")
                        {
                            Console.WriteLine("Введите диск, на который хотите перейти:");
                            input = Console.ReadLine();
                            DirectoryManager.ChangeDisk(input);
                        }
                        else if (arr_input[0] == "NewDir")
                        {
                            if (string.IsNullOrWhiteSpace(arr_input[1]))
                                throw new ArgumentException("Incorrect directory name. Please, try again.");

                            DirectoryManager.CreateDirectory(arr_input[1]);
                        }
                        else if (arr_input[0] == "deleteDir")
                        {
                            DirectoryManager.DeleteDirectory(arr_input[1]);
                        }
                        else if (arr_input[0] == "deleteFile")
                        {
                            FileHelper.DeleteFile(arr_input[1]);
                        }
                        else if (arr_input[0] == "copy")
                        {
                            if (arr_input[1].Length < 1 || string.IsNullOrWhiteSpace(arr_input[1]) ||
                                arr_input[1] == null)
                                throw new ArgumentException("Incorrect file name. Please, try again.");

                            string[] temp_arr = FileHelper.ParseTwoFiles(arr_input[1]);
                            DirectoryManager.CopyFileToDir(temp_arr[1], temp_arr[0]);
                        }
                        else if (arr_input[0] == "remove")
                        {
                            string[] temp_arr = FileHelper.ParseTwoFiles(arr_input[1]);
                            DirectoryManager.RemoveFileToNewDir(temp_arr[1], temp_arr[0]);
                        }
                        else if (arr_input[0] == "concat")
                        {
                            string[] temp_arr = FileHelper.ParseTwoFiles(arr_input[1]);
                            FileHelper.ConcatTwoFiles(temp_arr[0], temp_arr[1]);
                        }
                        else if (arr_input[0] == "CreateFile")
                        {
                            if (arr_input[1].Length < 1 || string.IsNullOrWhiteSpace(arr_input[1]))
                                throw new ArgumentException("Incorrect file name. Please, try again");

                            Console.WriteLine(
                                "Введите кодировку, в которой вы хотите создать файл. Названия кодировок следует вводить без пробелов и иных разделителей. \n" +
                                "В случае, если название введено неправильно или в названии присутсвует разделитель кодировка будет стандратной - UTF8\n" +
                                "Введите stop на новой строке для окончания записи текста в файл:\n");
                            input = Console.ReadLine();
                            FileHelper.CreateNewTextFile(null, null, FileHelper.GetEncodeToCreateFile(input),
                                arr_input[1], true);
                        }
                        else if (arr_input[0] == "EncodeHelp")
                        {
                            if (!String.IsNullOrWhiteSpace(arr_input[1]))
                                throw new ArgumentException("Incorrect input. Please, try again.");
                            GetCodeEncoding();
                        }
                        else if (arr_input[0] == "clear")
                        {
                            Console.Clear();
                        }
                        else if (arr_input[0] == "ReadFile")
                        {
                            if (arr_input[1].Length < 1 || string.IsNullOrWhiteSpace(arr_input[1]) ||
                                arr_input[1] == null)
                                throw new ArgumentException("Incorrect file name. Please, try again.");

                            Console.WriteLine(
                                "Если кодировка введена неправильно, то по умолчанию откроется в кодировке utf8.\n" +
                                "Введите кодировку, в которой хотите прочитать текст из файла: ");
                            input = Console.ReadLine();
                            FileHelper.ReadFileInEncoding(arr_input[1], FileHelper.GetEncodeToCreateFile(input));
                        }
                        else if (input == "exit" || input == "Exit")
                            break;
                        else
                            throw new ArgumentException("Unknown command. Please, try again.");
                    }
                    else
                    {
                        Console.WriteLine("Директория в которой вы находитесь перестала быть доступной! " +
                                          "Выберете новый диск из предложенных ниже:");
                        DirectoryManager.GetDiskNames();
                        input = Console.ReadLine();
                        DirectoryManager.ChangeDisk(input);
                        Console.WriteLine($"Вы успешно перешли на диск {input}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        // Main.
        static void Main(string[] args)
        {
            Help();
            Console.WriteLine(Directory.GetCurrentDirectory());
            Navigation();
        }
    }
}