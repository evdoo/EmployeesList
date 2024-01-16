using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace EmployeesList
{
    class Repository
    {
        private string pathToFile;

        public Repository(string pathToFile)
        {
            this.pathToFile = pathToFile;
        }

        public Employee[] GetAllEmployees()
        {
            Employee[] employees;

            if (File.Exists(pathToFile))
            {
                string[] strings = File.ReadAllLines(pathToFile);

                employees = new Employee[strings.Length];

                for (int i = 0; i < strings.Length; i++)
                {
                    employees[i] = ParseStringToEmployee(strings[i]);
                }
            } else
            {
                employees = new Employee[0];
            }

            return employees;
        }

        /// <summary>
        /// This method throws an exception
        /// </summary>
        public Employee GetEmployeeById(int id)
        {
            using (StreamReader reader = new StreamReader(pathToFile))
            {
                while (reader.Peek() >= 0)
                {
                    string str = reader.ReadLine();

                    if (Int32.Parse(str.Split("#")[0]) == id)
                    {
                        Employee employee = ParseStringToEmployee(str);
                        reader.Close();
                        return employee;
                    }
                }
            }

            throw new ArgumentException();
        }

        public Employee[] GetEmployeesByAnyMatch(string match)
        {
            Employee[] employees = new Employee[0];
            int index = 0;
            using (StreamReader reader = new StreamReader(pathToFile))
            {
                while (reader.Peek() >= 0)
                {
                    string str = reader.ReadLine();
                    string[] data = str.Split("#");

                    foreach (string s in data)
                    {
                        if (s.ToLower().Contains(match.ToLower()))
                        {
                            Employee employee = ParseStringToEmployee(str);

                            if (index > employees.Length - 1)
                            {
                                Array.Resize(ref employees, employees.Length + 1);
                            }
                            employees[index] = employee;
                            index++;
                            break;
                        }
                    }
                }
            }

            return employees;
        }

        public void AddEmployeeToFile(string[] strings)
        {
            string id;
            if (File.Exists(pathToFile))
            {
                id = (File.ReadAllLines(pathToFile).Length + 1).ToString();
            }
            else
            {
                id = "1";
            }

            string date = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

            string str = String.Join("#", new string[7] { id, date, strings[0], strings[1], strings[2], strings[3], strings[4] });

            using (StreamWriter streamWriter = File.AppendText(pathToFile))
            {
                streamWriter.WriteLine(str);
            }
        }

        /* Кто должен отвечать за обработку всяческих исключений? Репозиторий или сама программа?
         * То есть тут можно пойти тремя путями:
         * 1. Если сотрудника с таким id не найдено, то репозиторий выдает сообщение в консоль.
         * 2. Если сотрудника с таким id не найдено, то репозиторий выбрасывает ошибку, которую ловит и обрабатывает программа.
         * 3. Делаем методу возвращаемое значение, которое будет показывать, что удаление прошло успешно (или нет).
         * Более правильным с точки зрения ООП мне видится второй вариант. Но я не уверена.
         * При этом использование выбрасывания исключений в шарпе мне видится опасным - так как тут, в отличие от джавы, нет throws-методов.
         * */
        /// <summary>
        /// This method throws an exception
        /// </summary>
        /// <param name="id"></param>
        public void DeleteEmployeeFromFile(int id)
        {
            StringBuilder employees = new StringBuilder();

            string[] strings = File.ReadAllLines(pathToFile);

            bool isFind = false;

            for (int i = 0; i < strings.Length; i++)
            {
                if (Int32.Parse(strings[i].Split("#")[0]) != id)
                {
                    employees.Append(strings[i]);
                    if (i < strings.Length - 1)
                    {
                        employees.Append("\n");
                    }
                }
                else
                {
                    isFind = true;
                }
            }

            if (isFind)
            {
                RewriteFile(employees.ToString());
            }
            else
            {
                throw new ArgumentException("No employee with this id");
            }
        }

        public Employee[] GetEmployeesBetweenDates(DateTime dateFrom, DateTime dateTo)
        {
            //Бессмысленное чтение файла по строкам - мы же не выходим из цикла при нахождении первого нужного элемента, а проходим все. Поэтому можно заменить.
            Employee[] employees = new Employee[0];
            int index = 0;
            using (StreamReader reader = new StreamReader(pathToFile))
            {
                while (reader.Peek() >= 0)
                {
                    string str = reader.ReadLine();

                    DateTime date = DateTime.Parse(str.Split("#")[1]);

                    if (dateFrom.CompareTo(date) == -1 && date.CompareTo(dateTo) == -1)
                    {
                        Employee employee = ParseStringToEmployee(str);

                        if (index > employees.Length - 1)
                        {
                            Array.Resize(ref employees, employees.Length + 1);
                        }
                        employees[index] = employee;
                        index++;
                    }
                }
            }

            return employees;
        }

        public Employee[] GetEmployeesWithBirthdayInThisMonth()
        {
            string[] strings = File.ReadAllLines(pathToFile);

            Employee[] employees = new Employee[0];

            int index = 0;

            for (int i = 0; i < strings.Length; i++)
            {
                Employee employee = ParseStringToEmployee(strings[i]);

                if (DateTime.Now.Month == employee.DateOfBirth.Month)
                {
                    if (index > employees.Length - 1)
                    {
                        Array.Resize(ref employees, employees.Length + 1);
                    }
                    employees[index] = employee;
                    index++;
                }
            }

            return employees;
        }

        public void EditEmployee(string[] employeeStrings)
        {
            string[] strings = File.ReadAllLines(pathToFile);

            bool isFind = false;

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < strings.Length; i++)
            {

                if (Int32.Parse(strings[i].Split("#")[0]) == Int32.Parse(employeeStrings[0]))
                {
                    isFind = true;

                    string str = String.Join("#", employeeStrings);

                    builder.Append(str);
                } else
                {
                    builder.Append(strings[i]);
                }

                if (i < strings.Length - 1)
                {
                    builder.Append("\n");
                }
            }

            if (isFind)
            {
                RewriteFile(builder.ToString());
            } else
            {
                throw new ArgumentException("No employee with this id");
            }
        }

        private void RewriteFile(string str)
        {
            File.WriteAllText(pathToFile, str);
        }

        private Employee ParseStringToEmployee(string str)
        {
            string[] data = str.Split("#");

            string format = "dd.MM.yyyy";

            DateTime addingDate;
            if (!DateTime.TryParseExact(data[1], format, new CultureInfo("en-US"), DateTimeStyles.None, out addingDate))
            {
                addingDate = new DateTime(1960, 1, 1);
            }

            DateTime birthDate;
            if (!DateTime.TryParseExact(data[5], format, new CultureInfo("en-US"), DateTimeStyles.None, out birthDate))
            {
                birthDate = new DateTime(1960, 1, 1);
            }

            Employee employee = new Employee(Int32.Parse(data[0]), addingDate, data[2], Int32.Parse(data[3]), Int32.Parse(data[4]), birthDate, data[6]);

            return employee;
        }
    }
}