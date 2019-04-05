// Lic:
// TeddyBear C#
// Object Selector
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
using TrickyUnits;
using TeddyBear;

namespace TeddyEdit.Stages
{
    class ObjectSelector : BasisStage {

        struct ObjectItem {
            public int x, y;
            public string ObjName;
            public TQMGText ObjText;
            public ObjectItem(int ax,int ay, string aObjName) {
                x = ax;
                y = ay;
                ObjName = aObjName;
                ObjText = UI.font20.Text(aObjName);
                ProjectData.Log($"- Object list item created {ObjName} -- Set onto ({x},{y})");
            }
        }

        List<ObjectItem> ObjectItems = new List<ObjectItem>();
        static ObjectSelector me;
        //int ScrollY = 0;
        TQMGText Caption = UI.font32.Text("Please select a proper object");
        MouseState mstate = new MouseState(0, 0, 0, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);
        

        public override void Draw(Game1 game, GameTime gameTime) {
            TQMG.Color(127, 127, 127);
            TQMG.SimpleTile(UI.back, 0, 0, UI.ScrWidth, UI.ScrHeight);
            TQMG.Color(255, 180, 0);
            Caption.Draw(UI.ScrWidth / 2, 25, TQMG_TextAlign.Center);
            foreach(ObjectItem item in ObjectItems) {
                TQMG.Color(180, 0, 255);
                if (mstate.X>item.x && mstate.X<item.x+95 && mstate.Y>item.y && mstate.Y<item.y + 20) {
                    TQMG.Color(0, 180, 255);
                    if (mstate.LeftButton == ButtonState.Pressed) {
                        UI.CurrentObject = item.ObjName;
                        Main.ComeToMe();
                        UI.DontMouse = true;

                    }
                }
            item.ObjText.Draw(item.x + 3, item.y);
            }
        }

        public override void Update(Game1 game, GameTime gameTime, MouseState mouse, GamePadState gamepad, KeyboardState kb) {
            mstate = mouse;
        }

        ObjectSelector() {
            var maxcols = (int)(Math.Floor((double)(UI.ScrWidth / 100)));
            var objs = ProjectData.ProjectConfig.List("Objects").ToArray(); Array.Sort(objs);
            var percol = (int)(Math.Ceiling((double)(objs.Length / 100)));
            var ix = 0;
            var iy = 0;
            foreach(string obj in objs) {
                var item = new ObjectItem(ix*100, (iy*25) + 80, obj);
                ObjectItems.Add(item);
                iy++;
                if (iy>percol) { iy = 0; ix++; }
            }
            ProjectData.Log($"Objects in list: {ObjectItems.Count}");
        }

        static public void ComeToMe() {
            if (me == null)
                me = new ObjectSelector();
            if (ProjectData.ProjectConfig.List("Objects").Count == 0)
                UI.ErrorNotice = "I cannot go to the Object Selector! This project has no object list.";
            else
                ProjectData.Game.SetStage(me);
        }
    }

    class ObjectEditor:BasisStage {
        static ObjectEditor me = new ObjectEditor();
        TeddyObject obj;
        int x, y;
        TQMGFont font32 => UI.font32;
        TQMGFont font20 => UI.font20;
        string curField = "";
        MouseState muis;

        public override void Draw(Game1 game, GameTime gameTime) {
            var d = DateTime.Now.Second;
            var c = "|";
            var y = 100;
            if (d % 2 == 0) c = "";
            TQMG.Color(127, 127, 127);
            TQMG.SimpleTile(UI.back, 0, 0, UI.ScrWidth, UI.ScrHeight);
            TQMG.Color(255, 180, 0);
            font32.DrawText($"Object ({x},{y}): {obj.Cl("TeddyID")}", UI.ScrWidth / 2, 50, TQMG_TextAlign.Center);
            foreach(string key in ProjectData.ProjectConfig.List($"OBJECT.{obj.ObjType}")) {
                if (curField == "") curField = key;
                var cur = "";
                var r = (byte)180;
                var g = (byte)0;
                var b = (byte)255;
                if (key==curField) {
                    cur = c;
                    r = 0;
                    g = 180;
                    b = 255;
                }
                TQMG.Color((byte)(r / 10), (byte)(g / 10), (byte)(b / 10));
                TQMG.DrawRectangle(250, y, UI.ScrWidth - 300, 21);
                TQMG.Color(r, g, b);
                font20.DrawText($"{obj.Cl(key)}{cur}", 252, y);
                TQMG.Color((byte)(255-r), (byte)(255-g), (byte)(255-b));
                font20.DrawText(key,240,y,TQMG_TextAlign.Right);
                if (muis.LeftButton == ButtonState.Pressed && muis.Y > y && muis.Y < y + 20) curField = key;
                y += 23;
            }
        }

        public override void Update(Game1 game, GameTime gameTime, MouseState mouse, GamePadState gamepad, KeyboardState kb) {
            TQMGKey.Start(kb);
            var ch = TQMGKey.GetChar();
            muis = mouse;
            if (kb.IsKeyDown(Keys.Escape)) { UI.DontMouse = true; Main.ComeToMe(); }
            if (kb.IsKeyDown(Keys.Delete) && (kb.IsKeyDown(Keys.LeftShift) || kb.IsKeyDown(Keys.RightShift))){
                ProjectData.Log($"Removed object: {obj.Cl("TeddyID")}");
                ProjectData.Map.ObjectList(x, y).Remove(obj);
                UI.DontMouse = true;
                Main.ComeToMe();
            }
            if (curField != "") {
                if (ch > 31 && font20.TextWidth(obj.Cl(curField)) < UI.ScrWidth - 290) obj.Df(curField, $"{obj.Cl(curField)}{ch}");
                if (TQMGKey.Hit(Keys.Back) && obj.Cl(curField) != "") obj.Df(curField,qstr.Left(obj.Cl(curField), obj.Cl(curField).Length-1));
            }
            
        }

        public static void ComeToMe(int x, int y, TeddyObject obj) {
            me.obj = obj;
            me.x = x;
            me.y = y;
            ProjectData.Game.SetStage(me);
        }
    }
}
