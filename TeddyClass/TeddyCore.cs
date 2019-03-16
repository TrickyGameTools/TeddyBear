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
        TJCRDIR TexDir;
        SortedDictionary<string, TeddyLayer> Layers = new SortedDictionary<string, TeddyLayer>();
        public SortedDictionary<string, string> MetaData = new SortedDictionary<string, string>();

        private TeddyMap() { } // Does that block from using 'new' in stead?

        static public TeddyMap Load(TJCRDIR MapDir,TJCRDIR TexDir,string Map) {
            var ret = new TeddyMap();
            // Load code comes later!
            return ret;
        }

        static public TeddyMap Load(TJCRDIR Dir,string map) => Load(Dir, Dir, map);
        static public TeddyMap Load(string MapDir, string TexDir, string Map) => Load(JCR6.Dir(MapDir), JCR6.Dir(TexDir), Map);
        static public TeddyMap Load(string Dir,string map) {
            var jcr = JCR6.Dir(Dir);
            return Load(jcr, jcr, map);
        }
        static public TeddyMap Load(string map) => Load(map, "");

        public void NewLayer(string name,int w,int h) { Layers[name] = new TeddyLayer(w, h); }
        public void DelLayer(string name) => Layers.Remove(name); 

    }

    class Core{
        static void Init(){
            MKL.Version("TeddyBear - TeddyCore.cs","19.03.16");
            MKL.Lic    ("TeddyBear - TeddyCore.cs","ZLib License");
        }
    }
}
