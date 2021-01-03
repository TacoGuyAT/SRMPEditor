using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SRMPEditor
{
    /// <summary>
    /// Interaction logic for zList.xaml
    /// </summary>


    public partial class zList : UserControl
    {
        public zList()
        {
            InitializeComponent();
            scr.Padding = padding;
            panel.Margin = new Thickness(0, -8, 0, 0);
            color = Color.FromRgb(20, 20, 20);
            setPadding(8);
        }

        private Thickness padding = new Thickness(8);
        private List<Rectangle> spacingBetween = new List<Rectangle>();
        private Color color;

        public void Add(UIElement uc)
        {
            Rectangle spacing = new Rectangle { Height = padding.Top, Opacity = 0 };
            spacingBetween.Add(spacing);
            panel.Children.Add(spacing);
            panel.Children.Add(uc);
        }
        public void Remove(int index)
        {
            panel.Children.RemoveAt(index);
        }
        public void Remove(UIElement uc)
        {
            panel.Children.Remove(uc);
        }
        public void setPadding(int pixels)
        {
            padding = new Thickness(pixels);
            foreach(Rectangle rect in spacingBetween)
            {
                rect.Height = pixels;
            }
            scr.Padding = padding;
            panel.Margin = new Thickness(0, -pixels, 0, 0);
            brd.CornerRadius = new CornerRadius( pixels ); //Margin="0,8,8,8" CornerRadius="4"
        }
    }
}
