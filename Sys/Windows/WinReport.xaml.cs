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
using Microsoft.EntityFrameworkCore;
using Excel = Microsoft.Office.Interop.Excel;

namespace Sys.Windows
{
    /// <summary>
    /// Логика взаимодействия для WinReport.xaml
    /// </summary>



    // Класс для представления пропусков по группе
    class groupsk
    {
        public groupsk(int ID_Студента, int N, string Имя, string Фамилия, string Отчество, int Количество_пропусков)
        {
            this.ID_Студента = ID_Студента;
            this.N = N;
            this.Фамилия = Фамилия;
            this.Имя = Имя;
            this.Отчество = Отчество;
            this.Количество_пропусков = Количество_пропусков;

        }
        public int ID_Студента { get; set; }
        public int N { get; set; }
        public string Фамилия { get; set; }
        public string Имя { get; set; }
        public string Отчество { get; set; }
        public int Количество_пропусков { get; set; }

    }

    // Класс для представления пропусков по студенту
    class studentsk
    {
        public studentsk(int N, string Предмет, DateTime Дата, int Номер_пары, string Комментарий)
        {
            this.N = N;
            this.Предмет = Предмет;
            this.Дата = Дата;
            this.Номер_пары = Номер_пары;
            this.Комментарий = Комментарий;
        }
        public int N { get; set; }
        public string Предмет { get; set; }
        public DateTime Дата { get; set; }
        public int Номер_пары { get; set; }
        public string Комментарий { get; set; }

    }

    // Класс для представления пропусков по колледжу
    class collegesk
    {
        public collegesk(int N, string Наименование_группы, string год_поступления, int общее_количество_пропусков)
        {
            this.N = N;
            this.Наименование_группы = Наименование_группы;
            this.год_поступления = год_поступления;
            this.общее_количество_пропусков = общее_количество_пропусков;
        }
        public int N { get; set; }
        public string Наименование_группы { get; set; }
        public string год_поступления { get; set; }
        public int общее_количество_пропусков { get; set; }

    }
    public partial class WinReport : Window
    {
        public WinReport(int mode, Users user)
        {
            InitializeComponent();
            RadioGroup.IsChecked = true;
            using (SysItems db = new SysItems())
            {
                var gr = db.Groups.Where(r => r.Статус == true);
                var tc = db.Groups.Where(r => r.ID_Пользователя == user.ID_Пользователя);
                var sb = db.Subjects;
                foreach (Groups r in gr) { Group_ComboBox.Items.Add(r.Название_группы); }
                foreach (Subjects r in sb) { Subject_ComboBox.Items.Add(r.Наименование_предмета); Subjects_ComboBox.Items.Add(r.Наименование_предмета); }
                inactivesub_Click(null, null);
                if (tc.FirstOrDefault() != null)
                    currentGroupTeacher = tc.FirstOrDefault();
                date_1.SelectedDate = DateTime.Now.Date.AddDays(-1);
                date_2.SelectedDate = DateTime.Now.Date;
                date_3.SelectedDate = DateTime.Now.Date.AddDays(-1);
                date_4.SelectedDate = DateTime.Now.Date;
                Subject_ComboBox.SelectedIndex = 0;
                Subjects_ComboBox.SelectedIndex = 0;
                Subject_ComboBox_SelectionChanged(null, null);
                Group_ComboBox.SelectedIndex = 0;
                Group_ComboBox_SelectionChanged(null, null);
                //Student_ComboBox.SelectedIndex = 0;
                // Student_ComboBox_SelectionChanged(null, null);

            }
            currentUser = user;
            YourGroupLabel.Content = "Ваша группа: " + currentGroupTeacher.Название_группы;


            if (mode == 1)
            {
                tabcontrol.Items.RemoveAt(0);
                currentGroup = currentGroupTeacher;
            }
            else
            {
                tabcontrol.Items.RemoveAt(1);
            }

            //ListStudents.Items.Clear();
            using (SysItems db = new SysItems())
            {

                var st = db.Students.Where(r => r.ID_Группы == currentGroupTeacher.ID_Группы).OrderBy(r => r.Фамилия);
                foreach (Students r in st) { ListStudents.Items.Add(r.Фамилия + " " + r.Имя + " " + r.Отчество); }


            }
            if (ListStudents.Items.Count > 0)
            {
                ListStudents.SelectedIndex = 0;
            }
        }







        List<groupsk> GroupSkips = new List<groupsk>();
        List<studentsk> StudentSkips = new List<studentsk>();
        List<collegesk> CollegeSkips = new List<collegesk>();

