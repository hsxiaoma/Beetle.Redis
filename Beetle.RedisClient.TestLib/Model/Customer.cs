using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
namespace Beetle.RedisClient.TestLib
{
    [ProtoContract]
    public class Customer
    {
        [ProtoMember(1)]
        public string CustomerID { get; set; }
        [ProtoMember(2)]
        public string CompanyName { get; set; }
        [ProtoMember(3)]
        public string ContactName { get; set; }
        [ProtoMember(4)]
        public string ContactTitle { get; set; }
        [ProtoMember(5)]
        public string Address { get; set; }
        [ProtoMember(6)]
        public string City { get; set; }
        [ProtoMember(7)]
        public string PostalCode { get; set; }
        [ProtoMember(8)]
        public string Country { get; set; }
        [ProtoMember(9)]
        public string Phone { get; set; }
        [ProtoMember(10)]
        public string Fax { get; set; }
    }
}
