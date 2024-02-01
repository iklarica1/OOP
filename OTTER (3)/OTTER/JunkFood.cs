using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTTER
{
    class JunkFood : Sprite
    {
        public bool Aktivan { get; set; }
        public JunkFood(string s,int x,int y):base(s,x,y)
        {
            Aktivan = false;
        }
    }
    
}
