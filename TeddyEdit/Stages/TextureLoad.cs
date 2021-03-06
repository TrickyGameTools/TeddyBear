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
// Version: 19.04.05
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
        static int maxScrollY = 0;
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
                    maxScrollY = Texes.Count * 20;
                }                
            }
        }

        bool AllowFormat(string ext) => FormatAllowed.Contains(ext.ToUpper());

        public override void Draw(Game1 game, GameTime gameTime) {
            #region draw list itself
            foreach(TL_Item item in Texes) {
                if (item.high)
                    TQMG.Color(255, 180, 0);
                else
                    TQMG.Color(255, 255, 255);
                item.dt.Draw(20, item.y - scrollY);
            }
            #endregion
            #region Draw scroll arrow
            #region up
            if (ms.X > ProjectData.Game.Window.ClientBounds.Width - 32 && ms.Y < 32)
                TQMG.Color(180, 0, 255);
            else
                TQMG.Color(0,180, 255);
            UI.ArrowUp.Draw(ProjectData.Game.Window.ClientBounds.Width - 32, 0);
            #endregion
            #region Down
            if (ms.X > ProjectData.Game.Window.ClientBounds.Width - 32 && ms.Y > ProjectData.Game.Window.ClientBounds.Height - 32)
                TQMG.Color(180, 0, 255);
            else
                TQMG.Color(0, 180, 255);
            UI.ArrowDn.Draw(ProjectData.Game.Window.ClientBounds.Width - 32, ProjectData.Game.Window.ClientBounds.Height - 32);
            #endregion
            #endregion
        }

        public override void Update(Game1 game, GameTime gameTime, MouseState mouse, GamePadState gamepad, KeyboardState kb) {
            ms = mouse;
            if (ms.LeftButton==ButtonState.Pressed && ms.X > ProjectData.Game.Window.ClientBounds.Width - 32) {
                if (scrollY > 0 && ms.Y < 32) scrollY -= 6;
                else if (scrollY < ProjectData.Game.Window.ClientBounds.Width - 32 && scrollY < maxScrollY - 50) scrollY += 6;
                if (scrollY < 0) scrollY = 0;
                if (scrollY > maxScrollY) scrollY = maxScrollY;
            } else {
                if (ms.LeftButton == ButtonState.Pressed) {
                    foreach (TL_Item item in Texes) {
                        if (item.high) {
                            ProjectData.Map.Texture[texSpot] = item.filename;
                            Main.ComeToMe();
                        }
                    }
                }
            }
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


    class AllTextures : BasisStage {

        readonly static AllTextures me = new AllTextures();
        byte spot;
        string prefix;
        
        void Go() {
            int i = spot;
            foreach (TJCREntry entry in ProjectData.texJCR.Entries.Values) {
                if (qstr.Prefixed(entry.Entry.ToUpper(), prefix.ToUpper())) {
                    while (i == 0 || !(ProjectData.Map.Texture[i] == "" || ProjectData.Map.Texture[i]==null)) {
                        ProjectData.Log($"{entry.Entry}: Not on spot {i.ToString("X2")} / \"{ProjectData.Map.Texture[i]}\"");
                        i++;
                        if (i > 255) i = 0;
                        if (i == spot) {
                            UI.ErrorNotice = $"I could not assign {entry.Entry}, as I had no more free spots";
                            Main.ComeToMe();
                            return;
                        }
                    }
                    ProjectData.Log($"Assigning {entry.Entry} to slot {i.ToString("X2")}");
                    ProjectData.Map.Texture[i] = entry.Entry;
                }
            }
            Main.ComeToMe();
        }

        public override void Draw(Game1 game, GameTime gameTime)  {
            TQMG.Color(0, 180, 255); // This effect was originall a bug, but in stead of fixing it, I decided to "seal" it in :P
            TQMG.SimpleTile(UI.back, 0, 0, UI.ScrWidth, UI.ScrHeight);
            TQMG.Color(255, 180, 0);
            UI.font32.DrawText($"All Textures! Starting at {spot.ToString("X2")}",UI.ScrWidth/2,50,TQMG_TextAlign.Center);
            TQMG.Color(180, 0, 255);
            UI.font20.DrawText("Prefix:", 10, 100);
            TQMG.Color(0, 18, 25);
            TQMG.DrawRectangle(10, 125, UI.ScrWidth - 20, 22);
            TQMG.Color(0, 180, 255);
            UI.font20.DrawText($"{prefix}|",12,126);
        }

        public override void Update(Game1 game, GameTime gameTime, MouseState mouse, GamePadState gamepad, KeyboardState kb) {
            TQMGKey.Start(kb);
            var ch = TQMGKey.GetChar();
            if (ch > 31 && UI.font20.TextWidth(prefix) < UI.ScrWidth - 25) prefix += ch;
            if (TQMGKey.Hit(Keys.Back) && prefix != "") prefix = qstr.Left(prefix, prefix.Length - 1);
            if (TQMGKey.Hit(Keys.Escape)) Main.ComeToMe();
            if (TQMGKey.Hit(Keys.Enter) && prefix != "") Go();
        }

        public static void ComeToMe(byte spot) {
            me.spot = spot;
            ProjectData.Game.SetStage(me);
            me.prefix = "";
        }

    }
}
