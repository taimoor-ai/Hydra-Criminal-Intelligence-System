// ============================================================
// Models/Person.cs
// Base class for Criminal, Officer, Victim (Inheritance)
// Demonstrates: Encapsulation, Abstraction
// ============================================================
using CriminalRecordMS.Interfaces;

namespace CriminalRecordMS.Models
{
    public abstract class Person : IReportable
    {
        // Encapsulation: private backing fields
        private string _id;
        private string _name;
        private string _cnic;
        private string _address;
        private string _phoneNumber;
        private DateTime _dateOfBirth;

        // Properties (getters/setters)
        public string Id
        {
            get => _id;
            protected set => _id = !string.IsNullOrWhiteSpace(value) ? value
                : throw new ArgumentException("ID cannot be empty.");
        }

        public string Name
        {
            get => _name;
            set => _name = !string.IsNullOrWhiteSpace(value) ? value
                : throw new ArgumentException("Name cannot be empty.");
        }

        public string Cnic
        {
            get => _cnic;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length < 13)
                    throw new ArgumentException("CNIC must be at least 13 characters.");
                _cnic = value;
            }
        }

        public string Address
        {
            get => _address;
            set => _address = value ?? "";
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set => _phoneNumber = value ?? "";
        }

        public DateTime DateOfBirth
        {
            get => _dateOfBirth;
            set
            {
                if (value > DateTime.Now)
                    throw new ArgumentException("Date of birth cannot be in the future.");
                _dateOfBirth = value;
            }
        }

        public int Age => DateTime.Now.Year - _dateOfBirth.Year;

        // Constructor
        protected Person(string id, string name, string cnic,
                         string address, string phone, DateTime dob)
        {
            Id = id;
            Name = name;
            Cnic = cnic;
            Address = address;
            PhoneNumber = phone;
            DateOfBirth = dob;
        }

        // Abstract methods - must be overridden (Polymorphism)
        public abstract string GenerateReport();

        public virtual void PrintReport()
        {
            Console.WriteLine(GenerateReport());
        }

        public override string ToString() => $"[{Id}] {Name} | CNIC: {Cnic}";
    }
}