using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TrickyUnits;
using UseJCR6;

namespace TeddyEdit.Stages
{
    class SetAllowance : BasisStage {

        static SetAllowance me = null;
        static int Tex = 0;
        static TQMGText Head;
        static SortedDictionary<TexAllow, TQMGText> AllowText = new SortedDictionary<TexAllow, TQMGText>();
        static Dictionary<string, TQMGText> LayerText = new Dictionary<string, TQMGText>();

        public override void Draw(Game1 game, GameTime gameTime) {
            TQMG.Color(127, 127, 127);
            TQMG.Color(255, 180, 0);
            Head.Draw(UI.ScrWidth / 2, 50, TQMG_TextAlign.Center);

            // Todo: Tile the requested texture
            for (int i = 0; i < UI.LayerList.Length; i++) {
                var cy = 100 + (i * 21);
                TQMG.Color(255, 255, 255);
                UI.LayerText[i].Draw(5, cy);
                if (MapConfig.Allow(ProjectData.Map.Texture[Tex], UI.LayerList[i])==TexAllow.NotSet)
                    MapConfig.Allow(ProjectData.Map.Texture[Tex], UI.LayerList[i], TexAllow.Allow);
                foreach(TexAllow a in AllowText.Keys) {
                    var cx = ((int)a) * 150;
                    if (MapConfig.Allow(ProjectData.Map.Texture[Tex], UI.LayerList[i]) == a)
                        TQMG.Color(0, 180, 255);
                    else
                        TQMG.Color(180, 0, 255);
                    AllowText[a].Draw(cx, cy, TQMG_TextAlign.Center);
                    // TODO: Click this
                }
            }

        }

        public override void Update(Game1 game, GameTime gameTime, MouseState mouse, GamePadState gamepad, KeyboardState kb) {
            
        }

        static void LAL(TexAllow a,string txt) {
            if (!AllowText.ContainsKey(a)) AllowText[a] = UI.font20.Text(txt);
        }

        public static void ComeToMe(int aTex) {
            if (me == null) me = new SetAllowance();
            Tex = aTex;
            Head = UI.font32.Text($"Please set up the allowance state for {Tex.ToString("X2")}");
            LAL(TexAllow.Allow, "Allow");
            LAL(TexAllow.Deny, "Deny");
            LAL(TexAllow.Annoy, "Annoy");
            LAL(TexAllow.Auto, "Auto");            
        }

    }
}
