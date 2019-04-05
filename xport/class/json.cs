
using TeddyBear;

namespace TeddyXport{

    class XPort_JSON : XPort_Base {
        
        public override string TransFile(string MapName) => $"{MapName}.json";
        public override string Translate(TeddyMap map) {
            Out("{\n"); // start!
            #region Defs
            Out("\t\"Defs\":{\n"); // start Defs
            Out($"\t\t\"Grid\":[{map.GridX}, {map.GridY}],\n");
            Out($"\t\t\"Width\":{map.SizeX},\n");
            Out($"\t\t\"Height\":{map.SizeY},\n");
            Out("\t},"); // end Defs
                        
            // Textures and zone names will get their own section. TexResize will not be implemented in any exporter until the feature is implemented, which will likely happen, say... never.
            // Layers is not needed here, as the map will define all layers in one go. In the native format, things were a littlebit more sensitve, ya see.
            #endregion
            #region Textures
            Out("\t\"Textures\":[\n"); // Start Textures
            for(int i=0; i<256; i++) {
                if (map.Texture[i] == null) map.Texture[i] = "";
                if (i > 0) Out(",\n");
                Out($"\t\t\"{map.Texture[i]}\"");
            }
            Out("\t],"); // end Textures
            #endregion
            #region Meta Data
            Out("\t\"MetaData\":{\n"); // start MetaData
            {
                var f = false; // I only want this variable to work within this scope and this scope alone!
                foreach (string key in map.MetaData.Keys) {
                    if (f) Out(",\n"); f = true;
                    Out($"\t\t\"{key}\" : \"{map.MetaData[key]}\"");
                }
                Out("\t],"); // end Textures
            }
            Out("\t},\n");
            #endregion
            #region Layers
            #endregion
            #region Objects
            #endregion

            Out("}\n"); // end!
            return output.ToString();
        }

    }
}