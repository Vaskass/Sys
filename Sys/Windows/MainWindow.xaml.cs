using System;
using System.Collections.Generic;
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
using Sys.Windows;
using System.Data.Entity;

namespace Sys
{//как это работает? 
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SysItems SI = new SysItems();
        public MainWindow()
        {
            InitializeComponent();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Users currentUser = new Users();
            var Src = SI.Users.Where(p => p.Логин == Login.Text.ToString() && p.Пароль == Password.Password.ToString());
            Src.Load();
            if(Src.FirstOrDefault()!=null)
            {
            currentUser = Src.FirstOrDefault();
            
                MessageBox.Show("Добро пожаловать, " + currentUser.Имя + " " + currentUser.Фамилия);
                switch (currentUser.Должность)
                { case "Администратор":
                        new WinAdmin().Show();
                        break;
                    case "Преподаватель":
                        new WinTeachers(currentUser).Show();
                        break;
                    case "Завуч":
                        new WinReport(0, currentUser).Show();
                        break;
                    default:
                        break;
                }
                Close();
            }
            else
                MessageBox.Show("Неверный логин или пароль");
            //new WinTeachers().Show();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            SI.Users.Load();
            Login.Items.Clear();

            foreach (Users u in SI.Users.Where(p=> p.Статус==true))
            {
                Login.Items.Add(u.Логин);
            }

        }
    }
}
