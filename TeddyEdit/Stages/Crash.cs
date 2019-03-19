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
            foreach (TQMGText T in TextList) {
                T.Draw(10, y);
            }
        }

        static public void Error(Game1 getgame,string ErrorMessage) {
            Game = getgame;
            var fnt = TQMG.GetFont("fonts/SulphurPoint-Regular.12.jfbf");
            TextList.Add(fnt.Text("Error!"));
            foreach (string l in ErrorMessage.Split('\n'){
                TextList.Add(fnt.Text(l));
            }
            TextList.Add(fnt.Text("Hit enter or click the mouse to leave this program!"));
            Game.SetStage(new Crash());
        }
    }
}
