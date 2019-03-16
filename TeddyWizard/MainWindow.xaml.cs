// Lic:
// TeddyBear C#
// Project Creation Wizard
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
// Version: 19.03.16
// EndLic


using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace TeddyWizard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            MKL.Version("TeddyBear - MainWindow.xaml.cs","19.03.16");
            MKL.Lic    ("TeddyBear - MainWindow.xaml.cs","GNU General Public License 3");
            InitializeComponent();
            MessageBox.Show("This wizard will create only a very simplistic project. If you want TeddyBear to have more power than this wizard can provide, please seek out the Wiki pages on GitHub on what you all can do", "TeddyWizard");
            copyright.Content = $"(c) {MKL.CYear(2019)} Jeroen P. Broks, Licensed under the terms of the GPL3";
        }

        void Afgekeurd(string m) => MessageBox.Show(m, "Form rejected");

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            TGINI config;
            try {
                config = GINI.ReadFromFile(Dirry.C("$AppSupport$/TeddyBaseConfig.GINI"));
            } catch (Exception err) {
                Afgekeurd($"Failed to load the configuration file\n{err.Message}");
                return;
            }
            switch (config.C("PLATFORM")) {
                case "Windows":
                    Dirry.InitAltDrives(AltDrivePlaforms.Windows);
                    break;
                case "Linux":
                    Dirry.InitAltDrives(AltDrivePlaforms.Linux,config.C("LINUX_MEDIA"));
                    break;
                default:
                    Afgekeurd($"Platform {config.C("PLATFORM")} unknown");
                    return;
            }
            var workspace = config.C("WORKSPACE");
            if (workspace=="") { Afgekeurd("Workspace not configured. Please run the launcher first!"); return; }

            TGINI project = new TGINI();
            var prjallowregex = new Regex(@"^[a-zA-Z0-9_ ]+$");
            ProjectName.Text = ProjectName.Text.Trim();
            var projectdir = $"{workspace}/{ProjectName.Text}";
            if (ProjectName.Text == "") { Afgekeurd("No project name given!"); return; }
            if (!prjallowregex.IsMatch(ProjectName.Text)) { Afgekeurd("Only numbers letters underscores and spaces allowed in project name"); return; }
            if (Directory.Exists(projectdir)) { Afgekeurd("That project already exists!"); return; }
            var mapw = qstr.ToInt(MapW.Text);
            var maph = qstr.ToInt(MapH.Text);
            if (mapw<=0 || maph<=0) { Afgekeurd("The map format has incorrect values"); return; }
            if (mapw * maph > 50000) {
                var r = MessageBox.Show($"These settings will be very very costly on your RAM. {mapw * maph} bytes per layer at least, and then the object layer not counted. Are you SURE, you wanna do this?", "Are you crazy?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (r == MessageBoxResult.No) return;
            }
            var layers = Layers.Text.Split(';');
            if (layers.Length<1) { Afgekeurd("A TeddyBear project MUST have at least 1 layer"); }

            // Not really needed as the editor will never use it, but I guess it's just good practise
            project.D("PROJECTNAME", ProjectName.Text);

            // Create Project
            try {
                Directory.CreateDirectory(Dirry.AD(projectdir));
            } catch(Exception err) { Afgekeurd($"Project creation folder failed!\n\nDir: {Dirry.AD(projectdir)}\n\n{err.Message}"); return; }

            // Grid should when you are not an advanced user always be 32x32, and if you are an advanced user, why are you using this wizard?
            project.D("GRIDX", "32");
            project.D("GRIDY", "32");

            // Sizes
            project.D("SIZEX", $"{mapw}");
            project.D("SIZEY", $"{maph}");
            project.D("ResizeTextures", "FALSE");

            // Layers
            foreach(string layer in Layers.Text.Split(';')) {
                if (layer != "") {
                    project.Add("LAYERS", layer);
                    if (!qstr.Prefixed(layer, "Zone_")) project.D($"HOT.{layer}", "BC");
                }
            }

            // Map dir
            var mapdir = MapFileFolder.Text;
            if (mapdir == "*InProject*") mapdir = $"{projectdir}/Maps";
            project.D("LevelDir", mapdir);
            try {
                Directory.CreateDirectory(Dirry.AD(mapdir));
            } catch (Exception err) {
                Afgekeurd($"Creation/access of map folder failed! -- Please note that a (project dir is now already created, you may need to destroy it for another go)\n\n{err.Message}");
                return;
            }

            // Texture dirs
            var texdir = TextureFolder.Text;
            if (texdir == "*InProject*") texdir = $"{projectdir}/Textures";
            var texdirs = texdir.Split(';');
            foreach (string td in texdirs) {
                project.Add("textures", td);
                try {
                    Directory.CreateDirectory(Dirry.AD(td));
                } catch (Exception err) {
                    Afgekeurd($"Creation/access of texture folder {td} failed! -- Please note that a (project dir is now already created, you may need to destroy it for another go)\n\n{err.Message}");
                    return;
                }
            }

            // Meta data
            var metas = MetaData.Text.Split(';');
            foreach(string meta in metas) {
                project.Add("Data", meta.Trim());
            }

            // Create project file
            try {
                project.SaveSource(Dirry.AD($"{projectdir}/{ProjectName.Text}.Project.GINI"));
            } catch(Exception err) {
                Afgekeurd($"I could not create project file '{projectdir}/{ProjectName.Text}.Project.GINI'\n\n{err.Message}");
                return;
            }
            MessageBox.Show("Project succesfully created", ProjectName.Text);
        }

        
    }
}
