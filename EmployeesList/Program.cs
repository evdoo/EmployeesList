using System;
using System.Globalization;
using System.Text;
using System.Linq;

namespace EmployeesList
{
    class Program
    {
        public static string defaultPath;
        private static Repository repository;
        private static string[] backstack = new string[4];
        private static int backstackCount = 0;

        private static Employee[] bufferEmployees;

        static void Main(string[] args)
        {
            defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Employees.txt";
            ShowCommands();
            PrepareUserInput();

            repository = new Repository(defaultPath);

            while (true)
            {
                string input = GetInputOrExit();

                if (input == null)
                {
                    Environment.Exit(0);
                }
                string[] commands = input.Split(" ");
                string[] arguments;
                if (commands.Length > 1)
                {
                    arguments = commands[1..(commands.Length)];
                }
                else
                {
                    arguments = new string[0];
                }

                if (backstackCount == backstack.Length)
                {
                    Array.Resize(ref backstack, backstackCount * 2);
                }
                backstack[backstackCount] = input;
                backstackCount++;

                switch (commands[0])
                {
                    case "/show":
                        Show(arguments);
                        break;
                    case "/del":
                        Delete(arguments);
                        break;
                    case "/edit":
                        Edit(arguments);
                        break;
                    case "/add":
                        Add();
                        break;
                    case "/help":
                        Help(arguments);
                        break;
                    case "/sort":
                        Sort(arguments);
                        break;
                    case "/find":
                        Find(arguments);
                        break;
                    case "/commands":
                        ShowCommands();
                        break;
                    default:
                        ShowSystemMessage("Команда не распознана.");
                        break;
                }

                PrepareUserInput();
            }
        }

        static void Delete(string[] args)
        {
            if (args.Length != 1)
            {
                ShowSystemMessage("Неверная команда. \\delete принимает 1 аргумент - id записи, которую надо удалить");
                return;
            }

            try
            {
                repository.DeleteEmployeeFromFile(Int32.Parse(args[0]));
                ShowSystemMessage("Запись о сотруднике удалена.");
            }
            catch (Exception e)
            {
                ShowSystemMessage("Нет записи с таким id.");
            }
        }

        static void Show(string[] args)
        {
            Employee[] employees;
            if (args.Length == 0)
            {
                employees = repository.GetAllEmployees();
            }
            else if (args.Length == 1)
            {
                if (args[0].Equals("b"))
                {
                    employees = repository.GetEmployeesWithBirthdayInThisMonth();
                }
                else
                {
                    try
                    {
                        Employee? employee = repository.GetEmployeeById(Int32.Parse(args[0]));
                        if (employee == null)
                        {
                            ShowSystemMessage("Нет записи с таким id.");
                            return;
                        }
                        else
                        {
                            employees = new[] { (Employee)employee };
                        }
                    }
                    catch (Exception e)
                    {
                        ShowSystemMessage("Нет записи с таким id.");
                        return;
                    }
                }
            }
            else if (args.Length == 2)
            {
                DateTime startDate;
                DateTime finishDate;
                try
                {
                    startDate = DateTime.Parse(args[0]);
                    finishDate = DateTime.Parse(args[1]);
                }
                catch (FormatException e)
                {
                    ShowSystemMessage("Некорректные даты.");
                    return;
                }

                employees = repository.GetEmployeesBetweenDates(startDate, finishDate);

            }
            else
            {
                ShowSystemMessage("Неверная команда.");
                return;
            }

            Program.bufferEmployees = employees;

            if (employees.Length == 0)
            {
                ShowSystemMessage("Ничего нет :(");
            }
            else
            {
                PrintEmploees(employees);
            }
        }

