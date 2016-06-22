using System;
using Beetle.Redis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Beetle.Redis;
namespace Beetle.RedisClient.TestLib
{
    [TestClass]
    public class RedisClientTest
    {
        [TestMethod]
        public void EXPIRE_TTL()
        {
            StringKey key = "HENRY_EMAIL";
            key.Set("HENRYFAN@MSN.COM");
            key.Expire(800);
            long value = key.TTL();
            Assert.AreNotEqual(value, 0);
        }
        [TestMethod]
        public void PEXPIRE_PTTL()
        {
            long dt = -1;
            StringKey key = "HENRY_EMAIL";
            key.Set("HENRYFAN@MSN.COM");
            key.PExpire(2000);
            long value = key.PTTL();
            Assert.AreNotEqual(value, 0);
            System.Threading.Thread.Sleep(2000);
            value = key.PTTL();
            Assert.AreEqual(value, dt);
        }
        [TestMethod]
        public void Set_GET_String()
        {
            StringKey key = "HENRY";
            string Remark = "henryfan gz cn 18 henryfan@msn.com 28304340";
            key.Set(Remark);
            Assert.AreEqual(Remark, key.Get<string>());
        }

        [TestMethod]
        public void SET_GET_String_Values()
        {
            StringKey keys = new string[] { "QQ", "EMAIL" };
            string[] VALUE = new string[] { "28304340", "HENRYFAN@MSN.COM" };
            keys.SetValues(VALUE);
            IList<object> data = keys.Get<string,string>();
            Assert.AreEqual(VALUE[0], data[0]);
            Assert.AreEqual(VALUE[1], data[1]);
        }

        [TestMethod]
        public void GET_SET_Json()
        {
            JsonKey rk = "henry_json";
            UserBase ub = new UserBase();
            ub.Name = "henryfan";
            ub.City = "gz";
            ub.Counrty = "cn";
            ub.Age = 10;
            rk.Set(ub);
            Assert.AreEqual(ub.Name, rk.Get<UserBase>().Name);
        }

        [TestMethod]
        public void GET_SET_Json_Objects()
        {
            JsonKey keys = new string[]{"henry_info", "henry_contact"};
            UserBase ub = new UserBase();
            ub.Name = "henryfan";
            ub.City = "gz";
            ub.Counrty = "cn";
            ub.Age = 10;
            Contact contact = new Contact();
            contact.EMail = "hernyfan@msn.com";
            contact.QQ = "28304340";
            contact.Phone = "13660223497";
            keys.SetValues(new object[] { ub, contact });
            IList<object> data = keys.Get<UserBase, Contact>();
            Assert.AreEqual(ub.Name, ((UserBase)data[0]).Name);
            Assert.AreEqual(contact.Phone, ((Contact)data[1]).Phone);

        }

        [TestMethod]
        public void GET_SET_Protobuf()
        {
         
            ProtobufKey rk = "henry_protobuf";
            UserBase ub = new UserBase();
            ub.Name = "henryfan";
            ub.City = "gz";
            ub.Counrty = "cn";
            ub.Age = 10;
            rk.Set(ub);
            Assert.AreEqual(ub.Name, rk.Get<UserBase>().Name);
        }

        [TestMethod]
        public void GET_SET_Protobuf_Objects()
        {
            ProtobufKey key = new string[] { "henry_info_p", "henry_contact_p" };
            UserBase ub = new UserBase();
            ub.Name = "henryfan";
            ub.City = "gz";
            ub.Counrty = "cn";
            ub.Age = 10;
            Contact contact = new Contact();
            contact.EMail = "hernyfan@msn.com";
            contact.QQ = "28304340";
            contact.Phone = "13660223497";
            key.SetValues(new object[] { ub, contact });
            IList<object> data = key.Get<UserBase, Contact>();
            Assert.AreEqual(ub.Name, ((UserBase)data[0]).Name);
            Assert.AreEqual(contact.Phone, ((Contact)data[1]).Phone);

        }

        [TestMethod]
        public void Keys()
        {
            JsonKey key = "*";
            IList<string> keys = key.Keys();
            Console.WriteLine(keys.Count);
            key = "henry*";
            keys = key.Keys();
            Console.WriteLine(keys.Count);
        }

