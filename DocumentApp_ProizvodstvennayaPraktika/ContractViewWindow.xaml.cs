using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Configuration;
using Dapper;
using Newtonsoft.Json;

namespace DocumentApp_ProizvodstvennayaPraktika
{
    /// <summary>
    /// Логика взаимодействия для ContractViewWindow.xaml
    /// </summary>
    public partial class ContractViewWindow : Window
    {
        private ClientContracts _contract;
        private Users _currentUser;
        private bool _isEditMode = false;
        private Dictionary<string, Control> _fieldControls = new Dictionary<string, Control>();

        public ContractViewWindow(ClientContracts contract, Users user)
        {
            InitializeComponent();
            _contract = contract;
            _currentUser = user;
            DataContext = _contract;

            LoadContractFields();
        }

        private void LoadContractFields()
        {
            FieldsContainer.Items.Clear();
            _fieldControls.Clear();

            try
            {
                using (var context = Entities.GetContext())
                {
                    // Загрузка полей шаблона
                    var fields = context.TemplateFields.Where(tf => tf.TemplateId == _contract.TemplateId).ToList();

                    // Загрузка данных договора (если есть)
                    var contractData = string.IsNullOrEmpty(_contract.ContractData)
                        ? new Dictionary<string, string>()
                        : JsonConvert.DeserializeObject<Dictionary<string, string>>(_contract.ContractData);

                    foreach (var field in fields)
                    {
                        // Создаем элементы управления для каждого поля
                        var stackPanel = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(0, 5, 0, 5) };
                        var label = new Label { Content = field.FieldLabel, Style = (Style)FindResource("LabelStyle") };
                        stackPanel.Children.Add(label);

                        Control inputControl;
                        switch (field.FieldType.ToLower())
                        {
                            case "date":
                                var datePicker = new DatePicker { Style = (Style)FindResource("TextBoxStyle") };
                                if (contractData.ContainsKey(field.FieldName) && DateTime.TryParse(contractData[field.FieldName], out var date))
                                    datePicker.SelectedDate = date;
                                inputControl = datePicker;
                                break;
                            case "number":
                                var numBox = new TextBox { Style = (Style)FindResource("TextBoxStyle") };
                                if (contractData.ContainsKey(field.FieldName))
                                    numBox.Text = contractData[field.FieldName];
                                inputControl = numBox;
                                break;
                            default: // text и другие
                                var textBox = new TextBox { Style = (Style)FindResource("TextBoxStyle") };
                                if (contractData.ContainsKey(field.FieldName))
                                    textBox.Text = contractData[field.FieldName];
                                inputControl = textBox;
                                break;
                        }

                        stackPanel.Children.Add(inputControl);
                        FieldsContainer.Items.Add(stackPanel);
                        _fieldControls.Add(field.FieldName, inputControl);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки полей договора: {ex.Message}");
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            //if (_isEditMode)
            //{
            //    // Сохранение изменений
            //    try
            //    {
            //        var contractData = new Dictionary<string, string>();
            //        foreach (var field in _fieldControls)
            //        {
            //            string value = field.Value switch
            //            {
            //                TextBox textBox => textBox.Text,
            //                DatePicker datePicker => datePicker.SelectedDate?.ToString("yyyy-MM-dd"),
            //                _ => string.Empty
            //            };
            //            contractData.Add(field.Key, value);
            //        }

            //        _contract.ContractData = JsonConvert.SerializeObject(contractData);
            //        _contract.UpdatedAt = DateTime.Now;

            //        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            //        {
            //            string updateQuery = @"UPDATE ClientContracts 
            //                             SET ContractData = @ContractData, 
            //                                 UpdatedAt = @UpdatedAt,
            //                                 Status = @Status
            //                             WHERE ContractId = @ContractId";

            //            connection.Execute(updateQuery, new
            //            {
            //                _contract.ContractData,
            //                _contract.UpdatedAt,
            //                _contract.Status,
            //                _contract.ContractId
            //            });
            //        }

            //        MessageBox.Show("Договор успешно сохранен", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show($"Ошибка сохранения договора: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            //    }

            //    EditBtn.Content = "Заполнить договор";
            //    ContentBox.IsReadOnly = true;
            //    _isEditMode = false;
            //}
            //else
            //{
            //    EditBtn.Content = "Сохранить";
            //    ContentBox.IsReadOnly = false;
            //    _isEditMode = true;
            //}
        }

        private void ExportToWord_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    // Создаем документ Word
            //    using (var doc = DocX.Create($"{_contract.Template.TemplateName}_{_contract.ContractId}.docx"))
            //    {
            //        // Добавляем заголовок
            //        var title = doc.InsertParagraph(_contract.Template.TemplateName);
            //        title.FontSize(16).Bold().Alignment = AlignmentX.Center;

            //        // Добавляем информацию о договоре
            //        doc.InsertParagraph($"Номер: {_contract.ContractNumber}").Alignment = AlignmentX.Left;
            //        doc.InsertParagraph($"Дата: {_contract.ContractDate:dd.MM.yyyy}").Alignment = AlignmentX.Left;
            //        doc.InsertParagraph($"Статус: {_contract.Status}").Alignment = AlignmentX.Left;
            //        doc.InsertParagraph("").Alignment = AlignmentX.Left;

            //        // Добавляем заполненные данные
            //        if (!string.IsNullOrEmpty(_contract.ContractData))
            //        {
            //            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(_contract.ContractData);
            //            foreach (var item in data)
            //            {
            //                doc.InsertParagraph($"{item.Key}: {item.Value}").Alignment = AlignmentX.Left;
            //            }
            //            doc.InsertParagraph("").Alignment = AlignmentX.Left;
            //        }

            //        // Добавляем основной текст договора
            //        doc.InsertParagraph(_contract.Template.Content).Alignment = AlignmentX.Left;

            //        doc.Save();
            //        MessageBox.Show("Договор успешно экспортирован в Word", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Ошибка экспорта в Word: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            var sendWindow = new ExportWindow(_contract);
            sendWindow.ShowDialog();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
