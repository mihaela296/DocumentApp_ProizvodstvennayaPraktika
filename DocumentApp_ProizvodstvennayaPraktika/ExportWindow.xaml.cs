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

namespace DocumentApp_ProizvodstvennayaPraktika
{
    /// <summary>
    /// Логика взаимодействия для ExportWindow.xaml
    /// </summary>
    public partial class ExportWindow : Window
    {
        private ClientContracts _contract;

        public ExportWindow(ClientContracts contract)
        {
            InitializeComponent();
            _contract = contract;
        }

        private void ExportAndSend_Click(object sender, RoutedEventArgs e)
        {
            // Здесь будет код для экспорта и отправки
            MessageBox.Show("Договор успешно экспортирован и отправлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
