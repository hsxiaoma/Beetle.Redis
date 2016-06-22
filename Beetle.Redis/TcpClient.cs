using System;
using System.Collections.Generic;
//********************************************************
// 	Copyright © henryfan 2013		 
//	Email:		henryfan@msn.com	
//	HomePage:	http://www.ikende.com		
//	CreateTime:	2013/6/15 14:55:07
//********************************************************	 
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace Beetle.Redis
{
    public class TcpClient
    {

        public TcpClient(string host, int port)
        {
            mSAEA.SetBuffer(new byte[1024 * 2], 0, 1024 * 2);
            mHost = host;
            mPort = port;
            DB = 0;
        }

        private string mHost;

        private int mPort;

        private bool mConnected = false;

        private Socket mSocket;

        private Exception mLastError;

        private SocketAsyncEventArgs mSAEA = new SocketAsyncEventArgs();

        public void DisConnect()
        {
            mConnected = false;
            try
            {
                if (mSocket != null)
                {
                    mSocket.Shutdown(SocketShutdown.Both);

                }
            }
            catch
            {
            }
            try
            {
                if (mSocket != null)
                {
                    mSocket.Close();

                }
            }
            catch
            {
            }
            mSocket = null;
        }

        public Exception LastError
        {
            get
            {
                return mLastError;
            }
        }

        public Socket Socket
        {
            get
            {
                return mSocket;
            }

        }

        public bool Connected
        {
            get
            {
                return mConnected;
            }
        }

        public bool Connect()
        {
            if (mConnected)
                return true;
            IPAddress[] ips = Dns.GetHostAddresses(mHost);
            if (ips.Length == 0)
                throw new Exception("get host's IPAddress error");

            var address = ips[0];

            try
            {
                mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                mSocket.ReceiveTimeout = 5000;
                mSocket.SendTimeout = 5000;
                mSocket.Connect(address, mPort);
                mConnected = true;
                return true;
            }
            catch (Exception e_)
            {
                DisConnect();
                mLastError = e_;
                return false;
            }


        }
        public TcpReceiveArgs Receive(int count)
        {
            try
            {
                int rcount = Socket.Receive(mSAEA.Buffer, count, SocketFlags.None);
                if (rcount == 0)
                    throw new Exception(string.Format("{0} client disconnect!", mHost));
                return new TcpReceiveArgs() { Client = this, Count = rcount, Data = mSAEA.Buffer, Offset = 0 };

            }
            catch (Exception e_)
            {
                DisConnect();
                mLastError = e_;
                throw e_;
            }
        }
        public TcpReceiveArgs Receive()
        {
            try
            {
                int count = Socket.Receive(mSAEA.Buffer, SocketFlags.None);
                if (count == 0)
                    throw new Exception(string.Format("{0} client disconnect!", mHost)); 
                return new TcpReceiveArgs() { Client = this, Count = count, Data = mSAEA.Buffer, Offset = 0 };

            }
            catch (Exception e_)
            {
                DisConnect();
                mLastError = e_;
                throw e_;
            }
        }

        public bool Send(string value)
        {
            return Send(value, Encoding.UTF8);
        }

        public bool Send(string value, Encoding coding)
        {
            return Send(coding.GetBytes(value));
        }

        public bool Send(byte[] data)
        {
            return Send(data, 0, data.Length);
        }

        public bool Send(byte[] data, int offset, int count)
        {
            if (Connect())
            {
                try
                {

                    while (count > 0)
                    {
                        int sends = mSocket.Send(data, offset, count, SocketFlags.None);
                        count -= sends;
                        offset += sends;
                    }
                    return true;
                }
                catch (Exception e_)
                {
                    DisConnect();
                    mLastError = e_;
                    return false;
                }
            }
            return false;

        }

        public bool Send(ArraySegment<byte> data)
        {
            return Send(data.Array, data.Offset, data.Count);

        }

        public int DB
        {
            get;
            set;
        }

        internal static Result Send(Command cmd, TcpClient client)
        {
            byte[] sdata = BufferPool.Single.Pop();
            try
            {
                int count = cmd.toData(sdata);
                if (!client.Send(sdata, 0, count))
                {
                    throw new Exception(string.Format("{0} client disconnect!", client.mHost));
                }
            }
            catch (Exception e_)
            {
                client.mLastError = e_;
              
                throw new Exception(string.Format("send to {0} error!", client.mHost), e_);
            }
            finally
            {
                BufferPool.Single.Push(sdata);
            }
            Result result = new Result();

            try
            {
                while (true)
                {
                    TcpReceiveArgs res = client.Receive();
                    if (result.Import(res.Data, res.Offset, res.Count))
                    {
                        break;
                    }
                }
                if (result.mImportOffset > 0)
                {
                    client.Receive(result.mImportOffset);
                    
                }
            }
            catch (Exception e_)
            {
                result.Dispose();
                client.mLastError = e_;
                throw new Exception(string.Format("receive {0} error!", client.mHost), e_);
            }
            return result;
        }

    }
}
