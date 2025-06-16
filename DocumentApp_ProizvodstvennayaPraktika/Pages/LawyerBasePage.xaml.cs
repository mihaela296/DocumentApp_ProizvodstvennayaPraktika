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

namespace DocumentApp_ProizvodstvennayaPraktika.Pages
{
    /// <summary>
    /// Логика взаимодействия для LaywerBasePage.xaml
    /// </summary>
    public partial class LawyerBasePage : Page
    {
        public LawyerBasePage()
        {
            InitializeComponent();
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            using (var context = new Entities())
            {
                ActiveContractsCount.Text = context.ClientContracts.Count(c => c.Status == "Active").ToString();
                ClientsCount.Text = context.Users.Count(u => u.RoleId == 1).ToString(); 
                TemplatesCount.Text = context.ContractTemplates.Count().ToString();
            }
        }

        private void ManageTemplates_Click(object sender, RoutedEventArgs e)
        {
            var window = new TemplateManagementWindow();
            window.ShowDialog();
            LoadStatistics();
        }

        private void ManageClients_Click(object sender, RoutedEventArgs e)
        {
            var window = new ClientManagementWindow();
            window.ShowDialog();
            LoadStatistics();
        }

        private void ViewAllContracts_Click(object sender, RoutedEventArgs e)
        {
            var window = new ContractManagementWindow();
            window.ShowDialog();
        }
    }
}
