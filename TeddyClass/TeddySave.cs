// Lic:
// TeddyClass/TeddySave.cs
// TeddyBear C#
// version: 19.04.02
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





/* The reason I decided to put this in a separate class, and even in a separate file for that matter
 * is because saving may normally not be needed, at least not in games. I know some games (even some
 * professional ones) do save the entire map into the savegame file, but I've always considered that
 * a waste of space, to be honest, and that is also why the games I wrote with TeddyBear do not do it
 * (nope, not even The Secrets of Dyrt. Come on, I'd have to save all maps you've visted before, like
 * the savegames weren't already big enough by themselves).
 * 
 * Of course, since the classes used by the editor are the same as the ones used in games (that is, if
 * you use C# to make your TeddyBear games, of course), I did need a proper save game class, which would
 * still allow you to save maps, even if it's only to allow you to create your own map editors, as well
 * getting my own to work :P
 * 
 * It goes without saying that this file does NOT work without including TeddyCore.cs to your project.
 */

using TrickyUnits;
using UseJCR6;

namespace TeddyBear { 
    class TeddySave {
        static void Init() {
            Core.Init();
            MKL.Lic    ("TeddyBear - TeddySave.cs","ZLib License");
            MKL.Version("TeddyBear - TeddySave.cs","19.04.02");
        }

        public delegate void DLog(string message);
        static DLog Log = delegate { };


        /// <summary>

        /// Can be used to attach your own log function. Debugging can be easier this way.

        /// </summary>

        /// <param name="L"></param>
        public static void SetLog(DLog L) { Log = L; }


