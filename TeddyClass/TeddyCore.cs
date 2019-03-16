// Lic:
// TeddyClass/TeddyCore.cs
// TeddyBear Core
// version: 19.03.16
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
using System;
using System.Collections.Generic;
using UseJCR6;

namespace TeddyBear{

    class TeddyLayer {

        public byte[,] buf;



        public TeddyLayer(int width,int height) {

            if (width < 2 || height < 2) throw new Exception($"Format of layer must be at least 2x2. {width}x{height} is therefore not valid!");

            buf = new byte[height, width];

        }



        public byte Get(int x, int y) => buf[y, x];

        public void Put(int x, int y, byte v) { buf[y, x] = v; }

    }

    class TeddyMap {

        SortedDictionary<string, TeddyLayer> Layers = new SortedDictionary<string, TeddyLayer>();



        public void NewLayer(string name,int w,int h) { Layers[name] = new TeddyLayer(w, h); }

    }

    class Core{
        static void Init(){
            MKL.Version("TeddyBear - TeddyCore.cs","19.03.16");
            MKL.Lic    ("TeddyBear - TeddyCore.cs","ZLib License");
        }
    }
}
