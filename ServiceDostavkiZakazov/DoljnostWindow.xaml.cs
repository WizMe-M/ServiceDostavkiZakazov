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
    /// Логика взаимодействия для DoljnostWindow.xaml
    /// </summary>
    public partial class DoljnostWindow : Window
    {

        DataSet dataSet = new DataSet();
        DoljnostTableAdapter doljnostTableAdapter = new DoljnostTableAdapter();
        public DoljnostWindow()
        {
            InitializeComponent();

            doljnostTableAdapter.Fill(dataSet.Doljnost);
            Data.ItemsSource = dataSet.Doljnost.DefaultView;
            Data.SelectedValuePath = "ID_Doljnost";
            Data.CanUserAddRows = false;
            Data.CanUserDeleteRows = false;
            Data.CanUserSortColumns = false;
            Data.CanUserResizeRows = false;
            Data.CanUserResizeColumns = false;
            Data.CanUserReorderColumns = false;
            Data.IsReadOnly = true;
            Data.SelectionMode = DataGridSelectionMode.Single;
        }

        private void Data_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)Data.SelectedItem;
            if (selectedRow != null)
            {
                dolj.Text = selectedRow.Row.ItemArray[1].ToString();
                oklad.Text = selectedRow.Row.ItemArray[2].ToString();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Data.Columns[0].Visibility = Visibility.Hidden;
            Data.Columns[3].Visibility = Visibility.Hidden;
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckException();
                doljnostTableAdapter.Insert(dolj.Text, int.Parse(oklad.Text));
                doljnostTableAdapter.Fill(dataSet.Doljnost);
            }
            catch (Exception exc)
            { MessageBox.Show(exc.Message, "Вызвано исключение!", MessageBoxButton.OK); }
        }

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckException();
                doljnostTableAdapter.UpdateQuery(dolj.Text, int.Parse(oklad.Text), (int)Data.SelectedValue);
                doljnostTableAdapter.Fill(dataSet.Doljnost);
            }
            catch (Exception exc)
            { MessageBox.Show(exc.Message, "Вызвано исключение!", MessageBoxButton.OK); }
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                if (Data.SelectedItem != null)
                {
                    doljnostTableAdapter.DeleteQuery((int)Data.SelectedValue);
                    doljnostTableAdapter.Fill(dataSet.Doljnost);
                }
            }
            catch (Exception exc)
            { MessageBox.Show(exc.Message, "Вызвано исключение!", MessageBoxButton.OK); }
        }
        void back_Click(object sender, RoutedEventArgs e)
        {
            Close();
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
        void CheckException()
        {
            try
            {
                foreach (TextBox textBox in grid.Children.OfType<TextBox>())
                    textBox.Text = textBox.Text.Replace(" ", "");

                //Обработка исключений и ошибок
                if (string.IsNullOrWhiteSpace(oklad.Text) || string.IsNullOrWhiteSpace(dolj.Text))
                    throw new Exception("Одно или несколько полей не заполнены");

                string doljnost_pattern = @"^[А-я]{1}[а-я]{5,99}$";
                if (!Regex.IsMatch(dolj.Text, doljnost_pattern, RegexOptions.Singleline))
                    throw new Exception("Неверный формат названия должности\nДопустимо только написание должности на русском языке, с заглавной буквы и должность должна не короче 6 букв");

                string oklad_pattern = @"^[1-9]\d{4,11}$";
                if (!Regex.IsMatch(oklad.Text, oklad_pattern, RegexOptions.Singleline))
                    throw new Exception("Неверный формат размера оклада\nДопустимы только числа, от 5 до 12 цифр, не начинающиеся с 0");

            }
            catch { throw; }
        }


        private void SelectText(object sender, RoutedEventArgs e)
        {
            TextBox text = sender as TextBox;
            if (text != null)
                text.Dispatcher.BeginInvoke(new Action(() => text.SelectAll()));
        }

    }
}
