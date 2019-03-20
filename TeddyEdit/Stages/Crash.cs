// Lic:
// TeddyBear C#
// Crash Stage (does that make any sense?)
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
// Version: 19.03.20
// EndLic




using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using TrickyUnits;


namespace TeddyEdit.Stages
{
    class Crash : BasisStage
    {
        static List<TQMGText> TextList = new List<TQMGText>();
        static Game1 Game;

        override public void Update(Game1 game, GameTime gameTime, MouseState mouse, GamePadState gamepad, KeyboardState kb) {
            if (kb.IsKeyDown(Keys.Enter) || mouse.LeftButton == ButtonState.Pressed) Game.Exit();

        }

        override public void Draw(Game1 game, GameTime gameTime) {
            var y = 10;
            // throw new Exception("FORCE"); // must be rem in release
            TQMG.Color(255, 0, 0);
            foreach (TQMGText T in TextList) {
                T.Draw(10, y);
                TQMG.Color(255, 180, 0);
                y += 20;
            }
        }

        static public void Error(Game1 getgame,string ErrorMessage) {
            Game = getgame;
            var fnt = TQMG.GetFont("fonts/SulphurPoint-Regular.12.jfbf");
            TextList.Add(fnt.Text("Error!"));
            foreach (string l in ErrorMessage.Split('\n')){
                TextList.Add(fnt.Text(l));
            }
            TextList.Add(fnt.Text("Hit enter or click the mouse to leave this program!"));
            ProjectData.Log($"{(char)27}[31mERROR!!!{(char)27}[0m\n{ErrorMessage}");
            Game.SetStage(new Crash());
        }
    }
}
