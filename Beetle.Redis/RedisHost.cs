using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beetle.Redis
{
    public class RedisHost
    {

        private System.Collections.Concurrent.ConcurrentStack<TcpClient> mClients = new System.Collections.Concurrent.ConcurrentStack<TcpClient>();

        private bool Detecting = false;

        public RedisHost(string host,int count)
        {
           Port = 6379;
           Available = true;
           string[] ps = host.Split(':');
           Host = ps[0];
           if (ps.Length > 1)
               Port = int.Parse(ps[1]);
           for (int i = 0; i < count; i++)
           {
               Push(CreateClient());
           }
           
        }

        private TcpClient CreateClient()
        {
            return new TcpClient(Host, Port);
        }

        public IList<string> Info()
        {
            IList<string> result = new List<string>();
            using (ClientItem c = Pop())
            {
                using (Command cmd = new Command())
                {
                    cmd.Add(CONST_VALURES.REDIS_COMMAND_INFO);
                    using (Result r = TcpClient.Send(cmd, c.Client))
                    {
                        foreach (ArraySegment<byte> item in r.ResultDataBlock)
                        {
                            result.Add(item.GetString());
                        }
                    }
                }
            }
            return result;
        }

        private void OnDetect(object state)
        {
           
            try
            {
                TcpClient client = CreateClient();
                using (Command cmd = new Command())
                {
                    cmd.Add(CONST_VALURES.REDIS_COMMAND_PING);
                    using (Result result =TcpClient.Send(cmd,client))
                    {

                    }
                }
                Push(client);
                Available = true;
            }
            catch (Exception e_)
            {
                LastError = e_;
                Available = false;
            }
            Detecting = false;
        }

        public void Detect()
        {
            lock (this)
            {
                if (!Available && !Detecting && (Math.Abs(Environment.TickCount) - LastCheckTime) > 10000)
                {
                    Detecting = true;
                    LastCheckTime = Math.Abs(Environment.TickCount);
                    System.Threading.ThreadPool.QueueUserWorkItem(OnDetect, null);
                }
            }
            
        }

        public void Push(TcpClient client)
        {
            if (client != null)
            {
                if (client.Connected)
                {
                    mClients.Push(client);
                }
                else
                {
                    Detect();
                }
            }
        }

        internal ClientItem Pop()
        {
            TcpClient client = null;
            if (mClients.TryPop(out client))
            {
                return new ClientItem(this, client);
            }
            return new ClientItem(this, CreateClient());

        }

        public string Host
        {
            get;
            set;
        }

        public int Port
        {
            get;
            set;
        }

        public bool Available
        {
            get;
            set;
        }

        public int LastCheckTime
        {
            get;
            set;
        }

        public Exception LastError
        {
            get;
            set;
        }

        public class ClientItem : IDisposable
        {
            public ClientItem(RedisHost host, TcpClient client)
            {
                mHost = host;
                mClient = client;
            }

            private RedisHost mHost;

            private TcpClient mClient;

            public TcpClient Client
            {
                get
                {
                    return mClient;
                }
            }

            public void Dispose()
            {
                mHost.Push(mClient);
                mClient = null;
                mHost = null;
            }
        }
    }
}
