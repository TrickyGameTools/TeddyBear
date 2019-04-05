// Lic:
// TeddyBear Exporter
// JSON
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
// Version: 19.04.05
// EndLic



using TeddyBear;
using TrickyUnits;

namespace TeddyXport
{

    class XPort_JSON : XPort_Base
    {

        public override string TransFile(string MapName) => $"{MapName}.json";
        public override string Translate(TeddyMap map)
        {
            Out("{\n"); // start!
            #region Defs
            Out("\t\"Defs\":{\n"); // start Defs
            Out($"\t\t\"Grid\":[{map.GridX}, {map.GridY}],\n");
            Out($"\t\t\"Width\":{map.SizeX},\n");
            Out($"\t\t\"Height\":{map.SizeY}\n");
            Out("\t},\n"); // end Defs

            // Textures and zone names will get their own section. TexResize will not be implemented in any exporter until the feature is implemented, which will likely happen, say... never.
            // Layers is not needed here, as the map will define all layers in one go. In the native format, things were a littlebit more sensitve, ya see.
            #endregion
            #region Textures
            Out("\t\"Textures\":[\n"); // Start Textures
            for (int i = 0; i < 256; i++) {
                if (map.Texture[i] == null) map.Texture[i] = "";
                if (i > 0) Out(",\n");
                Out($"\t\t\"{map.Texture[i]}\"");
            }
            Out("],\n"); // end Textures
            #endregion
            #region Meta Data
            Out("\t\"MetaData\":{\n"); // start MetaData
            {
                var f = false; // I only want this variable to work within this scope and this scope alone!
                foreach (string key in map.MetaData.Keys) {
                    if (f) Out(",\n"); f = true;
                    Out($"\t\t\"{key}\" : \"{map.MetaData[key]}\"");
                }
            }
            Out("\t},\n");
            #endregion
            #region Layers
            Out("\t\"Layers\":{\n");
            foreach (string key in map.Layers.Keys) {
                var lay = map.Layers[key];
                Out($"\t\t\"{key}\" : {'{'}\n");
                Out($"\t\t\t\"Fields\":[\n");
                for (int y = 0; y <= map.SizeY; y++) {
                    if (y > 0) Out(",\n");
                    Out("\t\t\t\t[");
                    for (int x = 0; x <= map.SizeX; x++) {
                        if (x > 0) Out(", ");
                        Out(string.Format("{0,3}", lay.Get(x, y)));
                    }
                    Out("]");
                }
                Out("],\n");
                if (qstr.Prefixed(key, "Zone_")) {
                    var zn = map.ZName(key).Name;
                    Out("\t\t\t\"Names\":[");
                    for (int i = 0; i < 256; i++) {
                        if (i > 0) Out(", ");
                        Out($"\"{zn[i]}\"");
                    }
                    Out("]");
                }
                Out("\t\t},\n");
            }
            Out("},\n");
            #endregion
            #region Objects
            // Really, this is gonna look terrible!
            // And since JSON doesn't support comments... Oh, boy!
            Out("\t\"Objects\": [\n");
            for (int y = 0; y <= map.OHeight; y++) {
                if (y > 0) Out(",\n");
                Out("\t\t[");
                for (int x = 0; x <= map.OWidth; x++) {
                    if (x > 0) Out(", ");
                    Out("[");
                    var c = false;
                    var ol = map.ObjectList(x, y);
                    // System.Console.WriteLine($"({x},{y}) Num: {ol.Count}"); // debug
                    foreach (TeddyObject obj in ol) {
                        if (c) Out(", ");
                        Out("{");
                        foreach (string k in obj.Keys) {
                            Out($"\"{k}\":\"{obj.Cl(k)}\", ");
                        }
                        Out($"\"ObjType\":\"{obj.ObjType}\"");
                        Out("}");
                    }
                    Out("]");
                }
                Out("]");
            }
            Out("]\n");
            #endregion

            Out("\t}\n"); // end!
            return output.ToString();
        }

    }
}

