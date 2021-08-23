using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;

namespace HOICK.DialogWindows
{
    /// <summary>
    /// Логика взаимодействия для NewFocusSettings.xaml
    /// </summary>
    public partial class NewFocusSettings : Window
    {
        public string id, name, desc;
        public List<string> searchFilters;
        public bool? aic, cii;

        public NewFocusSettings()
        {
            InitializeComponent();
        }

        private void CreateFocus_Click(object sender, RoutedEventArgs e)
        {
            id = idInput.Text;
            name = nameInput.Text;
            desc = descInput.Text;

            aic = availableIfCapitulatedInput.IsChecked;
            cii = cancelIfInvalidInput.IsChecked;

            DialogResult = true;
        }
    }
}
