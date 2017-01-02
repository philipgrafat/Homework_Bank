using System;

namespace Bank
{
    public class Person
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public DateTime BirthDate { get; private set; }

        public Person(string firstName, string lastName, DateTime birthDate)
        {
            FirstName = firstName;
            LastName = lastName;
            BirthDate = birthDate;
        }

        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }
    }
}