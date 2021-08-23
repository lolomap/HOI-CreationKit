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

namespace HOICK.DialogWindows
{
    /// <summary>
    /// Логика взаимодействия для NewProjectSettings.xaml
    /// </summary>
    public partial class NewProjectSettings : Window
    {
        public string name, description;
        public string[] tags;

        public NewProjectSettings()
        {
            InitializeComponent();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            name = nameInput.Text;
            description = descInput.Text;
            tags = tagsInput.Text.Split(';');

            if (name == "")
            {
                DialogResult = false;
                return;
            }
            DialogResult = true;
        }
    }
}
