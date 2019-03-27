// Lic:
// TeddyBear C#
// User Interface
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
// Version: 19.03.27
// EndLic









#undef supportscript

#region Use this!
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UseJCR6;
using TrickyUnits;
using Microsoft.Xna.Framework.Input;
using TeddyEdit.Stages;
#endregion

namespace TeddyEdit {

    enum PDMEN { File, Textures, Objects,
#if supportscript
                   Script
#endif
    }


    class PDMenu {
        int idout;
        string caption;
        int parent;

        public PDMenu(int a_parent, string a_caption, int a_idout) {
            idout = a_idout;
            caption = a_caption;
            parent = a_parent;            
        }
    }

    
    static class UI {

        #region Classes and void pointers (delegates) for the tool box
        delegate void ToolMouse(MouseState ms);
        delegate void ToolDraw(MouseState ms);

        class ToolKind {
            TQMGImage _icon=null;
            public TQMGImage Icon { get {
                    if (_icon==null) _icon = TQMG.GetImage($"Tool_{Name}.png");
                    if (_icon == null) throw new Exception($"UI.GetIcon(\"{Name}\"): {JCR6.JERROR}");
                    return _icon;
                }
            }
            public bool selected = false;
            readonly public int x;
            string Name;
            readonly public ToolDraw Draw;
            readonly public ToolMouse Mouse;
            public ToolKind(int getx, string getname, ToolDraw getdraw, ToolMouse getmouse) {
                selected = getx == 0;
                if (selected) CurrentTool = this;
                x = getx;
                Name = getname;
                Draw = getdraw;
                Mouse = getmouse;                
                // TODO: Load the icon
            }
        }
        #endregion

        static SortedDictionary<PDMEN, string> PDM_Bar = new SortedDictionary<PDMEN, string>();
        static SortedDictionary<PDMEN, TQMGText> PDM_Caption = new SortedDictionary<PDMEN, TQMGText>();
        static public TQMGFont font20 { get; private set; }
        static public TQMGFont font32 { get; private set; }
        static public TJCRDIR JCR { get; private set; }
        static public Game1 Game { get; private set; }
        static public TQMGImage back { get; private set; }
        static TQMGText ProjectAndFile;
        static public TQMGImage ArrowUp { get; private set; }
        static public TQMGImage ArrowDn { get; private set; }
        static public TQMGImage ArrowDown => ArrowDn;
        static int TexSpot { get => Main.CurTexSpot; set { Main.CurTexSpot = value; } }

        static public int ScrWidth => ProjectData.Game.Window.ClientBounds.Width;
        static public int ScrHeight => ProjectData.Game.Window.ClientBounds.Height;

        static Dictionary<bool, TQMGImage> ToolButton = new Dictionary<bool, TQMGImage>();

        static bool MenuOpen = false;

        static List<ToolKind> Tools = new List<ToolKind>();
        static ToolKind CurrentTool;
        static int ToolX = 0;
        static TQMGText TxNULL;

        static UI() {
            
            MKL.Version("TeddyBear - UserInterface.cs","19.03.27");
            MKL.Lic    ("TeddyBear - UserInterface.cs","GNU General Public License 3");
            PDM_Bar[PDMEN.File] = "File";
            PDM_Bar[PDMEN.Textures] = "Textures";
            PDM_Bar[PDMEN.Objects] = "Objects";
#if supportscript
            PDM_Bar[PDMEN.Script] = "Script"
#endif
            font20 = TQMG.GetFont("fonts/SulphurPoint-Regular.20.jfbf");
            font32 = TQMG.GetFont("fonts/SulphurPoint-Regular.32.jfbf");
            TxNULL = font32.Text("<NULL>");
            foreach (PDMEN i in PDM_Bar.Keys) PDM_Caption[i] = font20.Text(PDM_Bar[i]);
            back = TQMG.GetImage("metal.jpg");
            ProjectAndFile = font20.Text($"Project: {ProjectData.Project}; Map: {ProjectData.MapFile}");
            ArrowUp = TQMG.GetImage("Arrow_Up.png");
            ArrowDn = TQMG.GetImage("Arrow_Down.png");

            #region Create the tools
            void NewTool(string name,ToolDraw draw, ToolMouse mouse) {
                Tools.Add( new ToolKind(Tools.Count*65,name,draw,mouse) );                
            }
            NewTool("Layers", Tool_Layers, Tool_LayersUpdate);
            NewTool("Objects", delegate { font20.DrawText("Objects not yet implemented", ToolX, 200); }, delegate { });
            NewTool("Zones", delegate { font20.DrawText("Zones not yet implemented", ToolX, 200); }, delegate { });
            NewTool("Script", delegate { font20.DrawText("Script not yet implemented", ToolX, 200); }, delegate { }); // TODO: Script
            ToolButton[false] = TQMG.GetImage("toolbutton_0.png");
            ToolButton[true] = TQMG.GetImage("toolbutton_1.png");
            #endregion
        }

