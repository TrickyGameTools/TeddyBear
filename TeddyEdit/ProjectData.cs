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
// Version: 19.03.16
// EndLic

#define debuglog


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

using TrickyUnits;
using UseJCR6;

namespace TeddyEdit
{
    class ProjectData {
#if debuglog
        static QuickStream dbglogbt = QuickStream.WriteFile("E:/Home/Temp/TeddyLog");
#endif
        static public string MyExe => System.Reflection.Assembly.GetEntryAssembly().Location;
        static public string[] args => Environment.GetCommandLineArgs();
        static string _prj;
        static public TGINI ProjectConfig { get; private set; } = null;
        static public bool AllWell { get; private set; } = true;
        static public string Project
        {
            get => _prj;
            set
            {
                if (_prj != "") throw new Exception("Project overdefinition");
                _prj = value;
                ProjectConfig = GINI.ReadFromFile(value);
                if (ProjectConfig == null) AllWell = false;
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
        }

        public static void Log(string message) {
#if debuglog
            dbglogbt.WriteString($"{message}\n", true);
#endif
        }
    }
}
