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
// Version: 19.04.02
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
    enum TexAllow { NotSet, Deny, Allow, Annoy, Auto }
    static class MapConfig {
        static TGINI GINI => ProjectData.MapConfigGINI;
        static int scrollcount = 1000;
        static Dictionary<TexAllow, string> AllowCode2String = new Dictionary<TexAllow, string>();
        static void Save() {
            scrollcount = 1000;
            ProjectData.Log($"Saving mapconfig: {ProjectData.MapConfigFile}");
            Directory.CreateDirectory(qstr.ExtractDir(ProjectData.MapConfigFile));
            GINI.SaveSource(ProjectData.MapConfigFile);
        }

        static public int ScrollX { get => qstr.ToInt(GINI.C("ScrollX")); set {
                GINI.D("ScrollX", $"{value}");
                scrollcount--;
                if (scrollcount <= 0) Save();
            }
        }
        static public int ScrollY  {
            get => qstr.ToInt(GINI.C("ScrollY"));
            set {
                GINI.D("ScrollY", $"{value}");
                scrollcount--;
                if (scrollcount <= 0) Save();
            }
        }

        static public bool ShowGrid {
            get => GINI.C("ShowGrid") != "FALSE";
            set {
                switch (value) {
                    case true:
                        GINI.D("ShowGrid", "TRUE");
                        break;
                    case false:
                        GINI.D("ShowGrid", "FALSE");
                        break;
                }
                Save();
            }            
        }

        static public void GridCol(bool b) {
            if (GINI.C($"GRIDCOL{b}") == "") {
                if (b) GINI.D($"GRIDCOL{b}", "10,7,0"); else GINI.D($"GRIDCOL{b}", "0,7,10");
            }
            var s = GINI.C($"GRIDCOL{b}").Split(',');
            if (s.Length < 3) Crash.Error(ProjectData.Game, $"Syntax error in GRIDCODL{b} definition!   {GINI.C($"GRIDCOL{b}")}");
            TQMG.Color((byte)qstr.ToInt(s[0]), (byte)qstr.ToInt(s[1]), (byte)qstr.ToInt(s[2]));
        }


        // Get the allow setting
        static public TexAllow Allow(string Tex, string Lay) {
            //TexAllow ret = TexAllow.NotSet;
            var v = GINI.C($"ALLOW.{Tex}.{Lay}");
            if (v == "") return TexAllow.NotSet;
            foreach(TexAllow k in AllowCode2String.Keys) {
                if (AllowCode2String[k] == v) return k;
            }
            UI.ErrorNotice = $"I don't have a clue what allow code {v} means!";
            return TexAllow.NotSet;
        }

        // Set the allow setting (and thanks to overloading we can give both functions the same name).
        static public void Allow (string Tex, string Lay, TexAllow a) {
            ProjectData.Log($"Modifying allowance for texture\"{Tex}\" on layer {Lay} to {a}");
            GINI.D($"ALLOW.{Tex}.{Lay}", AllowCode2String[a]);
            Save();
        }

        static public bool AllowSet(string Tex) {
            var ret = false;
            foreach(string lay in ProjectData.Map.Layers.Keys) {
                ret = ret || Allow(Tex, lay) != TexAllow.NotSet;
            }
            return ret;
        }

        static MapConfig() {
            AllowCode2String[TexAllow.NotSet] = "Not Set"; // Safety precaution!
            AllowCode2String[TexAllow.Deny] = "Deny";
            AllowCode2String[TexAllow.Allow] = "Allow";
            AllowCode2String[TexAllow.Annoy] = "Annoy";
            AllowCode2String[TexAllow.Auto] = "Auto";
        }
        

    }

    class ProjectData {

        const string DefaultStorage = "lzma";

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
        static public TGINI MapConfigGINI { get; private set; }
        static public string MapConfigFile{ get; private set; } = "";
        static public TGINI ProjectConfig { get; private set; } = null;
        static public bool AllWell { get; private set; } = true;
        static public TeddyMap Map { get; private set; } = null;
        static public TJCRDIR texJCR { get; private set; } = null;
        static public int MapWidth => qstr.ToInt(ProjectConfig.C("SIZEX"));
        static public int MapHeight => qstr.ToInt(ProjectConfig.C("SIZEY"));
        static public int MapGridX => qstr.ToInt(ProjectConfig.C("GRIDX"));
        static public int MapGridY => qstr.ToInt(ProjectConfig.C("GRIDY"));
        static public string[] MapLayers => ProjectConfig.List("Layers").ToArray();
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
                    Log($"Creating a map with {MapLayers.Length} layers");
                    Map = TeddyMap.Create(MapWidth, MapHeight, MapGridX, MapGridY, MapLayers, texJCR);                    
                } else {
                    var MJ = JCR6.Dir(_map);
                    if (MJ == null) { Crash.Error(Game, $"Error loading map: {JCR6.JERROR}"); AllWell = false; return; }
                    Map = TeddyMap.Load(MJ, texJCR, "");
                    foreach (string lay in MapLayers)
                        if (!Map.Layers.ContainsKey(lay)) Map.NewLayer(lay);
                }
                if (Map==null) { Crash.Error(Game,$"Error loading map \"{_map}\""); AllWell = false; return; }
                // The settings file is kept apart from the map file for the main reason that TeddyBear has primarily been designed
                // for JCR6 based game engines for which JCR6 can merge the map files straight into the game main data file.
                // This settings file is only for the benefit of this editor, and it would be pointless to keep in the game.
                // Maybe not very useful for when you plan to use exporters, but for TeddyBear's primary setup essential.
                // Also these settings are NOT compatible with the old TeddyBear, sorry!
                MapConfigFile = $"{WorkSpace}/{Project}/MapSettings/{qstr.StripDir(value)}.Settings.GINI";
                Directory.CreateDirectory(qstr.ExtractDir(MapConfigFile));
                if (File.Exists($"{MapConfigFile}"))
                    MapConfigGINI = GINI.ReadFromFile($"{MapConfigFile}");
                else
                    MapConfigGINI = new TGINI();            
            }
        }

        static public string MapCompression  {
            get {
                var Storage = ProjectConfig.C("Compression").Trim();
                if (Storage == "")
                    return DefaultStorage;
                return Storage;
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
            System.Diagnostics.Debug.WriteLine(message);
        }

        static public void SetGame(Game1 g) { Game = g; }
    }
}
