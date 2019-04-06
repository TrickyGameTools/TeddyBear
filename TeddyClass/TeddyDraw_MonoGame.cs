// Lic:
// TeddyClass/TeddyDraw_MonoGame.cs
// TeddyBear C#
// version: 19.04.06
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



#undef donttry

using TrickyUnits;

// MonoGame class
namespace TeddyBear

{

    delegate void TDMGLog(string msg);

    static class TeddyDraw_MonoGame

    {
        static TQMGImage Unknown;
        static TQMGImage[] Texture = new TQMGImage[256];
        static public string Error { get; private set; }
        static public TDMGLog Log = delegate (string msg) { System.Diagnostics.Debug.WriteLine($"Log: {msg}"); };

        public static void SetUnknown(TQMGImage ui) => Unknown = ui;

        public static void Init()
        {
            Error = "";
            MKL.Version("TeddyBear - TeddyDraw_MonoGame.cs","19.04.06");
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
                if (map.Texture[b] == null || map.Texture[b] == "") { Texture[b] = Unknown; Log($"No texture set on {b}"); }
                if (Texture[b] == null) {

#if !donttry
                    try {
#endif
                        Log($"Loading texture for spot {b} => {map.Texture[b]}");
                        var bt = map.OpenTexture(b);
                        if (bt == null) { Texture[b] = Unknown; Log($"JCR6 failed to open the texture file! {UseJCR6.JCR6.JERROR}"); return; }
                        Texture[b] = TQMG.GetImage(bt);

                        if (Texture[b] == null) { Texture[b] = Unknown; Log($"Texture load failed! {map.Texture[b]}"); } else { Log("Texture appears to be loaded succesfully!"); }

#if !donttry
                    } catch (System.Exception er) {

                        Log($"An error happened when loading a texture: {er.Message} >> {er.StackTrace}");
                        Texture[b] = Unknown;

                    }
#endif
                }

                if (Texture[b] != null) {
                    var modix = 0;
                    var modiy = 0;
                    var dtex = Texture[b];
                    var dlay = map.Layers[layer];
                    // X
                    switch (dlay.Hot[1]) {

                        case 'L':

                            break;

                        case 'C':

                            modix = (map.GridX/2) - (dtex.Width / 2);

                            break;

                        case 'R':

                            modix = map.GridX - dtex.Width;

                            break;

                        default:

                            return; // ERROR!

                    }
                    // Y
                    switch (dlay.Hot[0]) {

                        case 'T':

                            break;

                        case 'C':

                            modiy = (map.GridY / 2) - (dtex.Height / 2);

                            break;

                        case 'B':

                            modiy = map.GridY - dtex.Height;

                            break;

                        default:

                            return; // ERROR!

                    }
                    var dx = ((screenstart_x + (posx * map.GridX)) - scroll_x) + modix;
                    var dy = ((screenstart_y + (posy * map.GridY)) - scroll_y) + modiy;
                    dtex.Draw(dx, dy);
                    //TeddyEdit.UI.font32.DrawText(dlay.Hot, dx, dy); // debug!!!

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
