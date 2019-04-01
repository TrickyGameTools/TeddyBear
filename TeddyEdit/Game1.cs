// Lic:
// TeddyBear C#
// Editor Core features
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
// Version: 19.03.31
// EndLic


#define TeddyCrashout






#region Using statements... in case you cared...
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TrickyUnits;
using TeddyEdit.Stages;
using System.Collections.Generic;
#endregion

namespace TeddyEdit
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        int MX = 0;
        int MY = 0;
        Texture2D MousePointer;
        SpriteBatch SB;
        BasisStage Current = null;
        Dictionary<string, BasisStage> Stages = new Dictionary<string, BasisStage>();


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            ProjectData.InitJCRDrivers();
            ProjectData.SetGame(this);

            #region Screen Size
            //graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            //graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            graphics.HardwareModeSwitch = false;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            #endregion

            #region TeddyBear Draw MonoGame Driver
            TeddyBear.TeddyDraw_MonoGame.Init();
            #endregion

            // Sprite Batch
            SB = new SpriteBatch(GraphicsDevice);

            // Mouse Pointer
            MousePointer = ProjectData.GetTex(GraphicsDevice,"MousePointer.png");

            // TQMG
            TQMG.Init(graphics, GraphicsDevice, SB, ProjectData.JCR);
            TQMG.RegLog( ProjectData.Log);

            // Unknown
            TeddyBear.TeddyDraw_MonoGame.SetUnknown(TQMG.GetImage("Unknown.png"));
            TeddyBear.TeddyDraw_MonoGame.Log = ProjectData.Log;
                

            // Do we have a project and a map?
#if DEBUG
            if (false) { } // just some crap as things are different while debugging.
#else
            if (ProjectData.args.Length < 3)
                Crash.Error(this,"No arguments given!\nUsage: TeddyEdit <project> <map>\n \n If you are not sure how to use this tool, use the launcher in stead!");
#endif
            else {
#if DEBUG
                ProjectData.Project = "Test";
#else
                ProjectData.Project = ProjectData.args[1];
#endif
                if (!ProjectData.AllWell) { Crash.Error(this, $"Project loading failed! {ProjectData.Project}"); } else {
#if DEBUG
                    ProjectData.MapFile = $"{Dirry.AD(ProjectData.ProjectConfig.C("LevelDir"))}/Test Map";
#else
                    ProjectData.MapFile =  $"{Dirry.AD(ProjectData.ProjectConfig.C("LevelDir"))}{ProjectData.args[2]}";
#endif
                }
            }


            // Teddy Save Log
            TeddyBear.TeddySave.SetLog(ProjectData.Log);

            // Final
            base.Initialize();

            if (ProjectData.AllWell) SetStage(Main.Me);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            var gpstate = GamePad.GetState(PlayerIndex.One);
            var kbstate = Keyboard.GetState();
            // This line will for now remain, but may have to leave once the editor is really getting to move 
            if (kbstate.IsKeyDown(Keys.Q) && (kbstate.IsKeyDown(Keys.LeftControl) || kbstate.IsKeyDown(Keys.RightControl))) {
                Main.Me.SaveMap();
                Exit();
            }

            // TODO: Add your update logic here
            var mstate = Mouse.GetState();
            MX = mstate.X;
            MY = mstate.Y;
#if TeddyCrashout
            try {
#endif
                if (Current != null) Current.Update(this,gameTime, mstate, gpstate, kbstate);
#if TeddyCrashout
            } catch (System.Exception ex) {
                Crash.Error(this, $"U-Flow Error:\n{ex.Message}\n\nTraceback:\n{ex.StackTrace}\n\nTarget:\n{ex.TargetSite}\n\nSource:\n{ex.Source}\n\nIf you see this message you very likely fell victim to a bug!\n\nPlease go to my issue tracker and report it, if it hasn't been done before.\nhttps://github.com/TrickyGameTools/TeddyBear/issues\n\nThank you!");
            }
#endif
            base.Update(gameTime);
        }

        Vector2 DrawTexVec = new Vector2();
        void DrawTex(Texture2D tex,int x,int y)
        {
            DrawTexVec.X = x;
            DrawTexVec.Y = y;
            SB.Draw(tex, DrawTexVec, Color.White);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            SB.Begin();
#if TeddyCrashout
            try {
#endif
                if (Current != null) Current.Draw(this, gameTime);
#if TeddyCrashout
            } catch (System.Exception ex) {
                Crash.Error(this,$"D-Flow Error:\n{ex.Message}\n\nTraceback:\n{ex.StackTrace}\n\nTarget:\n{ex.TargetSite}\n\nSource:\n{ex.Source}\n\nIf you see this message you very likely fell victim to a bug!\n\nPlease go to my issue tracker and report it, if it hasn't been done before.\nhttps://github.com/TrickyGameTools/TeddyBear/issues\n\nThank you!");
            }
#endif
            DrawTex(MousePointer, MX, MY);            
            SB.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        public void SetStage(string stagename) { SetStage(Stages[stagename]); }
        public void SetStage(object stage) { Current = (BasisStage)stage; }

    }
}
