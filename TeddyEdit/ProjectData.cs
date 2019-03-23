// Lic:
// TeddyBear C#
// Project Data
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
// Version: 19.03.20
// EndLic




#define debuglog


using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

using TrickyUnits;
using UseJCR6;
using TeddyBear;
using TeddyEdit.Stages;

namespace TeddyEdit
{
    class ProjectData {
#if debuglog
        static QuickStream dbglogbt = QuickStream.WriteFile("E:/Home/Temp/TeddyLog");
#endif
        static string GlobalConfigFile => Dirry.C("$AppSupport$/TeddyBaseConfig.GINI");
        static readonly TGINI GlobalConfig = GINI.ReadFromFile(GlobalConfigFile);
        static public string WorkSpace => Dirry.AD(GlobalConfig.C("Workspace"));
        static string Platform => GlobalConfig.C("Platform");
        static AltDrivePlaforms PlatID { get {
                switch (Platform) {
                    case "Windows": return AltDrivePlaforms.Windows;
                    case "Linux": return AltDrivePlaforms.Linux;
                    default: throw new Exception($"Unknown platform {Platform}");
                }
            } }
        static public Game1 Game { get; private set; }
        static public string MyExe => System.Reflection.Assembly.GetEntryAssembly().Location;
        static public string[] args => Environment.GetCommandLineArgs();
        static string _prj="";
        static public TGINI ProjectConfig { get; private set; } = null;
        static public bool AllWell { get; private set; } = true;
        static public TeddyMap Map { get; private set; } = null;
        static public TJCRDIR texJCR { get; private set; } = null;
        static public int MapWidth => qstr.ToInt(GlobalConfig.C("SIZEX"));
        static public int MapHeight => qstr.ToInt(GlobalConfig.C("SIZEY"));
        static public int MapGridX => qstr.ToInt(GlobalConfig.C("GRIDX"));
        static public int MapGridY => qstr.ToInt(GlobalConfig.C("GRIDY"));
        static public string[] MapLayers => GlobalConfig.List("Layers").ToArray();
        static public string Project
        {
            get => _prj;
            set
            {
                if (_prj != "") throw new Exception($"Project overdefinition\n({_prj})");
                Dirry.InitAltDrives(PlatID);
                _prj = value;                
                ProjectConfig = GINI.ReadFromFile($"{WorkSpace}/{value}/{value}.Project.GINI");
                if (ProjectConfig == null) AllWell = false;
            }
        }

        static string _map="";
        static public string MapFile {
            get => _map;
            set {
                if (_map != "") throw new Exception("Map overdefinition");
                _map = value;
                texJCR = new TJCRDIR();
                foreach (string patch in ProjectConfig.List("Textures")) texJCR.PatchFile(Dirry.AD(patch));
                if (JCR6.JERROR != "") Log($"{(char)27}[31mJCR6 ERROR: {(char)27}[0m{JCR6.JERROR}");
                if (!File.Exists(_map)) {
                    Map = TeddyMap.Create(MapWidth, MapHeight, MapGridX, MapGridY, MapLayers, texJCR);
                } else {
                    var MJ = JCR6.Dir(_map);
                    if (MJ == null) { Crash.Error(Game, $"Error loading map: {JCR6.JERROR}"); AllWell = false; return; }
                    Map = TeddyMap.Load(MJ, texJCR, "");
                }
                if (Map==null) { Crash.Error(Game,$"Error loading map \"{_map}\""); AllWell = false; return; }
            }
        }

        static public string JCRFile {
#if DEBUG
            get => "E:/Projects/Applications/VisualStudio/TeddyBear/Release/TeddyEdit.jcr";
#else
            get => $"{qstr.Left(MyExe,MyExe.Length-4)}.jcr";
#endif

        }

        static TJCRDIR MYJCR6DIR;

        static public TJCRDIR JCR {
            get {
                if (MYJCR6DIR == null) MYJCR6DIR = JCR6.Dir(JCRFile);
                if (MYJCR6DIR == null) throw new Exception($"JCR6 failed to load {JCRFile}\n\n{JCR6.JERROR}");
                return MYJCR6DIR;
            }
        }

        public static Texture2D GetTex(GraphicsDevice g,string file) {
            Texture2D ret;
            var bt = JCR.ReadFile(file);
            ret = Texture2D.FromStream(g,bt.GetStream());
            bt.Close();
            if (ret == null) throw new Exception($"I could not load {file} properly from {JCRFile}");
            return ret;
        }

        public static void InitJCRDrivers() {
            JCR6_lzma.Init();
            new JCR6_RealDir();
        }

        public static void Log(string message) {
#if debuglog
            dbglogbt.WriteString($"{message}\n", true);
#endif
        }

        static public void SetGame(Game1 g) { Game = g; }
    }
}
