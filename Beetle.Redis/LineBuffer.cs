using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beetle.Redis
{
    public class LineBuffer
    {
        private byte[] mBuffer = new byte[256];

        private int mIndex = 0;

        private int mCount = 0;

        private static System.Collections.Concurrent.ConcurrentStack<LineBuffer> LineBUfferPool = new System.Collections.Concurrent.ConcurrentStack<LineBuffer>();

        public static LineBuffer Pop()
        {
            LineBuffer buffer = null;
            if (!LineBUfferPool.TryPop(out buffer))
                return new LineBuffer();
            buffer.Reset();
            return buffer;
        }
        public static void Push(LineBuffer value)
        {
            LineBUfferPool.Push(value);
        }

        public bool Import(byte[] data, ref int offset, ref  int count)
        {
            while (count > 0)
            {
                mBuffer[mIndex] = data[offset];
                mCount++;
                offset++;
                count--;
                if (mCount > 2)
                {
                    if (mBuffer[mIndex] == utils.Eof[1] && mBuffer[mIndex - 1] == utils.Eof[0])
                        return true;
                }
                mIndex++;
            }
            return false;
        }

        public string GetLineString()
        {
            return Encoding.UTF8.GetString(mBuffer, 0, mCount);
        }

        public void Reset()
        {
            mIndex = 0;
            mCount = 0;
        }
    }
}
