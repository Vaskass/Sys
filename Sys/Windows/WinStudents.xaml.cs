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
using System.Windows.Threading;

namespace Sys.Windows
{
    /// <summary>
    /// Логика взаимодействия для WinStudents.xaml
    /// </summary>
    class Table
    {
        public Table(int ID_Студента,int N, string Имя, string Фамилия, string Отчество, bool Пропуск, string Комментарий, int Номер_пары)
        {
            this.ID_Студента = ID_Студента;
            this.N = N;
            this.Фамилия = Фамилия;
            this.Имя = Имя;
            this.Отчество = Отчество;
            this.Пропуск = Пропуск;
            this.Комментарий = Комментарий;
            this.Номер_пары = Номер_пары;
        }
        public int ID_Студента { get; set; }
        public int N { get; set; }
        public string Фамилия { get; set; }
        public string Имя { get; set; }
        public string Отчество { get; set; }
        public bool Пропуск { get; set; }
        public string Комментарий { get; set; }
        public int Номер_пары { get; set; }

    }
    public partial class WinStudents : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        Users currentUser;
        Subjects currentSubject;
        Groups currentGroup;
        System.DateTime dt;
        int timeTimer = 4;

        List<Table> Students_Table = new List<Table>();
        int NumPar;
        public WinStudents(Groups g, Subjects s, Users u, System.DateTime d, int numPar)
        {
            currentUser = new Users(); currentSubject = new Subjects(); currentGroup = new Groups(); dt = new DateTime();
            InitializeComponent();
            currentGroup = g;
            currentSubject = s;
            currentUser = u;
            dt = d;
            NumPar = numPar;
            this.Title = "Группа: "  + currentGroup.Название_группы + " Пара: " + numPar + " Дата: " + dt.Date.ToShortDateString();
            Grid_Initialized(null, null);

            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timeTimer--;
            if (timeTimer == 0)
            {
                timer.Stop();
                timeTimer = 4;
                SaveButton.Content = "Сохранить";
                InfoText.Content = "Последнее сохранение: " + System.DateTime.Now.ToLongTimeString();
            }
            else
            {
                if (timeTimer == 3)SaveButton.Content = "Сохранение.";
                if (timeTimer == 2) SaveButton.Content = "Сохранение..";
                if (timeTimer == 1) SaveButton.Content = "Сохранение...";
            }
        }

        private void Table_size(object sender, EventArgs e)
        {
                StudTable_LoadingRow(null,null);
        }

        private void Grid_Initialized(object sender, EventArgs e)
        {
            int numerik = 0;
            using (SysItems db = new SysItems())
            {
                var gr = db.Students.Where(r => r.ID_Группы == currentGroup.ID_Группы).OrderBy(p=>p.Фамилия);
                foreach (Students p in gr)
                {
                    numerik++;
                    Table Add = new Table(0,numerik,null, null, null, false, "",1);
                    //проверка есть ли пропуск на выбранную дату
                    Add.Пропуск = false;
                    var dr = db.Skips.Where(r => r.ID_Студента == p.ID_Студента && r.Дата == dt && r.ID_Предмета == currentSubject.ID_Предмета && r.ID_Пользователя == currentUser.ID_Пользователя && r.Номер_пары == NumPar);
                    foreach (Skips r in dr)
                    {
                        Add.Пропуск = true;
                        Add.Комментарий = (string)r.Комментарий;
                    }
                    ///////////////////////////////////////////
                    Add.Имя = p.Имя;
                    Add.Фамилия = p.Фамилия;
                    Add.Отчество = p.Отчество;
                    Add.ID_Студента = p.ID_Студента;
                    Add.N = numerik;

                    Students_Table.Add(Add);

                }
                StudTable.ItemsSource = Students_Table;





            }
        }

        private void StudTable_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            StudTable.Columns[0].Visibility = Visibility.Hidden;
            StudTable.Columns[7].Visibility = Visibility.Hidden;
            StudTable.Columns[2].IsReadOnly = true;
            StudTable.Columns[3].IsReadOnly = true;
            StudTable.Columns[4].IsReadOnly = true;

            double size = this.ActualWidth;
            StudTable.Columns[1].Width = size * 0.03;

            StudTable.Columns[2].Width = size * 0.2;

            StudTable.Columns[3].Width = size * 0.2;

            StudTable.Columns[4].Width = size * 0.2;

            StudTable.Columns[5].Width = size * 0.1;

            StudTable.Columns[6].Width = size * 0.23;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            timer.Start();
            List<Table> SaveTable = new List<Table>();
            SaveTable = (List<Table>)StudTable.ItemsSource;
            using (SysItems db = new SysItems())
            {
                for (int i = 0; i < SaveTable.Count; i++)
            {
                int j = SaveTable[i].ID_Студента;
                    Skips skip = new Skips();
                    if (SaveTable[i].Пропуск == false)
                    {
                        var dr = db.Skips.Where(r => r.ID_Студента == j && r.Дата == dt && r.ID_Предмета == currentSubject.ID_Предмета && r.ID_Пользователя == currentUser.ID_Пользователя && r.Номер_пары == NumPar);
                        foreach (Skips r in dr)
                        {
                            db.Skips.Remove(r);
                        }
                    } else
                    {
                        var zp = db.Skips.Where(r => r.ID_Студента == j && r.Дата == dt && r.ID_Предмета == currentSubject.ID_Предмета && r.ID_Пользователя == currentUser.ID_Пользователя && r.Номер_пары == NumPar);
                        foreach (Skips r in zp)
                        {
                            db.Skips.Remove(r); break;
                        }
                            skip.ID_Пользователя = currentUser.ID_Пользователя;
                            skip.ID_Предмета = currentSubject.ID_Предмета;
                            skip.ID_Студента = SaveTable[i].ID_Студента;
                            skip.Дата = dt;
                            skip.Комментарий = SaveTable[i].Комментарий;
                            skip.Номер_пары = NumPar;
                            db.Skips.Add(skip);
                       
                    }
            }
            db.SaveChanges();
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            StudTable_LoadingRow(null, null);
        }



    }
}
