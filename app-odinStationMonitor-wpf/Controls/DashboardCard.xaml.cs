using System.Windows;
using System.Windows.Controls;


namespace TacoStationMonitor.Controls
{
    public partial class DashboardCard : UserControl // System.Windows.Controls.UserControl // UserControl
    {
        public DashboardCard()
        {
            InitializeComponent();
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title),
                typeof(string),
                typeof(DashboardCard));

        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value),
                typeof(string),
                typeof(DashboardCard));
    }
}
