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

namespace HOICK
{
    /// <summary>
    /// Логика взаимодействия для Workplace.xaml
    /// </summary>
    public partial class Workplace : Window
    {
        private FocusTree CurrentFocusTree;

        public void UpdateFocusCanvas()
        {
            foreach (NationalFocus focus in CurrentFocusTree.Focuses)
            {
                focus.Render(FocusCanvas);
            }
        }

        public Workplace()
        {
            InitializeComponent();
            if (FocusTreeInput.SelectedItem == null)
            {
                FocusMainTools.IsEnabled = false;
            }
        }

        private void CreateFocus_Click(object sender, RoutedEventArgs e)
        {
            DialogWindows.NewFocusSettings newFocusDialog = new DialogWindows.NewFocusSettings();
            if (newFocusDialog.ShowDialog() == true)
            {
                string loc = App.Language.Name;
                NationalFocus newFocus = new NationalFocus()
                {
                    Id = newFocusDialog.id,
                    NameLocalizations = new Dictionary<string, string>() { { loc, newFocusDialog.name } },
                    DescriptionLocalization = new Dictionary<string, string>() { { loc, newFocusDialog.desc } },
                    //SearchFilters = newFocusDialog.searchFilters,
                    AvailableIfCapitulated = newFocusDialog.aic.GetValueOrDefault(),
                    CancelIfInvalid = newFocusDialog.cii.GetValueOrDefault()
                };
                CurrentFocusTree.Focuses.Add(newFocus);
                UpdateFocusCanvas();
            }
        }

        private void NewFocusTree_Click(object sender, RoutedEventArgs e)
        {
            DialogWindows.NewFocusTreeSettings newFocusTreeDialog = new DialogWindows.NewFocusTreeSettings();
            if (newFocusTreeDialog.ShowDialog() == true)
            {
                FocusTree ft = new FocusTree()
                {
                    Id = newFocusTreeDialog.ID.Text,
                    Tag = newFocusTreeDialog.CountryTag,
                    IsDefault = newFocusTreeDialog.IsFTDefault.IsChecked.GetValueOrDefault(),
                    ResetOnCivwar = newFocusTreeDialog.ResetOnCivwar.IsChecked.GetValueOrDefault()
                };
                ProjectData.FocusTrees.Add(ft);
                FocusTreeInput.Items.Add(ft.Id);
            }
        }

        private void FocusTreeInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FocusTreeInput.SelectedItem == null)
            {
                FocusMainTools.IsEnabled = false;
            }
            else
            {
                FocusMainTools.IsEnabled = true;
            }
            CurrentFocusTree = ProjectData.FocusTrees.FirstOrDefault(i => i.Id == FocusTreeInput.SelectedItem.ToString());
        }
        
    }
}
