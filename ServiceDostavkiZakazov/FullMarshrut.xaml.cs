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
    /// Логика взаимодействия для FullMarshrut.xaml
    /// </summary>
    public partial class FullMarshrut : Window
    {
        DataSet dataSet = new DataSet();
        FullMarshrutTableAdapter fullMarshrutTableAdapter = new FullMarshrutTableAdapter();
        MarshrutTableAdapter marshrutTableAdapter = new MarshrutTableAdapter();
        TSTableAdapter tsTableAdapter = new TSTableAdapter();
        ZayavkaTableAdapter zayavkaTableAdapter = new ZayavkaTableAdapter();

        public FullMarshrut()
        {
            InitializeComponent();

            fullMarshrutTableAdapter.Fill(dataSet.FullMarshrut);
            marshrutTableAdapter.Fill(dataSet.Marshrut);
            tsTableAdapter.Fill(dataSet.TS);
            zayavkaTableAdapter.Fill(dataSet.Zayavka);

            Data.ItemsSource = dataSet.FullMarshrut.DefaultView;
            Data.SelectedValuePath = "ID_Marshruta";
            Data.CanUserAddRows = false;
            Data.CanUserDeleteRows = false;
            Data.CanUserSortColumns = false;
            Data.CanUserResizeRows = false;
            Data.CanUserResizeColumns = false;
            Data.CanUserReorderColumns = false;
            Data.IsReadOnly = true;
            Data.SelectionMode = DataGridSelectionMode.Single;

            nomer.ItemsSource = dataSet.Zayavka.DefaultView;
            nomer.SelectedValuePath = "ID_Zayavki";
            nomer.DisplayMemberPath = "Nomer";
            nomer.SelectedIndex = 0;

            ts.ItemsSource = dataSet.TS.DefaultView;
            ts.SelectedValuePath = "ID_TS";
            ts.DisplayMemberPath = "Naimenovanie";
            ts.SelectedIndex = 0;
        }

        void addBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckException();
                marshrutTableAdapter.Insert(decimal.Parse(stoimost.Text), vremya.Text, (int)ts.SelectedValue, (int)nomer.SelectedValue);
                fullMarshrutTableAdapter.Fill(dataSet.FullMarshrut);
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
                    marshrutTableAdapter.UpdateQuery(decimal.Parse(stoimost.Text), vremya.Text, (int)ts.SelectedValue, (int)nomer.SelectedValue, (int)Data.SelectedValue);
                    fullMarshrutTableAdapter.Fill(dataSet.FullMarshrut);
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
                    marshrutTableAdapter.DeleteQuery((int)Data.SelectedValue);
                    fullMarshrutTableAdapter.Fill(dataSet.FullMarshrut);
                }
            }
            catch (Exception exc)
            { MessageBox.Show(exc.Message, "Вызвано исключение!", MessageBoxButton.OK); }

        }

        void CheckException()
        {
            try
            {
                foreach (TextBox textBox in grid.Children.OfType<TextBox>())
                    textBox.Text = textBox.Text.Replace(" ", "");

                //Обработка исключений и ошибок
                if (string.IsNullOrWhiteSpace(stoimost.Text) || string.IsNullOrWhiteSpace(vremya.Text))
                    throw new Exception("Одно или несколько полей не заполнены");

                string stoimost_pattern = @"^\d+\.?\d*$";
                if (!Regex.IsMatch(stoimost.Text, stoimost_pattern, RegexOptions.Singleline) )
                    throw new Exception("Неверный формат стоимости\nДопустимы только целые числа, либо десятичные дроби через точку");

                string vremya_pattern = @"^[1-9]\d*$";
                if (!Regex.IsMatch(vremya.Text, vremya_pattern, RegexOptions.Singleline))
                    throw new Exception("Неверный формат времени\nДопустимы только числа не меньше 1");
            }
            catch { throw; }
        }

        private void Data_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)Data.SelectedItem;
            if (selectedRow != null)
            {
                stoimost.Text = selectedRow.Row.ItemArray[6].ToString();
                vremya.Text = selectedRow.Row.ItemArray[7].ToString();
                nomer.SelectedValue = selectedRow.Row.ItemArray[1];
                ts.SelectedValue = selectedRow.Row.ItemArray[2];
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Data.Columns[0].Visibility = Visibility.Hidden;
            Data.Columns[1].Visibility = Visibility.Hidden;
            Data.Columns[2].Visibility = Visibility.Hidden;
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            Close();
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
