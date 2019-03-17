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
        static public string MyExe => System.Reflection.Assembly.GetEntryAssembly().Location;
        

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
    }
}