        Groups currentGroup = new Groups();
        Students currentStudent = new Students();
        Subjects currentSubject = new Subjects();
        Users currentUser = new Users();
        Groups currentGroupTeacher = new Groups();




        private void ShowGroupSkips()
        {
            int num = 0;
            int countSkips = 0;
            GroupSkips.Clear();
            using (SysItems db = new SysItems())
            {
                if (RadioGroup.IsChecked == true)
                {
                    if (Group_ComboBox.SelectedItem != null)
                    {
                        var st = db.Students.Where(r => r.ID_Группы == currentGroup.ID_Группы).OrderBy(r => r.Фамилия);
                        foreach (Students r in st)
                        {
                            num++;
                            countSkips = 0;
                            if (CheckSubject.IsChecked == false)
                            {
                                var skips = db.Skips.Where(d => d.ID_Студента == r.ID_Студента);
                                if (AllTimeCheckButton.IsChecked == true)
                                {
                                    skips = db.Skips.Where(d => d.ID_Студента == r.ID_Студента);
                                }
                                else
                                {
                                    skips = db.Skips.Where(d => d.ID_Студента == r.ID_Студента && d.Дата >= date_1.SelectedDate && d.Дата <= date_2.SelectedDate);
                                }
                                foreach (Skips d in skips) { countSkips++; }
                                groupsk AddString = new groupsk(r.ID_Студента, num, r.Имя, r.Фамилия, r.Отчество, countSkips);
                                GroupSkips.Add(AddString);

                            }
                            else
                            {
                                var skips = db.Skips.Where(d => d.ID_Студента == r.ID_Студента && d.ID_Предмета == currentSubject.ID_Предмета);
                                if (AllTimeCheckButton.IsChecked == true)
                                {
                                    skips = db.Skips.Where(d => d.ID_Студента == r.ID_Студента && d.ID_Предмета == currentSubject.ID_Предмета);
                                }
                                else
                                {
                                    skips = db.Skips.Where(d => d.ID_Студента == r.ID_Студента && d.Дата >= date_1.SelectedDate && d.Дата <= date_2.SelectedDate && d.ID_Предмета == currentSubject.ID_Предмета);
                                }
                                foreach (Skips d in skips) { countSkips++; }
                                groupsk AddString = new groupsk(r.ID_Студента, num, r.Имя, r.Фамилия, r.Отчество, countSkips);
                                GroupSkips.Add(AddString);
                            }
                        }
                        createExcelTable(0);
                    }
                }
            }
        }







        private void ShowStudentSkips()
        {
            int num = 0;
            string subname = "";
            StudentSkips.Clear();
            using (SysItems db = new SysItems())
            {
                if (RadioStudent.IsChecked == true)
                {
                    if (Student_ComboBox.SelectedItem != null)
                    {
                        var st = db.Skips.Where(r => r.ID_Студента == currentStudent.ID_Студента).OrderBy(r => r.Дата);
                        if (AllTimeCheckButton.IsChecked == true)
                        {
                            st = db.Skips.Where(r => r.ID_Студента == currentStudent.ID_Студента).OrderBy(r => r.Дата);
                        }
                        else
                        {
                            st = db.Skips.Where(r => r.ID_Студента == currentStudent.ID_Студента && r.Дата >= date_1.SelectedDate && r.Дата <= date_2.SelectedDate).OrderBy(r => r.Дата);
                        }
                        foreach (Skips r in st)
                        {
                            num++;
                            var sub = db.Subjects.Where(s => s.ID_Предмета == r.ID_Предмета);
                            if (sub.FirstOrDefault() != null)
                                subname = sub.FirstOrDefault().Наименование_предмета;
                            if (CheckSubject.IsChecked == true)
                            {
                                if (Subject_ComboBox.SelectedItem != null)
                                    if (subname == Subject_ComboBox.SelectedItem.ToString())
                                    {
                                        studentsk AddSkip = new studentsk(num, subname, (DateTime)r.Дата, (int)r.Номер_пары, r.Комментарий);
                                        StudentSkips.Add(AddSkip);
                                    }
                            }
                            else
                            {
                                studentsk AddSkip = new studentsk(num, subname, (DateTime)r.Дата, (int)r.Номер_пары, r.Комментарий);
                                StudentSkips.Add(AddSkip);
                            }
                        }

                        createExcelTable(1);
                    }
                }
            }
        }




