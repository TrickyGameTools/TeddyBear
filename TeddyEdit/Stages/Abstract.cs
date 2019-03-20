// Lic:
// TeddyBear C#
// Abstract Stage Class
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



ÃÆÃÆÃâ ÃâÃÆÃâÃâ ÃâÃÆÃÆÃÂ¢Ãâ¬ÃÅ¡ÃÆÃâÃâÃÂ¯ÃÆÃÆÃâ ÃâÃÆÃâÃÂ¢Ãâ¬ÃÅ¡ÃÆÃÆÃÂ¢Ãâ¬ÃÅ¡ÃÆÃâÃâÃÂ»ÃÆÃÆÃâ ÃâÃÆÃâÃÂ¢Ãâ¬ÃÅ¡ÃÆÃÆÃÂ¢Ãâ¬ÃÅ¡ÃÆÃâÃâÃÂ¿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace TeddyEdit.Stages
{
    abstract class BasisStage
    {
        abstract public void Update(Game1 game,GameTime gameTime,MouseState mouse, GamePadState gamepad, KeyboardState kb);
        abstract public void Draw(Game1 game, GameTime gameTime);
    }
}
