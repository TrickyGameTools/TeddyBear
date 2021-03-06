// Lic:
// TeddyBear C#
// Launcher
// 
// 
// 
// (c) Jeroen P. Broks, 
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
// Please note that some references to data like pictures or audio, do not automatically
// fall under this licenses. Mostly this is noted in the respective files.
// 
// Version: 19.04.05
// EndLic




#region using a lot of stuff
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
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;

using TrickyUnits;

#endregion region

namespace TeddyBear
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static public string MyExe => System.Reflection.Assembly.GetEntryAssembly().Location;
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
                NewNameLabel.Visibility = Visibility.Hidden;
                NewName.Visibility=Visibility.Hidden;
                return;
            }
            var newstuff = Visibility.Hidden;
            PrjSelect.IsEnabled = true;
            PrjMapSelect.IsEnabled = PrjSelect.SelectedItem != null;
            PrjLoad.IsEnabled = PrjMapSelect.SelectedItem != null;
            PrjLoad.Content = "Load";
            string SelPrj = (string)PrjSelect.SelectedValue;
            string SelMap = (string)PrjMapSelect.SelectedValue;
            if (SelPrj=="**NEW PROJECT**") {
                PrjLoad.Content = "Create Project";
                PrjLoad.IsEnabled = true;
                PrjMapSelect.IsEnabled = false;
            }  else {
                //MessageBox.Show(SelMap);
                if (SelMap == "**NEW MAP**") newstuff = Visibility.Visible;
            }
            NewName.Visibility = newstuff;
            NewNameLabel.Visibility = newstuff;


        }

        public MainWindow()
        {
            InitializeComponent();
            if (!File.Exists(GINIFILE)) QuickStream.SaveString(GINIFILE, "[rem]\nTeddybear knows nothing yet! Boring, huh?\n");
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
                    System.Diagnostics.Debug.WriteLine("Init Alt Drive: Windows");
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

        #region Chaining with the wizard or the editor    
        private void PrjLoad_Click(object sender, RoutedEventArgs e) {
            string SelPrj = (string)PrjSelect.SelectedValue;
            string SelMap = (string)PrjMapSelect.SelectedValue;
            string mapfile = "";
            if (SelPrj=="**NEW PROJECT**") {
                var wizard = $"{qstr.ExtractDir(MyExe)}/TeddyWizard.exe".Replace("\\","/");
                try {
                    Process.Start(wizard);
                } catch (Exception err) {
                    MessageBox.Show($"Launching the project creation wizard failed!\n{err.Message}\n\n{wizard}");                    
                }
                return;
            }
            if (SelMap == "**NEW MAP**") {
                var prjallowregex = new Regex(@"^[a-zA-Z0-9_ ]+$");
                if (!prjallowregex.IsMatch(NewName.Text)) {
                    MessageBox.Show("Map names may only contain letters, numbers underscores and spaces");
                    return;
                }
                mapfile = NewName.Text.Trim();
            } else { mapfile = SelMap; }
            var editor = $"{qstr.ExtractDir(MyExe)}/TeddyEdit.exe".Replace("\\","/");
            try {
                Process.Start(editor, $"\"{SelPrj}\" \"{mapfile}\"");
            } catch (Exception err) {
                MessageBox.Show($"Launching the edit failed!\n{err.Message}\n\n{editor}");
            }
        }
        #endregion

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

        void ScanMaps() {            
            string SelPrj = (string)PrjSelect.SelectedValue;
            PrjMapSelect.Items.Clear();
            if (SelPrj == "" || SelPrj[0] == '*') return;
            PrjMapSelect.Items.Add("**NEW MAP**");
            //System.Diagnostics.Debug.WriteLine($"Alt Drives {Dirry.ADrives()}"); // Debug

            var pfile = Dirry.AD($"{DirWorkSpace}/{SelPrj}/{SelPrj}.Project.GINI");
            TGINI TProject = GINI.ReadFromFile(pfile);
            if (TProject == null)
                MessageBox.Show($"Failed to load the project file!\n{pfile}");
            else {
                var ls = FileList.GetDir(Dirry.AD(TProject.C("LEVELDIR")));
                foreach (string f in ls) PrjMapSelect.Items.Add(f);
            }
        }


        private void PrjMapSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {            
            AutoEnable();
        }

        private void PrjSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ScanMaps();
            AutoEnable();
        }
    }
}
