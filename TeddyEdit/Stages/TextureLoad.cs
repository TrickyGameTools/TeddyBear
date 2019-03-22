using System;
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
