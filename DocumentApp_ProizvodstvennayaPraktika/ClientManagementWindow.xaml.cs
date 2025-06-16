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
    /// Логика взаимодействия для ClientManagementWindow.xaml
    /// </summary>
    public partial class ClientManagementWindow : Window
    {
        public ClientManagementWindow()
        {
            InitializeComponent();
            LoadClients();
        }

        private void LoadClients()
        {
            using (var context = new Entities())
            {
                ClientsGrid.ItemsSource = context.Users
                    .Where(u => u.RoleId == 1) // Только клиенты
                    .ToList();
            }
        }

        private void AddClient_Click(object sender, RoutedEventArgs e)
        {
            var editor = new ClientEditorWindow(new Users { RoleId = 1 });
            if (editor.ShowDialog() == true)
            {
                LoadClients();
            }
        }

        private void EditClient_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsGrid.SelectedItem is Users client)
            {
                var editor = new ClientEditorWindow(client);
                if (editor.ShowDialog() == true)
                {
                    LoadClients();
                }
            }
        }

        private void ViewContracts_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsGrid.SelectedItem is Users client)
            {
                var window = new ClientContractsWindow(client.UserId);
                window.ShowDialog();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