        static void Edit(string[] args)
        {
            if (args.Length != 1)
            {
                ShowSystemMessage("Неверная команда.");
                return;
            }

            Employee employee;
            try
            {
                employee = repository.GetEmployeeById(Int32.Parse(args[0]));
                Employee[] employees = new[] { employee };
                PrintEmploees(employees);
            }
            catch (Exception e)
            {
                ShowSystemMessage("Нет записи с таким id.");
                return;
            }

            ShowSystemMessage("Что нужно поменять?");
            ShowSystemMessage("1 - имя");
            ShowSystemMessage("2 - возраст");
            ShowSystemMessage("3 - рост");
            ShowSystemMessage("4 - дата рождения");
            ShowSystemMessage("5 - место рождения");
            ShowSystemMessage("Esc - отмена");

            bool getCommand = false;
            string newData;
            int field = 0;

            string[] employeeStrings = {
                    employee.Id.ToString(),
                    employee.AddingDate.ToString("dd.MM.yyyy HH:mm"),
                    employee.Name,
                    employee.Age.ToString(),
                    employee.Height.ToString(),
                    employee.DateOfBirth.ToString("dd.MM.yyyy"),
                    employee.PlaceOfBirth
                };

            bool isExit = false;

            while (!getCommand)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.D1:
                        getCommand = true;
                        field = 2;
                        ShowSystemMessage("Новое имя:");
                        break;
                    case ConsoleKey.D2:
                        getCommand = true;
                        field = 3;
                        ShowSystemMessage("Новый возраст:");
                        break;
                    case ConsoleKey.D3:
                        getCommand = true;
                        field = 4;
                        ShowSystemMessage("Новый рост:");
                        break;
                    case ConsoleKey.D4:
                        getCommand = true;
                        field = 5;
                        ShowSystemMessage("Новая дата рождения:");
                        break;
                    case ConsoleKey.D5:
                        getCommand = true;
                        field = 6;
                        ShowSystemMessage("Новое место рождения:");
                        break;
                    case ConsoleKey.Escape:
                        getCommand = true;
                        isExit = true;
                        break;
                    default:
                        ShowSystemMessage("Неверная команда.");
                        break;
                }
            }

            if (isExit)
            {
                return;
            }

