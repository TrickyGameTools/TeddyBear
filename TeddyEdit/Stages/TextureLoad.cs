// Lic:
// TeddyBear C#
// Texture Load
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
// Version: 19.03.22
// EndLic

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using UseJCR6;
using TrickyUnits;

namespace TeddyEdit.Stages
{
    class TextureLoad : BasisStage {

        class TL_Item {
            public string filename;
            public int y;
            public TQMGText dt;
            public TL_Item(string af,int ay) { filename = af; y = ay; dt = UI.font20.Text(filename); }
            public bool high {
                get {
                    var ty = y - scrollY;
                    return ms.Y >= ty && ms.Y <= ty + 20 && ms.X < ProjectData.Game.Window.ClientBounds.Width - 100;
                }
            }
        }

        static TextureLoad me = null;
        static List<string> FormatAllowed = new List<string>();
        static List<TL_Item> Texes = new List<TL_Item>();
        static int scrollY = 0;
        static int texSpot = 0;
        static MouseState ms;

        void Init() {            
            FormatAllowed.Add("PNG");
            foreach (string f in ProjectData.ProjectConfig.List("ImageExtAllowed")) {
                FormatAllowed.Add(f.ToUpper());
            }


            bool TexesContains(string e) {
                var ec = e.ToUpper();
                foreach (TL_Item it in Texes)
                    if (it.filename.ToUpper() == ec)
                        return true;
                return false;
            }

            foreach (TJCREntry ent in ProjectData.texJCR.Entries.Values) {
                var fn = ent.Entry;
                var e = qstr.ExtractExt(fn);
                var sqn = fn.Split('/');
                var bundle = false;
                foreach (string folder in sqn) bundle = bundle || qstr.ExtractExt(folder).ToUpper() == "JPBF";
                if (bundle) {
                    var dir = qstr.ExtractDir(fn);
                    if (!TexesContains(dir)) Texes.Add(new TL_Item(dir, Texes.Count * 20));
                } else {
                    Texes.Add(new TL_Item(fn, Texes.Count * 20));
                }                
            }
        }

        bool AllowFormat(string ext) => FormatAllowed.Contains(ext.ToUpper());

        public override void Draw(Game1 game, GameTime gameTime) {
            // list
            foreach(TL_Item item in Texes) {
                if (item.high)
                    TQMG.Color(255, 180, 0);
                else
                    TQMG.Color(255, 255, 255);
                item.dt.Draw(20, item.y - scrollY);
            }
            // tools
        }

        public override void Update(Game1 game, GameTime gameTime, MouseState mouse, GamePadState gamepad, KeyboardState kb) {
            ms = mouse;
        }

        public static void ComeToMe(int texspot)
        {
            if (me == null) {
                me = new TextureLoad();
                me.Init();
            }
            scrollY = 0;
            texSpot = texspot;
            ProjectData.Game.SetStage(me);
        }

    }
}
