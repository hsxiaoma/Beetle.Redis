using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beetle.Redis
{
    public abstract class RedisList<T>
    {
        public RedisList(string key, DataType dataType)
        {
            mDataType = dataType;
            mDataKey = key;
        }

        private string mDataKey;

        private DataType mDataType;

        #region lst

        public int Count(RedisClient db = null)
        {
            db = RedisClient.GetClient(db);
            return db.ListLength((string)mDataKey);
        }

        public T Pop(RedisClient db = null)
        {
            db = RedisClient.GetClient(db);
            return db.ListPop<T>((string)mDataKey, mDataType);
        }

        public T Remove(RedisClient db = null)
        {
            db = RedisClient.GetClient(db);
            return db.ListRemove<T>((string)mDataKey, mDataType);
        }

        public void Add(T value, RedisClient db = null)
        {
            db = RedisClient.GetClient(db);
            db.ListAdd((string)mDataKey, value, mDataType);
        }

        public void Push(T value, RedisClient db = null)
        {
            db = RedisClient.GetClient(db);
            db.ListPush((string)mDataKey, value, mDataType);
        }

        public IList<T> Range(int start, int end, RedisClient db = null)
        {
            db = RedisClient.GetClient(db);
            return db.ListRange<T>((string)mDataKey, start, end, mDataType);
        }

        public IList<T> Range(RedisClient db = null)
        {
            db = RedisClient.GetClient(db);
            return db.ListRange<T>((string)mDataKey, mDataType);
        }

        public T GetItem(int index, RedisClient db = null)
        {
            db = RedisClient.GetClient(db);
            return db.GetListItem<T>((string)mDataKey, index, mDataType);
        }

        public void SetItem(int index, object value, RedisClient db = null)
        {
            db = RedisClient.GetClient(db);
            db.SetListItem((string)mDataKey, index, value, mDataType);
        }

        public T this[int index,RedisClient db=null]
        {
            get
            {
                return GetItem(index,db);
            }
            set
            {
                SetItem(index, value, db);
            }
        }

        public void Clear(RedisClient db = null)
        {
            db = RedisClient.GetClient(db);
            db.Delete(mDataKey);

        }

        #endregion

    }
    public class StringList : RedisList<string>
    {
        public StringList(string key)
            : base(key, DataType.String)
        {
        }
        public static implicit operator StringList(string key)
        {
            return new StringList(key);
        }
    }

    public class JsonList<T> : RedisList<T>
    {
        public JsonList(string key):base(key, DataType.Json)
        {
        }
        public static implicit operator JsonList<T>(string key)
        {
            return new JsonList<T>(key);
        }
    }

    public class ProtobufList<T> : RedisList<T>
    {
        public ProtobufList(string key)
            : base(key, DataType.Protobuf)
        {
        }
        public static implicit operator ProtobufList<T>(string key)
        {
            return new ProtobufList<T>(key);
        }
    }
}
