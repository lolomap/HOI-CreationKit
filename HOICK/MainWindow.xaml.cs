using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace HOICK
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            App.LanguageChanged += LanguageChanged;

            CultureInfo currLang = App.Language;

            menuLanguage.Items.Clear();
            foreach (CultureInfo lang in App.Languages)
            {
                MenuItem menuLang = new MenuItem();
                menuLang.Header = lang.DisplayName;
                menuLang.Tag = lang;
                menuLang.IsChecked = lang.Equals(currLang);
                menuLang.Click += ChangeLanguageClick;
                _ = menuLanguage.Items.Add(menuLang);
            }
        }

        private void LanguageChanged(object sender, EventArgs e)
        {
            CultureInfo currLang = App.Language;

            foreach (MenuItem i in menuLanguage.Items)
            {
                i.IsChecked = i.Tag is CultureInfo ci && ci.Equals(currLang);
            }
        }

        private void ChangeLanguageClick(object sender, EventArgs e)
        {
            if (sender is MenuItem mi)
            {
                if (mi.Tag is CultureInfo lang)
                {
                    App.Language = lang;
                }
            }
        }

        private void NewProject_Click(object sender, RoutedEventArgs e)
        {
            DialogWindows.NewProjectSettings newProjectDialog = new DialogWindows.NewProjectSettings();
            if (newProjectDialog.ShowDialog() == true)
            {
                string modFolderName = Regex.Replace(newProjectDialog.name, @"\s+", "");
                string descriptorPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                    "/Paradox Interactive/Hearts of Iron IV/mod/" + modFolderName + ".mod";
                string descriptorData = "name=\"" + newProjectDialog.name + "\"\n"
                    + "tags={\n";
                if (newProjectDialog.tags.Length != 1 && (newProjectDialog.tags[0] != ""))
                {
                    foreach (string tag in newProjectDialog.tags)
                    {
                        descriptorData += "\"" + tag + "\"\n";
                    }
                }

                descriptorData += "}\npath=\"mod/" + modFolderName + "\"\n";
                using (StreamWriter descriptorF = new StreamWriter(descriptorPath))
                {
                    descriptorF.Write(descriptorData);
                }
                Directory.CreateDirectory(descriptorPath.Substring(0, descriptorPath.Length - 4));

                Visibility = Visibility.Hidden;
                Application.Current.MainWindow = new Workplace();
                Application.Current.MainWindow.Show();
            }
        }
    }
}
