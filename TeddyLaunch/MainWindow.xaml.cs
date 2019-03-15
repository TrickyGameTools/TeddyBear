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
using System.IO;

using TrickyUnits;

namespace TeddyBear
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool wpchanged = false;
        bool scanned = false;
        TGINI config = null;
        string GINIFILE = Dirry.C("$AppSupport$/TeddyBaseConfig.GINI");
        string DirWorkSpace => Dirry.AD(config.C("WORKSPACE"));

        void AutoEnable() {            
            PrjRenew.IsEnabled = DirWorkSpace != "";
            if (wpchanged || (!scanned) || (!PrjRenew.IsEnabled)) {
                PrjSelect.IsEnabled = false;
                PrjMapSelect.IsEnabled = false;
                PrjLoad.IsEnabled = false;
                return;
            }
            PrjSelect.IsEnabled = true;
            PrjMapSelect.IsEnabled = PrjSelect.SelectedItem != null;
            PrjLoad.IsEnabled = PrjMapSelect.SelectedItem != null;
            PrjLoad.Content = "Load";
            string SelPrj = (string)PrjSelect.SelectedValue;
            if (SelPrj=="**NEW PROJECT**") {
                PrjLoad.Content = "Create Project";
                PrjLoad.IsEnabled = true;
                PrjMapSelect.IsEnabled = false;
            } 

        }

        public MainWindow()
        {
            InitializeComponent();
            if (!System.IO.File.Exists(GINIFILE)) QuickStream.SaveString(GINIFILE, "[rem]\nTeddybear knows nothing yet! Boring, huh?\n");
            config = GINI.ReadFromFile(GINIFILE);
            if (config.C("Platform") == "") {
                var p = new string[] { "Windows", "Linux","***" };
                foreach(string pl in p) {
                    if (pl=="***") {
                        MessageBox.Show("Then I'm afraid you are on a non-supported system, sorry!");
                        Environment.Exit(1);
                    }
                    var r = MessageBox.Show($"Are you on {pl}?", "The current version of .NET has no PROPER platform detection, so I have to ask you:",MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (r == MessageBoxResult.Yes) {
                        config.D("Platform", pl);
                        config.SaveSource(GINIFILE);
                        break;
                    }
                }
            }
            switch (config.C("Platform")) {
                case "Windows":
                    Dirry.InitAltDrives(AltDrivePlaforms.Windows);
                    break;
                case "Linux":
                    if (config.C("LINUX_MEDIA")=="") {
                        MessageBox.Show($"I cannot find out myself where to find the folder in which Linux automatically mounts drives to. You can help me by editing {GINIFILE} and add the line LINUX_MEDIA=<your folder here> under the [vars] section.");
                        Environment.Exit(2);
                    }
                    Dirry.InitAltDrives(AltDrivePlaforms.Linux, config.C("LINUX_MEDIA"));
                    break;
                default:
                    MessageBox.Show("Unknown or unsupported platform!");
                    Environment.Exit(3);
                    break; // Not needed, but the C# compiler is not smart enough to notice, so it won't compile if not present.... DUH! :P
            }
            WorkSpace.Text = config.C("WORKSPACE");
            wpchanged = false;
            ScanProjects();
            AutoEnable();
        }

        private void WorkSpace_TextChanged(object sender, TextChangedEventArgs e)
        {
            wpchanged = true;
            config.D("WORKSPACE",WorkSpace.Text);
            AutoEnable();
        }

        private void PrjLoad_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PrjRenew_Click(object sender, RoutedEventArgs e) {
            if (wpchanged) {
                config.SaveSource(GINIFILE);
                wpchanged = false;
            }
            ScanProjects();            
        }

        void ScanProjects() {
            if (!Directory.Exists(DirWorkSpace)) {
                var r = MessageBox.Show($"Directory {DirWorkSpace} does not exist!\n\nDo you want me to create it?", "Hey!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (r) {
                    case MessageBoxResult.Yes:
                        try {
                            Directory.CreateDirectory(DirWorkSpace);
                        } catch (Exception e) {
                            MessageBox.Show($"Creation of {DirWorkSpace} failed\n\n{e.Message}");
                            return;
                        }
                        break;
                    default:
                        MessageBox.Show("Then there's nothing I can do. Change your workspace directory to another directory, or try it again");
                        break;
                }
            }
            PrjSelect.Items.Clear();
            PrjSelect.Items.Add("**NEW PROJECT**");
            var ds = FileList.GetDir(DirWorkSpace, 2);
            foreach (string d in ds) PrjSelect.Items.Add(d);
            scanned = true;
            AutoEnable();
        }


        private void PrjMapSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AutoEnable();

        }

        private void PrjSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //MessageBox.Show("Test");
            AutoEnable();
        }
    }
}
