namespace CompanyEmployeesSQL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.CompilerServices;

    public partial class Employee: INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private int? _age;
        public int? Age
        {
            get => _age; set { _age = value; OnPropertyChanged(); }
        }

        private float? _salary;
        public float? Salary
        {
            get => _salary; set { _salary = value; OnPropertyChanged(); }
        }

        public int DepartmentId { get; set; }

        public Department Department
        {get => m_Department; set {m_Department = value; OnPropertyChanged();}}
        private Department m_Department;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
