// Lic:
// TeddyBear Exporter - Basis
// Abstract class
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


using System.Collections.Generic;
using System.Text;
using TeddyBear;

namespace TeddyXport {

    delegate void XPPrint(string m);

    abstract class XPort_Base
    {
        static public readonly SortedDictionary<string, XPort_Base> Drivers = new SortedDictionary<string, XPort_Base>();
        static public XPort_Base json => Drivers["json"]; // Both the JavaScript driver and the Python driver can work 99% out through JSON. All I need to do is to add a few definitions.
        public XPPrint Print;
        protected StringBuilder output = new StringBuilder(10000);

        protected void Out(string k) => output.Append(k);

        public abstract string TransFile(string MapName);
        abstract public string Translate(TeddyMap map);
    }

    class XPort
    {
        static public void init()
        {
            XPort_Base.Drivers["json"] = new XPort_JSON();
            XPort_Base.Drivers["python"] = new XPort_Python();
            XPort_Base.Drivers["javascript"] = new XPort_JavaScript();
            XPort_Base.Drivers["lua"] = new XPort_Lua();
        }
    }
}



