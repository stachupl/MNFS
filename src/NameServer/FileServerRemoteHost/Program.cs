using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using nameServerClass;

namespace FileServerRemoteHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Host uruchomiony");
            HttpChannel c = new HttpChannel(3300);
            ChannelServices.RegisterChannel(c, false);
            Type ServerType = typeof(nameServerClass.NameServer);
            RemotingConfiguration.RegisterWellKnownServiceType(
                ServerType,
                "Object",
                WellKnownObjectMode.Singleton);

            Console.Read();
        }
    }
}
