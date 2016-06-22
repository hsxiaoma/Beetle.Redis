using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Beetle.Redis;
namespace Beetle.Redis
{
    public static class BeetleRedisExtensionsMethod
    {


        public static string GetString(this ArraySegment<byte> data)
        {
            return utils.GetString(data);
        }

        public static T GetJson<T>(this ArraySegment<byte> data)
        {
            return (T)utils.GetJson(data, typeof(T));
        }

        public static T GetProtobuf<T>(this ArraySegment<byte> data)
        {
            return (T)utils.GetProtobuf(data, typeof(T));
        }

        public static object GetJson(this ArraySegment<byte> data, Type type)
        {
            return utils.GetJson(data, type);
        }

        public static object GetProtobuf(this ArraySegment<byte> data, Type type)
        {
            return utils.GetProtobuf(data, type);
        }
        public static IList<object> FieldValueToList(this IEnumerable<Field> items)
        {
            List<object> result = new List<object>();
            foreach (Field item in items)
            {
                result.Add(item.Value);
            }
            return result;
        }
    }

    public static class BeetleRedisGetExtensionsMethod
    {

        public static RedisKey String(this IEnumerable<string> key)
        {
            return new StringKey(key.ToArray());
        }
        public static RedisKey Json(this IEnumerable<string> key)
        {
            return new JsonKey(key.ToArray());
        }

        public static RedisKey Protobuf(this IEnumerable<string> key)
        {
            return new ProtobufKey(key.ToArray());
        }
        public static RedisKey String(this string key)
        {
            return new StringKey(key);
        }
        public static RedisKey Json(this string key)
        {
            return new JsonKey(key);
        }

        public static RedisKey Protobuf(this string key)
        {
            return new ProtobufKey(key);
        }
        #region get set protobuf
        public static void SetProtobuf(this string key, object value,RedisClient db=null)
        {
            new ProtobufKey(key).Set(value,db);
        }
        public static T GetProtobuf<T>(this string key, RedisClient db = null)
        {
            return new ProtobufKey(key).Get<T>(db);
        }

        public static void SetProtobuf(this IEnumerable<string> key, object[] value, RedisClient db = null)
        {
            new ProtobufKey(key.ToArray()).SetValues(value, db);
        }

      
        public static IList<object> GetProtobuf<T, T1>(this IEnumerable<string> key, RedisClient db = null)
        {
            return new ProtobufKey(key.ToArray()).Get(new Type[] { typeof(T), typeof(T1) }, db);
        }
        public static IList<object> GetProtobuf<T, T1, T2>(this IEnumerable<string> key, RedisClient db = null)
        {
            return new ProtobufKey(key.ToArray()).Get(new Type[] { typeof(T), typeof(T1), typeof(T2) }, db);
        }
        public static IList<object> GetProtobuf<T, T1, T2, T3>(this IEnumerable<string> key, RedisClient db = null)
        {
            return new ProtobufKey(key.ToArray()).Get(new Type[] { typeof(T), typeof(T1), typeof(T2), typeof(T3) }, db);
        }
        public static IList<object> GetProtobuf<T, T1, T2, T3, T4>(this IEnumerable<string> key, RedisClient db = null)
        {
            return new ProtobufKey(key.ToArray()).Get(new Type[] { typeof(T), typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, db);
        }
        #endregion


        #region get set string
        public static void SetString(this string key, string value, RedisClient db = null)
        {
            new StringKey(key).Set(value, db);
        }

        public static string GetString(this string key, RedisClient db = null)
        {
            return new StringKey(key).Get<string>(db);
        }
        public static void SetString(this IEnumerable<string> key, string[] value, RedisClient db = null)
        {
            new StringKey(key.ToArray()).SetValues(value, db);
        }

        public static IList<object> GetString(this IEnumerable<string> key, RedisClient db = null)
        {
            string[] keys = key.ToArray();
            Type[] types = new Type[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                types[i] = typeof(string);
            }
            return (new StringKey(keys).Get(types, db));
        }

       
        #endregion

        #region get set json
        public static void SetJson(this string key, object value, RedisClient db = null)
        {
            new JsonKey(key).Set(value, db);
        }

        public static T GetJson<T>(this string key, RedisClient db = null)
        {
            return new JsonKey(key).Get<T>(db);
        }

        public static void SetJson(this IEnumerable<string> key, object[] value, RedisClient db = null)
        {
            new JsonKey(key.ToArray()).SetValues(value, db);
        }

       

        public static IList<object> GetJson<T,T1>(this IEnumerable<string> key, RedisClient db = null)
        {
            return new JsonKey(key.ToArray()).Get(new Type[] { typeof(T),typeof(T1) }, db);
        }
        
        public static IList<object> GetJson<T, T1,T2>(this IEnumerable<string> key, RedisClient db = null)
        {
            return new JsonKey(key.ToArray()).Get(new Type[] { typeof(T), typeof(T1),typeof(T2) }, db);
        }
        
        public static IList<object> GetJson<T, T1, T2,T3>(this IEnumerable<string> key, RedisClient db = null)
        {
            return new JsonKey(key.ToArray()).Get(new Type[] { typeof(T), typeof(T1), typeof(T2),typeof(T3) }, db);
        }
        
        public static IList<object> GetJson<T, T1, T2, T3,T4>(this IEnumerable<string> key, RedisClient db = null)
        {
            return new JsonKey(key.ToArray()).Get(new Type[] { typeof(T), typeof(T1), typeof(T2), typeof(T3),typeof(T4) }, db);
        }
        #endregion
    }

   
}
