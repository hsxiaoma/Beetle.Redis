using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beetle.Redis
{
    public class NameType
    {
        public NameType(Type type)
        {
            Name = type.Name;
            Type = type;
            Index = 0;
        }
        public NameType(Type type, string key,int index)
        {
            Type = type;
            Name = key;
            Index = index;
        }
        public int Index;
        public Type Type;

        public string Name;
      
    }
}
