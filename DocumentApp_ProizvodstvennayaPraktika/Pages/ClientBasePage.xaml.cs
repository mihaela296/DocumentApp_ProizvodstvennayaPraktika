using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DocumentApp_ProizvodstvennayaPraktika.Pages
{
    /// <summary>
    /// Логика взаимодействия для ClientBasePage.xaml
    /// </summary>
    public partial class ClientBasePage : Page
    {
        private Users _currentUser;
        private List<ClientContracts> _allContracts;
        private List<ContractCategories> _categories;

        public ClientBasePage(Users user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (var context = new Entities())
                {
                    // Загрузка договоров клиента с связанными данными
                    _allContracts = context.ClientContracts
                        .Include(c => c.ContractTemplates)
                        .Include(c => c.ContractTemplates.ContractCategories)
                        .Where(cc => cc.ClientId == _currentUser.UserId)
                        .ToList();

                    ContractsGrid.ItemsSource = _allContracts;

                    // Загрузка категорий для фильтра
                    _categories = context.ContractCategories.ToList();
                    FilterComboBox.ItemsSource = _categories;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                ContractsGrid.ItemsSource = _allContracts;
                return;
            }

            var searchText = SearchBox.Text.ToLower();
            var filtered = _allContracts
                .Where(c => c.ContractTemplates.TemplateName.ToLower().Contains(searchText))
                .ToList();

            ContractsGrid.ItemsSource = filtered;
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilterComboBox.SelectedItem is ContractCategories selectedCategory)
            {
                var filtered = _allContracts
                    .Where(c => c.ContractTemplates.CategoryId == selectedCategory.CategoryId)
                    .ToList();

                ContractsGrid.ItemsSource = filtered;
            }
            else
            {
                ContractsGrid.ItemsSource = _allContracts;
            }
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            var sorted = _allContracts.OrderBy(c => c.ContractId).ToList();
            ContractsGrid.ItemsSource = sorted;
        }

        private void ContractsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ContractsGrid.SelectedItem is ClientContracts selectedContract)
            {
                try
                {
                    using (var context = new Entities())
                    {
                        // Явно загружаем все необходимые данные
                        var fullContract = context.ClientContracts
                            .Include(c => c.ContractTemplates)
                            .Include(c => c.ContractTemplates.TemplateFields)
                            .Include(c => c.ContractTemplates.ContractCategories)
                            .FirstOrDefault(c => c.ContractId == selectedContract.ContractId);

                        if (fullContract != null)
                        {
                            var viewWindow = new ContractViewWindow(fullContract, _currentUser);
                            viewWindow.ShowDialog();
                            LoadData(); // Обновляем данные
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка открытия договора: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}