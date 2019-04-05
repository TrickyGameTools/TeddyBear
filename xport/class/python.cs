using TeddyBear;

namespace TeddyXport {

    class XPort_Python : XPort_Base {
        public override string TransFile(string MapName) => $"{MapName}.py";
        public override string Translate(TeddyMap map) => $"TeddyMap = {json.Translate(map)}\n";
    }
}