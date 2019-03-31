// Lic:
// TeddyClass/TeddyDraw_MonoGame.cs
// TeddyBear C#
// version: 19.03.31
// Copyright (C)  Jeroen P. Broks
// This software is provided 'as-is', without any express or implied
// warranty.  In no event will the authors be held liable for any damages
// arising from the use of this software.
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it
// freely, subject to the following restrictions:
// 1. The origin of this software must not be misrepresented; you must not
// claim that you wrote the original software. If you use this software
// in a product, an acknowledgment in the product documentation would be
// appreciated but is not required.
// 2. Altered source versions must be plainly marked as such, and must not be
// misrepresented as being the original software.
// 3. This notice may not be removed or altered from any source distribution.
// EndLic



using TrickyUnits;

// MonoGame class
namespace TeddyBear {



    static class TeddyDraw_MonoGame {



        static TQMGImage Unknown;

        static TQMGImage[] Texture = new TQMGImage[256];

        static public string Error { get; private set; }



        public static void SetUnknown(TQMGImage ui) => Unknown = ui;











        public static void Init()

        {

            Error = "";

            MKL.Version("TeddyBear - TeddyDraw_MonoGame.cs","19.03.31");

            MKL.Lic    ("TeddyBear - TeddyDraw_MonoGame.cs","ZLib License");

            // This item will allow MonoGame to draw tiles in the map

            TeddyDraw.DrawTileItem = delegate (TeddyMap map, string layer, int screenstart_x, int screenstart_y, int scroll_x, int scroll_y, int posx, int posy)

            {

                //TeddyMap map, string layer, int screenstart_x, int screenstart_y, int scroll_x, int scroll_y, int posx, int posy

                Error = "";

                if (!map.Layers.ContainsKey(layer)) { Error = $"Layer '{layer}' does not exist in this map!"; return; }

                if (qstr.Prefixed(layer, "Zone_")) { TeddyDraw.DrawZoneItem(map, layer, screenstart_x, screenstart_y, scroll_x, scroll_y, posx, posy); return; }

                var b = map.Layers[layer].Get(posx, posy);

                if (b == 0) return; // 0 stands for nothing and should therefore always be ignored!

                if (map.Texture[b] == null || map.Texture[b] == "") Texture[b] = Unknown;

                if (Texture[b] == null) {

                    try {

                        var bt = map.OpenTexture(b);

                        if (bt == null) { Texture[b] = Unknown; return; }

                        Texture[b] = TQMG.GetImage(bt);

                        if (Texture[b] == null) Texture[b] = Unknown;

                    } catch (System.Exception er) {

                        System.Diagnostics.Debug.WriteLine($"An error happened when loading a texture: {er.Message}");

                        Texture[b] = Unknown;

                    }

                }

                if (Texture[b] != null) {

                    Texture[b].Draw((screenstart_x + (posx * map.GridX)) - scroll_x, (screenstart_y + (posy * map.GridY)) - scroll_y);

                }

            };



            // This item will allow MonoGame to draw the zone layer. 
            // Basically only required for editors, as zones should be invisible in games.
            TeddyDraw.DrawZoneItem = delegate

            {
                Error = "";
            };



            TeddyDraw.TexReset = delegate (byte b)

            {
                if (b == 0) {
                    for (byte i = 255; i > 0; i--) TeddyDraw.TexReset(i);
                    return;
                }
                Texture[b] = null;
            };
        }



    }
}
