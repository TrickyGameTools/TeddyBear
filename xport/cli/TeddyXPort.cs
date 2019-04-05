// Lic:
// TeddyBear Xporter
// Main source for the CLI tool
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



using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using TrickyUnits;
using UseJCR6;

namespace TeddyXport{


    class XMain {
        static XMain p = new XMain();
        ConsoleColor oricol = Console.ForegroundColor;
        string GINIFILE => Dirry.C("$AppSupport$/TeddyBaseConfig.GINI").Replace("\\","/");
        string ProjectFile;
        TGINI MainConfig;
        TGINI ProjectConfig;
        string WorkSpace => MainConfig.C("WorkSpace");
        string XProject, XTarget, XTo;

        void cwrite(ConsoleColor c,string m) {
            Console.ForegroundColor = c;
            Console.Write(m);
            Debug.Write($"C:{m}");
        }

        void cwriteln(ConsoleColor c, string m) => cwrite(c, $"{m}\n");

        void dwrite(string m) => Debug.WriteLine($"D:{m}");
        void dwriteln(string m) => dwrite($"{m}\n");
        void Halt(int ecode) {
            Console.ForegroundColor = oricol;
            dwriteln($"\nHalt({ecode});");
            Environment.Exit(ecode);
        }


        void Crash(string ErrorMessage) {
            cwrite(ConsoleColor.Red,"ERROR! ");
            cwriteln(ConsoleColor.Yellow, ErrorMessage);
            Halt(1);
            
        }

        void Crash(Exception E) {
            cwrite(ConsoleColor.Red, "ERROR! ");
            cwriteln(ConsoleColor.Yellow, E.Message);
            cwriteln(ConsoleColor.DarkCyan, E.StackTrace);
            cwriteln(ConsoleColor.Magenta, "\nIf you believe this happened because of a bug, please report it on my issue tracker:\nhttps://github.com/TrickyGameTools/TeddyBear/issues");
            Halt(2);
        }

        void LoadMainConfig() {
            if (!File.Exists(GINIFILE)) Crash($"I need configuration first!\nLoading the launcher will do that for you!");
            dwriteln($"Reading: {GINIFILE}");
            MainConfig = GINI.ReadFromFile(GINIFILE);
            switch (MainConfig.C("Platform").ToUpper()) {
                case "WINDOWS": Dirry.InitAltDrives(AltDrivePlaforms.Windows); break;
                case "LINUX": Dirry.InitAltDrives(AltDrivePlaforms.Linux); break;
                default:
                    Crash($"Unknown platform setting in project file: {MainConfig.C("Platform")}");
                    break;
            }
        }

        void SetupProject(string[] args) {
            dwriteln("Project setup started");
            FlagParse fp = new FlagParse(args);
            fp.CrString("Target", "");
            fp.CrString("To", "");
            fp.CrString("Project", "");
            if (!fp.Parse(
#if DEBUG
                true
#endif
                )) Crash("Flag parsing failed!");
            XProject = fp.GetString("Project");
            XTarget = fp.GetString("Target");
            XTo = fp.GetString("To");
            if (XProject=="" && XTarget=="" && XTo=="") {
                cwrite(ConsoleColor.Cyan, "Usage: ");
                cwrite(ConsoleColor.DarkGreen, $"{qstr.RemSuffix(qstr.StripDir(System.Reflection.Assembly.GetEntryAssembly().Location),".exe")} ");
                cwrite(ConsoleColor.DarkMagenta, "-<flag> <value>\n\n");
                cwrite(ConsoleColor.Red, string.Format("{0,15}", "-Project ")); cwriteln(ConsoleColor.Green, "Define the project");
                cwrite(ConsoleColor.Red, string.Format("{0,15}", "-Target ")); cwriteln(ConsoleColor.Green, "Define the target language");
                cwrite(ConsoleColor.Red, string.Format("{0,15}", "-To ")); cwriteln(ConsoleColor.Green, "Define the folder where the translations should go to");
                Console.WriteLine("");
                cwriteln(ConsoleColor.Yellow, "Supported target languages:");
                foreach(string drv in XPort_Base.Drivers.Keys){
                    cwrite(ConsoleColor.Red, "= ");
                    cwriteln(ConsoleColor.DarkYellow, drv);
                }
                Console.WriteLine("");
                cwriteln(ConsoleColor.White, "Please note that \"To\" and \"Target\" can also be defined in the project file, so you don't have to name them here.\nWhen using them here on the cli even if set in the project file, the value set in the cli will take priority!");
                Halt(0);
            }
            if (XProject == "") Crash("No Project!");
            ProjectFile = Dirry.AD($"{MainConfig.C("WorkSpace")}/{XProject}/{XProject}.project.GINI");
            dwriteln($"Reading: {ProjectFile}");
            ProjectConfig = GINI.ReadFromFile(ProjectFile);
            cwrite(ConsoleColor.Yellow, "Reading Project: ");
            cwriteln(ConsoleColor.Cyan, XProject);
            if (ProjectConfig == null) Crash($"Reading {ProjectFile} failed!");
            if (XTarget == "") XTarget = ProjectConfig.C("XPORT.TARGET");
            if (XTarget == "") Crash("And to what language do you want to translate this to? Without the -Target flag, I don't know!");
            if (XTo == "") XTo = ProjectConfig.C("XPORT.TO");
            if (XTo == "") Crash("And where do you want the translations to be put? Without the -To flag, I don't know!");            
            XTarget = XTarget.ToLower();
            if (!XPort_Base.Drivers.ContainsKey(XTarget)) {
                Crash($"I cannot export to {XTarget}");
            }
        }

        void CreateTo() {
            try {
                Directory.CreateDirectory(XTo);
            } catch (Exception E) {
                Crash(E.Message); // I don't want these errors in my bug tracker, thank you!
            }
        }

        void Doing(string a, string b) {
            cwrite(ConsoleColor.Yellow, $"{a}: ");
            cwriteln(ConsoleColor.Cyan, b);
        }

        void TranslateMaps() {
            Doing("Analysing Maps for Project", XProject);
            var tdrv = XPort_Base.Drivers[XTarget];
            var mapdir = Dirry.AD(ProjectConfig.C("LevelDir"));
            var fl = FileList.GetDir(mapdir);
            foreach(string mapname in fl) {
                Doing("Translating", mapname);
                var map = TeddyBear.TeddyMap.Load($"{mapdir}/{mapname}");
                if (map == null) Crash($"Map Load Error\n{JCR6.JERROR}");
                var translation = tdrv.Translate(map);
                QuickStream.SaveString(tdrv.TransFile($"{XTo}/{mapname}"), translation);
            }
        }

        void main(string[] args) {
            MKL.Version("TeddyBear - TeddyXPort.cs","19.04.05");
            MKL.Lic    ("TeddyBear - TeddyXPort.cs","GNU General Public License 3");
            cwrite(ConsoleColor.Yellow, "TeddyBear Exporter ");
            cwriteln(ConsoleColor.Cyan, "\tCoded by: Jeroen P. Broks");
            cwriteln(ConsoleColor.Magenta, $"(c) {MKL.CYear(2019)} Jeroen P. Broks, Released under the terms of the GPL 3\n");
            XPort.init();
            JCR6_lzma.Init();
            JCR6_zlib.Init();
            LoadMainConfig();
            SetupProject(args);
            CreateTo();
            TranslateMaps();
        }

        static void Main(string[] args) {
            try {
                p.main(args); // I just hate all that static crap!
            } catch(Exception E) { p.Crash(E); }
            p.Halt(0);
        }

    }
}

