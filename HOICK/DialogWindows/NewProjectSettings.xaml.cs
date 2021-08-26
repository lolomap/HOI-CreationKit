using System.Windows;
using System.IO;
using System;

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

            if (name == string.Empty || File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                    "/Paradox Interactive/Hearts of Iron IV/mod/" + name + ".mod"))
            {
                System.Media.SystemSounds.Beep.Play();

                _ = MessageBox.Show(Application.Current.Resources["CreateProjectError"] as string);

                return;
            }
            DialogResult = true;
        }
    }
}
