using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
namespace Beetle.RedisClient.TestLib
{
    [ProtoContract]
    public class Contact 
    {
        [ProtoMember(1)]
        public string Phone
        {
            get;
            set;
        }
        [ProtoMember(2)]
        public string EMail
        {
            get;
            set;
        }
        [ProtoMember(3)]
        public string QQ
        {
            get;
            set;
        }
    }
}
