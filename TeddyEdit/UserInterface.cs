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
// Version: 19.04.04
// EndLic




#define TeddyUICrash
#undef supportscript

#region Use this!
using System;
using System.Collections.Generic;
using System.Linq;
using TeddyBear;
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
            TQMGImage _icon = null;
            public TQMGImage Icon { get {
                    if (_icon == null) _icon = TQMG.GetImage($"Tool_{Name}.png");
                    if (_icon == null) throw new Exception($"UI.GetIcon(\"{Name}\"): {JCR6.JERROR}");
                    return _icon;
                }
            }
            public bool selected = false;
            public bool boxdraw = true;
            readonly public int x;
            readonly public string Name;
            readonly public ToolDraw Draw;
            readonly public ToolMouse Mouse;
            public ToolKind(int getx, string getname, ToolDraw getdraw, ToolMouse getmouse, bool aboxdraw=true) {
                selected = getx == 0;
                if (selected) CurrentTool = this;
                x = getx;
                Name = getname;
                Draw = getdraw;
                Mouse = getmouse;
                boxdraw = aboxdraw;
                // TODO: Load the icon
            }
        }
        #endregion

        const int EditStartY = 30;
        #region Declarations Pull Down Menus
        class PDM_Item {
            readonly public PDMEN Parent;
            readonly public string CaptString;
            readonly public TQMGText CaptText;
            readonly public string qkeyString;
            readonly public TQMGText qkeyText;
            readonly public int MenuEventCode;
            public bool Enabled = true;

            PDM_Item(PDMEN aParent, string Caption, int EventCode, string qkey) {
                PDM_Items[aParent].Add(this);
                Parent = aParent;
                CaptString = Caption;
                CaptText = font20.Text(Caption);
                qkeyString = qkey;
                qkeyText = font20.Text(qkey);
                MenuEventCode = EventCode;
            }

            public static PDM_Item New(PDMEN Parent, string Caption, int EventCode,string qkey="") => new PDM_Item(Parent, Caption, EventCode,qkey);
            
        }

        static SortedDictionary<PDMEN, string> PDM_Bar = new SortedDictionary<PDMEN, string>();
        static SortedDictionary<PDMEN, TQMGText> PDM_Caption = new SortedDictionary<PDMEN, TQMGText>();
        static SortedDictionary<PDMEN, List<PDM_Item>> PDM_Items;
        static Dictionary<PDMEN, int> PDM_Width = new Dictionary<PDMEN, int>();
        static Dictionary<PDMEN, int> PDM_Height = new Dictionary<PDMEN, int>();
        static private int TrueMenuEvent=0;
        static public int MenuEvent {
            get { if (DontMouse) return 0; var r = TrueMenuEvent; TrueMenuEvent = 0; return r; }
            private set { TrueMenuEvent = value; ProjectData.Log($"Menu event set to {value}"); }
        }
        static public bool DontMouse = false;

        #endregion

        static public TQMGFont font12 { get; private set; }
        static public TQMGFont font20 { get; private set; }
        static public TQMGFont font32 { get; private set; }
        static public TJCRDIR JCR { get; private set; }
        static public Game1 Game { get; private set; }
        static public TQMGImage back { get; private set; }
        static public TQMGImage Visible { get; private set; }
        static TQMGText ProjectAndFile;
        static public TQMGImage ArrowUp { get; private set; }
        static public TQMGImage ArrowDn { get; private set; }
        static public TQMGImage ArrowDown => ArrowDn;
        static int TexSpot { get => Main.CurTexSpot; set { Main.CurTexSpot = value; } }

        #region Layer data
        static public string[] LayerList { get; private set; }
        static public TQMGText[] LayerText { get; private set; }
        static bool[] LayerVisible;
        static string CurrentLayer;
        static string CurrentZone;
        static string czonename {
            get => ProjectData.Map.ZName(CurrentZone).Name[TexSpot];
            set => ProjectData.Map.ZName(CurrentZone).Name[TexSpot] = value;
        }
        #endregion

        static public int ScrWidth => ProjectData.Game.Window.ClientBounds.Width;
        static public int ScrHeight => ProjectData.Game.Window.ClientBounds.Height;


        static Dictionary<bool, TQMGImage> ToolButton = new Dictionary<bool, TQMGImage>();

        static public bool MenuOpen = false;
        static public PDMEN MenuPulled = (PDMEN)0;

        static List<ToolKind> Tools = new List<ToolKind>();
        static ToolKind CurrentTool;
        static int ToolX = 0;
        static TQMGText TxNULL;
        static TeddyMap Map => ProjectData.Map;
        static public string ErrorNotice = "";

        #region object selection vars
        static public string CurrentObject = "";
        static public int cobjx = 0;
        static public int cobjy = 0;
        static TeddyObject cobj = null;
        #endregion

        #region position
        static public int startX = -1;
        static public int startY = -1;
        static MouseState _mouse;
        static public int ScrollX { get => MapConfig.ScrollX; set => MapConfig.ScrollX = value; }
        static public int ScrollY { get => MapConfig.ScrollY; set => MapConfig.ScrollY = value; }
        static public int PosX => (int)Math.Floor((decimal)((float)(_mouse.X + ScrollX) / (float)Map.GridX));
        static public int PosY => (int)Math.Floor((decimal)((float)((_mouse.Y - EditStartY) + ScrollY) / (float)Map.GridY));
        #endregion

        static UI() {

            MKL.Version("TeddyBear - UserInterface.cs","19.04.04");
            MKL.Lic    ("TeddyBear - UserInterface.cs","GNU General Public License 3");

            #region load fonts
            font12 = TQMG.GetFont("fonts/SulphurPoint-Regular.12.jfbf");
            font20 = TQMG.GetFont("fonts/SulphurPoint-Regular.20.jfbf");
            font32 = TQMG.GetFont("fonts/SulphurPoint-Regular.32.jfbf");
            #endregion

            #region Define Pull-Down menus
            try {
                PDM_Items = new SortedDictionary<PDMEN, List<PDM_Item>>();
                PDM_Bar[PDMEN.File] = "File"; PDM_Items[PDMEN.File] = new List<PDM_Item>();
                PDM_Item.New(PDMEN.File, "Edit Meta Data Data", 1001, "^M");
                PDM_Item.New(PDMEN.File, "Save", 1002, "^S");
                PDM_Item.New(PDMEN.File, "Quit", 1003, "^Q");
                PDM_Bar[PDMEN.Textures] = "Textures"; PDM_Items[PDMEN.Textures] = new List<PDM_Item>();
                PDM_Item.New(PDMEN.Textures, "Load Texture", 2001, "^T");
                PDM_Item.New(PDMEN.Textures, "All Textures", 2002, "^R");
                PDM_Item.New(PDMEN.Textures, "Remove Texture", 2003, "^D");
                PDM_Item.New(PDMEN.Textures, "Texture Allowance", 2004, "^O");
                PDM_Bar[PDMEN.Objects] = "Objects"; PDM_Items[PDMEN.Objects] = new List<PDM_Item>();
                PDM_Item.New(PDMEN.Objects, "Select Object", 3001, "^B");
#if supportscript
            PDM_Bar[PDMEN.Script] = "Script"
#endif
            } catch (Exception E) {
                Crash.Error(ProjectData.Game, $"{E.Message}\n\n{E.StackTrace}");
            }
            foreach (PDMEN i in PDM_Bar.Keys) {
                PDM_Caption[i] = font20.Text(PDM_Bar[i]);
                var w = 0;
                var h = 0;
                foreach(PDM_Item item in PDM_Items[i]) {
                    var wd = item.CaptText.Width + 40 + item.qkeyText.Width;
                    if (wd > w) w = wd;
                    h += 25;
                }
                PDM_Width[i] = w;
                PDM_Height[i] = h;
            }
            #endregion


            TxNULL = font32.Text("<NULL>");
            back = TQMG.GetImage("metal.jpg");
            ProjectAndFile = font20.Text($"Project: {ProjectData.Project}; Map: {ProjectData.MapFile}");
            ArrowUp = TQMG.GetImage("Arrow_Up.png");
            ArrowDn = TQMG.GetImage("Arrow_Down.png");
            Visible = TQMG.GetImage("Visible.png");

            LayerList = ProjectData.ProjectConfig.List("Layers").ToArray(); // Hopefully this fastens things up (a bit).
            LayerText = new TQMGText[LayerList.Length];
            LayerVisible = new bool[LayerList.Length];
            for (int i = 0; i < LayerList.Length; i++) {
                LayerText[i] = font20.Text(LayerList[i]);
                LayerVisible[i] = true;
            }
            CurrentLayer = LayerList[0];

            #region Create the tools
            void NewTool(string name, ToolDraw draw, ToolMouse mouse,bool blockdraw=true) {
                Tools.Add(new ToolKind(Tools.Count * 65, name, draw, mouse,blockdraw));
            }
            NewTool("Layers", Tool_Layers, Tool_LayersUpdate);
            NewTool("Objects", Tool_Objects,Tool_ObjectsUpdate ,false);
            NewTool("Zones", Tool_Zones, Tool_ZonesUpdate);
            NewTool("Script", delegate { font20.DrawText("Script not yet implemented", ToolX, 200); }, delegate { }); // TODO: Script
            ToolButton[false] = TQMG.GetImage("toolbutton_0.png");
            ToolButton[true] = TQMG.GetImage("toolbutton_1.png");
            #endregion
        }

        static void Tool_Objects(MouseState mouse) {
            if (CurrentObject == "") {
                TQMG.Color(255, 0, 0);
                TxNULL.Draw(ToolX + 25, 150);
            } else {
                TQMG.Color(255, 180, 0);
                font20.DrawText(CurrentObject, ToolX + 5, 150);
            }
            TQMG.Color(255, 255, 255);
            if (cobjx >= 0 && cobjy >= 0 && Map.ObjectList(cobjx, cobjy).Count > 0) {
                font20.DrawText($"Object List ({cobjx},{cobjy})",ToolX+5,200);
                var oy = 250;
                foreach(TeddyObject o in Map.ObjectList(cobjx, cobjy)) {
                    TQMG.Color(180, 0, 255);
                    var f = "TeddyID";
                    if (ProjectData.ProjectConfig.C($"ListObject.{o.ObjType}") != "") f = ProjectData.ProjectConfig.C($"ListObject.{o.ObjType}");
                    if (mouse.X>ToolX && mouse.Y>oy && mouse.Y < oy + 20) {
                        TQMG.Color(0, 180, 255);
                        if (mouse.LeftButton == ButtonState.Pressed && !DontMouse) ObjectEditor.ComeToMe(cobjx,cobjy,o);
                    }
                    font20.DrawText($"{o.ObjType}:{o.Cl(f)}",ToolX+5,oy);
                    oy += 22;
                }
            }
        }

        static void Tool_ObjectsUpdate(MouseState mouse) {
            if ( mouse.X<ToolX && mouse.Y>EditStartY && (!DontMouse) ) {
                if (PosX<=Map.OWidth && PosY <= Map.OHeight) {
                    if (CurrentObject != "" && mouse.LeftButton == ButtonState.Pressed) {
                        var o = new TeddyObject();
                        o.ObjType = CurrentObject;
                        Map.ObjectList(PosX, PosY).Add(o);
                        ObjectEditor.ComeToMe(PosX, PosY, o);
                    } else if (mouse.RightButton==ButtonState.Pressed) {
                        cobjx = PosX;
                        cobjy = PosY;
                    }
                }
            }
        }

        static bool ArrowWasPressed = false;
        static MouseState Tool_LastMouse;
        static void Tool_Layers(MouseState mouse) {
            TQMG.Color(0, 180, 255);
            if (mouse.X > ToolX && mouse.X < ToolX + 32 && mouse.Y > 150 && mouse.Y < 182) TQMG.Color(180, 0, 255);
            ArrowUp.Draw(ToolX, 150);
            TQMG.Color(0, 180, 255);
            if (mouse.X > ToolX + 40 && mouse.X < ToolX + 72 && mouse.Y > 150 && mouse.Y < 182) TQMG.Color(180, 0, 255);
            ArrowDn.Draw(ToolX + 40, 150);
            TQMG.Color(255, 180, 0);
            font32.DrawText(TexSpot.ToString("X2"), ToolX + 80, 150);
            if (TexSpot == 0) {
                TQMG.Color(255, 0, 0);
                TxNULL.Draw(ToolX + 200, 150);
            } else {
                TQMG.Color(180, 255, 0);
                var texfil = Map.Texture[TexSpot];
                if (texfil == null) texfil = "";
                var texdirs = texfil.Split('/');
                if (texdirs.Length > 7) {
                    var y = (172 + 16) - (12 * 7);
                    for (int i = 0; i < texdirs.Length; i++) {
                        if (i < 3 || i < texdirs.Length - 4) {
                            font12.DrawText(texdirs[i], ToolX + 110, y);
                            y += 12;
                        } else if (i == 3) {
                            font12.DrawText("   <=====> ", ToolX + 110, y);
                            y += 12;
                        }
                    }
                } else {
                    var y = (172 + 16) - (12 * texdirs.Length);
                    for (int i = 0; i < texdirs.Length; i++) {
                        font12.DrawText(texdirs[i], ToolX + 110, y);
                        y += 12;
                    }
                }
            }
            for (int i = 0; i < LayerList.Length; i++) {
                if (!qstr.Prefixed(LayerList[i], "Zone_")) {
                    int PosY = 210 + (i * 21);
                    TQMG.Color(255, 255, 255);
                    if (LayerVisible[i]) { Visible.Draw(ToolX + 5, PosY); }
                    if (LayerList[i] == CurrentLayer) TQMG.Color(255, 180, 0);
                    LayerText[i].Draw(ToolX + 100, PosY);
                    if (mouse.Y >= PosY && mouse.Y <= PosY + 20 && mouse.LeftButton == ButtonState.Pressed && !MenuOpen) {
                        if (mouse.X > ToolX + 100)
                            CurrentLayer = LayerList[i];
                        else if (mouse.X > ToolX && Tool_LastMouse.LeftButton != ButtonState.Pressed && !MenuOpen)
                            LayerVisible[i] = !LayerVisible[i];
                    }
                }
            }
            Tool_LastMouse = mouse;
        }

        static void Tool_LayersUpdate(MouseState mouse) {
            if (mouse.Y > 150 && mouse.Y < 182 && !ArrowWasPressed && mouse.LeftButton == ButtonState.Pressed && !MenuOpen) {
                if (mouse.X > ToolX && mouse.X < ToolX + 32 && TexSpot > 0) TexSpot--;
                if (mouse.X > ToolX + 40 && mouse.X < ToolX + 72 && TexSpot < 255) TexSpot++;
            }
            ArrowWasPressed = mouse.LeftButton == ButtonState.Pressed;
        }

        static void Tool_Zones(MouseState mouse)
        {
            TQMG.Color(0, 180, 255);
            if (mouse.X > ToolX && mouse.X < ToolX + 32 && mouse.Y > 150 && mouse.Y < 182) TQMG.Color(180, 0, 255);
            ArrowUp.Draw(ToolX, 150);
            TQMG.Color(0, 180, 255);
            if (mouse.X > ToolX + 40 && mouse.X < ToolX + 72 && mouse.Y > 150 && mouse.Y < 182) TQMG.Color(180, 0, 255);
            ArrowDn.Draw(ToolX + 40, 150);
            TQMG.Color(255, 180, 0);
            font32.DrawText(TexSpot.ToString("X2"), ToolX + 80, 150);
            if (TexSpot == 0 || CurrentZone == null) {
                TQMG.Color(255, 0, 0);
                TxNULL.Draw(ToolX + 200, 150);
            } else if (CurrentZone != null) {
                TQMG.Color(180, 255, 0);
                font12.DrawText($"{czonename}|", ToolX + 120, 150);
            }
            var y = 200;
            foreach (string z in LayerList) {
                if (qstr.Prefixed(z, "Zone_")) {
                    if (CurrentZone == null) CurrentZone = z;
                    if (CurrentZone == z)
                        TQMG.Color(255, 180, 0);
                    else
                        TQMG.Color(255, 255, 255);
                    font20.DrawText(z, ToolX + 5, y);
                    if (mouse.LeftButton == ButtonState.Pressed && !MenuOpen && mouse.X > ToolX && mouse.Y > y && mouse.Y < y + 20)
                        CurrentZone = z;
                    y += 23;
                }
            }
        }

        static void Tool_ZonesUpdate(MouseState mouse)
        {
            if (mouse.Y > 150 && mouse.Y < 182 && !ArrowWasPressed && mouse.LeftButton == ButtonState.Pressed && !MenuOpen) {
                if (mouse.X > ToolX && mouse.X < ToolX + 32 && TexSpot > 0) TexSpot--;
                if (mouse.X > ToolX + 40 && mouse.X < ToolX + 72 && TexSpot < 255) TexSpot++;
            }
            ArrowWasPressed = mouse.LeftButton == ButtonState.Pressed;
            if (!(MenuOpen || TQMGKey.Held(Keys.LeftControl) || TQMGKey.Held(Keys.RightControl))) {
                var ch = TQMGKey.GetChar();
                if ((byte)ch >= 32 && font12.TextWidth(czonename) < (ScrWidth - (ToolX + 130))) czonename += ch;
                if (TQMGKey.Hit(Keys.Back) && czonename != "") czonename = qstr.Left(czonename, czonename.Length - 1);
            }
        }


        static public void DrawPDMenu(MouseState mouse) {
            int x = 10;
            TQMG.Color(255, 255, 255);
            TQMG.SimpleTile(back, 0, 0, ProjectData.Game.Window.ClientBounds.Width, EditStartY);
            foreach (PDMEN i in PDM_Caption.Keys) {
                if (MenuOpen && MenuPulled == i) {
                    TQMG.Color(20, 15, 0);
                    TQMG.SetAlpha(20);
                    TQMG.DrawRectangle(x - 5, 0,  20 + PDM_Caption[i].Width,EditStartY);
                    TQMG.SetAlpha(255);
                    TQMG.Color(255, 180, 0);
                }
                PDM_Caption[i].Draw(x, 5);
                if (MenuOpen && MenuPulled==i) {
                    TQMG.Color(255, 255, 255);
                    TQMG.SimpleTile(back,x, EditStartY + 5, PDM_Width[i], PDM_Height[i]);
                    var y = EditStartY + 6;
                    var w = PDM_Width[i];
                    foreach(PDM_Item item in PDM_Items[i]) {
                        TQMG.Color(255, 255, 255);
                        if (mouse.X>x && mouse.X<x+w && mouse.Y>y && mouse.Y < y + 21) {
                            TQMG.Color(20, 15, 0);
                            TQMG.SetAlpha(20);
                            TQMG.DrawRectangle(x, y, w, 22);
                            TQMG.SetAlpha(255);
                            TQMG.Color(255, 180, 0);
                            if (mouse.LeftButton == ButtonState.Pressed) {
                                MenuOpen = false;
                                MenuEvent = item.MenuEventCode;
                                DontMouse = true;
                            }
                        }
                        item.CaptText.Draw(x + 5, y);
                        item.qkeyText.Draw((x + w) - 5, y, TQMG_TextAlign.Right);
                        TQMG.Color(255, 255, 255);
                        y += 22;
                    }
                    if (mouse.LeftButton == ButtonState.Pressed && (mouse.X < x || mouse.X > x + w || mouse.Y > EditStartY + PDM_Height[i])) { MenuOpen = false; DontMouse = true; }
                } else if (mouse.LeftButton==ButtonState.Pressed && mouse.Y<EditStartY && mouse.X>x && mouse.X<x+20+PDM_Caption[i].Width) {
                    MenuOpen = true;
                    MenuPulled = i;
                }
                x += 20 + PDM_Caption[i].Width;
            }

        }

        static public void DrawToolBox(MouseState mouse) {
            var ScrMod = ScrWidth % back.Width;
            var ToolWidth = back.Width + ScrMod;
            TQMG.Color(255, 255, 255);
            ToolX = ScrWidth - ToolWidth;
            TQMG.UglyTile(back, ToolX, EditStartY, ToolWidth, ScrHeight);
            //font20.DrawText($"{ToolX}/{back.Width}x{back.Height}/{ToolWidth}/{ScrWidth}x{ScrHeight}/{ScrMod}",ToolX,100,TQMG_TextAlign.Right); // debug line!
            foreach (ToolKind tool in Tools) {
                tool.Icon.Draw(ToolX + tool.x, 50);
                ToolButton[tool.selected].Draw(ToolX + tool.x, 50);
                #region Dirty code straight from Hell, but I don't care!
                if (mouse.X > tool.x + ToolX && mouse.Y > 50 && mouse.Y < 114 && mouse.LeftButton == ButtonState.Pressed && !MenuOpen) {
                    foreach (ToolKind ModToolTab in Tools) {
                        ModToolTab.selected = ModToolTab == tool; // Yeah, this must have the same reference!!!
                        if (ModToolTab.selected) CurrentTool = ModToolTab;
                    }
                }
                #endregion
            }
            CurrentTool.Draw(mouse);
        }

        static void SetUpRecFill(int TexSpot, string Layer, int sx, int sy, int ex, int ey)
        {
            for (int ix = sx; ix <= ex; ix++) {
                for (int iy = sy; iy <= ey; iy++) {
                    if (ix <= ProjectData.MapWidth && ix >= 0 && iy >= 0 && iy <= ProjectData.MapHeight) {
                        Map.Layers[Layer].Put(ix, iy, (byte)TexSpot);
                    }
                }
            }
        }

        static public void DrawGrid(MouseState mouse) {
            bool bc;
            bool ac = true;
            for (int y = 0; y < ScrHeight; y += Map.GridY) {
                ac = !ac;
                bc = ac;
                for (int x = 0; x <= ToolX; x += Map.GridX) {
                    // The numbers in the for loop defs above will draw a bit more than needed, but this way I can be 100% sure no bugs occur!
                    bc = !bc;
                    MapConfig.GridCol(bc);
                    TQMG.DrawRectangle(x, y + EditStartY, Map.GridX, Map.GridY);
                }
            }
        }

        static Microsoft.Xna.Framework.Color[] ZoneCol = new Microsoft.Xna.Framework.Color[] {
            new Microsoft.Xna.Framework.Color(127,127,127), //  0
            new Microsoft.Xna.Framework.Color(127,127,255), //  1
            new Microsoft.Xna.Framework.Color(255,  0,  0), //  2
            new Microsoft.Xna.Framework.Color(  0,127,  0), //  3
            new Microsoft.Xna.Framework.Color(255,180,180), //  4
            new Microsoft.Xna.Framework.Color(180,120,  0), //  5
            new Microsoft.Xna.Framework.Color(  0,  0,255), //  6
            new Microsoft.Xna.Framework.Color(100, 60,255), //  7
            new Microsoft.Xna.Framework.Color(180,255,  0), //  8
            new Microsoft.Xna.Framework.Color(  0,255,255), //  9
            new Microsoft.Xna.Framework.Color(180,  0,255), // 10
            new Microsoft.Xna.Framework.Color(255,255,255), // 11
            new Microsoft.Xna.Framework.Color(255,255,  0), // 12
            new Microsoft.Xna.Framework.Color(255,155, 55), // 13
            new Microsoft.Xna.Framework.Color(  0,180,255), // 14
            new Microsoft.Xna.Framework.Color(255,180,  0)  // 15
        };
        static void DrawZone(string zone) {            
            var z = Map.Layers[zone];
            for (int y=0; y<=Map.SizeY; y++) {
                var dy = EditStartY - ScrollY; dy += (y * Map.GridY);
                for (int x = 0; x <= Map.SizeX; x++) {
                    var zn = z.Get(x, y);
                    if (zn != 0) {
                        var dx = 0 - ScrollX; dx += (x * Map.GridX);
                        var cl = zn % 16;
                        TQMG.Color(ZoneCol[cl]);
                        TQMG.SetAlpha((byte)30);
                        TQMG.DrawRectangle(dx, dy, Map.GridX, Map.GridY);
                        TQMG.SetAlpha((byte)255);
                        TQMG.Color(0, 0, 0);
                        for (int puk = -1; puk <= 1; puk += 2) {
                            font20.DrawText(zn.ToString("X2"), dx + 2 + puk, dy + 2 + puk);
                            font20.DrawText(zn.ToString("X2"), dx + 2 + puk, dy + 2 - puk);
                        }
                        TQMG.Color(ZoneCol[cl]);
                        font20.DrawText(zn.ToString("X2"), dx + 2, dy + 2);
                    }
                }
            }
        }

        static public void DrawObjects(MouseState mouse) {
            TQMG.Color(255, 255, 255);
            for (int y = 0;  y < Map.OHeight;y++)
                for (int x=0; x < Map.OWidth; x++) {
                    var ol = Map.ObjectList(x, y);
                    if (ol.Count > 0) {
                        var dx = (0 - ScrollX) + (x * Map.GridX);
                        var dy = (0 - ScrollY) + (y * Map.GridY);
                        if (dx>-Map.GridX && dy>-Map.GridY && dx<ScrWidth && dy < ScrHeight) {
                            TQMG.DrawLineRect(dx, dy + EditStartY, Map.GridX, Map.GridY);
                            font20.DrawText($"O:{ol.Count}",dx+2,EditStartY+dy+2);
                        }
                    }
                }
        }

        static public void DrawMap(MouseState mouse) {
            const int mapy = EditStartY;
            if (MapConfig.ShowGrid) DrawGrid(mouse);
            TQMG.Color(255, 255, 255);
            foreach(string lay in LayerList) {
                TeddyDraw.DrawLayer(ProjectData.Map,lay,0,mapy,ScrollX,ScrollY);
            }
            DrawObjects(mouse);
            if (CurrentTool.Name == "Zones" && CurrentZone != null) DrawZone(CurrentZone);
            if (mouse.Y < mapy || mouse.X > ToolX) startX = -1;
            else if ((!MenuOpen) && TrueMenuEvent==0 && mouse.LeftButton==ButtonState.Pressed && startX<0 && CurrentTool.boxdraw) {
                /* BAAAAAD!
                startX = ScrollX + mouse.X; startY = ScrollY + mouse.Y;
                */
                startX = PosX;
                startY = PosY;
            }
            if (startX >= 0 && startY >= 0 ) {
                ErrorNotice = "";
                var sx = 0;
                var sy = 0;
                var ex = 0;
                var ey = 0;
                var gx = ProjectData.Map.GridX;
                var gy = ProjectData.Map.GridY;
                /* I am certainly not happy about this!
                var mx = ScrollX;
                var my = ScrollY;
                my -= mapy;
                mx += (int)Math.Floor((decimal)(mouse.X / ProjectData.Map.GridX)) * ProjectData.Map.GridX; // I guess this doesn't make sense to you? :P
                my += (int)Math.Floor((decimal)(mouse.Y / ProjectData.Map.GridY)) * ProjectData.Map.GridY;
                if (mx == startX) { sx = mx; ex = mx; } else if (mx < startX) { sx = mx; ex = startX; } else { sx = startX; ex = mx; }
                if (my == startY) { sy = my; ey = my; } else if (my < startY) { sy = my; ey = startX; } else { sy = startX; ey = my; }
                TQMG.SetAlpha(25);
                TQMG.Color(255, 0, 0);
                TQMG.DrawRectangle(sx, sy, ex - sx, ey - sy);
                TQMG.SetAlpha(255);
                */
                if (PosX == startX) { sx = PosX; ex = PosX; } else if (PosX < startX) { sx = PosX; ex = startX; } else { sx = startX; ex = PosX; }
                if (PosY == startY) { sy = PosY; ey = PosY; } else if (PosY < startY) { sy = PosY; ey = startY; } else { sy = startY; ey = PosY; }
                TQMG.SetAlpha(25);
                TQMG.Color(255, 0, 0);
                TQMG.DrawRectangle((sx*gx)-ScrollX, ((sy*gy)-ScrollY)+EditStartY, (((ex+1) - sx)*gx)-ScrollX, (((ey+1) - sy)*gy)-ScrollY);
                TQMG.SetAlpha(255);
                if (mouse.LeftButton != ButtonState.Pressed) {
                    if (CurrentTool.Name == "Zones") {
                        SetUpRecFill(TexSpot, CurrentZone, sx, sy, ex, ey);
                    }
                    if (CurrentTool.Name == "Layers") {
                        if (!MapConfig.AllowSet(Map.Texture[TexSpot]) && TexSpot != 0) {
                            ErrorNotice = "Allowance not yet set! Please hit ctrl-o in order to configure that!";
                        } else {
                            foreach (string l in ProjectData.Map.Layers.Keys) if (MapConfig.Allow(Map.Texture[TexSpot], l) == TexAllow.Auto) CurrentLayer = l;
                            switch (MapConfig.Allow(Map.Texture[TexSpot], CurrentLayer)) {
                                case TexAllow.NotSet: break; // Just ignore the whole thing!
                                case TexAllow.Allow:
                                case TexAllow.Auto:
                                    SetUpRecFill(TexSpot, CurrentLayer, sx, sy, ex, ey);
                                    break;
                                case TexAllow.Annoy:
                                    ErrorNotice = "The 'Annoy' setting has not yet been supported!";
                                    break;
                                case TexAllow.Deny:
                                    ErrorNotice = "You denied that texture yourself from this spot!";
                                    break;
                                default:
                                    ErrorNotice = "INTERNAL ERROR! Unknown allowance setting! Version conflicts between source files?";
                                    break;
                            }
                        }
                    }
                    startX = -1;
                    startY = -1;
                }
            }
        }

        static public void DrawStatusBar(MouseState ms) {
            TQMG.UglyTile(back, 0, ProjectData.Game.Window.ClientBounds.Height - 25, ProjectData.Game.Window.ClientBounds.Width, 25);
            if (ErrorNotice!="") {
                TQMG.Color(255, 0, 0);
                font20.DrawText("ERROR!",20, ProjectData.Game.Window.ClientBounds.Height - 22);
                TQMG.Color(255, 255, 0);
                font20.DrawText(ErrorNotice, 100, ProjectData.Game.Window.ClientBounds.Height - 22);
                return;
            }
            if (ms.Y>10 && (!MenuOpen)) {
                ProjectAndFile.Draw(5, ProjectData.Game.Window.ClientBounds.Height - 22);
                font20.DrawText($"Screen: {ProjectData.Game.Window.ClientBounds.Width}x{ProjectData.Game.Window.ClientBounds.Height}; ScreenPos: ({ScrollX},{ScrollY}); Mouse: ({ms.X},{ms.Y}); Tile({PosX},{PosY})", ProjectData.Game.Window.ClientBounds.Width - 5, ProjectData.Game.Window.ClientBounds.Height - 22, TQMG_TextAlign.Right);
            }
        }

        static public void DrawScreen(MouseState ams) {
            try {
                var ms = ams;
                if (DontMouse) ms = new MouseState(ams.X, ams.Y, 0, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);
                DrawMap(ms);
                DrawToolBox(ms); // Toolbox MUST come BEFORE pull down menu and status bar (conflict prevention)
                DrawPDMenu(ms);
                DrawStatusBar(ms);
            } catch (Exception Ex) {
                ProjectData.Log($"Exception caught: {Ex.Message}\n{Ex.StackTrace}");
#if TeddyUICrash
                Crash.Error(ProjectData.Game, $"DUI-Flow Error:\n{Ex.Message}\n\nTraceback:\n{Ex.StackTrace}\n\nTarget:\n{Ex.TargetSite}\n\nSource:\n{Ex.Source}\n\nIf you see this message you very likely fell victim to a bug!\n\nPlease go to my issue tracker and report it, if it hasn't been done before.\nhttps://github.com/TrickyGameTools/TeddyBear/issues\n\nThank you!");
#endif
            }
        }

        static public void UpdateScreen(MouseState ms) {
            try {
                if (ms.LeftButton == ButtonState.Released) DontMouse = false;
                if (DontMouse) _mouse = new MouseState(ms.X, ms.Y, 0, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);
                _mouse = ms;
                CurrentTool.Mouse(_mouse);
            } catch (Exception Ex) {
                ProjectData.Log($"Exception caught: {Ex.Message}\n{Ex.StackTrace}");
#if TeddyUICrash
                Crash.Error(ProjectData.Game, $"UUI-Flow Error:\n{Ex.Message}\n\nTraceback:\n{Ex.StackTrace}\n\nTarget:\n{Ex.TargetSite}\n\nSource:\n{Ex.Source}\n\nIf you see this message you very likely fell victim to a bug!\n\nPlease go to my issue tracker and report it, if it hasn't been done before.\nhttps://github.com/TrickyGameTools/TeddyBear/issues\n\nThank you!");
#endif
            }
        }

    }
}
