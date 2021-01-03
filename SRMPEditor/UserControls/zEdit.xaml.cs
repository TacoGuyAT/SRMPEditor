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
    /// Логика взаимодействия для zEdit.xaml
    /// </summary>
    public partial class zEdit : UserControl
    {
        public zEdit()
        {
            InitializeComponent();
            SetValue(StringsCountProperty, tx.Text.IndexOf('\r') + (tx.Text.Length != 0 ? 1 : 0));
            tx.AcceptsReturn = (bool)GetValue(Multi_Property);
            if ((bool)GetValue(Multi_Property))
                tx.TextWrapping = TextWrapping.Wrap;
            else
                tx.TextWrapping = TextWrapping.NoWrap;
        }

        public event EventHandler stringCountChanged;
        public event TextChangedEventHandler TextChanged;


        public static readonly DependencyProperty StringsCountProperty = DependencyProperty.Register("StringsCount", typeof(int), typeof(zEdit), new PropertyMetadata(0));
        public static readonly DependencyProperty Multi_Property = DependencyProperty.Register("MultiProperty", typeof(bool), typeof(zEdit), new PropertyMetadata(false));

        public string Text
        {
            get
            {
                return tx.Text;
            }
            set
            {
                if(tx.Text != value)
                    tx.Text = value;
            }
        }
        public bool Multi_
        {
            get
            {
                return (bool)GetValue(Multi_Property);
            }
            set
            {
                tx.AcceptsReturn = value;
                if (!value)
                {
                    tx.Text = tx.Text.Replace("\r", "");
                    tx.TextWrapping = TextWrapping.NoWrap;
                }
                else
                    tx.TextWrapping = TextWrapping.Wrap;
                SetValue(Multi_Property, value);
            }
        }
        public int StringsCount
        {
            get
            {
                return (int)GetValue(StringsCountProperty);
            }
        }
        

        private void Tx_TextChanged(object sender, TextChangedEventArgs e)
        {
            int a = tx.Text.IndexOf('\r') + (tx.Text.Length != 0 ? 1 : 0);
            if (a != StringsCount)
            {
                SetValue(StringsCountProperty, a);
                if (stringCountChanged != null) stringCountChanged.Invoke(sender, (EventArgs)e);
            }
            if(TextChanged != null) TextChanged.Invoke(sender, e);
        }
    }
}
