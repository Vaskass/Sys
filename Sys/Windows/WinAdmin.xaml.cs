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
using Sys.Windows;
using System.Data.Entity;

namespace Sys.Windows
{
    /// <summary>
    /// Логика взаимодействия для WinAdmin.xaml
    /// </summary>
    public partial class WinAdmin : Window
    {
        SysItems SI = new SysItems();
        List<Users> usersList = new List<Users>();
        List<Subjects> subjectsList = new List<Subjects>();
        List<Enabled_Subjects> enabledSubjectsList = new List<Enabled_Subjects>();
        List<Groups> groupsList = new List<Groups>();
        List<Students> studentsList = new List<Students>();

        public WinAdmin()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            refreshUsersBox();
            refreshSubjectBox();
            refreshStudentsBox();
            refreshGroupBox();
        }

        private void refreshSubjectBox()
        {
            subjectsList.Clear();
            subjectBox.Items.Clear();
            subjectsBox.Items.Clear();
            if (deletedSubjectsShow.IsChecked.Value)
            {
                foreach (Subjects s in SI.Subjects)
                {
                    subjectsList.Add(s);
                    subjectBox.Items.Add(subjectsList.LastOrDefault().Наименование_предмета);
                    subjectsBox.Items.Add(subjectsList.LastOrDefault().Наименование_предмета);
                }
            }
            else
            {
                foreach (Subjects s in SI.Subjects.Where(p => p.Статус == true))
                {
                    subjectsList.Add(s);
                    subjectBox.Items.Add(subjectsList.LastOrDefault().Наименование_предмета);
                    subjectsBox.Items.Add(subjectsList.LastOrDefault().Наименование_предмета);
                }
            }
        }

        private void refreshStudentsBox()
        {
            studentsList.Clear();
            studentsBox.Items.Clear();
            IQueryable<Students> Src;

            if (deletedStudentsShow.IsChecked.Value) //удалённый
            {
                if (srcStudentGroupBox.SelectedItem != null) //группа
                {
                    if (!string.IsNullOrWhiteSpace(srcStudentSurnameText.Text)) //Фамилия
                    { // удалённые, с группой и фамилией
                        int idGroup = groupsList[srcStudentGroupBox.SelectedIndex].ID_Группы;
                        Src = SI.Students.Where(p => p.Фамилия.Contains(srcStudentSurnameText.Text) && p.ID_Группы == idGroup);
                    }
                    else // нет фамилии
                    {
                        //Удалённые, с группой, БЕЗ фамилии
                        int idGroup = groupsList[srcStudentGroupBox.SelectedIndex].ID_Группы;
                        Src = SI.Students.Where(p => p.ID_Группы == idGroup);
                    }
                }
                else // нет группы
                {
                    if (!string.IsNullOrWhiteSpace(srcStudentSurnameText.Text)) //Фамилия введена
                    {
                        Src = SI.Students.Where(p => p.Фамилия.Contains(srcStudentSurnameText.Text));
                        // удалённые, без группы и с фамилией
                    }
                    else
                    {
                        Src = SI.Students;
                        //Удалённые, без группы, БЕЗ фамилии
                    }
                }
            }
            else //не удалённые
            {
                if (srcStudentGroupBox.SelectedItem != null) //группа
                {
                    if (!string.IsNullOrWhiteSpace(srcStudentSurnameText.Text)) //Фамилия
                    {
                        // НЕ удалённые, с группой и фамилией
                        int idGroup = groupsList[srcStudentGroupBox.SelectedIndex].ID_Группы;
                        Src = SI.Students.Where(p => p.Фамилия.Contains(srcStudentSurnameText.Text) && p.ID_Группы == idGroup && p.Статус == true);
                    }
                    else //без фамилии
                    {
                        // Не Удалённые, с группой, БЕЗ фамилии
                        int idGroup = groupsList[srcStudentGroupBox.SelectedIndex].ID_Группы;
                        Src = SI.Students.Where(p => p.ID_Группы == idGroup && p.Статус == true);
                    }
                }
                else // без группы
                {
                    if (!string.IsNullOrWhiteSpace(srcStudentSurnameText.Text)) //Фамилия введена
                    {
                        // НЕ удалённые, без группы и с фамилией
                        Src = SI.Students.Where(p => p.Фамилия.Contains(srcStudentSurnameText.Text) && p.Статус == true);

                    }
                    else
                    {
                        //Удалённые, без группы, БЕЗ фамилии
                        Src = SI.Students.Where(p => p.Статус == true);
                    }
                }
            }
            foreach (Students s in Src)
            {
                studentsList.Add(s);
                studentsBox.Items.Add(studentsList.LastOrDefault().Фамилия + " " + studentsList.LastOrDefault().Имя + " " + studentsList.LastOrDefault().Отчество);
            }
        }

