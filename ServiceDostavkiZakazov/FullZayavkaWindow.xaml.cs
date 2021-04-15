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
using System.Data;
using ServiceDostavkiZakazov.DataSetTableAdapters;
using System.Text.RegularExpressions;

namespace ServiceDostavkiZakazov
{
    /// <summary>
    /// Логика взаимодействия для FullZayavkaWindow.xaml
    /// </summary>
    public partial class FullZayavkaWindow : Window
    {
        DataSet dataSet = new DataSet();
        FullZayavkaTableAdapter fullZayavkaTableAdapter = new FullZayavkaTableAdapter();
        ZayavkaTableAdapter zayavkaTableAdapter = new ZayavkaTableAdapter();
        SotrudnikTableAdapter sotrudnikTableAdapter = new SotrudnikTableAdapter();
        TipDostavkiTableAdapter tipTableAdapter = new TipDostavkiTableAdapter();

        public FullZayavkaWindow()
        {
            InitializeComponent();

            fullZayavkaTableAdapter.Fill(dataSet.FullZayavka);
            sotrudnikTableAdapter.Fill(dataSet.Sotrudnik);
            tipTableAdapter.Fill(dataSet.TipDostavki);


            Data.ItemsSource = dataSet.FullZayavka.DefaultView;
            Data.SelectedValuePath = "ID_Zayavki";
            Data.CanUserAddRows = false;
            Data.CanUserDeleteRows = false;
            Data.CanUserSortColumns = false;
            Data.CanUserResizeRows = false;
            Data.CanUserResizeColumns = false;
            Data.CanUserReorderColumns = false;
            Data.IsReadOnly = true;
            Data.SelectionMode = DataGridSelectionMode.Single;

            tip_dost.ItemsSource = dataSet.TipDostavki.DefaultView;
            tip_dost.SelectedValuePath = "ID_TipaDostavki";
            tip_dost.DisplayMemberPath = "Naimenovanie";
            tip_dost.SelectedIndex = 0;

            sotr.ItemsSource = dataSet.Sotrudnik.DefaultView;
            sotr.SelectedValuePath = "ID_Sotrudnik";
            sotr.DisplayMemberPath = "Familiya";
            sotr.SelectedIndex = 0;
        }


        void addBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckException();
                zayavkaTableAdapter.Insert(int.Parse(nomer.Text), infa.Text, adres_pol.Text, 
                    adres_otp.Text, (int)tip_dost.SelectedValue, (int)sotr.SelectedValue);
                fullZayavkaTableAdapter.Fill(dataSet.FullZayavka);
            }
            catch (Exception exc)
            { MessageBox.Show(exc.Message, "Вызвано исключение!", MessageBoxButton.OK); }
        }
        void editBTN_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (Data.SelectedItem != null)
                {
                    CheckException();
                    zayavkaTableAdapter.UpdateQuery(int.Parse(nomer.Text), infa.Text, adres_pol.Text,
                    adres_otp.Text, (int)tip_dost.SelectedValue, (int)sotr.SelectedValue, (int)Data.SelectedValue);
                    fullZayavkaTableAdapter.Fill(dataSet.FullZayavka);
                }
            }
            catch (Exception exc)
            { MessageBox.Show(exc.Message, "Вызвано исключение!", MessageBoxButton.OK); }

        }

        private void deleteBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Data.SelectedItem != null)
                {
                    zayavkaTableAdapter.DeleteQuery((int)Data.SelectedValue);
                    fullZayavkaTableAdapter.Fill(dataSet.FullZayavka);
                }
            }
            catch (Exception exc)
            { MessageBox.Show(exc.Message, "Вызвано исключение!", MessageBoxButton.OK); }

        }
        private void back_Click(object sender, RoutedEventArgs e)
        {
            Close();
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        void CheckException()
        {
            try
            {
                nomer.Text = nomer.Text.Replace(" ", "");

                //Обработка исключений и ошибок
                if (string.IsNullOrWhiteSpace(nomer.Text) || string.IsNullOrWhiteSpace(adres_pol.Text) || string.IsNullOrWhiteSpace(adres_otp.Text))
                    throw new Exception("Одно или несколько полей не заполнены \n(заполнение поля отчества необязательно)");

                string nomer_pattern = @"^[1-9]\d{2}$";
                if (!Regex.IsMatch(nomer.Text, nomer_pattern, RegexOptions.Singleline))
                    throw new Exception("Неверный формат номера\nДопустимы только числа длиной в 3 цифры, не начинающиеся с нуля");
            }
            catch { throw; }
        }



        void Data_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)Data.SelectedItem;
            if (selectedRow != null)
            {
                nomer.Text = selectedRow.Row.ItemArray[3].ToString();
                infa.Text = selectedRow.Row.ItemArray[4].ToString();
                adres_pol.Text = selectedRow.Row.ItemArray[5].ToString();
                adres_otp.Text = selectedRow.Row.ItemArray[6].ToString();
                tip_dost.SelectedValue = selectedRow.Row.ItemArray[1];
                sotr.SelectedValue = selectedRow.Row.ItemArray[2];
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Data.Columns[0].Visibility = Visibility.Hidden;
            Data.Columns[1].Visibility = Visibility.Hidden;
            Data.Columns[2].Visibility = Visibility.Hidden;
        }
    }
}
