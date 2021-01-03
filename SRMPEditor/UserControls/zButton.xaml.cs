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
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace SRMPEditor
{
    /// <summary>
    /// Логика взаимодействия для zButton.xaml
    /// </summary>
    public partial class zButton : UserControl
    {
        static double AnimationDelay = 0.1d;
        static double ClickDelay = 250;
        public zButton()
        {
            InitializeComponent();
            border.BorderThickness = new Thickness(0);
            /*
            color = Color.FromRgb(224, 75, 115);
            colorHover = Color.FromRgb(248, 115, 141);
            colorClicked = Color.FromRgb(216, 55, 85);
            colorDisabled = Color.FromRgb(38,38,38);
            TextColor = Color.FromRgb(255, 255, 255);
            */
            bg.Color = color;

            this.Loaded += onLoad;
            rect.MouseUp += onMouseUp;
            rect.MouseDown += onMouseDown;
            rect.MouseLeave += onMouseLeave;
            rect.MouseEnter += onMouseEnter;
            timer.Tick += timerTick;
        }
        private void onLoad(object sender, RoutedEventArgs e)
        {

        }
        private void onMouseUp(object sender, MouseButtonEventArgs e)
        {
            IsHolding = false;
            timer.Stop();
            if (cl && click != null) click.Invoke(this, e);
        }
        private void onMouseDown(object sender, MouseButtonEventArgs e)
        {
            IsHolding = true;
            cl = true;
            timer.Start();
        }
        private void onMouseEnter(object sender, MouseEventArgs e)
        {
            IsHovered = true;
        }
        private void onMouseLeave(object sender, MouseEventArgs e)
        {
            IsHovered = false;
            cl = false;
        }

        private void timerTick(object sender, EventArgs e)
        {
            timer.Stop();
            if(!allowHolding)
            cl = false;
        }

        bool allowHolding = true;
        bool cl = false;
        bool ClickHolding = false;
        bool MouseHovered = false;
        
        public bool AllowHolding
        {
            get
            {
                return allowHolding;
            }
            set
            {
                allowHolding = value;
            }
        }

        bool IsHolding
        {
            get
            {
                return ClickHolding;
            }
            set
            {
                if (value) bg.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorClicked, Duration = TimeSpan.FromSeconds(AnimationDelay) });
                else if (IsHovered) bg.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorHover, Duration = TimeSpan.FromSeconds(AnimationDelay) });
                else bg.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = color, Duration = TimeSpan.FromSeconds(AnimationDelay) });
                ClickHolding = value;
            }
        }


        bool IsHovered
        {
            get
            {
                return MouseHovered;
            }
            set
            {
                IsHolding = !value ? false : IsHolding;
                if (IsHolding) bg.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorClicked, Duration = TimeSpan.FromSeconds(AnimationDelay) });
                else if(value) bg.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorHover, Duration = TimeSpan.FromSeconds(AnimationDelay) });
                else bg.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = color, Duration = TimeSpan.FromSeconds(AnimationDelay) });
                MouseHovered = value;
            }
        }

        public DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(ClickDelay) };

        public Color color
        {
            get
            {
                return (Color)GetValue(ColorProperty);
            }
            set
            {
                SetValue(ColorProperty, value);
                bg.Color = value;
            }
        }
        public Color colorHover
        {
            get
            {
                return (Color)GetValue(ColorHoverProperty);
            }
            set
            {
                SetValue(ColorHoverProperty, value);
            }
        }
        public Color colorClicked
        {
            get
            {
                return (Color)GetValue(ColorClickedProperty);
            }
            set
            {
                SetValue(ColorClickedProperty, value);
            }
        }
        public Color colorDisabled
        {
            get
            {
                return (Color)GetValue(ColorDisabledProperty);
            }
            set
            {
                SetValue(ColorDisabledProperty, value);
            }
        }

        public event RoutedEventHandler click;
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(zButton), new PropertyMetadata("Sample Text"));
        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), typeof(zButton));
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(Uri), typeof(zButton));
        public static readonly DependencyProperty CornerProperty = DependencyProperty.Register("Corner", typeof(CornerRadius), typeof(zButton), new PropertyMetadata(new CornerRadius(2.5)));
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("color", typeof(Color), typeof(zButton), new PropertyMetadata(Color.FromRgb(224, 75, 115)));
        public static readonly DependencyProperty ColorHoverProperty = DependencyProperty.Register("colorHover", typeof(Color), typeof(zButton), new PropertyMetadata(Color.FromRgb(248, 115, 141)));
        public static readonly DependencyProperty ColorClickedProperty = DependencyProperty.Register("colorClicked", typeof(Color), typeof(zButton), new PropertyMetadata(Color.FromRgb(216, 55, 85)));
        public static readonly DependencyProperty ColorDisabledProperty = DependencyProperty.Register("colorDisabled", typeof(Color), typeof(zButton), new PropertyMetadata(Color.FromRgb(38, 38, 38)));
        public static readonly DependencyProperty TextColorProperty = DependencyProperty.Register("TextColor", typeof(Color), typeof(zButton), new PropertyMetadata(Color.FromRgb(255, 255, 255)));

        public CornerRadius Corner
        {
            get
            {
                return (CornerRadius)GetValue(CornerProperty);
            }
            set
            {
                SetValue(CornerProperty, value);
            }
        }
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }
        public double FontSize
        {
            get
            {
                return (double)GetValue(FontSizeProperty);
            }
            set
            {
                tx.FontSize = value;
                SetValue(FontSizeProperty, value);
            }
        }
        public Uri ImgSource
        {
            get
            {
                return (Uri)GetValue(ImageSourceProperty);
            }
            set
            {
                if (value != null)
                {
                    w.Width = new GridLength(24);
                    img.Source = new BitmapImage(value);
                }
                else
                    w.Width = new GridLength(0);

                SetValue(ImageSourceProperty, value);
            }
        }
        public Color TextColor
        {
            get
            {
                return (Color)GetValue(TextColorProperty);
            }
            set
            {
                SetValue(TextColorProperty, value);
            }
        }
    }
}
