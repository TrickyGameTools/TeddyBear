using System;
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
