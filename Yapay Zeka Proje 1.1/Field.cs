using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yapay_Zeka_Proje_1._1
{
    class Field
    {
        public string name;
        public List<int> parents;
        public Field(string name)
        {
            this.name = name;
            parents = new List<int>();
        }
    }
}
