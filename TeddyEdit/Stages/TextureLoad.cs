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
            
            public TL_Item(string af,int ay) { filename = af; y = ay; }
        }

        static TextureLoad me = new TextureLoad();
        List<string> FormatAllowed = new List<string>();
        List<TL_Item> Texes = new List<TL_Item>();

        TextureLoad() {
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
            
        }

        public override void Update(Game1 game, GameTime gameTime, MouseState mouse, GamePadState gamepad, KeyboardState kb)
        {
            
        }

        public static void ComeToMe()
        {
            ProjectData.Game.SetStage(me);
        }

    }
}
