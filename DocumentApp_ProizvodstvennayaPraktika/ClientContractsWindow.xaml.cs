using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;

namespace DocumentApp_ProizvodstvennayaPraktika
{
    public partial class ClientContractsWindow : Window
    {
        private int _clientId;
        private string _clientName;

        public string ClientName => $"Договоры клиента: {_clientName}";

        public ClientContractsWindow(int clientId)
        {
            InitializeComponent();
            _clientId = clientId;
            LoadClientData();
            DataContext = this;
        }

        private void LoadClientData()
        {
            using (var context = new Entities())
            {
                var client = context.Users.Find(_clientId);
                _clientName = client?.FullName ?? "Неизвестный клиент";

                ContractsGrid.ItemsSource = context.ClientContracts
                    .Include(c => c.ContractTemplates) // Исправлено: лямбда-выражение вместо строки
                    .Where(c => c.ClientId == _clientId)
                    .ToList();
            }
        }

        private void ViewContract_Click(object sender, RoutedEventArgs e)
        {
            if (ContractsGrid.SelectedItem is ClientContracts contract)
            {
                var client = new Users { UserId = _clientId, FullName = _clientName };
                var viewWindow = new ContractViewWindow(contract, client);
                viewWindow.ShowDialog();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}