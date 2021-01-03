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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SRMPEditor
{
    /// <summary>
    /// Interaction logic for zDropList.xaml
    /// </summary>
    public partial class zDropList : UserControl
    {
        public static zList list = new zList();
        public zDropList()
        {
            InitializeComponent();
            btn.click += openList;
            btn.Text = "\\/";
            list.setPadding(1);
            list.Height = 100;
        }

        private void openList(object sender, RoutedEventArgs e)
        {
            Window listWindow = new Window();
            listWindow.WindowStyle = WindowStyle.None;
            listWindow.AllowsTransparency = true;
            listWindow.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            listWindow.Width = this.ActualWidth;
            listWindow.Height = list.Height;
            listWindow.Content = list;
//            listWindow.Left = sender.
            listWindow.Show();
            listWindow.Deactivated += List_LostFocus;
        }
        private void List_LostFocus(object sender, EventArgs e)
        {
            ((Window)sender).Close();
        }
    }
}
