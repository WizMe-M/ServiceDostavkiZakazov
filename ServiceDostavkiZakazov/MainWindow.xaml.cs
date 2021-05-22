using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using ServiceDostavkiZakazov.DataSetTableAdapters;
using System.Text.RegularExpressions;

namespace ServiceDostavkiZakazov
{
    public partial class MainWindow : Window
    {
        DataSet dataSet;
        FullSotrudnikTableAdapter fullSotrudnikTableAdapter;
        SotrudnikTableAdapter sotrudnikTableAdapter;
        DoljnostTableAdapter doljnostTableAdapter;
        public MainWindow()
        {
            InitializeComponent();

            //Здесь мы подключаем все необходимые компоненты нашей БД
            dataSet = new DataSet();
            sotrudnikTableAdapter = new SotrudnikTableAdapter();
            sotrudnikTableAdapter.Fill(dataSet.Sotrudnik);
            doljnostTableAdapter = new DoljnostTableAdapter();
            doljnostTableAdapter.Fill(dataSet.Doljnost);
            fullSotrudnikTableAdapter = new FullSotrudnikTableAdapter();
            fullSotrudnikTableAdapter.Fill(dataSet.FullSotrudnik);

            /* Настраиваем DataGrid:
             *Отображаем таблицу сотрудников,
             *Сортируем по ID сотрудников,
             *Запрещаем изменение таблицы,
             *Разрешаем выбирать только одну строку (для изменения)
             */
            Data.ItemsSource = dataSet.FullSotrudnik.DefaultView;
            Data.SelectedValuePath = "ID_Sotrudnik";
            Data.CanUserAddRows = false;
            Data.CanUserDeleteRows = false;
            Data.CanUserSortColumns = false;
            Data.CanUserResizeRows = false;
            Data.CanUserResizeColumns = false;
            Data.CanUserReorderColumns = false;
            Data.IsReadOnly = true;
            Data.SelectionMode = DataGridSelectionMode.Single;


            /* Настраиваем ComboBox:
             *Получаем содержимое из таблицы должностей (чтобы при выборе должности данные брались из таблицы)
             *Отображать (MemberPath) будем названия должностей, тогда как обращаться к ним будем по ID (ValuePath)
             */
            doljnostCB.ItemsSource = dataSet.Doljnost.DefaultView;
            doljnostCB.SelectedValuePath = "ID_Doljnost";
            doljnostCB.DisplayMemberPath = "Naimenovanie";
            doljnostCB.SelectedIndex = 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //При загрузке окна часть столбцов в таблице становится скрытой (ибо, а зачем пользователю видеть ID?)
            Data.Columns[0].Visibility = Visibility.Hidden;
            Data.Columns[1].Visibility = Visibility.Hidden;
        }


