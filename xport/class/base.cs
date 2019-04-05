using System.Collections.Generic;
using System.Text;
using TeddyBear;

namespace TeddyXport{

    delegate void XPPrint(string m);

    abstract class XPort_Base {
        static public readonly SortedDictionary<string, XPort_Base> Drivers = new SortedDictionary<string, XPort_Base>();
        static public XPort_Base json => Drivers["json"]; // Both the JavaScript driver and the Python driver can work 99% out through JSON. All I need to do is to add a few definitions.
        public XPPrint Print;
        protected StringBuilder output = new StringBuilder(10000);

        protected void Out(string k) => output.Append(k);

        public abstract string TransFile(string MapName);
        abstract public string Translate(TeddyMap map);
    }

    class XPort { static public void init() {
            XPort_Base.Drivers["json"] = new XPort_JSON();
        }
    }
}