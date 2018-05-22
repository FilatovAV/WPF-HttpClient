using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace CompanyEmployeesSQL
{
    /// <summary>
    /// Логика взаимодействия для WinEditDepartments.xaml
    /// </summary>
    public partial class WinEditDepartments : Window
    {
        Service serv;

        public WinEditDepartments(Service sServ)
        {
            InitializeComponent();

            serv = sServ;

            BtnAddNewDepartment.Click += (s, e) => sServ.AddNewDepartment(DgDepartments.Items.Count + 1);
            BtnRemoveDepartment.Click += (s, e) => sServ.RemoveDepartment(DgDepartments.SelectedItem as Department);

            DgDepartments.RowEditEnding += (s, e) => { { serv.AfterEditDepartments(e); } };
            DgDepartments.ItemsSource = serv.OcDepartment;
            DgDepartments.LostFocus += (s, e) => { { serv.SaveAfterEditDgDepartments(); } };
            DgDepartments.SelectionChanged += (s, e) => { { serv.SelectedDepartment = DgDepartments.SelectedItem as Department; } };
        }
    }
}
