using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace CompanyEmployeesSQL
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Service serv;
        public MainWindow()
        {
            InitializeComponent();

            serv = new Service(this);

            serv.OcEmployees = new ObservableCollection<Employee>();

            serv.LoadDB();

            CbDepartmentSet.ItemsSource = serv.OcDepartment;
            CbDepartmentSet.SelectionChanged += (s, e) =>
            {
                {
                    serv.DepartmentSet(DgEmployee.SelectedItems.Cast<Employee>(),
                        CbDepartmentSet.SelectedItem as Department);
                }
            };

            DgEmployee.ItemsSource = serv.OcEmployees;
            DgEmployee.RowEditEnding += (s, e) => { { serv.AfterEditEmployee(e); } };
            DgEmployee.SelectionChanged += (s, e) => { {serv.SelectedEmployee = DgEmployee.SelectedItem as Employee; } };
            DgEmployee.LostFocus += (s, e) => { { serv.SaveAfterEditDG(); } };

            BtnDepartmentsEdit.Click += (s, e) => { { serv.OpenDepartmentsEdit(); } };

            BtnAddNewEmployee.Click += (s, e) => { { serv.AddNewEmployee(CbDepartmentSet.SelectedItem as Department); } };
            BtnRemoveEmployee.Click += (s, e) => { { serv.RemoveEmployee(DgEmployee.SelectedItem as Employee); } };
        }
    }
}