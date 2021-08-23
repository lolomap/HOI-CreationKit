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
    /// Логика взаимодействия для NewFocusTreeSettings.xaml
    /// </summary>
    public partial class NewFocusTreeSettings : Window
    {
        public string CountryTag;

        public NewFocusTreeSettings()
        {
            InitializeComponent();
        }

        private void CreateFocusTree_Click(object sender, RoutedEventArgs e)
        {
            if (FTCountryTag.SelectedItem != null)
            {
                Country c = ProjectData.Countries.FirstOrDefault(i => i.NameLocalizations[App.Language.Name]
                    == (string)FTCountryTag.SelectedItem);
                CountryTag = c.Tag;
            }

            if ((FTCountryTag.SelectedItem == null && !IsFTDefault.IsChecked.GetValueOrDefault()) || ID.Text == string.Empty)
            {
                System.Media.SystemSounds.Beep.Play();
                return;
            }

            DialogResult = true;
        }

        private void IsFTDefault_Checked(object sender, RoutedEventArgs e)
        {
            if (IsFTDefault.IsChecked.GetValueOrDefault())
            {
                FTCountryTag.IsEnabled = false;
            }
        }
    }
}
