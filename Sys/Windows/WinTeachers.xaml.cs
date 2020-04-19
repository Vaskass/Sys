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
using System.Windows.Shapes;

namespace Sys.Windows
{
    /// <summary>
    /// Логика взаимодействия для WinTeachers.xaml
    /// </summary>
    public partial class WinTeachers : Window
    {
        public SysItems SI = new SysItems();
        public Users currentUser;
        public Subjects currentSubject;
        public Groups currentGroup;
        System.DateTime dt = System.DateTime.Now;
        public WinTeachers(Users u)
        {
            currentUser = u;

            InitializeComponent();
            Title = currentUser.Фамилия + " " + currentUser.Имя + " " + currentUser.Отчество;
        }

        private void DatePicker_Initialized(object sender, EventArgs e)
        {
            date.SelectedDate = DateTime.Now;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(currentSubject.ID_Предмета + " " + currentUser.ID_Пользователя +" "+currentGroup.ID_Группы+" "+dt);
            new WinStudents(currentGroup, currentSubject, currentUser, dt, Convert.ToInt32(NumConbo.Text)).Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            Close();
        }

        private void ComboBox_Initialized(object sender, EventArgs e)
        {
            using (SysItems db = new SysItems())
            {
                List<int> subjectss = new List<int>();
                var IDsub = db.Enabled_Subjects.Where(p => p.ID_Пользователя == currentUser.ID_Пользователя);
                foreach (Enabled_Subjects p in IDsub)
                {
                    subjectss.Add((int)p.ID_Предмета);
                }
                for (int i = 0; i < subjectss.Count; i++)
                {
                    int j = subjectss[i];
                    var sub = db.Subjects.Where(r => r.ID_Предмета == j);
                    foreach (Subjects r in sub) { if (!enabledSubjectsBox.Items.Contains(r.Наименование_предмета)) enabledSubjectsBox.Items.Add(r.Наименование_предмета); break; }

                }

            }
        }

        private void enabledSubjectsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (SysItems db = new SysItems())
            {
                int ID = 0;
                List<int> groups = new List<int>();
                var IDsub = db.Subjects.Where(p => p.Наименование_предмета == enabledSubjectsBox.SelectedItem.ToString());
                foreach (Subjects p in IDsub)
                {
                    ID = p.ID_Предмета;
                    currentSubject = p;
                }
                var sub = db.Enabled_Subjects.Where(r => r.ID_Предмета == ID && r.ID_Пользователя == currentUser.ID_Пользователя);
                foreach (Enabled_Subjects r in sub) { groups.Add((int)r.ID_Группы); } 
                enabledGroupsBox.Items.Clear();
                for (int i = 0; i < groups.Count; i++)
                {
                    int j = groups[i];
                    var gr = db.Groups.Where(r => r.ID_Группы == j);                
                    foreach (Groups r in gr) {  enabledGroupsBox.Items.Add(r.Название_группы);}
                }
            }
            if (enabledGroupsBox.Items.Count > 0)
            {
                OpenButton.IsEnabled = true;
            }
            else { OpenButton.IsEnabled = false; }
            enabledGroupsBox.SelectedIndex = 0;
        }

        private void enabledGroupsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (SysItems db = new SysItems())
            {
                try
                {
                    var gr = db.Groups.Where(r => r.Название_группы == enabledGroupsBox.SelectedItem.ToString());
                    if (gr.FirstOrDefault() != null)
                        currentGroup = gr.FirstOrDefault();
                }
                catch
                {
                    enabledGroupsBox.SelectedItem = -1;
                }
            
            }
        }

        private void date_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            dt = (System.DateTime)date.SelectedDate;
        }

        private void WinReportButton_Click(object sender, RoutedEventArgs e)
        {
            new WinReport(1, currentUser).Show();
        }
    }
}
