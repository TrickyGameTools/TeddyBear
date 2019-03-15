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

using TrickyUnits;

namespace TeddyBear
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool wpchanged = false;

        void AutoEnable() {
            if (wpchanged) {
                PrjSelect.IsEnabled = false;
                PrjMapSelect.IsEnabled = false;
                PrjLoad.IsEnabled = false;
                wpchanged = false;
                return;
            }

        }

        public MainWindow()
        {
            InitializeComponent();
            AutoEnable();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void WorkSpace_TextChanged(object sender, TextChangedEventArgs e)
        {
            wpchanged = true;
            AutoEnable();
        }
    }
}
