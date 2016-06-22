using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Beetle.RedisClient.TestLib
{
    [ProtoContract]
    public class UserBase
    {
        [ProtoMember(1)]
        public string Name
        {
            get;
            set;
        }
        [ProtoMember(2)]
        public string City
        {
            get;
            set;
        }
        [ProtoMember(3)]
        public string Counrty
        {
            get;
            set;
        }
        [ProtoMember(4)]
        public short Age
        {
            get;
            set;
        }
    }
}
