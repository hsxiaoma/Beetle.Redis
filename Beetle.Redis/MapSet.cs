using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beetle.Redis
{
    public abstract class MapSet
    {
        public MapSet(string key, DataType dataType)
        {
            mDataKey = key;
            mDataType = dataType;
        }

        private string mDataKey;

        private DataType mDataType;

        public T Get<T>(RedisClient db = null)
        {
            db = RedisClient.GetClient(db);
            return (T)db.GetFields((string)mDataKey, new NameType[] {
            new NameType(typeof(T))}, mDataType).FieldValueToList()[0];
        }

        public IList<object> Get<T, T1>(RedisClient db = null)
        {
            db = RedisClient.GetClient(db);
            return db.GetFields((string)mDataKey, new NameType[] {
            new NameType(typeof(T)),new NameType(typeof(T1))}, mDataType).FieldValueToList();
        }

        public IList<object> Get<T, T1, T2>(RedisClient db = null)
        {
            db = RedisClient.GetClient(db);
            return db.GetFields((string)mDataKey, new NameType[] {
            new NameType(typeof(T))
            ,new NameType(typeof(T1))
            ,new NameType(typeof(T2))}, mDataType).FieldValueToList();
        }

        public IList<object> Get<T, T1, T2, T3>(RedisClient db = null)
        {
            db = RedisClient.GetClient(db);
            return db.GetFields((string)mDataKey, new NameType[] {
            new NameType(typeof(T))
            ,new NameType(typeof(T1))
            ,new NameType(typeof(T2))
            ,new NameType(typeof(T3))}, mDataType).FieldValueToList();
        }

        public IList<object> Get<T, T1, T2, T3, T4>(RedisClient db = null)
        {
            db = RedisClient.GetClient(db);
            return db.GetFields((string)mDataKey, new NameType[] {
            new NameType(typeof(T))
            ,new NameType(typeof(T1))
            ,new NameType(typeof(T2))
            ,new NameType(typeof(T3)),
            new NameType(typeof(T4))}, mDataType).FieldValueToList();
        }

        public IList<object> Get(string[] fields,Type[] types,RedisClient db=null)
        {
            NameType[] nts = new NameType[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                nts[0] = new NameType(types[i], fields[i],i);
            }
            db = RedisClient.GetClient(db);
            return db.GetFields(mDataKey, nts, mDataType).FieldValueToList();

        }

        public void Set(string field, object value, RedisClient db = null)
        {
            Set(new Field { Name = field, Value = value }, db);
        }

        public void Set(Field field, RedisClient db = null)
        {
            Set(new Field[] { field }, db);
        }

        public void Set(params object[] values)
        {
            Set(values, null);
        }

        public void Set(object[] value, RedisClient db = null)
        {

            List<Field> fields = new List<Field>();
            foreach (object item in value)
            {
                fields.Add(new Field { Name = item.GetType().Name, Value = item });
            }
            Set(fields.ToArray(), db);
        }

        public void Set(IList<object> value, RedisClient db = null)
        {
            Set(value.ToArray(), db);
        }

        public void Set(params Field[] fields)
        {
            Set(fields, null);
        }

        public void Set(Field[] fields, RedisClient db = null)
        {
            db = RedisClient.GetClient(db);
            db.SetFields((string)mDataKey, fields, mDataType);
        }

        public void Set(string[] fields, object[] values, RedisClient db = null)
        {
            Field[] fds = new Field[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                fds[i] = new Field { Name= fields[i], Value= values[i] };
            }
            Set(fds, db);
        }

        public void Clear(RedisClient db = null)
        {
            db = RedisClient.GetClient(db);
            db.Delete(mDataKey);
        }

        public void Remove(string[] fields, RedisClient db = null)
        {
            db = RedisClient.GetClient(db);
            db.HDEL(mDataKey, fields);
        }
       
        
        public void Remove(params string[] fields)
        {
            Remove(fields, null);
        }

        public void Remove<T>(RedisClient db = null)
        {
          
            Remove(new string[] { typeof(T).Name},db);
        }

        public void Remove<T, T1>(RedisClient db = null)
        {
            Remove(new string[] { typeof(T).Name,typeof(T1).Name }, db);
        }

        public void Remove<T, T1, T2>(RedisClient db = null)
        {
            Remove(new string[] { typeof(T).Name, typeof(T1).Name,typeof(T2).Name }, db);
        }

        public void Remove<T, T1, T2, T3>(RedisClient db = null)
        {
            Remove(new string[] { typeof(T).Name, typeof(T1).Name, typeof(T2).Name,typeof(T3).Name }, db);
        }

        public void Remove<T, T1, T2, T3, T4>(RedisClient db = null)
        {
            Remove(new string[] { typeof(T).Name, typeof(T1).Name, typeof(T2).Name, typeof(T3).Name,typeof(T4).Name }, db);
        }

    }

    public class StringMapSet : MapSet
    {
        public StringMapSet(string key)
            : base(key, DataType.String)
        {
        }
        public static implicit operator StringMapSet(string key)
        {
            return new StringMapSet(key);
        }
    }

    public class JsonMapSet : MapSet
    {
        public JsonMapSet(string key)
            : base(key, DataType.Json)
        {
        }
        public static implicit operator JsonMapSet(string key)
        {
            return new JsonMapSet(key);
        }
    }

    public class ProtobufMapSet : MapSet
    {
        public ProtobufMapSet(string key)
            : base(key, DataType.Protobuf)
        {
        }
        public static implicit operator ProtobufMapSet(string key)
        {
            return new ProtobufMapSet(key);
        }
    }
}
