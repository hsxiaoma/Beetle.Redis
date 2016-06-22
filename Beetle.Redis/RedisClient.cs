using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beetle.Redis
{
    public class RedisClient
    {
        public RedisClient(string section)
        {
            RedisClientSection rclient = (RedisClientSection)System.Configuration.ConfigurationManager.GetSection(section);
            Init(rclient);
        }

        public RedisClient(RedisClientSection section)
        {
            Init(section);
        }

        private void Init(RedisClientSection section)
        {
            if (!string.IsNullOrEmpty(section.Cached))
                mCached = new KCached.CacheManager(section.Cached);
            foreach (HostItem item in section.Writes)
            {
                Writes.Add(new RedisHost(item.Host, item.Connections));
            }
            foreach (HostItem item in section.Reads)
            {
                Reads.Add(new RedisHost(item.Host, item.Connections));
            }
            DB = section.DB;
        }

        static RedisClient()
        {
            mDefaultDB = new RedisClient(RedisClientSection.Instance);
        }

        public static RedisClient GetClient(RedisClient db)
        {
            if (db == null)
                return DefaultDB;
            return db;
        }

        private static RedisClient mDefaultDB;

        public static RedisClient DefaultDB
        {
            get
            {
                return mDefaultDB;
            }
        }

        private int mReadHostIndex = 0;

        private int mWriteHostIndex = 0;

        public RedisHost.ClientItem GetWriter()
        {
            RedisHost host;
            RedisHost.ClientItem client;
            for (int i = 0; i < Writes.Count; i++)
            {
                host = Writes[mWriteHostIndex % Writes.Count];
                if (host.Available)
                {
                    client = host.Pop();
                    SelectDB(client.Client);
                    return client;
                }
                else
                {
                    host.Detect();
                }
                mWriteHostIndex++;
            }
            throw new Exception("write host not Available!");
        }

        public RedisHost.ClientItem GetReader()
        {
            RedisHost host;
            RedisHost.ClientItem client;
            for (int i = 0; i < Writes.Count; i++)
            {
                host = Reads[mReadHostIndex % Writes.Count];
                if (host.Available)
                {

                    client = host.Pop();
                    SelectDB(client.Client);
                    return client;
                }
                else
                {
                    host.Detect();
                }
                mReadHostIndex++;
            }
            throw new Exception("read host not Available!");
        }

        private IList<RedisHost> Reads = new List<RedisHost>();

        public IList<RedisHost> ReadHosts
        {
            get
            {
                return Reads;
            }
        }

        public IList<RedisHost> WriteHosts
        {
            get
            {
                return Writes;
            }
        }

        private IList<RedisHost> Writes = new List<RedisHost>();

        private void SelectDB(TcpClient client)
        {
            if (client.DB != DB)
            {
                client.DB = DB;
                using (Command cmd = new Command())
                {
                    cmd.Add(CONST_VALURES.REDIS_COMMAND_SELECT);
                    cmd.Add(DB.ToString());
                    using (Result result = TcpClient.Send(cmd, client))
                    {
                    }
                }
            }
        }

        public int DB
        {
            get;
            set;
        }

        private KCached.CacheManager mCached = null;

        public object GetByCache(string key)
        {
            if (mCached != null)
                return mCached.Get(key);
            return null;
        }

        public void DelCache(string key)
        {
            if (mCached != null)
                mCached.Delete(key);
        }

        public void SetCache(string key, object value)
        {
            if (value != null && mCached != null)
                mCached.Set(key, value);
        }

        public int Delete(params string[] keys)
        {
            if (mCached != null)
                mCached.Delete(keys);
            using (RedisHost.ClientItem c = GetWriter())
            {
                using (Command cmd = new Command())
                {
                    cmd.Add(CONST_VALURES.REDIS_COMMAND_DEL);
                    foreach (string key in keys)
                    {
                        cmd.Add(key);
                    }
                    using (Result result = TcpClient.Send(cmd, c.Client))
                    {
                        return int.Parse(result.ResultData.ToString());
                    }
                }
            }
        }

        public long TTL(string key)
        {
            using (RedisHost.ClientItem c = GetReader())
            {
                using (Command cmd = new Command())
                {
                    cmd.Add(CONST_VALURES.REDIS_COMMAND_TTL);
                    cmd.Add(key);
                   
                    using (Result result = TcpClient.Send(cmd, c.Client))
                    {
                        return long.Parse(result.ResultData.ToString());
                    }
                }
            }
        }

        public int EXPIRE(string key, long seconds)
        {
            using (RedisHost.ClientItem c = GetWriter())
            {
                using (Command cmd = new Command())
                {
                    cmd.Add(CONST_VALURES.REDIS_COMMAND_EXPIRE);
                    cmd.Add(key);
                    cmd.Add(seconds.ToString());
                    using (Result result = TcpClient.Send(cmd, c.Client))
                    {
                        return int.Parse(result.ResultData.ToString());
                    }
                }
            }
        }

        public long PTTL(string key)
        {
            using (RedisHost.ClientItem c = GetReader())
            {
                using (Command cmd = new Command())
                {
                    cmd.Add(CONST_VALURES.REDIS_COMMAND_PTTL);
                    cmd.Add(key);
                   
                    using (Result result = TcpClient.Send(cmd, c.Client))
                    {
                        return long.Parse(result.ResultData.ToString());
                    }
                }
            }
        }

        public int PEXPIRE(string key, long milliseconds)
        {
            using (RedisHost.ClientItem c = GetWriter())
            {
                using (Command cmd = new Command())
                {
                    cmd.Add(CONST_VALURES.REDIS_COMMAND_PEXPIRE);
                    cmd.Add(key);
                    cmd.Add(milliseconds.ToString());
                    using (Result result = TcpClient.Send(cmd, c.Client))
                    {
                        return int.Parse(result.ResultData.ToString());
                    }
                }
            }
        }

        public void HDEL(string key, params string[] fields)
        {
            
            using (RedisHost.ClientItem c = GetWriter())
            {
                using (Command cmd = new Command())
                {
                    cmd.Add(CONST_VALURES.REDIS_COMMAND_HDEL);
                    cmd.Add(key);
                    foreach (string field in fields)
                    {
                        DelCache(key + "_field_" + field);
                        cmd.Add(field);
                    }
                    using (Result result = TcpClient.Send(cmd, c.Client))
                    {
                        
                    }
                }
            }
        }

        public void Delete(IList<string> keys)
        {
            Delete(keys.ToArray());
        }

        public IList<string> Keys(string match)
        {
            IList<string> r = null;
            using (RedisHost.ClientItem c = GetReader())
            {
                using (Command cmd = new Command())
                {
                    cmd.Add(CONST_VALURES.REDIS_COMMAND_KEYS);

                    cmd.Add(match);

                    using (Result result = TcpClient.Send(cmd, c.Client))
                    {
                        r = new List<string>(result.ResultDataBlock.Count);
                        foreach (ArraySegment<byte> i in result.ResultDataBlock)
                        {
                            r.Add(i.GetString());
                        }
                    }
                }
            }
            return r;
        }

        public int ListLength(string key)
        {
            using (RedisHost.ClientItem c = GetReader())
            {
                using (Command cmd = new Command())
                {
                    cmd.Add(CONST_VALURES.REDIS_COMMAND_LLEN);
                    cmd.Add(key);

                    using (Result result = TcpClient.Send(cmd, c.Client))
                    {
                        return int.Parse(result.ResultData.ToString());
                    }
                }
            }

        }

        public T ListPop<T>(string key,DataType dtype)
        {
            using (RedisHost.ClientItem c = GetWriter())
            {
                using (Command cmd = new Command())
                {
                    cmd.Add(CONST_VALURES.REDIS_COMMAND_LPOP);
                    cmd.Add(key);
                    using (Result result = TcpClient.Send(cmd, c.Client))
                    {
                        return (T)FromRedis(result.ResultDataBlock[0], dtype, typeof(T));
                    }
                }
            }
        }

        public T ListRemove<T>(string key,DataType dtype)
        {
            using (RedisHost.ClientItem c = GetWriter())
            {
                using (Command cmd = new Command())
                {
                    cmd.Add(CONST_VALURES.REDIS_COMMAND_RPOP);
                    cmd.Add(key);
                    using (Result result = TcpClient.Send(cmd, c.Client))
                    {
                        return (T)FromRedis(result.ResultDataBlock[0], dtype, typeof(T));
                    }
                }
            }
        }

        public void ListPush(string key, object value,DataType dtype)
        {
            using (RedisHost.ClientItem c = GetWriter())
            {
                using (Command cmd = new Command())
                {
                    cmd.Add(CONST_VALURES.REDIS_COMMAND_LPUSH);
                    cmd.Add(key);
                    ToRedis(value, dtype, cmd);
                    using (Result result = TcpClient.Send(cmd, c.Client))
                    {

                    }
                }
            }
        }

        public void SetListItem(string key, int index, object value,DataType dtype)
        {
            using (RedisHost.ClientItem c = GetWriter())
            {
                using (Command cmd = new Command())
                {
                    cmd.Add(CONST_VALURES.REDIS_COMMAND_LSET);
                    cmd.Add(key);
                    cmd.Add(index.ToString());
                    ToRedis(value, dtype, cmd);
                    using (Result result = TcpClient.Send(cmd, c.Client))
                    {

                    }
                }
            }
        }

        public void ListAdd(string key, object value,DataType dtype)
        {
            using (RedisHost.ClientItem c = GetWriter())
            {
                using (Command cmd = new Command())
                {
                    cmd.Add(CONST_VALURES.REDIS_COMMAND_RPUSH);
                    cmd.Add(key);
                    ToRedis(value,dtype,cmd);
                    using (Result result = TcpClient.Send(cmd, c.Client))
                    {

                    }
                }
            }
        }

        public IList<T> ListRange<T>(string key, int start, int end,DataType dtype)
        {
            string cachekey = string.Format("{0}_list_{1}_{2}", key, start, end);
            IList<T> lst = null;
            lst = (IList<T>)GetByCache(cachekey);
            using (RedisHost.ClientItem c = GetReader())
            {
                lst = new List<T>();
                using (Command cmd = new Command())
                {
                    cmd.Add(CONST_VALURES.REDIS_COMMAND_LRANGE);
                    cmd.Add(key);
                    cmd.Add(start.ToString());
                    cmd.Add(end.ToString());
                    using (Result result = TcpClient.Send(cmd, c.Client))
                    {
                        foreach (ArraySegment<byte> item in result.ResultDataBlock)
                        {
                            lst.Add((T)FromRedis(item, dtype, typeof(T)));
                        }
                    }
                }
                SetCache(cachekey, lst);
                return lst;
            }
        }

        public IList<T> ListRange<T>(string key,DataType dtype)
        {
            return ListRange<T>(key, 0, -1,dtype);
        }

        public T GetListItem<T>( string key, int index,DataType dtype)
        {
            using (RedisHost.ClientItem c = GetReader())
            {
                using (Command cmd = new Command())
                {
                    cmd.Add(CONST_VALURES.REDIS_COMMAND_LINDEX);
                    cmd.Add(key);
                    cmd.Add(index.ToString());
                    using (Result result = TcpClient.Send(cmd, c.Client))
                    {
                        return (T)FromRedis(result.ResultDataBlock[0],dtype,typeof(T));
                    }
                }
            }
        }

        public IList<Field> GetFields(string key, params string[] fields)
        {
            List<NameType> nts = new List<NameType>();
            foreach (string field in fields)
            {
                nts.Add(new NameType(typeof(String), field, 0));
            }
            return GetFields(key, nts.ToArray(), DataType.String);
        }

        public IList<Field> GetFields(string key, NameType[] fields, DataType type)
        {
            List<Field> result = GetResultSpace<Field>(fields.Length);
            object value;
            NameType item;
            using (RedisHost.ClientItem c = GetReader())
            {
                List<NameType> inputs = new List<NameType>();
                using (Command cmd = new Command())
                {
                    cmd.Add(CONST_VALURES.REDIS_COMMAND_HMGET);
                    cmd.Add(key);
                    for (int i = 0; i < fields.Length; i++)
                    {
                        item = fields[i];
                        value = GetByCache(key + "_field_" + item.Name);
                        if (value != null)
                        {
                            result[i] = new Field { Name = item.Name, Value = value };
                        }
                        else
                        {
                            inputs.Add(item);
                            item.Index = i;
                            cmd.Add(item.Name);
                        }
                    }
                    using (Result R = TcpClient.Send(cmd, c.Client))
                    {
                        for (int i = 0; i < inputs.Count; i++)
                        {
                            item = inputs[i];

                            value = FromRedis(R.ResultDataBlock[i], type, item.Type);
                            SetCache(key + "_field_" + item.Name, value);
                            result[item.Index] = new Field { Name = item.Name, Value = value };
                        }
                    }
                }
            }
            return result;
        }

        public IList<object> Get<T, T1>(string key, string key1, DataType type)
        {
            return Get(new Type[] { typeof(T), typeof(T1) }, new string[] { key, key1 }, type);
        }

        public IList<object> Get<T, T1, T2>(string key, string key1, string key2, DataType type)
        {
            return Get(new Type[] { typeof(T), typeof(T1), typeof(T2) }, new string[] { key, key1, key2 }, type);
        }

        public IList<object> Get<T, T1, T2, T3>(string key, string key1, string key2, string key3, DataType type)
        {
            return Get(new Type[] { typeof(T), typeof(T1), typeof(T2), typeof(T3) }, new string[] { key, key1, key2, key3 }, type);
        }

        public IList<object> Get<T, T1, T2, T3, T4>(string key, string key1, string key2, string key3, string key4, DataType type)
        {
            return Get(new Type[] { typeof(T), typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, new string[] { key, key1, key2, key3, key4 }, type);
        }

        public IList<object> Get(Type[] types, string[] keys, DataType dtype)
        {
            using (RedisHost.ClientItem c = GetReader())
            {
                List<object> result = GetResultSpace<object>(keys.Length);
                List<NameType> _types = new List<NameType>();
                using (Command cmd = new Command())
                {
                    cmd.Add(CONST_VALURES.REDIS_COMMAND_MGET);
                    for (int i = 0; i < keys.Length; i++)
                    {
                        string key = keys[i];
                        object data = GetByCache(key);
                        if (data != null)
                        {
                            result[i] = data;
                        }
                        else
                        {
                            cmd.Add(key);
                            _types.Add(new NameType(types[i], keys[i], i));
                        }
                    }
                    using (Result r = TcpClient.Send(cmd, c.Client))
                    {
                        for (int i = 0; i < _types.Count; i++)
                        {
                            object item = FromRedis(r.ResultDataBlock[i], dtype, _types[i].Type);
                            SetCache(_types[i].Name, item);
                            result[_types[i].Index] = item;
                        }
                    }
                }
                return result;
            }
        }

        private List<T> GetResultSpace<T>(int count)
        {
            List<T> result = new List<T>(count);
            for (int i = 0; i < count; i++)
            {
                result.Add(default(T));
            }
            return result;
        }

        public string Get(string key)
        {
            return Get<string>(key, DataType.String);
        }

        public T Get<T>(string key, DataType type)
        {
            T value = (T)GetByCache(key);
            if (value != null)
                return value;
            using (RedisHost.ClientItem c = GetReader())
            {
                using (Command cmd = new Command())
                {
                    cmd.Add(CONST_VALURES.REDIS_COMMAND_GET);
                    cmd.Add(key);
                    using (Result result = TcpClient.Send(cmd, c.Client))
                    {
                        if (result.ResultDataBlock.Count > 0)
                        {
                            value = (T)FromRedis(result.ResultDataBlock[0], type, typeof(T));
                            SetCache(key, value);
                            return value;
                        }
                    }

                }

                return default(T);
            }
        }

        public void SetFields(string key, IEnumerable<Field> items, DataType type)
        {
            using (RedisHost.ClientItem c = GetWriter())
            {
                using (Command cmd = new Command())
                {
                    cmd.Add(CONST_VALURES.REDIS_COMMAND_HMSET);
                    cmd.Add(key);
                    foreach (Field item in items)
                    {
                        cmd.Add(item.Name);
                        ToRedis(item.Value, type, cmd);
                    }
                    using (Result result = TcpClient.Send(cmd, c.Client))
                    {

                    }
                }
            }
        }

        public void Set(Field[] KValues, DataType dtype)
        {
            using (RedisHost.ClientItem c = GetWriter())
            {
                using (Command cmd = new Command())
                {
                    cmd.Add(CONST_VALURES.REDIS_COMMAND_MSET);
                    foreach (Field item in KValues)
                    {
                        cmd.Add(item.Name);
                        ToRedis(item.Value, dtype, cmd);
                    }
                    using (Result result = TcpClient.Send(cmd, c.Client))
                    {
                    }
                }
            }
        }

        public void Set(string key, object value, DataType type)
        {
            using (RedisHost.ClientItem c = GetWriter())
            {
                using (Command cmd = new Command())
                {
                    cmd.Add(CONST_VALURES.REDIS_COMMAND_SET);
                    cmd.Add(key);

                    ToRedis(value, type, cmd);
                    using (Result result = TcpClient.Send(cmd, c.Client))
                    {
                    }
                }
            }
        }


        public IList<object> Sort(string key, int? offset, int? count, string BYpattern, string GETpattern, bool ALPHA, string STOREdestination,
            SortOrderType orderby,Type type,DataType dtype)
        {
            List<object> result = new List<object>();
            using (RedisHost.ClientItem c = GetReader())
            {
                using (Command cmd = new Command())
                {
                    cmd.Add(CONST_VALURES.REDIS_COMMAND_SORT);
                    cmd.Add(key);
                    if (!string.IsNullOrEmpty(BYpattern))
                    {
                        cmd.Add("BY");
                        cmd.Add(BYpattern);
                    }
                    if (!string.IsNullOrEmpty(GETpattern))
                    {
                        cmd.Add("GET");
                        cmd.Add(GETpattern);
                    }
                    if (offset != null)
                    {
                        cmd.Add("LIMIT");
                        cmd.Add(offset.Value.ToString());
                        cmd.Add(count ==null?"1000":count.Value.ToString());
                    }
                    if (ALPHA)
                    {
                        cmd.Add("ALPHA");
                    }
                    cmd.Add(Enum.GetName(typeof(SortOrderType), orderby));
                    if (!string.IsNullOrEmpty(STOREdestination))
                    {
                        cmd.Add("STORE");
                        cmd.Add(STOREdestination);
                    }

                    using (Result rd = TcpClient.Send(cmd, c.Client))
                    {
                        foreach (ArraySegment<byte> item in rd.ResultDataBlock)
                        {
                            result.Add(FromRedis(item, dtype, type));
                        }
                    }
                }
                
            }
            return result;
        }

        private void ToRedis(object value, DataType type, Command cmd)
        {
            if (type == DataType.String)
            {
                cmd.Add((string)value);
            }
            else if (type == DataType.Json)
            {
                cmd.AddJson(value);
            }
            else
            {
                cmd.AddProtobuf(value);
            }
        }

        private object FromRedis(ArraySegment<byte> data, DataType type, Type otype)
        {
            if (type == DataType.String)
            {
                return data.GetString();
            }
            else if (type == DataType.Json)
            {
                return data.GetJson(otype);
            }
            else
            {
                try
                {
                    return data.GetProtobuf(otype);
                }
                catch (Exception e_)
                {
                    throw new Exception(string.Format("{0} type get error!", otype), e_);
                }
            }
        }
    }
}