        private void ShowCollegeSkips()
        {
            int num = 0;
            int count = 0;
            CollegeSkips.Clear();
            using (SysItems db = new SysItems())
            {
                if (RadioCollege.IsChecked == true)
                {

                    var st = db.Groups.Where(r => r.Статус == true).OrderBy(r => r.Год_поступления);
                    if (InactiveCheckButton.IsChecked == true)
                    {
                        st = db.Groups.Where(r => r.Статус == false).OrderBy(r => r.Год_поступления);
                    }
                    foreach (Groups r in st)
                    {
                        count = 0;
                        num++;
                        var zapros = (from sa in db.Students where sa.ID_Группы == r.ID_Группы select sa.ID_Студента);
                        foreach (var j in zapros)
                        {
                            var command = db.Skips.Where(s => s.ID_Студента == j);
                            if (AllTimeCheckButton.IsChecked == true)
                            {
                                command = db.Skips.Where(s => s.ID_Студента == j);
                                if (CheckSubject.IsChecked == true)
                                {
                                    command = db.Skips.Where(s => s.ID_Студента == j && s.ID_Предмета == currentSubject.ID_Предмета);
                                }
                            }
                            else
                            {
                                command = db.Skips.Where(s => s.ID_Студента == j && s.Дата >= date_1.SelectedDate && s.Дата <= date_2.SelectedDate);
                                if (CheckSubject.IsChecked == true)
                                {
                                    command = db.Skips.Where(s => s.ID_Студента == j && s.Дата >= date_1.SelectedDate && s.Дата <= date_2.SelectedDate && s.ID_Предмета == currentSubject.ID_Предмета);
                                }
                            }
                            foreach (var l in command) { count++; }
                        }

                        collegesk AddSkip = new collegesk(num, r.Название_группы, r.Год_поступления.ToString(), count);
                        CollegeSkips.Add(AddSkip);

                    }
                    createExcelTable(2);
                }
            }
        }


        private void buttonCreateReport_Click(object sender, RoutedEventArgs e)
        {
            ShowGroupSkips();
            ShowStudentSkips();
            ShowCollegeSkips();
        }

        private void CheckSubject_Click(object sender, RoutedEventArgs e)
        {
            if (CheckSubject.IsChecked == true)
            {
                Subject.IsEnabled = true;
            }
            else
            {
                Subject.IsEnabled = false;
            }
        }

        private void RadioStudent_Checked(object sender, RoutedEventArgs e)
        {
            Student.IsEnabled = true;
            Group.IsEnabled = true;
            Student_ComboBox.SelectedIndex = 0;
            Student_ComboBox_SelectionChanged(null, null);
        }

        private void RadioGroup_Checked(object sender, RoutedEventArgs e)
        {
            Student.IsEnabled = false;
            Group.IsEnabled = true;
        }

        private void RadioCollege_Checked(object sender, RoutedEventArgs e)
        {
            Student.IsEnabled = false;
            Group.IsEnabled = false;
        }

        private void Group_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Student_ComboBox.Items.Clear();
            if (Group_ComboBox.SelectedItem != null)
            {

                using (SysItems db = new SysItems())
                {
                    var gr = db.Groups.Where(r => r.Название_группы == Group_ComboBox.SelectedItem.ToString());
                    if (gr.FirstOrDefault() != null)
                    {
                        currentGroup = gr.FirstOrDefault();
                        var st = db.Students.Where(r => r.ID_Группы == currentGroup.ID_Группы && r.Статус == true).OrderBy(r => r.Фамилия);
                        if (inactivest.IsChecked == true) st = db.Students.Where(r => r.ID_Группы == currentGroup.ID_Группы && r.Статус == false).OrderBy(r => r.Фамилия);
                        foreach (Students r in st) { Student_ComboBox.Items.Add(r.Фамилия + " " + r.Имя + " " + r.Отчество); }
                    }
                }
                Student_ComboBox.SelectedIndex = 0;
                if (Student_ComboBox.SelectedItem != null)
                    Student_ComboBox_SelectionChanged(null, null);
            }
        }