        [TestMethod]
        public void LST_POP_PUSH()
        {
            ProtobufList<UserBase> lst = "USERS";
            lst.Push(new UserBase { Name = "henry", Age = 18, City = "gz", Counrty = "cn" });
            Assert.AreEqual("henry", lst.Pop().Name);
        }
        [TestMethod]
        public void LST_REMOVE_ADD()
        {
            ProtobufList<UserBase> lst = "USERS";
            lst.Add(new UserBase { Name = "henry", Age = 18, City = "gz", Counrty = "cn" });
            lst.Add(new UserBase { Name = "bbq", Age = 18, City = "gz", Counrty = "cn" });
            Assert.AreEqual("bbq", lst.Remove().Name);
        }
        [TestMethod]
        public void LST_Length()
        {
            ProtobufList<UserBase> lst = "USERS";
            lst.Clear();
            lst.Add(new UserBase { Name = "henry", Age = 18, City = "gz", Counrty = "cn" });
            lst.Add(new UserBase { Name = "bbq", Age = 18, City = "gz", Counrty = "cn" });
            Assert.AreEqual(lst.Count(), 2);
        }
        [TestMethod]
        public void LST_Region()
        {
            ProtobufList<UserBase> lst ="USERS";
            lst.Clear();
            for (int i = 0; i < 10; i++)
            {
                lst.Add(new UserBase { Name = "henry" + i, Age = 18, City = "gz", Counrty = "cn" });
            }
            IList<UserBase> items = lst.Range();
            Assert.AreEqual(items[0].Name, "henry0");
            Assert.AreEqual(items[9].Name, "henry9");
            items = lst.Range(5, 7);
            Assert.AreEqual(items[0].Name, "henry5");
            Assert.AreEqual(items[2].Name, "henry7");
        }

        [TestMethod]
        public void MapSet()
        {

            JsonMapSet map = "HENRY_INFO";
            UserBase ub = new UserBase();
            ub.Name = "henryfan";
            ub.City = "gz";
            ub.Counrty = "cn";
            ub.Age = 10;
            Contact contact = new Contact();
            contact.EMail = "hernyfan@msn.com";
            contact.QQ = "28304340";
            contact.Phone = "13660223497";
            map.Set(ub, contact);
            IList<object> data = map.Get<UserBase, Contact>();
            Assert.AreEqual(ub.Name, ((UserBase)data[0]).Name);
            Assert.AreEqual(contact.Phone, ((Contact)data[1]).Phone);

        }
        [TestMethod]
        public void MapSetdRemove()
        {
            JsonMapSet map = "HENRY_INFO";
            UserBase ub = new UserBase();
            ub.Name = "henryfan";
            ub.City = "gz";
            ub.Counrty = "cn";
            ub.Age = 10;
            Contact contact = new Contact();
            contact.EMail = "hernyfan@msn.com";
            contact.QQ = "28304340";
            contact.Phone = "13660223497";
            map.Set(ub, contact);
            map.Remove<Contact>();
            contact = map.Get<Contact>();
            Assert.AreEqual(null, contact);

        }
        [TestMethod]
        public void MapSetClear()
        {
            JsonMapSet map = "HENRY_INFO";
            UserBase ub = new UserBase();
            ub.Name = "henryfan";
            ub.City = "gz";
            ub.Counrty = "cn";
            ub.Age = 10;
            Contact contact = new Contact();
            contact.EMail = "hernyfan@msn.com";
            contact.QQ = "28304340";
            contact.Phone = "13660223497";
            map.Set(ub, contact);
            map.Clear();
            IList<object> data = map.Get<UserBase, Contact>();
            Assert.AreEqual(null, data[0]);
            Assert.AreEqual(null, data[1]);
        }

        [TestMethod]
        public void Info()
        {
            foreach (RedisHost host in Redis.RedisClient.DefaultDB.WriteHosts)
            {
                IList<string> result = host.Info();
                foreach (string item in result)
                {
                    Console.WriteLine(item);
                }
            }
        }
    }
}
