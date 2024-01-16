using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace EmployeesList
{
    struct Employee
    {
        private int id;

        public int Id
        {
            get { return this.id; }
        }

        private DateTime addingDate;

        public DateTime AddingDate
        {
            get { return this.addingDate; }
        }

        private string name;

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        private int age;

        public int Age
        {
            get { return this.age; }
            set { this.age = value; }
        }

        private int height;

        public int Height
        {
            get { return this.height; }
            set { this.height = value; }
        }

        private DateTime dateOfBirth;

        public DateTime DateOfBirth
        {
            get { return this.dateOfBirth; }
            set { this.dateOfBirth = value; }
        }

        private string placeOfBirth;

        public string PlaceOfBirth
        {
            get { return this.placeOfBirth; }
            set { this.placeOfBirth = value; }
        }


        public Employee(int id, DateTime addingDate, string name, int age, int height, DateTime dateOfBirth, string placeOfBirth)
        {
            this.id = id;
            this.addingDate = addingDate;
            this.name = name;
            this.age = age;
            this.height = height;
            this.dateOfBirth = dateOfBirth;
            this.placeOfBirth = placeOfBirth;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("{0}.\tФИО: {1}", id, name);
            builder.AppendLine();
            builder.AppendFormat("\tВозраст: {0}\n\tРост: {1}\n\tДата рождения: {2}\n\tМесто рождения: {3}\n", age, height, dateOfBirth.ToString("dd.MM.yyyy"), placeOfBirth);

            return builder.ToString();
        }
    }
}