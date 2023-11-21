using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace EmployeesList
{
    class Program
    {
        public static string defaultPath;
        private static State state;

        static void Main(string[] args)
        {
            defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Employees.txt";
            state = State.MainMenu;

            while (true)
            {
                switch (state)
                {
                    case State.MainMenu:
                        ShowMainScreen();
                        break;
                    case State.Show:
                        ShowInfoScreen();
                        break;
                    case State.Edit:
                        ShowEditScreen();
                        break;
                    case State.Exit:
                        Environment.Exit(0);
                        break;
                }
            }
        }

        static void ShowMainScreen()
        {
            ClearConsole();

            ShowSystemMessage("Добрый день!\nЕсли вы хотите вывести данные о сотрудниках на экран, нажмите 1.");
            ShowSystemMessage("Если вы хотите заполнить данные и добавить новую запись о сотруднике, нажмите 2.");
            ShowSystemMessage("Для выхода из программы нажмите Escape");

            while (true)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.D1:
                        ChangeProgramState(State.Show);
                        return;
                    case ConsoleKey.D2:
                        ChangeProgramState(State.Edit);
                        return;
                    case ConsoleKey.Escape:
                        ChangeProgramState(State.Exit);
                        return;
                    default:
                        ShowSystemMessage("Неизвестная команда. Попробуйте еще раз.");
                        break;
                }
            }
        }

        static void ShowInfoScreen()
        {
            ClearConsole();

            string path = defaultPath;

            while (true)
            {
                if (File.Exists(path))
                {
                    ShowSystemMessage("Ниже представлены записи о сотрудниках.");
                    ShowSystemMessage("Для возвращения в главное меню нажмите Backspace.");
                    ShowSystemMessage("Для выхода из программы нажмите Esc.");

                    PrintEmploees(path);

                    ShowSystemMessage("Для возвращения в главное меню нажмите Backspace. Для выхода из программы нажмите Esc.");

                    while (true)
                    {
                        switch (Console.ReadKey(true).Key)
                        {
                            case ConsoleKey.Backspace:
                                ChangeProgramState(State.MainMenu);
                                return;
                            case ConsoleKey.Escape:
                                ChangeProgramState(State.Exit);
                                return;
                            default:
                                ShowSystemMessage("Неизвестная команда. Попробуйте еще раз.");
                                break;
                        }
                    }
                }
                else
                {
                    ShowSystemMessage("К сожалению, файл " + path + " не найден. Напишите путь до файла и нажмите Enter.");

                    string input = GetInputOrExit();

                    if (input == null)
                    {
                        return;
                    }
                    else
                    {
                        path = CompleteFilePath(input);
                    }
                }
            }
        }

        private static string GetInputOrExit()
        {
            string input = "";

            bool done = false;
            while (!done)
            {
                ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();

                switch (consoleKeyInfo.Key)
                {
                    case ConsoleKey.Escape:
                        ChangeProgramState(State.Exit);
                        return null;
                    case ConsoleKey.Backspace:
                        if (string.IsNullOrWhiteSpace(input))
                        {
                            ChangeProgramState(State.MainMenu);
                            return null;
                        }
                        else
                        {
                            input = input.Remove(input.Length - 1);
                            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
                            Console.Write(" ");
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        }
                        break;
                    case ConsoleKey.Enter:
                        done = true;
                        Console.Write("\n");
                        break;
                    default:
                        input += consoleKeyInfo.KeyChar;
                        break;
                }
            }

            return input;
        }

        private static string CompleteFilePath(string input)
        {
            string[] items = input.Trim().Split("\\");
            if (items[items.Length - 1].Equals("Employees.txt"))
            {
                return input;
            }
            else
            {
                return input + "\\Employees.txt";
            }
        }

        static void ShowEditScreen()
        {
            Console.Clear();

            ShowSystemMessage("Добавление нового сотрудника");

            string path = GetFilePath();

            if (path == null)
            {
                return;
            }

            ShowSystemMessage("Введите ФИО сотрудника:");
            string name = GetCheckedInput("String");

            if (name == null)
            {
                return;
            }

            ShowSystemMessage("Введите возраст сотрудника:");
            string age = GetCheckedInput("Int");

            if (age == null)
            {
                return;
            }

            ShowSystemMessage("Введите рост сотрудника:");
            string heigth = GetCheckedInput("Int");

            if (heigth == null)
            {
                return;
            }

            ShowSystemMessage("Введите дату рождения сотрудника (в формате дд.мм.гггг):");
            string dateOfBirth = GetCheckedInput("Date");

            if (dateOfBirth == null)
            {
                return;
            }

            ShowSystemMessage("Введите место рождения сотрудника:");
            string placeOfBirth = GetCheckedInput("String");

            if (placeOfBirth == null)
            {
                return;
            }

            string id;
            if (File.Exists(path))
            {
                id = (File.ReadAllLines(path).Length + 1).ToString();
            }
            else
            {
                id = "1";
            }

            string date = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

            string str = String.Join("#", new string[7] { id, date, name, age, heigth, dateOfBirth, placeOfBirth });

            WriteNewStringToFile(path, str);

            ShowSystemMessage("Сотрудник добавлен в файл.");
            ShowSystemMessage("Для возвращения в главное меню нажмите Backspace. Для выхода из программы нажмите Esc.");

            while (true)
            {
                if (Console.ReadKey(true).Key == ConsoleKey.Backspace)
                {
                    ChangeProgramState(State.MainMenu);
                    return;
                }

                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                {
                    ChangeProgramState(State.Exit);
                    return;
                }
            }
        }

        static string[] GetStringsFromFile(string filePath)
        {
            return File.ReadAllLines(filePath);
        }

        static void PrintEmploees(string pathToFile)
        {
            string[] employees = GetStringsFromFile(pathToFile);

            foreach (string str in employees)
            {
                string[] data = str.Split("#");

                StringBuilder builder = new StringBuilder();

                builder.AppendFormat("{0}.\tФИО: {1}", data[0], data[2]);
                builder.AppendLine();
                builder.AppendFormat("\tВозраст: {0}\n\tРост: {1}\n\tДата рождения: {2}\n\tМесто рождения: {3}\n", data[3], data[4], data[5], data[6]);

                Console.WriteLine(builder);
            }
        }

        static void WriteNewStringToFile(string pathToFile, string text)
        {
            using (StreamWriter streamWriter = File.AppendText(pathToFile))
            {
                streamWriter.WriteLine(text);
            }
        }

        static void ShowSystemMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        static string GetCheckedInput(string inputType)
        {
            while (true)
            {
                string input = GetInputOrExit();

                if (input == null)
                {
                    return null;
                }
                else
                {
                    switch (inputType)
                    {
                        case "String":
                            if (string.IsNullOrWhiteSpace(input))
                            {
                                ShowSystemMessage("Нужно ввести непустую строку.");
                                break;
                            }
                            else
                            {
                                return input;
                            }
                        case "Int":
                            try
                            {
                                Int32.Parse(input);
                                return input;
                            }
                            catch (Exception e)
                            {
                                ShowSystemMessage("Нужно ввести целое число.");
                            }
                            break;
                        case "Date":
                            string format = "dd.MM.yyyy";
                            DateTime date;
                            if (DateTime.TryParseExact(input, format, new CultureInfo("en-US"), DateTimeStyles.None, out date))
                            {
                                return input;
                            }
                            else
                            {
                                ShowSystemMessage("Нужно ввести дату в формате дд.мм.гггг");
                                break;
                            }
                    }
                }
            }
        }

        static string GetFilePath()
        {
            ShowSystemMessage("По умолчанию данные будут записаны в файл " + defaultPath);
            ShowSystemMessage("Если вы хотите изменить место хранения файла, напишите полный путь до него ниже, и нажмите Enter.");
            ShowSystemMessage("Если хотите оставить путь по умолчанию, просто нажмите Enter.");
            ShowSystemMessage("Для возвращения в главное меню нажмите Backspace при пустом вводе. Для выхода из программы нажмите Esc.");

            string path = GetInputOrExit();

            if (path == null)
            {
                return null;
            }
            else
            {
                if (path.Equals(""))
                {
                    return defaultPath;
                }
                else
                {
                    return CompleteFilePath(path);
                }
            }
        }

        static void ClearConsole()
        {
            Console.Clear();
            if (Environment.GetEnvironmentVariable("TERM") != null && Environment.GetEnvironmentVariable("TERM").StartsWith("xterm"))
            {
                Console.WriteLine("\x1b[3J");
            }
        }

        static void ChangeProgramState(State newState)
        {
            state = newState;
        }

        enum State
        {
            MainMenu,
            Show,
            Edit,
            Exit
        }
    }
}