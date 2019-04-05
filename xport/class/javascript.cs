using TeddyBear;

namespace TeddyXport {

    class XPort_JavaScript : XPort_Base    {
        public override string TransFile(string MapName) => $"{MapName}.js";
        public override string Translate(TeddyMap map) => $"var TeddyMap = {json.Translate(map)}\n";
    }
}