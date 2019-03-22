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

Ã¯Â»Â¿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TeddyEdit.Stages
{
    class TextureLoad : BasisStage {
        static TextureLoad me = new TextureLoad();
        List<string> FormatAllowed = new List<string>();

        TextureLoad() {
            FormatAllowed.Add("PNG");
            foreach(string f in ProjectData.ProjectConfig.List("ImageExtAllowed")) {
                FormatAllowed.Add(f.ToUpper());
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