        void Data_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)Data.SelectedItem;
            if (selectedRow != null)
            {
                surnameTB.Text = selectedRow.Row.ItemArray[2].ToString();
                nameTB.Text = selectedRow.Row.ItemArray[3].ToString();
                middlenameTB.Text = selectedRow.Row.ItemArray[4].ToString();
                seriaTB.Text = selectedRow.Row.ItemArray[5].ToString();
                nomerTB.Text = selectedRow.Row.ItemArray[6].ToString();
                phoneTB.Text = selectedRow.Row.ItemArray[7].ToString();
                pochtaTB.Text = selectedRow.Row.ItemArray[8].ToString();
                gorodTB.Text = selectedRow.Row.ItemArray[9].ToString();
                ulitsaTB.Text = selectedRow.Row.ItemArray[10].ToString();
                domTB.Text = selectedRow.Row.ItemArray[11].ToString();
                doljnostCB.SelectedValue = selectedRow.Row.ItemArray[1];
            }
        }
        void addBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckException();
                sotrudnikTableAdapter.Insert(surnameTB.Text, nameTB.Text, middlenameTB.Text,
                    seriaTB.Text, nomerTB.Text, phoneTB.Text, pochtaTB.Text,
                    gorodTB.Text, ulitsaTB.Text, domTB.Text, (int)doljnostCB.SelectedValue);

                fullSotrudnikTableAdapter.Fill(dataSet.FullSotrudnik);
            }
            catch (Exception exc)
            { MessageBox.Show(exc.Message, "Вызвано исключение!", MessageBoxButton.OK); }
        }
        void editBTN_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                CheckException();
                if (Data.SelectedItem != null)
                {
                    sotrudnikTableAdapter.UpdateQuery(surnameTB.Text, nameTB.Text, middlenameTB.Text,
                        seriaTB.Text, nomerTB.Text, phoneTB.Text, pochtaTB.Text,
                        gorodTB.Text, ulitsaTB.Text, domTB.Text,
                        (int)doljnostCB.SelectedValue, (int)Data.SelectedValue);
                    fullSotrudnikTableAdapter.Fill(dataSet.FullSotrudnik);
                }
            }
            catch (Exception exc)
            { MessageBox.Show(exc.Message, "Вызвано исключение!", MessageBoxButton.OK); }

        }

        void deleteBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Data.SelectedItem != null)
                {
                    sotrudnikTableAdapter.DeleteQuery((int)Data.SelectedValue);
                    fullSotrudnikTableAdapter.Fill(dataSet.FullSotrudnik);
                }
            }
            catch (Exception exc)
            { MessageBox.Show(exc.Message, "Вызвано исключение!", MessageBoxButton.OK); }

        }
        private void SelectText(object sender, RoutedEventArgs e)
        {
            TextBox text = sender as TextBox;
            if (text != null)
                text.Dispatcher.BeginInvoke(new Action(() => text.SelectAll()));
        }
        void CheckException()
        {
            try
            {
                foreach (TextBox textBox in grid.Children.OfType<TextBox>())
                    textBox.Text = textBox.Text.Replace(" ", "");

                //Обработка исключений и ошибок
                if (string.IsNullOrWhiteSpace(nameTB.Text) || string.IsNullOrWhiteSpace(surnameTB.Text) || string.IsNullOrWhiteSpace(phoneTB.Text)
                    || string.IsNullOrWhiteSpace(gorodTB.Text) || string.IsNullOrWhiteSpace(ulitsaTB.Text) || string.IsNullOrWhiteSpace(domTB.Text))
                    throw new Exception("Одно или несколько полей не заполнены \n(заполнение поля отчества необязательно)");

                string mail_pattern = @"^[a-z]{3,6}@dk.ru$";
                if (!Regex.IsMatch(pochtaTB.Text, mail_pattern, RegexOptions.Singleline))
                    throw new Exception("Неверный формат почты\nДопустим только формат \"***@dk.ru\"");

                string phone_pattern = @"^\+7\([0-9]{3}\)[0-9]{3}-[0-9]{2}-[0-9]{2}$";
                if (!Regex.IsMatch(phoneTB.Text, phone_pattern, RegexOptions.Singleline))
                    throw new Exception("Неверный формат номера телефона\nДопустим только формат \"+7(***)***-**-**\"");

                string dom_pattern = @"\d{1,3}[a-cа-с]?\d?";
                if (!Regex.IsMatch(domTB.Text, dom_pattern, RegexOptions.Singleline))
                    throw new Exception("Неверный формат номера дома\nДопустимы только форматы типов \"5\", \"15а\" или \"102а1\"");

                string seria_pat = @"^[0-9]{4}$"; string nomer_pat = @"^[0-9]{6}$";
                if (!Regex.IsMatch(seriaTB.Text, seria_pat, RegexOptions.Singleline) || !Regex.IsMatch(nomerTB.Text, nomer_pat, RegexOptions.Singleline))
                    throw new Exception("Неверный формат паспортных данных\nДопусим только формат \"****\" для серии и \"******\" для номера");
            }
            catch { throw; }
        }
        private void ChoseMenuItem(object sender, RoutedEventArgs e)
        {
            Hide();
            MenuItem item = sender as MenuItem;
            Window window = new Window();
            switch (item.Header)
            {
                case "Должности":
                    window = new DoljnostWindow();
                    break;
                case "Тип доставки":
                    window = new TipDostavki();
                    break;
                case "Тип ТС":
                    window = new TS();
                    break;
                case "Полная Заявки":
                    window = new FullZayavkaWindow();
                    break;
                case "Полная Маршруты":
                    window = new FullMarshrut();
                    break;
            }
            window.Show();
        }
    }
}
