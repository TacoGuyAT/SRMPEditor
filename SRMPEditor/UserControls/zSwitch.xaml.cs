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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SRMPEditor
{
    /// <summary>
    /// Interaction logic for zSwitch.xaml
    /// </summary>
    public partial class zSwitch : UserControl
    {
        public bool isChecked;
        static double ClickDelay = 250;
        static double AnimationDelay = 0.1d;
        public event RoutedEventHandler click;
        public zSwitch()
        {
            InitializeComponent();
            bg.Background = new SolidColorBrush(colorBackground);
            if(isChecked)
                inside.Background = new SolidColorBrush(colorChecked);
            else
                inside.Background = new SolidColorBrush(colorUnchecked);

            this.Loaded += onLoad;
            rect.MouseUp += onMouseUp;
            rect.MouseDown += onMouseDown;
            rect.MouseLeave += onMouseLeave;
            rect.MouseEnter += onMouseEnter;
            timer.Tick += timerTick;
        }

        private void onLoad(object sender, RoutedEventArgs e)
        {
            setCheckedInstant(isChecked);
        }
        public DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(ClickDelay) };

        private void onMouseUp(object sender, MouseButtonEventArgs e)
        {
            IsHolding = false;
            timer.Stop();
            if (cl && click != null)
            {
                setChecked(!isChecked, true);
                click.Invoke(this, e);
            }
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
            if (!allowHolding)
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
                if (isChecked)
                {
                    if (value) inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorCheckedClicked, Duration = TimeSpan.FromSeconds(AnimationDelay) });
                    else if (IsHovered) inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorCheckedHover, Duration = TimeSpan.FromSeconds(AnimationDelay) });
                    else inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorChecked, Duration = TimeSpan.FromSeconds(AnimationDelay) });
                } else {
                    if (value) inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorUncheckedClicked, Duration = TimeSpan.FromSeconds(AnimationDelay) });
                    else if (IsHovered) inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorUncheckedHover, Duration = TimeSpan.FromSeconds(AnimationDelay) });
                    else inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorUnchecked, Duration = TimeSpan.FromSeconds(AnimationDelay) });
                }
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
                if (isChecked) {
                    if (IsHolding) inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorCheckedClicked, Duration = TimeSpan.FromSeconds(AnimationDelay) });
                    else if (value) inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorCheckedHover, Duration = TimeSpan.FromSeconds(AnimationDelay) });
                    else inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorChecked, Duration = TimeSpan.FromSeconds(AnimationDelay) });
                } else {
                    if (IsHolding) inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorUncheckedClicked, Duration = TimeSpan.FromSeconds(AnimationDelay) });
                    else if (value) inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorUncheckedHover, Duration = TimeSpan.FromSeconds(AnimationDelay) });
                    else inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorUnchecked, Duration = TimeSpan.FromSeconds(AnimationDelay) });
                }
                MouseHovered = value;
            }
        }
        public static readonly DependencyProperty CornerProperty = DependencyProperty.Register("Corner", typeof(CornerRadius), typeof(zSwitch), new PropertyMetadata(new CornerRadius(2.5)));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(zSwitch), new PropertyMetadata("Sample Text"));
        public static readonly DependencyProperty ColorCheckedProperty = DependencyProperty.Register("colorChecked", typeof(Color), typeof(zSwitch), new PropertyMetadata(Color.FromRgb(224, 75, 115)));
        public static readonly DependencyProperty ColorCheckedHoverProperty = DependencyProperty.Register("colorCheckedHover", typeof(Color), typeof(zSwitch), new PropertyMetadata(Color.FromRgb(248, 115, 141)));
        public static readonly DependencyProperty ColorCheckedClickedProperty = DependencyProperty.Register("colorCheckedClicked", typeof(Color), typeof(zSwitch), new PropertyMetadata(Color.FromRgb(216, 55, 85)));
        public static readonly DependencyProperty ColorUncheckedProperty = DependencyProperty.Register("colorUnchecked", typeof(Color), typeof(zSwitch), new PropertyMetadata(Color.FromRgb(38, 38, 38)));
        public static readonly DependencyProperty ColorUncheckedHoverProperty = DependencyProperty.Register("colorUncheckedHover", typeof(Color), typeof(zSwitch), new PropertyMetadata(Color.FromRgb(106, 106, 106)));
        public static readonly DependencyProperty ColorUncheckedClickedProperty = DependencyProperty.Register("colorUncheckedClicked", typeof(Color), typeof(zSwitch), new PropertyMetadata(Color.FromRgb(48, 48, 48)));
        public static readonly DependencyProperty ColorBackgroundProperty = DependencyProperty.Register("colorBackground", typeof(Color), typeof(zSwitch), new PropertyMetadata(Color.FromRgb(20, 20, 20)));
        public Color colorChecked
        {
            get
            {
                return (Color)GetValue(ColorCheckedProperty);
            }
            set
            {
                SetValue(ColorCheckedProperty, value);
            }
        }
        public Color colorCheckedHover
        {
            get
            {
                return (Color)GetValue(ColorCheckedHoverProperty);
            }
            set
            {
                SetValue(ColorCheckedHoverProperty, value);
            }
        }
        public Color colorCheckedClicked
        {
            get
            {
                return (Color)GetValue(ColorCheckedClickedProperty);
            }
            set
            {
                SetValue(ColorCheckedClickedProperty, value);
            }
        }
        public Color colorUnchecked
        {
            get
            {
                return (Color)GetValue(ColorUncheckedProperty);
            }
            set
            {
                SetValue(ColorUncheckedProperty, value);
            }
        }
        public Color colorUncheckedHover
        {
            get
            {
                return (Color)GetValue(ColorUncheckedHoverProperty);
            }
            set
            {
                SetValue(ColorUncheckedHoverProperty, value);
            }
        }
        public Color colorUncheckedClicked
        {
            get
            {
                return (Color)GetValue(ColorUncheckedClickedProperty);
            }
            set
            {
                SetValue(ColorUncheckedClickedProperty, value);
            }
        }
        public Color colorBackground
        {
            get
            {
                return (Color)GetValue(ColorBackgroundProperty);
            }
            set
            {
                ColorAnimation anim = new ColorAnimation { To = value, Duration = TimeSpan.FromMilliseconds(AnimationDelay) };
                bg.Background.BeginAnimation(SolidColorBrush.ColorProperty, anim);
                SetValue(ColorBackgroundProperty, value);
            }
        }
        public CornerRadius Corner
        {
            get
            {
                return (CornerRadius)GetValue(CornerProperty);
            }
            set
            {
                SetValue(CornerProperty, value);
                inside.CornerRadius = value;
                bg.CornerRadius = value;
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
        public void setCheckedInstant(bool isCheckedNew)
        {
            isChecked = isCheckedNew;
            if (isChecked)
            {
                inside.Margin = new Thickness(inside.ActualWidth / 2 + inside.Margin.Top * 2, inside.Margin.Top, inside.Margin.Top, inside.Margin.Top);
                inside.Background = new SolidColorBrush(colorChecked);
            }
            else
            {
                inside.Margin = new Thickness(inside.Margin.Top);
                inside.Background = new SolidColorBrush(colorUnchecked);
            }
        }
        public void setChecked(bool isCheckedNew, bool hover)
        {
            isChecked = isCheckedNew;
            if (isChecked)
            {
                inside.BeginAnimation(MarginProperty, new ThicknessAnimation { To = new Thickness(inside.ActualWidth / 2 + inside.Margin.Top * 2, inside.Margin.Top, inside.Margin.Top, inside.Margin.Top), Duration = TimeSpan.FromSeconds(AnimationDelay) });
                if(hover)
                    inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorCheckedHover, Duration = TimeSpan.FromSeconds(AnimationDelay) });
                else
                    inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorChecked, Duration = TimeSpan.FromSeconds(AnimationDelay) });
            } else {
                inside.BeginAnimation(MarginProperty, new ThicknessAnimation { To = new Thickness(inside.Margin.Top), Duration = TimeSpan.FromSeconds(AnimationDelay) });
                if (hover)
                    inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorUncheckedHover, Duration = TimeSpan.FromSeconds(AnimationDelay) });
                else
                    inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorUnchecked, Duration = TimeSpan.FromSeconds(AnimationDelay) });
            }
        }
    }
}