            while (true)
            {
                newData = GetInputOrExit();

                if (newData == null) return;

                if (newData.Equals(""))
                {
                    ShowSystemMessage("Пустая строка.");
                }
                else
                {
                    employeeStrings[field] = newData;
                    try
                    {
                        repository.EditEmployee(employeeStrings);
                        ShowSystemMessage("Запись изменена.");
                    }
                    catch (Exception e)
                    {
                        ShowSystemMessage("Нет записи с таким id.");
                    }
                    return;
                }
            }
        }

        static void Add()
        {
            ShowSystemMessage("Добавление нового сотрудника");

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

            string[] strings = new string[5] { name, age, heigth, dateOfBirth, placeOfBirth };

            repository.AddEmployeeToFile(strings);

            ShowSystemMessage("Сотрудник добавлен в файл.");
        }

        // сделать проверку на корректность команды в начале, чтобы не делать ненужных операций
        static void Sort(string[] args)
        {
            if (args.Length < 1 || args.Length > 2)
            {
                ShowSystemMessage("Неверная команда.");
                return;
            }
            
            Employee[] employees;

            if (Program.bufferEmployees != null && Program.bufferEmployees.Length != 0)
            {
                employees = Program.bufferEmployees;
            } else
            {
                employees = repository.GetAllEmployees();
            }

            switch (args[0].ToLower())
            {
                case "id":
                    employees = employees.OrderBy(employee => employee.Id).ToArray();
                    break;
                case "name":
                    employees = employees.OrderBy(employee => employee.Name).ToArray();
                    break;
                case "age":
                    employees = employees.OrderBy(employee => employee.Age).ToArray();
                    break;
                case "height":
                    employees = employees.OrderBy(employee => employee.Height).ToArray();
                    break;
                case "birth date":
                    employees = employees.OrderBy(employee => employee.DateOfBirth).ToArray();
                    break;
                case "birth place":
                    employees = employees.OrderBy(employee => employee.PlaceOfBirth).ToArray();
                    break;
                default:
                    ShowSystemMessage("Неверная команда.");
                    return;
            }
            
            if (args.Length == 2)
            {
                if (args[1].Equals("d"))
                {
                    Array.Reverse(employees);
                } else
                {
                    ShowSystemMessage("Неверная команда.");
                    return;
                }
            }

            if (employees.Length == 0)
            {
                ShowSystemMessage("Ничего нет :(");
            }
            else
            {
                PrintEmploees(employees);
            }
        }

        static void Find(string[] args)
        {
            Employee[] employees;
            if (args.Length == 1)
            {
                employees = repository.GetEmployeesByAnyMatch(args[0]);
            }
            else
            {
                ShowSystemMessage("Неверная команда.");
                return;
            }

            if (employees.Length == 0)
            {
                ShowSystemMessage("Ничего нет :(");
            }
            else
            {
                PrintEmploees(employees);
            }

            Program.bufferEmployees = employees;
        }

        private static void ShowCommands()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Доступные команды:\n");
            builder.Append("/show - показать все записи\n");
            builder.Append("/show <id> - показать запись о конкретном сотруднике с указанным <id>\n");
            builder.Append("/show <date1> <date2> - показать записи, добавленные в файл в промежуток между <date1> и <date2>\n");
            builder.Append("/show b - показать записи сотрудников, у которых день рождения в текущем месяце\n");
            builder.Append("/del <id> - удалить запись о сотруднике с заданным <id>\n");
            builder.Append("/edit <id> - редактировать запись о сотруднике с заданным <id>\n");
            builder.Append("/add - добавить запись о сотруднике\n");
            builder.Append("/sort <field> - сортировать записи по заданному полю <field>\n");
            builder.Append("/sort <field> <option> - сортировать записи по заданному полю <field> с параметрами <option>\n");
            builder.Append("/find <match> - найти все записи по заданной строке <match>\n");
            builder.Append("/help - показать доступные команды");

            ShowSystemMessage(builder.ToString());
        }

        private static void Help(string[] args)
        {
            StringBuilder builder = new StringBuilder();

            switch(args[0])
            {
                case "/show":
                    builder.Append("Показывает записи о сотрудниках списком.\n");
                    builder.Append("/show - показывает все записи\n");
                    builder.Append("/show <id> - показывает запись по выбранному id\n");
                    builder.Append("/show <date1> <date2> - показывает записи, добавленные в файл в промежуток между выбранными датами. Даты должны быть в формате дд.мм.гггг\n");
                    builder.Append("/show b - показывает записи о сотрудниках, у которых день рождения в текущем месяце");
                    break;
                case "/del":
                    builder.Append("/del <id> - удаляет запись о сотруднике с выбранным id");
                    break;
                case "/edit":
                    builder.Append("/edit <id> - дает возможность редактировать запись с выбранным id\n");
                    builder.Append("Можно изменить ФИО, возраст, рост, дату или место рождения");
                    break;
                case "/add":
                    builder.Append("Добавляет нового сотрудника в список");
                    break;
                case "/sort":
                    builder.Append("/sort <field>\t- сортирует список в порядке возрастания по выбранному полю\n");
                    builder.Append("/sort <field> d\t- сортирует список в порядке убывания по выбранному полю\n");
                    builder.Append("Список для сортировки берется из результатов последней введенной команды, выдающей список. ");
                    builder.Append("Если команды, выдающей список, перед этим не было, берется весь список сотрудников из файла.\n");
                    builder.Append("То есть можно, например, сначала найти все записи по какой-то строке, а потом отсортировать их.\n");
                    builder.Append("Поля <field>:\n");
                    builder.Append("name - имя\nage - возраст\nheight - рост\nbirth_date - дата рождения\nbirth_place - место рождения");
                    break;
                case "/find":
                    builder.Append("/find <match>\t- показывает все записи, в которых найдена введенная строка. Ищет по всем полям");
                    break;
                case "/help":
                    builder.Append("Если вы тут, то вам не нужна справка по этой команде");
                    break;
                case "/commands":
                    builder.Append("Выдает список всех доступных команд");
                    break;
                default:
                    builder.Append("Нет такой команды");
                    break;
            }

            ShowSystemMessage(builder.ToString());
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
                        //ChangeProgramState(State.Exit);
                        return null;
                    case ConsoleKey.Backspace:
                        input = input.Remove(input.Length - 1);
                        Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
                        Console.Write(" ");
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
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

        static void PrintEmploees(Employee[] employees)
        {
            foreach (Employee employee in employees)
            {
                StringBuilder builder = new StringBuilder();

                builder.AppendFormat("{0}.\tФИО: {1}", employee.Id, employee.Name);
                builder.AppendLine();
                builder.AppendFormat("\tВозраст: {0}\n\tРост: {1}\n\tДата рождения: {2:dd.MM.yyyy}\n\tМесто рождения: {3}\n",
                    employee.Age,
                    employee.Height,
                    employee.DateOfBirth,
                    employee.PlaceOfBirth
                    );

                Console.WriteLine(builder);
            }
        }

        static void ShowSystemMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        static void PrepareUserInput()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            string userName = Environment.UserDomainName + "@" + Environment.UserName;
            Console.Write(userName + " " + DateTime.Now + ">> ");
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
    }
}