        private void Student_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Student_ComboBox.SelectedItem != null)
            {
                string stroke = Student_ComboBox.SelectedItem.ToString();
                string[] array = stroke.Split(' ');
                string name = array[1], famile = array[0], otch = array[2];
                using (SysItems db = new SysItems())
                {
                    var st = db.Students.Where(r => r.Фамилия == famile && r.Имя == name && r.Отчество == otch);
                    if (st.FirstOrDefault() != null)
                        currentStudent = st.FirstOrDefault();
                }
            }
        }

        private void Subject_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (SysItems db = new SysItems())
            {
                if (Subject_ComboBox.Items.Count>0)
                {
                    var st = db.Subjects.Where(r => r.Наименование_предмета == Subject_ComboBox.SelectedItem.ToString());
                    if (st.FirstOrDefault() != null)
                        currentSubject = st.FirstOrDefault();
                }
            }
        }

        private void InactiveCheckButton_Click(object sender, RoutedEventArgs e)
        {
            using (SysItems db = new SysItems())
            {
                Group_ComboBox.Items.Clear();
                if (InactiveCheckButton.IsChecked == true)
                {
                    var gr = db.Groups.Where(r => r.Статус == false);
                    foreach (Groups r in gr) { Group_ComboBox.Items.Add(r.Название_группы); }
                }
                else
                {
                    var gr = db.Groups.Where(r => r.Статус == true);
                    foreach (Groups r in gr) { Group_ComboBox.Items.Add(r.Название_группы); }
                }
                if (Group_ComboBox.Items.Count != 0)
                {
                    Group_ComboBox.SelectedIndex = 0;
                }
            }
        }

        // Метод создания таблицы эксель
        // 0 - по группе; 1 - по студенту; 2 - по колледжу
        private void createExcelTable(int Mod)
        {
            int num = 1;
            int count = 0;
            Excel.Application ex = new Microsoft.Office.Interop.Excel.Application();
            Excel.Worksheet sheet = new Excel.Worksheet();
            ex.SheetsInNewWorkbook = 1;
            Excel.Workbook workBook = ex.Workbooks.Add(Type.Missing);
            sheet = (Excel.Worksheet)ex.Worksheets.get_Item(1);
            ex.DisplayAlerts = false;

            if (Mod == 0)
            {
                sheet.Name = "Отчет по пропускам";
                sheet.Cells[1, 1] = "Номер";
                sheet.Cells[1, 2] = "Фамилия";
                sheet.Cells[1, 3] = "Имя";
                sheet.Cells[1, 4] = "Отчество";
                sheet.Cells[1, 5] = "Количество пропусков";
                for (int i = 0; i < GroupSkips.Count(); i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        switch (j)
                        {
                            case 0: sheet.Cells[i + 2, j + 1] = GroupSkips[i].N; break;
                            case 1: sheet.Cells[i + 2, j + 1] = GroupSkips[i].Фамилия; break;
                            case 2: sheet.Cells[i + 2, j + 1] = GroupSkips[i].Имя; break;
                            case 3: sheet.Cells[i + 2, j + 1] = GroupSkips[i].Отчество; break;
                            case 4: sheet.Cells[i + 2, j + 1] = GroupSkips[i].Количество_пропусков; count += GroupSkips[i].Количество_пропусков; break;
                        }
                    }
                    num++;
                }
                num++;
                sheet.Cells[num, 4] = "Всего:"; sheet.Cells[num, 5] = count;

                Excel.Range range = sheet.get_Range("B1", "E" + num);
                range.EntireColumn.AutoFit();
                range.EntireRow.AutoFit();
                range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;


                Excel.Range range2 = sheet.get_Range("A1", "A" + num);
                range2.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                range2.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;


                Excel.Range range3 = sheet.get_Range("A1", "E1");
                range3.Cells.Font.Bold = true;

                Excel.Range range4 = sheet.get_Range("D" + num, "D" + num);
                range4.Cells.Font.Bold = true;

                if (AllTimeCheckButton.IsChecked == false && TeacherAllTime.IsChecked == false)
                    sheet.Cells[num + 2, 1] = "Отчет по пропускам группы " + currentGroup.Название_группы + " за промежуток с " + date_1.SelectedDate.Value.ToShortDateString() + " по " + date_2.SelectedDate.Value.ToShortDateString();
                else
                    sheet.Cells[num + 2, 1] = "Отчет по пропускам группы " + currentGroup.Название_группы + " за всё время";

                if (CheckSubject.IsChecked == true || CheckSubject_2.IsChecked == true) { sheet.Cells[num + 4, 1] = "Пропуски предоставлены по данному предмету: " + currentSubject.Наименование_предмета; }

                Excel.Sheets worksheets = workBook.Worksheets;
            }
            if (Mod == 1)
            {
                sheet.Name = "Отчет по пропускам";
                sheet.Cells[2, 1] = "Номер";
                sheet.Cells[2, 2] = "Наименование пары";
                sheet.Cells[2, 3] = "Номер пары";
                sheet.Cells[2, 4] = "Дата";
                sheet.Cells[2, 5] = "Комментарий";
                for (int i = 0; i < StudentSkips.Count(); i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        switch (j)
                        {
                            case 0: sheet.Cells[i + 3, j + 1] = StudentSkips[i].N; break;
                            case 1: sheet.Cells[i + 3, j + 1] = StudentSkips[i].Предмет; break;
                            case 2: sheet.Cells[i + 3, j + 1] = StudentSkips[i].Номер_пары; break;
                            case 3: sheet.Cells[i + 3, j + 1] = StudentSkips[i].Дата; break;
                            case 4: sheet.Cells[i + 3, j + 1] = StudentSkips[i].Комментарий; break;
                        }
                    }
                    count++;
                    num++;
                }
                num += 2;
                sheet.Cells[num, 4] = "Всего:"; sheet.Cells[num, 5] = count;

                Excel.Range range = sheet.get_Range("B1", "E" + num);
                range.EntireColumn.AutoFit();
                range.EntireRow.AutoFit();
                range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;


                Excel.Range range2 = sheet.get_Range("A1", "A" + num);
                range2.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                range2.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                Excel.Range range3 = sheet.get_Range("A2", "E2");
                range3.Cells.Font.Bold = true;

                Excel.Range range4 = sheet.get_Range("D" + num, "D" + num);
                range4.Cells.Font.Bold = true;

                Excel.Range range5 = sheet.get_Range("A1", "E1");
                range5.Merge(true);

                if (AllTimeCheckButton.IsChecked == false && TeacherAllTime.IsChecked == false)
                    sheet.Cells[num + 2, 1] = "Отчет по пропускам за промежуток с " + date_1.SelectedDate.Value.ToShortDateString() + " по " + date_2.SelectedDate.Value.ToShortDateString();
                else
                    sheet.Cells[num + 2, 1] = "Отчет по пропускам за всё время";
                sheet.Cells[1, 1] = currentStudent.Фамилия + " " + currentStudent.Имя + " " + currentStudent.Отчество + " " + currentGroup.Название_группы;

                if (CheckSubject.IsChecked == true || CheckSubject_2.IsChecked == true) { sheet.Cells[num + 4, 1] = "Пропуски предоставлены по данному предмету: " + currentSubject.Наименование_предмета; }

                Excel.Sheets worksheets = workBook.Worksheets;
            }
            if (Mod == 2)
            {
                sheet.Name = "Отчет по пропускам";
                sheet.Cells[2, 1] = "Номер";
                sheet.Cells[2, 2] = "Наименование группы";
                sheet.Cells[2, 3] = "Год поступления";
                sheet.Cells[2, 4] = "Количество пропусков ";
                for (int i = 0; i < CollegeSkips.Count(); i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        switch (j)
                        {
                            case 0: sheet.Cells[i + 3, j + 1] = CollegeSkips[i].N; break;
                            case 1: sheet.Cells[i + 3, j + 1] = CollegeSkips[i].Наименование_группы; break;
                            case 2: sheet.Cells[i + 3, j + 1] = CollegeSkips[i].год_поступления; break;
                            case 3: sheet.Cells[i + 3, j + 1] = CollegeSkips[i].общее_количество_пропусков; count += CollegeSkips[i].общее_количество_пропусков; break;
                        }
                    }
                    num++;
                }
                num += 2;
                sheet.Cells[num, 3] = "Всего:"; sheet.Cells[num, 4] = count;

                Excel.Range range = sheet.get_Range("A1", "D" + num);
                range.EntireColumn.AutoFit();
                range.EntireRow.AutoFit();
                range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;


                Excel.Range range2 = sheet.get_Range("A1", "A" + num);
                range2.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                range2.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;


                Excel.Range range3 = sheet.get_Range("A2", "D2");
                range3.Cells.Font.Bold = true;

                Excel.Range range4 = sheet.get_Range("C" + num, "D" + num);
                range4.Cells.Font.Bold = true;

                Excel.Range range5 = sheet.get_Range("A1", "D1");
                range5.Merge(true);

                if (AllTimeCheckButton.IsChecked == false)
                    sheet.Cells[num + 2, 1] = "Отчет по пропускам за промежуток с " + date_1.SelectedDate.Value.ToShortDateString() + " по " + date_2.SelectedDate.Value.ToShortDateString();
                else
                    sheet.Cells[num + 2, 1] = "Отчет по пропускам за всё время";
                sheet.Cells[1, 1] = "Отчет по пропускам по колледжу";

                if (CheckSubject.IsChecked == true) { sheet.Cells[num + 4, 1] = "Пропуски предоставлены по данному предмету: " + currentSubject.Наименование_предмета; }

                Excel.Sheets worksheets = workBook.Worksheets;
            }
            ex.Visible = true;

        }

        private void date_2_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            date_2.SelectedDate = date_2.SelectedDate < date_1.SelectedDate ? date_1.SelectedDate : date_2.SelectedDate;
        }

        private void date_1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            date_2.SelectedDate = date_2.SelectedDate < date_1.SelectedDate ? date_1.SelectedDate : date_2.SelectedDate;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            Close();
        }

        private void ButtonReportGroup_Click(object sender, RoutedEventArgs e)
        {
            int num = 0;
            int countSkips = 0;
            GroupSkips.Clear();
            using (SysItems db = new SysItems())
            {
                var st = db.Students.Where(r => r.ID_Группы == currentGroupTeacher.ID_Группы).OrderBy(r => r.Фамилия);
                foreach (Students r in st)
                {
                    num++;
                    countSkips = 0;
                    if (CheckSubject_2.IsChecked == false)
                    {
                        var skips = db.Skips.Where(d => d.ID_Студента == r.ID_Студента);
                        if (TeacherAllTime.IsChecked == true)
                        {
                            skips = db.Skips.Where(d => d.ID_Студента == r.ID_Студента);
                        }
                        else
                        {
                            skips = db.Skips.Where(d => d.ID_Студента == r.ID_Студента && d.Дата >= date_3.SelectedDate && d.Дата <= date_4.SelectedDate);
                        }
                        foreach (Skips d in skips) { countSkips++; }
                        groupsk AddString = new groupsk(r.ID_Студента, num, r.Имя, r.Фамилия, r.Отчество, countSkips);
                        GroupSkips.Add(AddString);

                    }
                    else
                    {
                        var skips = db.Skips.Where(d => d.ID_Студента == r.ID_Студента && d.ID_Предмета == currentSubject.ID_Предмета);
                        if (TeacherAllTime.IsChecked == true)
                        {
                            skips = db.Skips.Where(d => d.ID_Студента == r.ID_Студента && d.ID_Предмета == currentSubject.ID_Предмета);
                        }
                        else
                        {
                            skips = db.Skips.Where(d => d.ID_Студента == r.ID_Студента && d.Дата >= date_3.SelectedDate && d.Дата <= date_4.SelectedDate && d.ID_Предмета == currentSubject.ID_Предмета);
                        }
                        foreach (Skips d in skips) { countSkips++; }
                        groupsk AddString = new groupsk(r.ID_Студента, num, r.Имя, r.Фамилия, r.Отчество, countSkips);
                        GroupSkips.Add(AddString);
                    }
                }
                createExcelTable(0);
            }
        }

        private void Subjects_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (SysItems db = new SysItems())
            {
                if (Subjects_ComboBox.Items.Count > 0)
                {
                    var st = db.Subjects.Where(r => r.Наименование_предмета == Subjects_ComboBox.SelectedItem.ToString());

                    if (st.FirstOrDefault() != null)
                        currentSubject = st.FirstOrDefault();

                }
            }

        }

        private void date_3_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            date_4.SelectedDate = date_4.SelectedDate < date_3.SelectedDate ? date_3.SelectedDate : date_4.SelectedDate;
        }

        private void date_4_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            date_4.SelectedDate = date_4.SelectedDate < date_3.SelectedDate ? date_3.SelectedDate : date_4.SelectedDate;
        }

        private void CheckSubject_2_Click(object sender, RoutedEventArgs e)
        {
            if (CheckSubject_2.IsChecked == false)
            {
                SubjectBox.IsEnabled = false;
            }
            else
            {
                SubjectBox.IsEnabled = true;
            }
        }

        private void ListStudents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ButtonReportStudent_Click(object sender, RoutedEventArgs e)
        {
            if (ListStudents.Items.Count > 0)
            {
                string stroke = ListStudents.SelectedItem.ToString();
                string[] array = stroke.Split(' ');
                string name = array[1], famile = array[0], otch = array[2];
                using (SysItems db = new SysItems())
                {
                    var st = db.Students.Where(r => r.Фамилия == famile && r.Имя == name && r.Отчество == otch);
                    if (st.FirstOrDefault() != null)
                        currentStudent = st.FirstOrDefault();
                }


                int num = 0;
                string subname = "";
                StudentSkips.Clear();
                using (SysItems db = new SysItems())
                {
                    var st = db.Skips.Where(r => r.ID_Студента == currentStudent.ID_Студента).OrderBy(r => r.Дата);
                    if (TeacherAllTime.IsChecked == true)
                    {
                        st = db.Skips.Where(r => r.ID_Студента == currentStudent.ID_Студента).OrderBy(r => r.Дата);
                    }
                    else
                    {
                        st = db.Skips.Where(r => r.ID_Студента == currentStudent.ID_Студента && r.Дата >= date_3.SelectedDate && r.Дата <= date_4.SelectedDate).OrderBy(r => r.Дата);
                    }
                    foreach (Skips r in st)
                    {
                        num++;
                        var sub = db.Subjects.Where(s => s.ID_Предмета == r.ID_Предмета);
                        if (sub.FirstOrDefault() != null)
                            subname = sub.FirstOrDefault().Наименование_предмета;
                        if (CheckSubject_2.IsChecked == true)
                        {
                            if (subname == Subjects_ComboBox.SelectedItem.ToString())
                            {
                                studentsk AddSkip = new studentsk(num, subname, (DateTime)r.Дата, (int)r.Номер_пары, r.Комментарий);
                                StudentSkips.Add(AddSkip);
                            }
                        }
                        else
                        {
                            studentsk AddSkip = new studentsk(num, subname, (DateTime)r.Дата, (int)r.Номер_пары, r.Комментарий);
                            StudentSkips.Add(AddSkip);
                        }
                    }

                    createExcelTable(1);


                }
            }
        }


        private void CreateUniversalExcelTable()
        {
            Groups cGroup = new Groups();
            Students cStudent = new Students();

            Excel.Application ex = new Microsoft.Office.Interop.Excel.Application();
            Excel.Worksheet sheet = new Excel.Worksheet();
            ex.SheetsInNewWorkbook = 1;
            Excel.Workbook workBook = ex.Workbooks.Add(Type.Missing);
            sheet = (Excel.Worksheet)ex.Worksheets.get_Item(1);
            ex.DisplayAlerts = false;

            Excel.Range range = sheet.get_Range("A1", "A1");

            int Gnum = 2, n1 = 0, n2 = 0, n3 = 0;
            int Gcount = 0, Scount = 0, Allcount = 0;
            int startG = 0, endG = 0, startS = 0, endS = 0;
            string subname = "";


            sheet.Cells[1, 1] = "FullReport";
            sheet.Cells[2, 1] = "Группы:";

            Gnum++;
            sheet.Name = "Отчет по пропускам";

            range = sheet.get_Range("A" + Gnum, "D" + Gnum);
            range.Cells.Font.Bold = true;

            sheet.Cells[Gnum, 1] = "Номер";
            sheet.Cells[Gnum, 2] = "Наименование группы";
            sheet.Cells[Gnum, 3] = "Год поступления";
            sheet.Cells[Gnum, 4] = "Количество пропусков ";


            using (SysItems db = new SysItems())
            {
                var groups = db.Groups.Where(g => g.Статус == InactiveCheckButton.IsChecked == false ? true : false);
                foreach (Groups g in groups)
                {
                    Gnum++;
                    sheet.Cells[Gnum, 1] = "Номер";
                    sheet.Cells[Gnum, 2] = "Фамилия";
                    sheet.Cells[Gnum, 3] = "Имя";
                    sheet.Cells[Gnum, 4] = "Отчество";
                    sheet.Cells[Gnum, 5] = "Количество пропусков";

                    range = sheet.get_Range("A" + Gnum, "E" + Gnum);
                    range.Cells.Font.Bold = true;
                    startG = Gnum;
                    var students = db.Students.Where(s => s.ID_Группы == g.ID_Группы);
                    foreach (Students s in students)
                    {
                        n3 = 0;
                        Gnum++;
                        sheet.Cells[Gnum, 1] = "Номер";
                        sheet.Cells[Gnum, 2] = "Наименование пары";
                        sheet.Cells[Gnum, 3] = "Номер пары";
                        sheet.Cells[Gnum, 4] = "Дата";
                        sheet.Cells[Gnum, 5] = "Комментарий";

                        range = sheet.get_Range("A" + Gnum, "E" + Gnum);
                        range.Cells.Font.Bold = true;

                        var skips = db.Skips.Where(sk => sk.ID_Студента == s.ID_Студента);
                        if (AllTimeCheckButton.IsChecked == false)
                        {
                            skips = db.Skips.Where(sk => sk.ID_Студента == s.ID_Студента && sk.Дата >= date_1.SelectedDate && sk.Дата <= date_2.SelectedDate);
                        }
                        startS = Gnum;
                        foreach (Skips sk in skips)
                        {
                            n3++;
                            Gnum++;
                            sheet.Cells[Gnum, 1] = n3;

                            //Получение имени предмета
                            var sub = db.Subjects.Where(su => su.ID_Предмета == sk.ID_Предмета);
                            if (sub.FirstOrDefault() != null)
                                subname = sub.FirstOrDefault().Наименование_предмета;
                            //.......................

                            sheet.Cells[Gnum, 2] = subname;
                            sheet.Cells[Gnum, 3] = sk.Номер_пары;
                            sheet.Cells[Gnum, 4] = sk.Дата;
                            sheet.Cells[Gnum, 5] = sk.Комментарий;

                            range = sheet.get_Range("A" + Gnum, "E" + Gnum);
                            range.Cells.Interior.Color = Excel.XlRgbColor.rgbLightYellow;
                            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        }
                        endS = Gnum;
                        range = sheet.get_Range("A" + startS, "E" + endS);
                        range.Rows.Group();
                        Gnum++;

                        n2++;
                        Scount = 0;
                        sheet.Cells[Gnum, 1] = n2;
                        sheet.Cells[Gnum, 2] = s.Фамилия;
                        sheet.Cells[Gnum, 3] = s.Имя;
                        sheet.Cells[Gnum, 4] = s.Отчество;

                        // Подсчет пропусков у студента
                        var Sskips = db.Skips.Where(d => d.ID_Студента == s.ID_Студента);
                        foreach (Skips d in Sskips) { if (AllTimeCheckButton.IsChecked == false) { if (d.Дата >= date_1.SelectedDate && d.Дата <= date_2.SelectedDate) Scount++; } else Scount++; }
                        //.............................

                        sheet.Cells[Gnum, 5] = Scount;

                        range = sheet.get_Range("A" + Gnum, "E" + Gnum);
                        range.Cells.Interior.Color = Excel.XlRgbColor.rgbLightSkyBlue;
                        range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    }
                    endG = Gnum;
                    range = sheet.get_Range("A" + startG, "E" + endG);
                    range.Rows.Group();
                    Gnum++;
                    n1++;
                    n2 = 0;
                    Gcount = 0;
                    sheet.Cells[Gnum, 1] = n1;
                    sheet.Cells[Gnum, 2] = g.Название_группы;
                    sheet.Cells[Gnum, 3] = g.Год_поступления;

                    //подсчет пропусков в группе
                    var zapros = (from sa in db.Students where sa.ID_Группы == g.ID_Группы select sa.ID_Студента);
                    foreach (var j in zapros)
                    {
                        var command = db.Skips.Where(s => s.ID_Студента == j);
                        foreach (var l in command) { if (AllTimeCheckButton.IsChecked == false) { if (l.Дата >= date_1.SelectedDate && l.Дата <= date_2.SelectedDate) Gcount++; } else Gcount++; }
                    }
                    //...........................

                    sheet.Cells[Gnum, 4] = Gcount;
                    Allcount += Gcount;

                    range = sheet.get_Range("A" + Gnum, "D" + Gnum);
                    range.Cells.Interior.Color = Excel.XlRgbColor.rgbLightGreen;

                }
                Gnum++;
                sheet.Cells[Gnum, 3] = "Всего:";
                sheet.Cells[Gnum, 4] = Allcount;
                range = sheet.get_Range("A" + 1, "D" + Gnum);

                //sheet.Rows.WrapText = true;


                range.EntireColumn.AutoFit();
                range.EntireRow.AutoFit();
                range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                ex.Visible = true;

                if (InactiveCheckButton.IsChecked == false)
                    sheet.Cells[Gnum + 3, 1] = "[По всем предметам][По всем активным группам]";
                else sheet.Cells[Gnum + 3, 1] = "[По всем предметам][По всем пассивным группам]";
                if (AllTimeCheckButton.IsChecked == false)
                    sheet.Cells[Gnum + 2, 1] = "Отчет по пропускам за промежуток с " + date_1.SelectedDate.Value.ToShortDateString() + " по " + date_2.SelectedDate.Value.ToShortDateString();
                else
                    sheet.Cells[Gnum + 2, 1] = "Отчет по пропускам за всё время";

            }
        }

        private void CreateReportButton_Click(object sender, RoutedEventArgs e)
        {
            CreateUniversalExcelTable();
        }

        private void inactivest_Click(object sender, RoutedEventArgs e)
        {
            Group_ComboBox_SelectionChanged(null, null);
        }

        private void inactivesub_Click(object sender, RoutedEventArgs e)
        {
            Subject_ComboBox.Items.Clear();

            using (SysItems db = new SysItems())
            {
                var sb = db.Subjects.Where(r => r.Статус == true).OrderBy(r => r.Наименование_предмета);
                if (inactivesub.IsChecked == true) sb = db.Subjects.Where(r => r.Статус == false).OrderBy(r => r.Наименование_предмета);

                if (sb.FirstOrDefault() != null)
                {
                    foreach (Subjects r in sb)
                    {
                        Subject_ComboBox.Items.Add(r.Наименование_предмета);
                    }
                    Subject_ComboBox.SelectedIndex = 0;
                }
            }
        }


    }
}
