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
using System.Data;
using ServiceDostavkiZakazov.DataSetTableAdapters;


namespace ServiceDostavkiZakazov
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataSet dataSet;
        SotrudnikTableAdapter sotrudnikTableAdapter;
        public MainWindow()
        {
            InitializeComponent();

            dataSet = new DataSet();
            sotrudnikTableAdapter = new SotrudnikTableAdapter();
            sotrudnikTableAdapter.Fill(dataSet.Sotrudnik);
            Data.ItemsSource = dataSet.Sotrudnik.DefaultView;
            Data.SelectedValuePath = "ID_Sotrudnik";
            Data.CanUserAddRows = false;
            Data.CanUserDeleteRows = false;
            //Data.SelectedItem 
        }
    }
}
