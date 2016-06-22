using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beetle.Redis
{
    class utils
    {
        public static byte[] Eof = Encoding.ASCII.GetBytes("\r\n");

        public static string GetString(ArraySegment<byte> data)
        {
            if (data.Count > 0)
                return Encoding.UTF8.GetString(data.Array, data.Offset, data.Count);
            return null;
        }

        public static object GetJson(ArraySegment<byte> data, Type type)
        {
            string value = GetString(data);
            if (string.IsNullOrEmpty(value))
                return null;
            return Newtonsoft.Json.JsonConvert.DeserializeObject(value, type);
           
        }

        public static object GetProtobuf(ArraySegment<byte> data, Type type)
        {
            if (data.Count > 0)
            {
                System.IO.MemoryStream stream = new System.IO.MemoryStream(data.Array, data.Offset, data.Count);
                stream.Position = 0;
                return ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(stream, null, type);
            }

            return null;
        }
    }

    
}