        private void refreshEnabledSubjectBox()
        {
            enabledSubjectsList.Clear();
            enabledSubjectBox.Items.Clear();
            int selectedUserID = usersList[usersBox.SelectedIndex].ID_Пользователя;
            foreach (Enabled_Subjects s in SI.Enabled_Subjects.Where(p => p.ID_Пользователя == selectedUserID))
            {
                enabledSubjectsList.Add(s);
                enabledSubjectBox.Items.Add(SI.Subjects.Find(enabledSubjectsList.LastOrDefault().ID_Предмета).Наименование_предмета + " " + SI.Groups.Find(enabledSubjectsList.LastOrDefault().ID_Группы).Название_группы);
            }
        }

        private void refreshGroupBox()
        {
            groupsList.Clear();
            srcStudentGroupBox.Items.Clear();
            groupBox.Items.Clear();
            studentGroupBox.Items.Clear();
            groupsBox.Items.Clear();
            if (deletedGroupsShow.IsChecked.Value)
            {
                foreach (Groups s in SI.Groups)
                {
                    groupsList.Add(s);
                    groupBox.Items.Add(groupsList.LastOrDefault().Название_группы);
                    groupsBox.Items.Add(groupsList.LastOrDefault().Название_группы);
                    studentGroupBox.Items.Add(groupsList.LastOrDefault().Название_группы);
                    srcStudentGroupBox.Items.Add(groupsList.LastOrDefault().Название_группы);
                }
            }
            else
            {
                foreach (Groups s in SI.Groups.Where(p => p.Статус == true))
                {
                    groupsList.Add(s);
                    groupBox.Items.Add(groupsList.LastOrDefault().Название_группы);
                    groupsBox.Items.Add(groupsList.LastOrDefault().Название_группы);
                    studentGroupBox.Items.Add(groupsList.LastOrDefault().Название_группы);
                    srcStudentGroupBox.Items.Add(groupsList.LastOrDefault().Название_группы);
                }
            }
        }

        private void refreshUsersBox()
        {
            usersList.Clear();
            usersBox.Items.Clear();
            grupHeadBox.Items.Clear();
            if (deletedUsersShow.IsChecked.Value)
            {

                foreach (Users u in SI.Users)
                {
                    usersList.Add(u);
                    usersBox.Items.Add(usersList.LastOrDefault().Логин);
                    grupHeadBox.Items.Add(usersList.LastOrDefault().Логин);
                }
            }
            else
            {
                foreach (Users u in SI.Users.Where(p => p.Статус == true))
                {
                    usersList.Add(u);
                    usersBox.Items.Add(usersList.LastOrDefault().Логин);
                    grupHeadBox.Items.Add(usersList.LastOrDefault().Логин);
                }
            }
        }

