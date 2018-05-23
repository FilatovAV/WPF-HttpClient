using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CompanyEmployeesSQL
{
    public class Service
    {
        HttpClient httpClient = new HttpClient();
        MainWindow WinMainWindow;

        public ObservableCollection<Employee> OcEmployees;
        public ObservableCollection<Department> OcDepartment;

        //Коллекция хранящая отредактированные строки
        ObservableCollection<Employee> EditOcEmployees = new ObservableCollection<Employee>();
        public Employee SelectedEmployee; //Выделенный сотрудник в данный момент
        ObservableCollection<Department> EditOcDepartments = new ObservableCollection<Department>();
        public Department SelectedDepartment; //Выделенный отдел в данный момент

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="mWindow"></param>
        public Service(System.Windows.Window mWindow)
        {
            if (mWindow.GetType() == typeof(MainWindow))
            { WinMainWindow = (MainWindow)mWindow; }
        }
        /// <summary>
        /// Загрузка всех данных из удаленной БД
        /// </summary>
        public void LoadDB()
        {
            var url2 = @"http://localhost:1173/api/Departments";
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var res = httpClient.GetStringAsync(url2).Result;
            OcDepartment = JsonConvert.DeserializeObject<ObservableCollection<Department>>(res);
            
            url2 = @"http://localhost:1173/api/Employees";
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            res = httpClient.GetStringAsync(url2).Result;
            OcEmployees = JsonConvert.DeserializeObject<ObservableCollection<Employee>>(res);
        }
        /// <summary>
        /// Удалить департамент
        /// </summary>
        /// <param name="dep"></param>
        public void RemoveDepartment(Department dep)
        {
            var url2 = @"http://localhost:1173/api/Departments/" + dep.Id; //Строка по которой производиться обращение к таблице
            var res = httpClient.DeleteAsync(url2).Result; //отправляем 
            OcDepartment.Remove(dep);
        }
        /// <summary>
        /// Открыть окно редактирования сотрудников
        /// </summary>
        public void OpenDepartmentsEdit()
        {
            WinEditDepartments wed = new WinEditDepartments(this) { Owner = WinMainWindow };
            wed.ShowDialog();
            //SaveDB();
        }
        /// <summary>
        /// Изменение департамента для сотрудника
        /// </summary>
        /// <param name="emps"></param>
        /// <param name="dep"></param>
        public void DepartmentSet(IEnumerable<Employee> emps, Department dep)
        {
            if (dep==null) { return; }
            foreach (var item in emps)
            {
                item.Department = dep; item.DepartmentId = dep.Id;
                var jsonEmp = JsonConvert.SerializeObject(item);
                var url2 = @"http://localhost:1173/api/Employees/" + item.Id.ToString(); //Строка по которой производиться обращение к таблице
                StringContent stringC = new StringContent(jsonEmp, Encoding.UTF8, "application/json"); //Строка которая будет передаваться web сервису
                var res = httpClient.PutAsync(url2, stringC).Result; //отправляем 
            }
            WinMainWindow.CbDepartmentSet.SelectedIndex = -1;
        }
        /// <summary>
        /// Удалить сотрудника
        /// </summary>
        /// <param name="emp"></param>
        public void RemoveEmployee(Employee emp)
        {
            var url2 = @"http://localhost:1173/api/Employees/" + emp.Id; //Строка по которой производиться обращение к таблице
            var res = httpClient.DeleteAsync(url2).Result; //отправляем 
            OcEmployees.Remove(emp);
        }
        /// <summary>
        /// Сохранить изменения в сущности - сотрудник
        /// </summary>
        /// <param name="e"></param>
        public void SaveEmployee(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Reset) { return; }

            Employee NewEmployee;
            foreach (var item in e.NewItems)
            {
                NewEmployee = item as Employee;
                var jsonEmp = JsonConvert.SerializeObject(NewEmployee);
                var url2 = @"http://localhost:1173/api/Employees/" + NewEmployee.Id.ToString(); //Строка по которой производиться обращение к таблице
                StringContent stringC = new StringContent(jsonEmp, Encoding.UTF8, "application/json"); //Строка которая будет передаваться web сервису
                var res = httpClient.PutAsync(url2, stringC).Result; //отправляем 
            }
        }
        /// <summary>
        /// Сохранить изменения в коллекции сущностей - сотрудник
        /// </summary>
        /// <param name="employees"></param>
        public void SaveEmployees(ObservableCollection<Employee> employees)
        {
            foreach (var item in employees)
            {
                var jsonEmp = JsonConvert.SerializeObject(item);
                var url2 = @"http://localhost:1173/api/Employees/" + item.Id.ToString(); //Строка по которой производиться обращение к таблице
                StringContent stringC = new StringContent(jsonEmp, Encoding.UTF8, "application/json"); //Строка которая будет передаваться web сервису
                var res = httpClient.PutAsync(url2, stringC).Result; //отправляем 
            }
            EditOcEmployees.Clear();
        }
        /// <summary>
        /// Сохранить изменения в коллекции сущностей - отдел
        /// </summary>
        /// <param name="departments"></param>
        public void SaveDepartments(ObservableCollection<Department> departments)
        {
            foreach (Department item in departments)
            {
                var jsonEmp = JsonConvert.SerializeObject(item);
                var url2 = @"http://localhost:1173/api/Departments/" + item.Id.ToString(); //Строка по которой производиться обращение к таблице
                StringContent stringC = new StringContent(jsonEmp, Encoding.UTF8, "application/json"); //Строка которая будет передаваться web сервису
                var res = httpClient.PutAsync(url2, stringC).Result; //отправляем 

                IEnumerable<Employee> query =
                OcEmployees.Where(x => x.DepartmentId == item.Id);
                foreach (Employee emp in query)
                {
                    emp.Department = item;
                    emp.DepartmentId = item.Id;
                }
            }
            EditOcDepartments.Clear();
        }
        /// <summary>
        /// Добавить сотрудника
        /// </summary>
        /// <param name="dep"></param>
        public void AddNewEmployee(Department dep)
        {
            if (dep==null)
            {
                if (OcDepartment.Count==0) { MessageBox.Show("Необходимо добавить отдел.", "Elementary sample", MessageBoxButton.OK, MessageBoxImage.Information); return; }
                dep = OcDepartment[0];
            }
            Employee emp = new Employee { Name = "NewNewNew", Age = 1, Salary = 111, Department = dep, DepartmentId = dep.Id };
            var jsonEmp = JsonConvert.SerializeObject(emp);

            var url2 = @"http://localhost:1173/api/Employees"; //Строка по которой производиться обращение к таблице
            StringContent stringC = new StringContent(jsonEmp, Encoding.UTF8, "application/json"); //Строка которая будет передаваться web сервису
            var res = httpClient.PostAsync(url2, stringC).Result; //отправляем 

            var jsonEmpRet = JsonConvert.DeserializeObject<Employee>(res.Content.ReadAsStringAsync().Result); //Получаем полученый объект в ходе выполнения записи в базу данных
            emp.Id = jsonEmpRet.Id; //передаем нашему объекту реальный ID из базы данных

            OcEmployees.Add(emp);
        }
        /// <summary>
        /// Добавить департамент
        /// </summary>
        /// <param name="inx"></param>
        public void AddNewDepartment(int inx)
        {
            Department dep = new Department { DepartmentName = "NewDepartment", Id = inx };
            var jsonEmp = JsonConvert.SerializeObject(dep);

            var url2 = @"http://localhost:1173/api/Departments"; //Строка по которой производиться обращение к таблице
            StringContent stringC = new StringContent(jsonEmp, Encoding.UTF8, "application/json"); //Строка которая будет передаваться web сервису
            var res = httpClient.PostAsync(url2, stringC).Result; //отправляем 

            var jsonEmpRet = JsonConvert.DeserializeObject<Department>(res.Content.ReadAsStringAsync().Result); //Получаем полученый объект в ходе выполнения записи в базу данных
            dep.Id = jsonEmpRet.Id; //передаем нашему объекту реальный ID из базы данных

            OcDepartment.Add(dep);
        }
        /// <summary>
        /// Добавить сотрудника в коллекцию сохранения
        /// </summary>
        /// <param name="e"></param>
        public void AfterEditEmployee(DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (!EditOcEmployees.Contains(SelectedEmployee))
                {
                    EditOcEmployees.Add(SelectedEmployee);
                }
            }
        }
        /// <summary>
        /// Выполнить сохранение после редактирования
        /// </summary>
        public void SaveAfterEditDG()
        {
            if (EditOcEmployees.Count>0)
            {
                SaveEmployees(EditOcEmployees);
            }
        }
        /// <summary>
        /// Добавить отдел в коллекцию измененных отделов
        /// </summary>
        /// <param name="e"></param>
        public void AfterEditDepartments(DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (!EditOcDepartments.Contains(SelectedDepartment))
                {
                    EditOcDepartments.Add(SelectedDepartment);
                }
            }
        }
        /// <summary>
        /// Сохранить измененные отделы
        /// </summary>
        public void SaveAfterEditDgDepartments()
        {
            if (EditOcDepartments.Count > 0)
            {
                SaveDepartments(EditOcDepartments);
            }
        }
    }
}
