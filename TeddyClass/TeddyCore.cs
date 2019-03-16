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
        public byte T = 0; // 0 means the buffer only has 1 byte... This is done to allow bigger numbers in future versions!
        public int W = 0, H = 0;
        public byte[,] buf;
        public string Hot = "";

        public TeddyLayer(int width,int height,int T=0, string HotSpot="TL") {
            if (width < 2 || height < 2) throw new Exception($"Format of layer must be at least 2x2. {width}x{height} is therefore not valid!");
            buf = new byte[height, width];
            Hot = HotSpot;
        }



        public byte Get(int x, int y) => buf[y, x];
        public void Put(int x, int y, byte v) { buf[y, x] = v; }

    }

    class TeddyObject {
        Dictionary<string, string> Data;
        public string ObjType = "";
        public void Df(string k,string v) { Data[k] = v; }
    }

    class TeddyZoneName { // This is only to form a return value to the ZName method in the 'TeddyBear' method
        public string[] Name = new string[256];
    }


    class TeddyMap {
        TJCRDIR TextureDir;
        SortedDictionary<string, TeddyZoneName> ZoneNameMap = new SortedDictionary<string, TeddyZoneName>();
        SortedDictionary<string, TeddyLayer> Layers = new SortedDictionary<string, TeddyLayer>();
        public SortedDictionary<string, string> MetaData = new SortedDictionary<string, string>();
        int GridX, GridY;
        int SizeX, SizeY;
        bool TexResize;
        string[] Texture=new string[256];
        // Objects
        int OW = 0;
        int OH = 0;
        List<TeddyObject>[,] Objects;

        List<TeddyObject> ObjectList(int x,int y) {
            if (Objects[x, y] == null) Objects[x, y] = new List<TeddyObject>();
            return Objects[x, y];
        }

        /// <summary>
        /// When set to 'true' your program will crash whenever an error happens and throw an exception. When set to 'false', you can just read out the 'Error' string
        /// </summary>
        public bool crash = false;

        private string LastError = "Ok";
        /// <summary>
        /// Contains the error message, if something went wrong.
        /// If no errors occurred this will contain "Ok".
        /// Of course, when 'crash' is set to true, using this will be pretty pointless :-P
        /// </summary>
        public string Error { get => LastError; }
        string TeddyError {
            set {
                LastError = value;
                if (crash && value!="Ok") throw new Exception($"TeddyBear Error: {value}");
            }
        }

        public TeddyZoneName ZName(string key) {
            var L = key;
            if (qstr.Left(L, 5) != "Zone_") L = "Zone_" + L;
            TeddyZoneName Ret;
            if (ZoneNameMap.ContainsKey(L)) {
                Ret = ZoneNameMap[L];
            } else {
                Ret = new TeddyZoneName();
                ZoneNameMap[L] = Ret;
            }
            return Ret;
        }

        private TeddyMap() { } // Does that block from using 'new' in stead?

        TeddyMap TeddyLE(string s)
        {
            TeddyError = s;
            return null;
        }

        static public TeddyMap Load(TJCRDIR MapDir, TJCRDIR TexDir, string Map) {
            /*
             * This is DIRTY CODE STRAIGHT FROM HELL!
             * 
             * Back in the day I wasn't much to clean code, and I could not really be fully arshed to completely write a new TeddyBear loader from scratch
             * So I just manually translated the Dirty BlitzMax code into C#.
             * Cut me some slack, will ya?
             * 
             */
            var ret = new TeddyMap();
            ret.TextureDir = TexDir;
            ret.TeddyError = "Ok";
            var MN = Map.Replace("\\", " / "); if (qstr.Right(MN, 1) != "/" && MN.Length > 0) MN += "/";
            var BT = MapDir.ReadFile(MN + "Defs");
            int Tag = 0, TID = 0;
            //var LayLoad = "";
            int X = 0, Y = 0;
            TeddyObject TedObj = null;
            var K = "";
            var LLayer = ""; // Last found layer.
            var Idx = 0;
            if (BT == null)
                return ret.TeddyLE($"Defs could not be retrieved from level.\n\nJCR returned:\n{JCR6.JERROR}");
            if (BT.EOF) {
                BT.Close();
                return ret.TeddyLE($"Defs file of map {Map} appears to be an empty file");
            }
            while (!BT.EOF) {
                Tag = BT.ReadByte();
                //debuglog "DefTag #"+Tag
                switch (Tag) {
                    case 0: ret.GridX = BT.ReadInt(); ret.GridY = BT.ReadInt(); break; // 'debuglog "DEF:>Grid = ("+Ret.GridX+","+ret.GridY+")"
                    case 1: ret.TexResize = BT.ReadByte() > 0; break; //'debuglog "DEF:Texture Resizing = "+Ret.TexResize
                    case 2: ret.SizeX = BT.ReadInt(); ret.SizeY = BT.ReadInt(); break; // 'debuglog "DEF:>Default Size = ("+Ret.GridX+","+ret.GridY+")"
                    case 3: TID = BT.ReadByte(); ret.Texture[TID] = BT.ReadString(); break; //debuglog "DEF:>Texture["+TID+"] = ~q"+Ret.Texture[TID]+"~q"
                    case 4: LLayer = BT.ReadString(); ret.NewLayer(LLayer, ret.SizeX, ret.SizeY, 0); break;
                    case 5: Idx = BT.ReadByte(); ret.ZName(LLayer).Name[Idx] = BT.ReadString(); break; //'debuglog "ZoneName[~q"+LLayer+"~q,"+Idx+"] = ~q"+Ret.ZName(LLayer).Name[Idx]+"~q; Len="+Len(Ret.ZName(LLayer).Name[Idx])+"~n"
                    default:
                        BT.Close();
                        return ret.TeddyLE($"Incorrect tag found ({Tag}) in loading the defs of level {Map}.\nYou are either trying to load an illegal level, or the level has been produced with a later version of Teddybear.~nTry to get ahold of the latest version, if the error persists, the level is most likely illegal or damaged!");
                }
            }
            BT.Close();
            //' Load the layers
            foreach (string LayLoad in ret.Layers.Keys) {
                var Prefix = "Layer_";
                if (qstr.Left(LayLoad, 5) == "Zone_") Prefix = "";
                BT = MapDir.ReadFile(MN + Prefix + LayLoad);
                if (BT == null) return ret.TeddyLE($"Layer \"{LayLoad}\" could not be retrieved from level.\n\nJCR returned:\n{JCR6.JERROR}");
                var laygoing = true;
                do { //Repeat
                    if (BT.EOF) {
                        BT.Close(); return ret.TeddyLE($"Layer \"{LayLoad} in map {Map} appears to be truncated! ($ffnf)");
                    }
                    Tag = BT.ReadByte();
                    switch (Tag) {
                        case 0: ret.Layers[LayLoad].T = BT.ReadByte(); if (ret.Layers[LayLoad].T > 0) { BT.Close(); return ret.TeddyLE("Sorry dude, higher bit levels are not supported in this level of Teddybear. Try a higher version."); } break;
                        case 1:
                            ret.Layers[LayLoad].W = BT.ReadInt(); ret.Layers[LayLoad].H = BT.ReadInt();
                            ret.Layers[LayLoad].buf = new byte[ret.Layers[LayLoad].W + 1, ret.Layers[LayLoad].H + 1];
                            break;
                        case 2: ret.Layers[LayLoad].Hot = BT.ReadString(2); break;
                        case 0xff:
                            for (X = 0; X <= ret.Layers[LayLoad].W; X++)
                                for (Y = 0; Y <= ret.Layers[LayLoad].H; Y++)
                                    ret.Layers[LayLoad].buf[Y, X] = BT.ReadByte();
                            laygoing = false;
                            break;
                        default:
                            BT.Close();
                            return ret.TeddyLE("Incorrect tag found (" + Tag + ") in loading the layer '" + LayLoad + "' of level " + Map + ".\nYou are either trying to load an illegal level, or the level has been produced with a later version of TeddyBear.~nTry to get ahold of the latest version, and if the error persists, the level is most likely illegal or damaged!");
                    }
                } while (laygoing);
                BT.Close();
            }
            // Load the objects (if available)
            if (MapDir.Exists($"{MN}Objects")) { //If MapContains(JCR.Entries, Upper(MN)+"OBJECTS")
                BT = MapDir.ReadFile(MN + "Objects");
                ret.OW = BT.ReadInt();
                ret.OH = BT.ReadInt();
                ret.Objects = new List<TeddyObject>[ret.OW + 1, ret.OH + 1];
                while (!BT.EOF) {
                    Tag = BT.ReadByte();
                    switch (Tag) {
                        case 0:
                            X = BT.ReadInt();
                            Y = BT.ReadInt();
                            TedObj = new TeddyObject();
                            ret.ObjectList(X, Y).Add(TedObj);
                            break;
                        case 1:
                            K = BT.ReadString();
                            TedObj.Df(K, BT.ReadString());
                            break;
                        case 2:
                            TedObj.ObjType = BT.ReadString();
                            break;
                        default:
                            BT.Close();
                            return ret.TeddyLE("Incorrect tag found (" + Tag + ") in loading objects of map " + Map + ".	\nYou are either trying to load an illegal level, or the level has been produced with a later version of TeddyBear.~nTry to get ahold of the latest version, and if the error persists, the level is most likely illegal or damaged!");
                    }
                }
                BT.Close();
            }
            //' Load the general data (if available)
            if (MapDir.Exists($"{MN}DATA")) { //MapContains(JCR.Entries, Upper(MN)+"DATA")
                ret.MetaData = MapDir.LoadStringMapSorted(MN + "Data");

            }
            return ret;
        }

        static public TeddyMap Load(TJCRDIR Dir,string map) => Load(Dir, Dir, map);
        static public TeddyMap Load(string MapDir, string TexDir, string Map) => Load(JCR6.Dir(MapDir), JCR6.Dir(TexDir), Map);
        static public TeddyMap Load(string Dir,string map) {
            var jcr = JCR6.Dir(Dir);
            return Load(jcr, jcr, map);
        }
        static public TeddyMap Load(string map) => Load(map, "");

        public void NewLayer(string name,int w,int h,int t=0) { Layers[name] = new TeddyLayer(w, h,t); }
        public void NewLayer(string name) => NewLayer(name, SizeX, SizeY);
        public void DelLayer(string name) => Layers.Remove(name); 

    }

    class Core{
        static void Init(){
            MKL.Version("TeddyBear - TeddyCore.cs","19.03.16");
            MKL.Lic    ("TeddyBear - TeddyCore.cs","ZLib License");
        }
    }
}
