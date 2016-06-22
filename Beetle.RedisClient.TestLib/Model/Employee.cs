using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
namespace Beetle.RedisClient.TestLib
{
    [ProtoContract]
    public class Employee
    {
        [ProtoMember(1)]
        public int EmployeeID
        {
            get;
            set;
        }
        [ProtoMember(2)]
        public string LastName
        {
            get;
            set;
        }
        [ProtoMember(3)]
        public string FirstName
        {
            get;
            set;
        }
        [ProtoMember(4)]
        public string Title
        {
            get;
            set;
        }
        [ProtoMember(5)]
        public string TitleOfCourtesy { get; set; }
        [ProtoMember(6)]
        public DateTime BirthDate { get; set; }
        [ProtoMember(7)]
        public DateTime HireDate { get; set; }
        [ProtoMember(8)]
        public string Address { get; set; }
        [ProtoMember(9)]
        public string City { get; set; }
        [ProtoMember(10)]
        public string Region { get; set; }
        [ProtoMember(11)]
        public string PostalCode { get; set; }
        [ProtoMember(12)]
        public string Country { get; set; }
        [ProtoMember(13)]
        public string HomePhone { get; set; }
        [ProtoMember(14)]
        public string Extension { get; set; }
        [ProtoMember(15)]
        public string Photo { get; set; }
        [ProtoMember(16)]
        public string Notes { get; set; }
    }
}
