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

namespace TeddyEdit.Stages
{
    class Main : BasisStage {
        public override void Update(Game1 game, GameTime gameTime, MouseState mouse, GamePadState gamepad, KeyboardState kb) {
            
        }

        public override void Draw(Game1 game, GameTime gameTime) {
            UI.DrawScreen();
        }

        readonly public static Main Me = new Main();
    }
}
