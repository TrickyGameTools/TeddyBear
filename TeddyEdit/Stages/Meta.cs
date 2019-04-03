// Lic:
// TeddyBear C#
// Meta data editor
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

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TrickyUnits;

namespace TeddyEdit.Stages
{
    class Meta : BasisStage {

        class MetaField {
            public string Key = "";
            public string Value {
                get {
                    if (!ProjectData.Map.MetaData.ContainsKey(Key)) return "";
                    return ProjectData.Map.MetaData[Key];
                }
                set => ProjectData.Map.MetaData[Key] = value;
            }
            public TQMGText Caption;
        }
        static Meta me = new Meta();
        TQMGText Caption = UI.font32.Text("Set meta data for this map:");
        static SortedDictionary<string, MetaField> Fields = new SortedDictionary<string, MetaField>();
        static Dictionary<bool, char> scursor = new Dictionary<bool, char>();
        static string chosen = "";
        static bool cursor = false;
        static MouseState mstate;
        static int MaxLength => UI.ScrWidth - 300;

        static Meta(){
            scursor[true] = '|';
            scursor[false] = ' ';
            }

        static public void ComeToMe() {
            ProjectData.Game.SetStage(me);
            // Init fields if that wasn't done before.
            foreach(string key in ProjectData.ProjectConfig.List("DATA")) {
                if (!Fields.ContainsKey(key)) {
                    Fields[key] = new MetaField();
                    Fields[key].Key = key;
                    Fields[key].Caption = UI.font20.Text(key);
                }
            }
        }

        public override void Draw(Game1 game, GameTime gameTime) {
            var y = 80;
            TQMG.Color(127, 127, 127);
            TQMG.SimpleTile(UI.back, 0, 0, UI.ScrWidth, UI.ScrHeight);
            TQMG.Color(255, 180, 0);
            Caption.Draw(UI.ScrWidth / 2, 50, TQMG_TextAlign.Center);
            cursor = !cursor;
            foreach (string key in Fields.Keys) {
                var f = Fields[key];
                y += 22;
                if (chosen == "") chosen = key;
                if (chosen == key) TQMG.Color(0, 180, 255); else TQMG.Color(180, 0, 255);
                f.Caption.Draw(25, y);
                TQMG.Color(64, 64, 64);
                TQMG.SetAlpha(127);
                TQMG.DrawRectangle(200, y, MaxLength, 21);
                TQMG.SetAlpha(255);
                if (chosen==key) {
                    TQMG.Color(180, 255, 0);
                    UI.font20.DrawText($"{f.Value}{scursor[cursor]}",205,y);
                } else {
                    TQMG.Color(255, 255, 255);
                    UI.font20.DrawText($"{f.Value}", 205, y);
                }
                if (mstate.Y > y && mstate.Y < y + 21 && mstate.LeftButton == ButtonState.Pressed) chosen = key;
            }
        }

        public override void Update(Game1 game, GameTime gameTime, MouseState mouse, GamePadState gamepad, KeyboardState kb) {
            mstate = mouse;
            TQMGKey.Start(kb);
            var ch = TQMGKey.GetChar();
            if (TQMGKey.Hit(Keys.Escape)) { Main.ComeToMe(); }
            if (chosen == "") return;
            var f = Fields[chosen];
            if (TQMGKey.Hit(Keys.Back) && f.Value.Length > 0) f.Value = qstr.Left(f.Value, f.Value.Length - 1);
            if (ch!='\0' && ch != '\n' && ch!='\t' && UI.font20.TextWidth(f.Value)<MaxLength-40) {
                f.Value += ch;
            }
        }
    }
}
