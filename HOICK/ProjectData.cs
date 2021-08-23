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
        public Tuple<int, int> Position;

        public List<Tuple<string, bool>> Prerequisites;
        public List<string> MutuallyExclusive, SearchFilters;
        public Dictionary<string, string> NameLocalizations, DescriptionLocalization;

        public bool AvailableIfCapitulated, CancelIfInvalid;

        public string Code;

        public void Render(Canvas canvas)
        {
            /*
            Grid gr = new Grid();
            Label fn = new Label() {
                Content = NameLocalizations[App.Language.Name],
                Background = Brushes.Gray
            };
            _ = gr.Children.Add(fn);

            _ = canvas.Children.Add(gr);
            */

            UserControls.NationalFocusControl f = new UserControls.NationalFocusControl();
            f.FocusName = NameLocalizations[App.Language.Name];
            _ = canvas.Children.Add(f);
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
