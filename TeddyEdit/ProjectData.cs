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
