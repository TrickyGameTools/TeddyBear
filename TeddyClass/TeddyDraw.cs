// Lic:
// TeddyClass/TeddyDraw.cs
// TeddyBear C#
// version: 19.03.30
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

namespace TeddyBear {



    delegate void d_DrawItem(TeddyMap map, string layer, int screenstart_x, int screenstart_y, int scroll_x, int scroll_y, int posx, int posy);

    delegate void d_TexReset(byte b); // 0 should mean complete reset, and any other number just that texture spot



    /// <summary>This class contains the basic draw routines. Please note, as I wanted TeddyBear to be as Flexible as possible, it's driver based. So you'll need to attach a driver to this class in order to make it work!</summary>
    class TeddyDraw {



        public static d_DrawItem DrawTileItem = delegate { throw new System.Exception("No driver has been set up to draw in!"); };

        public static d_DrawItem DrawZoneItem = delegate { throw new System.Exception("No driver has been set up to draw in!"); };

        public static d_TexReset TexReset = delegate { throw new System.Exception("No TexReset in the driver or driver has not yet been set."); };

       



        static public void DrawLayer(TeddyMap map, string Layer,  int x=0, int y= 0, int scrollx=0, int scrolly = 0, bool[,] visibilityMap=null) {

            d_DrawItem DrawItem = DrawTileItem;

            if (qstr.Prefixed(Layer.ToUpper(), "ZONE_")) DrawItem = DrawZoneItem;

            for (int iy = 0; iy < map.Layers[Layer].H; iy++) {

                for (int ix = 0; ix < map.Layers[Layer].W; ix++) {

                    DrawItem(map, Layer, x, y, scrollx, scrolly, ix, iy);

                }

            }

        }



    }



}
