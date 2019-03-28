// Lic:
// TeddyBear C#
// Main Stage
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
// Version: 19.03.28
// EndLic






using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeddyEdit;
using TeddyBear;
using TrickyUnits;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TeddyEdit.Stages {
    class Main : BasisStage {
        MouseState MS;
        static public int CurTexSpot = 1;

        void SaveMap() {
            UI.ErrorNotice = "";
            ProjectData.Log($"Compresion = {ProjectData.MapCompression}");
            var r = TeddySave.Save(ProjectData.Map, ProjectData.MapFile, "", ProjectData.MapCompression,ProjectData.MapCompression);
            if (r != "" && r != "Ok") UI.ErrorNotice = r;

        }

        public override void Update(Game1 game, GameTime gameTime, MouseState mouse, GamePadState gamepad, KeyboardState kb) {
            MS = mouse;            
            if (kb.IsKeyDown(Keys.LeftControl)) {
                if (kb.IsKeyDown(Keys.T) && CurTexSpot > 0)
                    TextureLoad.ComeToMe(CurTexSpot);
                if (kb.IsKeyDown(Keys.S)) SaveMap();
                if (kb.IsKeyDown(Keys.Left) && UI.ScrollX > 0) { UI.ScrollX -= ProjectData.Map.GridX / 2; if (UI.ScrollX < 0) UI.ScrollX = 0; }
                if (kb.IsKeyDown(Keys.Up) && UI.ScrollY > 0) { UI.ScrollY -= ProjectData.Map.GridY / 2; if (UI.ScrollY < 0) UI.ScrollY = 0; }
                if (kb.IsKeyDown(Keys.Down)) UI.ScrollY += ProjectData.Map.GridY / 2;
                if (kb.IsKeyDown(Keys.Right)) UI.ScrollX += ProjectData.Map.GridX / 2;
            }
            UI.UpdateScreen(mouse); // for buttons on the interface
        }

        public override void Draw(Game1 game, GameTime gameTime) {
            UI.DrawScreen(MS);
        }

        readonly public static Main Me = new Main();
        public static void ComeToMe() => ProjectData.Game.SetStage(Me);
    }
}
