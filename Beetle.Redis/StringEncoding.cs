using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beetle.Redis
{
    class StringEncoding
    {
        public StringEncoding(int length)
        {
            Buffer = new byte[length];
        }

        public byte[] Buffer;


        public int Encode(string value)
        {
           int count= Encoding.UTF8.GetBytes(value, 0, value.Length, Buffer, 0);
           return count;
        }
    }
}
