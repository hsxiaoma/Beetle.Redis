using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Beetle.RedisClient.TestLib
{
    [ProtoContract]
    public class Order
    {
        [ProtoMember(1)]
        public int OrderID { get; set; }
        [ProtoMember(2)]
        public string CustomerID { get; set; }
        [ProtoMember(3)]
        public int EmployeeID { get; set; }
        [ProtoMember(4)]
        public DateTime OrderDate { get; set; }
        [ProtoMember(5)]
        public DateTime RequiredDate { get; set; }
        [ProtoMember(6)]
        public DateTime ShippedDate { get; set; }
        [ProtoMember(7)]
        public int ShipVia { get; set; }
        [ProtoMember(8)]
        public double Freight { get;set;}
        [ProtoMember(9)]
        public string ShipName { get; set; }
        [ProtoMember(10)]
        public string ShipAddress { get; set; }
        [ProtoMember(11)]
        public string ShipCity { get; set; }
        [ProtoMember(12)]
        public string ShipPostalCode { get; set; }
        [ProtoMember(13)]
        public string ShipCountry { get; set; }
    }
}
