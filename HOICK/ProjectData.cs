using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HOICK
{

    public class Country
    {
        public string Tag;

        public Dictionary<string, string> NameLocalizations;
    }

    public class NationalFocus
    {

        public string Id, Icon, WillLeadToWarWith;

        public int Cost;
        public Tuple<double, double> Position = new Tuple<double, double>(0, 0);

        public List<Tuple<string, bool>> Prerequisites;
        public List<string> MutuallyExclusive, SearchFilters;
        public Dictionary<string, string> NameLocalizations, DescriptionLocalization;

        public bool AvailableIfCapitulated, CancelIfInvalid;

        public string Code;

        public void Render(Canvas canvas)
        {
            UserControls.NationalFocusControl f = new UserControls.NationalFocusControl
            {
                FocusName = NameLocalizations[App.Language.Name]
            };
            f.PreviewMouseDown += Focus_PreviewMouseDown;
            f.PreviewMouseUp += Focus_PreviewMouseUp;

            Canvas.SetLeft(f, Position.Item1);
            Canvas.SetTop(f, Position.Item2);

            _ = canvas.Children.Add(f);
        }

        private void Focus_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Workplace.DragObject = sender as UIElement;
            Workplace.offset = e.GetPosition(Application.Current.MainWindow.FindName("FocusCanvas") as Canvas);
            Vector p = VisualTreeHelper.GetOffset(Workplace.DragObject);
            Workplace.offset.X -= p.X;
            Workplace.offset.Y -= p.Y;
            _ = (Application.Current.MainWindow.FindName("FocusCanvas") as Canvas).CaptureMouse();
        }

        private void Focus_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            UIElement ui = sender as UIElement;
            Point pos = e.GetPosition(Application.Current.MainWindow.FindName("FocusCanvas") as Canvas);
            Vector p = VisualTreeHelper.GetOffset(ui);
            Position = new Tuple<double, double>(pos.X - p.X, pos.Y - p.Y);
        }
    }

    public class FocusTree
    {
        public List<NationalFocus> Focuses = new List<NationalFocus>();

        public string Id, Tag;

        public bool IsDefault, ResetOnCivwar;
    }

    public class ProjectData
    {
        public static List<FocusTree> FocusTrees = new List<FocusTree>();
        public static List<Country> Countries = new List<Country>();
    }
}
