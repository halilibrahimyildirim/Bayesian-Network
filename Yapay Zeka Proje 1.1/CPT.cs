using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yapay_Zeka_Proje_1._1
{
    class CPT
    {
        public Field field;
        public Dictionary<string, Dictionary<string, double>> map;
        public CPT(Field field)
        {
            this.field = field;
            map = new Dictionary<string, Dictionary<string, double>>();
        }
    }
}
