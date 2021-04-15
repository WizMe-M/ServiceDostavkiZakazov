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
using ServiceDostavkiZakazov.DataSetTableAdapters;
using System.Data;

namespace ServiceDostavkiZakazov
{
    /// <summary>
    /// Логика взаимодействия для TipDostavki.xaml
    /// </summary>
    public partial class TipDostavki : Window
    {
        DataSet dataSet = new DataSet();
        TipDostavkiTableAdapter tipDostavkiTableAdapter = new TipDostavkiTableAdapter();

        public TipDostavki()
        {
            InitializeComponent();

            tipDostavkiTableAdapter.Fill(dataSet.TipDostavki);
            
            Data.ItemsSource = dataSet.TipDostavki.DefaultView;
            Data.SelectedValuePath = "ID_TipaDostavki";
            Data.CanUserAddRows = false;
            Data.CanUserDeleteRows = false;
            Data.CanUserSortColumns = false;
            Data.CanUserResizeRows = false;
            Data.CanUserResizeColumns = false;
            Data.CanUserReorderColumns = false;
            Data.IsReadOnly = true;
            Data.SelectionMode = DataGridSelectionMode.Single;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Data.Columns[0].Visibility = Visibility.Hidden;
            Data.Columns[2].Visibility = Visibility.Hidden;
        }

        void addBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckException();
                tipDostavkiTableAdapter.Insert(naimenovanie.Text);
                tipDostavkiTableAdapter.Fill(dataSet.TipDostavki);
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
                    tipDostavkiTableAdapter.UpdateQuery(naimenovanie.Text, (int)Data.SelectedValue);
                    tipDostavkiTableAdapter.Fill(dataSet.TipDostavki);
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
                    tipDostavkiTableAdapter.DeleteQuery((int)Data.SelectedValue);
                    tipDostavkiTableAdapter.Fill(dataSet.TipDostavki);
                }
            }
            catch (Exception exc)
            { MessageBox.Show(exc.Message, "Вызвано исключение!", MessageBoxButton.OK); }

        }

        void CheckException()
        {
            try
            {
                //Обработка исключений и ошибок
                if (string.IsNullOrWhiteSpace(naimenovanie.Text))
                    throw new Exception("Поле не заполнено!");
            }
            catch { throw; }
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            Close();
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void Data_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)Data.SelectedItem;
            if (selectedRow != null)
            {
                naimenovanie.Text = selectedRow.Row.ItemArray[1].ToString();
            }
        }
    }
}
