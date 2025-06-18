using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace DocumentApp_ProizvodstvennayaPraktika
{
    public partial class ContractViewWindow : Window
    {
        private ClientContracts _contract;
        private Users _currentUser;
        private Dictionary<string, Control> _fieldControls = new Dictionary<string, Control>();
        private bool _isEditMode = false;

        public ContractViewWindow(ClientContracts contract, Users user)
        {
            InitializeComponent();
            _contract = contract;
            _currentUser = user;
            DataContext = _contract;

            LoadContractFields();
            UpdateUI();
        }

        private void LoadContractFields()
        {
            try
            {
                using (var context = new Entities())
                {
                    _contract = context.ClientContracts
                        .Include(c => c.ContractTemplates)
                        .Include(c => c.ContractTemplates.TemplateFields)
                        .FirstOrDefault(c => c.ContractId == _contract.ContractId);

                    if (_contract == null || _contract.ContractTemplates == null)
                    {
                        MessageBox.Show("Договор или шаблон не найден");
                        Close();
                        return;
                    }

                    FieldsContainer.Items.Clear();
                    _fieldControls.Clear();

                    var contractData = string.IsNullOrEmpty(_contract.ContractData)
                        ? new Dictionary<string, string>()
                        : JsonConvert.DeserializeObject<Dictionary<string, string>>(_contract.ContractData);

                    foreach (var field in _contract.ContractTemplates.TemplateFields)
                    {
                        var stackPanel = new StackPanel { Orientation = Orientation.Vertical };
                        var label = new Label { Content = field.FieldLabel };

                        Control inputControl;
                        if (field.FieldType.ToLower() == "date")
                        {
                            var datePicker = new DatePicker();
                            if (contractData.ContainsKey(field.FieldName))
                            {
                                DateTime date;
                                if (DateTime.TryParse(contractData[field.FieldName], out date))
                                {
                                    datePicker.SelectedDate = date;
                                }
                            }
                            inputControl = datePicker;
                        }
                        else
                        {
                            var textBox = new TextBox();
                            if (contractData.ContainsKey(field.FieldName))
                            {
                                textBox.Text = contractData[field.FieldName];
                            }
                            inputControl = textBox;
                        }

                        inputControl.IsEnabled = false;
                        stackPanel.Children.Add(label);
                        stackPanel.Children.Add(inputControl);
                        FieldsContainer.Items.Add(stackPanel);
                        _fieldControls.Add(field.FieldName, inputControl);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки полей: {ex.Message}");
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            _isEditMode = true;
            foreach (var control in _fieldControls.Values)
            {
                control.IsEnabled = true;
            }

            EditBtn.Visibility = Visibility.Collapsed;
            SaveBtn.Visibility = Visibility.Visible;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var contractData = new Dictionary<string, string>();
                foreach (var field in _fieldControls)
                {
                    string value = "";
                    if (field.Value is TextBox)
                    {
                        value = ((TextBox)field.Value).Text;
                    }
                    else if (field.Value is DatePicker)
                    {
                        var datePicker = (DatePicker)field.Value;
                        value = datePicker.SelectedDate?.ToString("yyyy-MM-dd") ?? "";
                    }
                    contractData.Add(field.Key, value);
                }

                using (var context = new Entities())
                {
                    var dbContract = context.ClientContracts.Find(_contract.ContractId);
                    if (dbContract != null)
                    {
                        dbContract.ContractData = JsonConvert.SerializeObject(contractData);
                        dbContract.UpdatedAt = DateTime.Now;

                        // Не меняем статус на Completed при повторном сохранении
                        if (dbContract.Status == "Draft")
                        {
                            dbContract.Status = "Completed";
                        }

                        context.SaveChanges();
                        _contract = dbContract;
                        DataContext = _contract;
                    }
                }

                _isEditMode = false;
                foreach (var control in _fieldControls.Values)
                {
                    control.IsEnabled = false;
                }

                EditBtn.Visibility = Visibility.Visible;
                SaveBtn.Visibility = Visibility.Collapsed;

                MessageBox.Show("Изменения сохранены", "Сохранение",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения договора: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateUI()
        {
            EditBtn.Visibility = Visibility.Visible;
            SaveBtn.Visibility = Visibility.Collapsed;
        }

        private void ExportToWord_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Экспорт в Word будет реализован в будущей версии", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            var sendWindow = new ExportWindow(_contract);
            sendWindow.ShowDialog();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isEditMode)
            {
                var result = MessageBox.Show("У вас есть несохраненные изменения. Закрыть без сохранения?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }
            Close();
        }
    }
}