        static bool ArrowWasPressed = false;
        static void Tool_Layers(MouseState mouse) {
            TQMG.Color(0, 180, 255);
            if (mouse.X > ToolX && mouse.X < ToolX + 32 && mouse.Y > 150 && mouse.Y < 182) TQMG.Color(180, 0, 255);
            ArrowUp.Draw(ToolX, 150);
            TQMG.Color(0, 180, 255);
            if (mouse.X > ToolX+40 && mouse.X < ToolX + 72 && mouse.Y > 150 && mouse.Y < 182) TQMG.Color(180, 0, 255);
            ArrowDn.Draw(ToolX + 40, 150);
            TQMG.Color(255, 180, 0);
            font32.DrawText(TexSpot.ToString("X2"),ToolX + 80, 150);
            if (TexSpot == 0) {
                TQMG.Color(255, 0, 0);
                TxNULL.Draw(ToolX + 200, 150);
            } else {
                TQMG.Color(180, 255, 0);

            }
        }

        static void Tool_LayersUpdate(MouseState mouse)  {
            if (mouse.Y > 150 && mouse.Y < 182 && !ArrowWasPressed && mouse.LeftButton == ButtonState.Pressed) {
                if (mouse.X > ToolX && mouse.X < ToolX + 32 && TexSpot > 0) TexSpot--;
                if (mouse.X > ToolX+40 && mouse.X < ToolX + 72 && TexSpot < 255) TexSpot++;
            }
            ArrowWasPressed = mouse.LeftButton == ButtonState.Pressed;
        }

        static public void DrawPDMenu()  {
            int x = 10;
            TQMG.Color(255, 255, 255);
            TQMG.UglyTile(back, 0, 0, ProjectData.Game.Window.ClientBounds.Width, 15);
            foreach(PDMEN i in PDM_Caption.Keys) {
                PDM_Caption[i].Draw(x, 5);
                x += 20 + PDM_Caption[i].Width;
            }

        }

        static public void DrawToolBox(MouseState mouse) {
            var ScrMod = ScrWidth % back.Width;
            var ToolWidth = back.Width + ScrMod;
            TQMG.Color(255, 255, 255);
            ToolX = ScrWidth - ToolWidth;
            TQMG.UglyTile(back,ToolX, back.Height, ToolWidth, ScrHeight);
            //font20.DrawText($"{ToolX}/{back.Width}x{back.Height}/{ToolWidth}/{ScrWidth}x{ScrHeight}/{ScrMod}",ToolX,100,TQMG_TextAlign.Right); // debug line!
            foreach(ToolKind tool in Tools) {
                tool.Icon.Draw(ToolX + tool.x, 50);
                ToolButton[tool.selected].Draw(ToolX + tool.x, 50);
                #region Dirty code straight from Hell, but I don't care!
                if (mouse.X>tool.x+ToolX && mouse.Y>50 && mouse.Y < 114 && mouse.LeftButton==ButtonState.Pressed) {
                    foreach(ToolKind ModToolTab in Tools) {
                        ModToolTab.selected = ModToolTab == tool; // Yeah, this must have the same reference!!!
                        if (ModToolTab.selected) CurrentTool = ModToolTab;
                    }
                }
                #endregion
            }
            CurrentTool.Draw(mouse);
        }

        static public void DrawStatusBar(MouseState ms) {
            TQMG.UglyTile(back, 0, ProjectData.Game.Window.ClientBounds.Height - 25, ProjectData.Game.Window.ClientBounds.Width, 25);
            if (ms.Y>10 && (!MenuOpen)) {
                ProjectAndFile.Draw(5, ProjectData.Game.Window.ClientBounds.Height - 22);
                font20.DrawText($"Screen: {ProjectData.Game.Window.ClientBounds.Width}x{ProjectData.Game.Window.ClientBounds.Height}; Mouse: ({ms.X},{ms.Y})", ProjectData.Game.Window.ClientBounds.Width - 5, ProjectData.Game.Window.ClientBounds.Height - 22, TQMG_TextAlign.Right);
            }
        }

        static public void DrawScreen(MouseState ms) {
            //DrawMap();
            DrawToolBox(ms); // Toolbos MUST come BEFORE pull down menu and status bar (conflict prevention)
            DrawPDMenu();
            DrawStatusBar(ms);
        }

        static public void UpdateScreen(MouseState ms) {
            CurrentTool.Mouse(ms);
        }

    }
}
