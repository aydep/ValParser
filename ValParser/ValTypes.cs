using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValParser
{
    public class Valutes
    {
        public string id { get; set; }
        public string numCode { get; set; }
        public string charCode { get; set; }
        public string nominal { get; set; }
        public string name { get; set; }
        public string vlalue { get; set; }
    }

    public class Mettals
    {
        public DateTime data { get; set; }
        public float buy { get; set; }
        public float sell { get; set; }
        public int code { get; set; }
    }

}
