using System.Windows.Controls;

namespace HOICK.UserControls
{
    /// <summary>
    /// Логика взаимодействия для NationalFocusControl.xaml
    /// </summary>
    public partial class NationalFocusControl : UserControl
    {
        public string FocusIcon { get; set; }
        public string FocusName { get; set; }
        public NationalFocusControl()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