        /// <summary>
        /// Saves the TeddyBear map inside a JCR6 resouce currently in creation. Returns "Ok" if succesful and otherwise an error message.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="jcr"></param>
        /// <param name="Entry"></param>
        /// <param name="Storage"></param>
        public static string Save(TeddyMap map,TJCRCreate jcr, string EntryPref, string Storage = "Store") {
            try {
                Init();
                //Local MP$ = Replace(MapFile, "\"," / ")
                TJCRCreate BT = jcr;
                TJCRCreateStream BTE;
                // var K = "";
                // Local Ak, X, Y
                // TeddyObject Obj;
                // Local Obj: TeddyObject
                //'If Right(MP,1)<>"/" MP:+"/"
                //'If Not CreateDir(MP,1) Return "Required path could not be created!"
                // BT = JCR_Create(MapFile)
                Log("Gonna save a Teddy Bear map");
                if (BT == null) return "Save file could not be created or null sent to saveJCR request!";
                // DEFS
                Log("Saving Defs");
                BTE = BT.NewEntry($"{EntryPref}Defs",Storage);
                if (BTE == null) return $"Entry creation failed: {JCR6.JERROR}";
                BTE.WriteByte(0); BTE.WriteInt(map.GridX); BTE.WriteInt(map.GridY);
                BTE.WriteByte(1); if (map.TexResize) BTE.WriteByte(1); else BTE.WriteByte(0);
                BTE.WriteByte(2); BTE.WriteInt(map.SizeX); BTE.WriteInt(map.SizeY);
                Log("\tWriting Textures");
                for (byte Ak = 255; Ak > 0; Ak--) {
                    // Log($"\tWriting texture {Ak}"); // Needless to log all the time, but I used this line for debugging purposes.
                    if (map.Texture[Ak]!=null && map.Texture[Ak] != "") {
                        BTE.WriteByte(3);
                        BTE.WriteByte(Ak);
                        BTE.WriteString(map.Texture[Ak]);
                    }
                }
                Log("\tWriting Layers");
                foreach (string K in map.Layers.Keys) {
                    Log($"\tWriting Layer: {K}");
                    BTE.WriteByte(4);
                    BTE.WriteString(K);
                    if (qstr.Left(K, 5) == "Zone_") {
                        for (byte Ak = 255; Ak > 0; Ak--) { // Zone Names (if the specific layer is a zone)
                            if (map.ZName(K).Name[Ak] == null) map.ZName(K).Name[Ak] = "";
                            BTE.WriteByte(5);
                            BTE.WriteByte(Ak);
                            BTE.WriteString(map.ZName(K).Name[Ak]);
                        }
                    }
                }
                BTE.Close();
                // Layers
                Log("Saving Layers");
                foreach (string K in map.Layers.Keys) {
                    Log($"Saving Layer {K}");
                    var Prefix = "Layer_";
                    var lay = map.Layers[K];
                    if (qstr.Left(K, 5) == "Zone_") Prefix = "";
                    BTE = BT.NewEntry($"{EntryPref}{Prefix}{K}", Storage);
                    // Layer Type (should in this version be 0 always)
                    BTE.WriteByte(0);
                    BTE.WriteByte(map.Layers[K].T);
                    // Format
                    BTE.WriteByte(1);
                    BTE.WriteInt(lay.W);
                    BTE.WriteInt(lay.H);
                    // Hot Spots
                    BTE.WriteByte(2);
                    BTE.WriteString(qstr.Left($"{lay.Hot}   ", 2), true);
                    // Saving the map itself. No compression is used in saving the data, as TeddyBear has been designed to be used in combination with JCR6 which already compresses everything by itself.
                    // Also note, the map itself tagged with $ff will cause the loader to end loading as soon as the map itself is read. In other words it must always come last in the file.
                    // This is simply a security measure as some tags above can cause the loaded level to be unloaded immediatly, when this comes last, this simply can't happen.
                    // A level without a $ff tagged map will also result into an error when loading.
                    // $fe is also reserved in case 32 bit maps may be added in the future, which is in the current state of things, not likely to happen, but just in case :)
                    // and for that reason $fd is reserved in case of the very small chance 64bit is used.
                    // Must I really reserve $fc for 128bit (which is not even supported by BlitzMax yet)? Nah.... :-P
                    BTE.WriteByte(0xff);
                    for (int X = 0; X <= lay.W; X++)
                        for (int Y = 0; Y <= lay.H; Y++)
                            BTE.WriteByte(lay.Get(X, Y));
                    BTE.Close();
                }
                // Objects
                Log("Saving Objects");
                BTE = BT.NewEntry($"{EntryPref}Objects", Storage);
                BTE.WriteInt(map.OWidth);
                BTE.WriteInt(map.OHeight);
                for (int X = 0; X <= map.OWidth; X++)
                    for (int Y = 0; Y <= map.OHeight; Y++)
                        foreach (TeddyObject Obj in map.ObjectList(X, Y)) {
                            BTE.WriteByte(0); // New Object on the next coordinates
                            BTE.WriteInt(X);
                            BTE.WriteInt(Y);
                            BTE.WriteByte(2);
                            BTE.WriteString(Obj.ObjType);
                            foreach (string K in Obj.Keys) { // New field, with the next data
                                BTE.WriteByte(1);
                                BTE.WriteString(K);
                                BTE.WriteString(Obj.Cl(K));


                            }
                        }
                BTE.Close();
                // General Data
                Log("Saving General Data");
                BT.NewStringMap(map.MetaData, $"{EntryPref}Data", Storage);
                // All done
                Log("Closing");
                BT.Close();
                Log("Map saved");
                return "Ok";
            } catch (System.Exception err) {
                return $"System Error: {err.Message}";
            }
        }

        /// <summary>
        /// Saves a TeddyBear map. If you are not really well versed in how JCR6 works, you may only need the first two paramters, I only added the others so more advanced users can benefit from the true power of JCR6 ;)
        /// </summary>
        /// <param name="map"></param>
        /// <param name="jcr"></param>
        /// <param name="Entry"></param>
        /// <param name="Storage"></param>
        /// <param name="FileTableStorage"></param>
        static public string Save(TeddyMap map,string filename, string EntryPref="", string Storage="Store", string FileTableStorage= "Store"){
            var bt = new TJCRCreate(filename, FileTableStorage);
            var r = Save(map, bt, EntryPref, Storage);
            bt.Close();
            return r;
        }

    }
}