        private void positionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (okUserButton != null)
                enabledSubjectsGroup.IsEnabled = positionBox.SelectedIndex == 0 && positionBox.IsEnabled && okUserButton.Content.ToString() != "Добавить";
        }

        private void UsersBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (usersBox.SelectedItem != null)
            {
                int selectedUserID = usersList[usersBox.SelectedIndex].ID_Пользователя;
                Users selectedUser = SI.Users.Find(selectedUserID);
                if (okUserButton.Content.ToString() == "Изменить")
                {
                    nameBox.Text = selectedUser.Имя;
                    surnameBox.Text = selectedUser.Фамилия;
                    patronymicBox.Text = selectedUser.Отчество;
                    loginBox.Text = selectedUser.Логин;
                    passwordBox.Text = selectedUser.Пароль;
                    positionBox.Text = selectedUser.Должность;
                    refreshEnabledSubjectBox();
                }

                if (selectedUser.Статус == false)
                {
                    deleteUserButton.Content = "Восстановить";
                }
                else
                {
                    deleteUserButton.Content = "Удалить";
                }
            }

        }

        private void changeUserButton_Click(object sender, RoutedEventArgs e)
        {
            userGroup.IsEnabled = true;
            okUserButton.Content = "Изменить";
            UsersBox_SelectionChanged(null, null);
            addUserButton.IsEnabled = false;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            if (okUserButton.Content.ToString() == "Изменить")
            {
                if (usersBox.SelectedItem != null)
                {
                    int selectedUserID = usersList[usersBox.SelectedIndex].ID_Пользователя;
                    Users u = SI.Users.Find(selectedUserID); ;
                    u.Имя = nameBox.Text;
                    u.Фамилия = surnameBox.Text;
                    u.Отчество = patronymicBox.Text;
                    u.Логин = loginBox.Text;
                    u.Пароль = passwordBox.Text;
                    u.Должность = positionBox.Text;
                }
                else
                {
                    MessageBox.Show("Выберите пользователя для изменения");
                }
            }
            else
            {
                Users newUser = new Users
                {
                    Имя = nameBox.Text,
                    Фамилия = surnameBox.Text,
                    Отчество = patronymicBox.Text,
                    Логин = loginBox.Text,
                    Пароль = passwordBox.Text,
                    Должность = positionBox.Text,
                    Статус = true
                };
                SI.Users.Add(newUser);
            }
            SI.SaveChanges();
            refreshUsersBox();
            userGroup.IsEnabled = false;
            clearUserGroup();
            okUserButton.Content = "OK";
            editUserButton.IsEnabled = true;
            addUserButton.IsEnabled = true;
        }

        private void positionBox_SelectionChange(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (okUserButton != null)
                enabledSubjectsGroup.IsEnabled = positionBox.SelectedIndex == 0 && positionBox.IsEnabled && okUserButton.Content.ToString() != "Добавить";
        }

        private void addUserButton_Click(object sender, RoutedEventArgs e)
        {
            clearUserGroup();
            okUserButton.Content = "Добавить";
            userGroup.IsEnabled = true;
            editUserButton.IsEnabled = false;
        }

        private void clearUserGroup()
        {
            nameBox.Text = "";
            surnameBox.Text = "";
            patronymicBox.Text = "";
            loginBox.Text = "";
            passwordBox.Text = "";
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            userGroup.IsEnabled = false;
            clearUserGroup();
            okUserButton.Content = "ОК";
            editUserButton.IsEnabled = true;
            addUserButton.IsEnabled = true;
        }

        private void deleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (usersBox.SelectedItem != null)
            {
                int selectedUserID = usersList[usersBox.SelectedIndex].ID_Пользователя;
                Users selectedUser = SI.Users.Find(selectedUserID);
                if (deleteUserButton.Content.ToString() == "Удалить")
                {
                    if (MessageBox.Show("Вы уверены что хотите удалить пользователя " + selectedUser.Логин + "?", "Удалить пользователя?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        selectedUser.Статус = false;
                    }
                }
                else
                {
                    selectedUser.Статус = true;
                }
                SI.SaveChanges();
                refreshUsersBox();
                clearUserGroup();
            }
            else
            {
                MessageBox.Show("Выберете пользователя");
            }
        }

        private void deletedUsersShow_Checked(object sender, RoutedEventArgs e)
        {
            refreshUsersBox();
        }

        private void deletedSubjectsShow_Checked(object sender, RoutedEventArgs e)
        {
            refreshSubjectBox();
        }

        private void deletedStudentsShow_Checked(object sender, RoutedEventArgs e)
        {
            refreshStudentsBox();
        }

        private void deletedGroupsShow_Checked(object sender, RoutedEventArgs e)
        {
            refreshGroupBox();
        }

        private void addEnadbledSubjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (subjectBox.SelectedItem != null && groupBox.SelectedItem != null)
            {
                int selectedSubjectID = subjectsList[subjectBox.SelectedIndex].ID_Предмета;
                int selectedGroupID = groupsList[groupBox.SelectedIndex].ID_Группы;
                int selectedUserID = usersList[usersBox.SelectedIndex].ID_Пользователя;
                var Src = SI.Enabled_Subjects.Where(p => p.ID_Группы == selectedGroupID && p.ID_Предмета == selectedSubjectID && p.ID_Пользователя == selectedUserID);
                if (Src.Any())
                {
                    MessageBox.Show("Предмет " + subjectsList[subjectBox.SelectedIndex].Наименование_предмета + " у группы " + groupsList[groupBox.SelectedIndex].Название_группы + " уже есть у предодователя " + usersList[usersBox.SelectedIndex].Логин);
                }
                else
                {
                    Enabled_Subjects es = new Enabled_Subjects
                    {
                        ID_Группы = selectedGroupID,
                        ID_Пользователя = selectedUserID,
                        ID_Предмета = selectedSubjectID
                    };
                    SI.Enabled_Subjects.Add(es);
                    SI.SaveChanges();
                }
                refreshEnabledSubjectBox();
            }
            else
            { MessageBox.Show("Выберите предмет и группу для добавления"); }
        }

        private void deleteEnadbledSubjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (enabledSubjectBox.SelectedItem != null)
            {
                SI.Enabled_Subjects.Remove(enabledSubjectsList[enabledSubjectBox.SelectedIndex]);
                SI.SaveChanges();
                refreshEnabledSubjectBox();
            }
            else
            {
                MessageBox.Show("Выберете строку для удаления");
            }
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show(); Close();
        }


        private void addSubjectButton_Click(object sender, RoutedEventArgs e)
        {
            subjectText.Text = "";
            subjectGroup.IsEnabled = true;
            okSubjectButton.Content = "Добавить";
            editSubjectButton.IsEnabled = false;
        }

        private void editSubjectButton_Click(object sender, RoutedEventArgs e)
        {
            subjectGroup.IsEnabled = true;
            okSubjectButton.Content = "Изменить";
            subjectsBox_SelectionChanged(null, null);
            addSubjectButton.IsEnabled = false;
        }

        private void subjectsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (subjectsBox.SelectedItem != null)
            {
                int selectedSubjectID = subjectsList[subjectsBox.SelectedIndex].ID_Предмета;
                Subjects selectedSubject = SI.Subjects.Find(selectedSubjectID);
                if (okSubjectButton.Content.ToString() == "Изменить")
                {
                    subjectText.Text = selectedSubject.Наименование_предмета;
                }

                if (selectedSubject.Статус == false)
                {
                    deleteSubjectButton.Content = "Восстановить";
                }
                else
                {
                    deleteSubjectButton.Content = "Удалить";
                }
            }
        }

        private void deleteSubjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (subjectsBox.SelectedItem != null)
            {
                int selectedSubjectID = subjectsList[subjectsBox.SelectedIndex].ID_Предмета;
                Subjects selectedSubject = SI.Subjects.Find(selectedSubjectID);
                if (deleteSubjectButton.Content.ToString() == "Удалить")
                {
                    if (MessageBox.Show("Вы уверены что хотите удалить предмет " + selectedSubject.Наименование_предмета + "?", "Удалить предмет?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        selectedSubject.Статус = false;
                    }
                }
                else
                {
                    selectedSubject.Статус = true;
                }
                SI.SaveChanges();
                refreshSubjectBox();
                subjectText.Text = "";
            }
            else
            {
                MessageBox.Show("Выберете предмет");
            }
        }

        private void okSubjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (okSubjectButton.Content.ToString() == "Изменить")
            {
                if (subjectsBox.SelectedItem != null)
                {
                    int selectedSubjectID = subjectsList[subjectsBox.SelectedIndex].ID_Предмета;
                    Subjects u = SI.Subjects.Find(selectedSubjectID);
                    u.Наименование_предмета = subjectText.Text;
                }
                else
                {
                    MessageBox.Show("Выберите предмет для изменения");
                    return;
                }
            }
            else
            {
                Subjects newSubject = new Subjects
                {
                    Наименование_предмета = subjectText.Text,
                    Статус = true
                };
                SI.Subjects.Add(newSubject);
            }
            SI.SaveChanges();
            refreshSubjectBox();
            subjectGroup.IsEnabled = false;
            subjectText.Text = "";
            okSubjectButton.Content = "OK";
            editSubjectButton.IsEnabled = true;
            addSubjectButton.IsEnabled = true;
        }

        private void cancelSubjectButton_Click(object sender, RoutedEventArgs e)
        {
            subjectGroup.IsEnabled = false;
            subjectText.Text = "";
            okSubjectButton.Content = "ОК";
            editSubjectButton.IsEnabled = true;
            addSubjectButton.IsEnabled = true;
        }

        private void clearStudentGroup()
        {
            studentNameText.Text = "";
            studentPatronymicText.Text = "";
            studentSurnameText.Text = "";
            studentGroupBox.Text = "";
        }
        private void addStudentButton_Click(object sender, RoutedEventArgs e)
        {
            clearStudentGroup();
            studentGroup.IsEnabled = true;
            okStudentButton.Content = "Добавить";
            editStudentButton.IsEnabled = false;
        }

        private void editStudentButton_Click(object sender, RoutedEventArgs e)
        {
            studentGroup.IsEnabled = true;
            okStudentButton.Content = "Изменить";
            studentsBox_SelectionChanged(null, null);
            addStudentButton.IsEnabled = false;
        }

        private void deleteStudentButton_Click(object sender, RoutedEventArgs e)
        {
            if (studentsBox.SelectedItem != null)
            {
                int selectedStudentID = studentsList[studentsBox.SelectedIndex].ID_Студента;
                Students selectedStudent = SI.Students.Find(selectedStudentID);
                if (deleteStudentButton.Content.ToString() == "Удалить")
                {
                    if (MessageBox.Show("Вы уверены что хотите удалить студента " + selectedStudent.Фамилия + " " + selectedStudent.Имя + " " + selectedStudent.Отчество + "?", "Удалить студента?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        selectedStudent.Статус = false;
                    }
                }
                else
                {
                    selectedStudent.Статус = true;
                }
                SI.SaveChanges();
                refreshStudentsBox();
                clearStudentGroup();
            }
            else
            {
                MessageBox.Show("Выберете пользователя");
            }
        }

        private void studentsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (studentsBox.SelectedItem != null)
            {
                int selectedStudentID = studentsList[studentsBox.SelectedIndex].ID_Студента;
                Students selectedStudent = SI.Students.Find(selectedStudentID);
                if (okStudentButton.Content.ToString() == "Изменить")
                {
                    studentNameText.Text = selectedStudent.Имя;
                    studentSurnameText.Text = selectedStudent.Фамилия;
                    studentPatronymicText.Text = selectedStudent.Отчество;
                    var s = groupsList.Find(p => p.ID_Группы == selectedStudent.ID_Группы);
                    if(s!=null)
                    studentGroupBox.SelectedItem = s.Название_группы;
                    else
                    {
                        if (MessageBox.Show("Данная группа удалена, включить отображение удалённых групп?", "???", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
                        {
                            deletedGroupsShow.IsChecked = true;
                            studentsBox_SelectionChanged(null, null);
                        }
                    }
                }

                if (selectedStudent.Статус == false)
                {
                    deleteStudentButton.Content = "Восстановить";
                }
                else
                {
                    deleteStudentButton.Content = "Удалить";
                }
            }
        }

        private void okStudentButton_Click(object sender, RoutedEventArgs e)
        {
            if (okStudentButton.Content.ToString() == "Изменить")
            {
                if (studentsBox.SelectedItem != null)
                {
                    int selectedStudentID = studentsList[studentsBox.SelectedIndex].ID_Студента;
                    Students u = SI.Students.Find(selectedStudentID); ;
                    u.Имя = studentNameText.Text;
                    u.Фамилия = studentSurnameText.Text;
                    u.Отчество = studentPatronymicText.Text;
                    u.ID_Группы = groupsList[studentGroupBox.SelectedIndex].ID_Группы;
                }
                else
                {
                    MessageBox.Show("Выберите пльзователя для именения");
                    return;
                }
            }
            else
            {
                if (studentGroupBox.SelectedItem != null)
                {
                    Students newStudent = new Students
                    {
                        Имя = studentNameText.Text,
                        Фамилия = studentSurnameText.Text,
                        Отчество = studentPatronymicText.Text,
                        ID_Группы = groupsList[studentGroupBox.SelectedIndex].ID_Группы,
                        Статус = true
                    };
                    SI.Students.Add(newStudent);
                }
                else
                {
                    MessageBox.Show("Выберите группу");
                    return;
                }
            }
            SI.SaveChanges();
            refreshStudentsBox();
            studentGroup.IsEnabled = false;
            clearStudentGroup();
            okStudentButton.Content = "OK";
            editStudentButton.IsEnabled = true;
            addStudentButton.IsEnabled = true;
        }

        private void cancelStudentButton_Click(object sender, RoutedEventArgs e)
        {
            studentGroup.IsEnabled = false;
            clearStudentGroup();
            okStudentButton.Content = "ОК";
            editStudentButton.IsEnabled = true;
            addStudentButton.IsEnabled = true;
        }


        private void cancelSrcButton_Click(object sender, RoutedEventArgs e)
        {
            srcStudentGroupBox.SelectedIndex = -1;
            srcStudentSurnameText.Text = "";
            refreshStudentsBox();
        }

        private void srcStudentSurnameText_TextChanged(object sender, TextChangedEventArgs e)
        {
            refreshStudentsBox();
        }

        private void srcStudentGroupBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            refreshStudentsBox();
        }

        private void addGroupButton_Click(object sender, RoutedEventArgs e)
        {
            clearGrupGroup();
            grupGroup.IsEnabled = true;
            okGrupButton.Content = "Добавить";
            editGrupButton.IsEnabled = false;
        }
        private void clearGrupGroup()
        {
            grupNameText.Text = "";
            grupYearText.Text = "";
            grupHeadBox.SelectedIndex = -1;
        }

        private void editGrupButton_Click(object sender, RoutedEventArgs e)
        {
            grupGroup.IsEnabled = true;
            okGrupButton.Content = "Изменить";
            groupsBox_SelectionChanged(null, null);
            addGrupButton.IsEnabled = false;
        }

        private void groupsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (groupsBox.SelectedItem != null)
            {
                int selectedGrupID = groupsList[groupsBox.SelectedIndex].ID_Группы;
                Groups selectedGrup = SI.Groups.Find(selectedGrupID);
                if (okGrupButton.Content.ToString() == "Изменить")
                {
                    grupNameText.Text = selectedGrup.Название_группы;
                    grupYearText.Text = selectedGrup.Год_поступления.ToString();
                    var s = usersList.Find(p => p.ID_Пользователя == selectedGrup.ID_Пользователя);
                    if (s != null)
                        grupHeadBox.SelectedItem = s.Логин;
                    else
                    {
                        if (MessageBox.Show("Данный руководитель удалён, включить отображение удалённых пользоватлей?","???",MessageBoxButton.YesNo,MessageBoxImage.Warning,MessageBoxResult.No) == MessageBoxResult.Yes)
                        {
                            deletedUsersShow.IsChecked = true;
                            groupsBox_SelectionChanged(null, null);
                        }
                    }
                }

                if (selectedGrup.Статус == false)
                {
                    deleteGrupButton.Content = "Восстановить";
                }
                else
                {
                    deleteGrupButton.Content = "Удалить";
                }
            }

        }

        private void deleteGrupButton_Click(object sender, RoutedEventArgs e)
        {
            if (groupsBox.SelectedItem != null)
            {
                int selectedGrupID = groupsList[groupsBox.SelectedIndex].ID_Группы;
                Groups selectedGrup = SI.Groups.Find(selectedGrupID);
                if (deleteGrupButton.Content.ToString() == "Удалить")
                {
                    if (MessageBox.Show("Вы уверены что хотите удалить группу " + selectedGrup.Название_группы + "?", "Удалить группу?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        selectedGrup.Статус = false;
                    }
                }
                else
                {
                    selectedGrup.Статус = true;
                }
                SI.SaveChanges();
                refreshGroupBox();
                clearGrupGroup();
            }
            else
            {
                MessageBox.Show("Выберете пользователя");
            }
        }

        private void okGrupButton_Click(object sender, RoutedEventArgs e)
        {
            int s;
            if (!int.TryParse(grupYearText.Text, out s))
            {
                MessageBox.Show("Некорретный ввод года поступления");
                return;
            };

            if (okGrupButton.Content.ToString() == "Изменить")
            {
                if (groupsBox.SelectedItem != null)
                {
                    int selectedGrupID = groupsList[groupsBox.SelectedIndex].ID_Группы;
                    Groups u = SI.Groups.Find(selectedGrupID);
                    u.Название_группы = grupNameText.Text;
                    u.Год_поступления = s;
                    u.ID_Пользователя = usersList[grupHeadBox.SelectedIndex].ID_Пользователя;
                }
                else
                {
                    MessageBox.Show("Выберите пользователя для изменения");
                    return;
                }
            }
            else
            {
                if (grupHeadBox.SelectedItem != null)
                {
                    Groups newGrup = new Groups
                    {
                        Название_группы = grupNameText.Text,
                        Год_поступления = s,
                        ID_Пользователя = usersList[grupHeadBox.SelectedIndex].ID_Пользователя,
                        Статус = true
                    };
                    SI.Groups.Add(newGrup);
                }
                else
                {
                    MessageBox.Show("Выберете классного руководителя");
                    return;
                }
            }
            SI.SaveChanges();
            refreshGroupBox();
            grupGroup.IsEnabled = false;
            clearGrupGroup();
            okGrupButton.Content = "OK";
            editGrupButton.IsEnabled = true;
            addGrupButton.IsEnabled = true;
        }

        private void cancelGrupButton_Click(object sender, RoutedEventArgs e)
        {
            grupGroup.IsEnabled = false;
            clearGrupGroup();
            okGrupButton.Content = "ОК";
            editGrupButton.IsEnabled = true;
            addGrupButton.IsEnabled = true;
        }

        private void grupYearText_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void userGroup_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void enabledSubjectsGroup_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (enabledSubjectBox != null)
            {
                subjectBox.SelectedIndex = -1;
                groupBox.SelectedIndex = -1;
                enabledSubjectBox.Items.Clear();
            }
        }
    }
}
