using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Beetle.Redis
{
    public class Command : IDisposable
    {
        public Command()
        {
            mData = BufferPool.Single.Pop();
            mStream = new MemoryStream(mData);
        }

        private System.IO.Stream mStream;

        private byte[] mData;

        public int Count = 0;

        [ThreadStatic]
        private static StringEncoding mEncodeHandler = null;

        internal static StringEncoding EncodeHandler
        {
            get
            {
                if (mEncodeHandler == null)
                {
                    mEncodeHandler = new StringEncoding(1024*64);

                }
                return mEncodeHandler;
            }
        }

        public void Add(string data)
        {
            int count = EncodeHandler.Encode(data);
            Add(EncodeHandler.Buffer, 0, count);
        }

        public void Add(byte[] data)
        {
            Add(data, 0, data.Length);
        }

        public void Add(byte[] data, int offset, int count)
        {
            string header = "$" + count;
            byte[] headerdata = Encoding.ASCII.GetBytes(header);
            mStream.Write(headerdata, 0, headerdata.Length);
            mStream.Write(utils.Eof, 0, 2);
            mStream.Write(data, offset, count);
            mStream.Write(utils.Eof, 0, 2);
            Count++;
        }

        public int toData(byte[] result)
        {
            try
            {
                string header = string.Format("*{0}\r\n", Count);
                byte[] headerdata = Encoding.ASCII.GetBytes(header);
                Buffer.BlockCopy(headerdata, 0, result, 0, headerdata.Length);
                int length = (int)mStream.Position;
                mStream.Position = 0;
                mStream.Read(result, headerdata.Length, length);
                return headerdata.Length + length;
            }
            catch (Exception e_)
            {
                throw new Exception("buffer overflow buffer max size:" + BufferPool.DEFAULT_BUFFERLENGTH, e_);
            }
        }

        public void AddJson(object obj)
        {
            Add(Newtonsoft.Json.JsonConvert.SerializeObject(obj));
        }

        public void AddProtobuf(object obj)
        {
           byte[] data = BufferPool.Single.Pop();
            try
            {
                MemoryStream stream = new MemoryStream(data);
                ProtoBuf.Meta.RuntimeTypeModel.Default.Serialize(stream, obj);
                Add(data, 0, (int)stream.Position);
            }
            finally
            {
                BufferPool.Single.Push(data);
            }
        }

        public void Dispose()
        {
            lock (this)
            {
                if (mData != null)
                    BufferPool.Single.Push(mData);
                mData = null;
            }
        }


    }
}
