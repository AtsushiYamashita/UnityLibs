﻿using System;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

namespace BasicExtends {
    public interface IReceiver {
        bool StartServer (  );
        void StopServer ();
        void Close ();
    }

    public interface ISender {
        void Setup ( string ip_r );
        void Close ();
    }

    public enum ClientType {
        Sender, Receiver
    }

    public interface IConnectionData {
        IPEndPoint Receiver { get; }
        IPEndPoint Sender { get; }
        UdpClient Client { get; }
        ThreadsafeCounter Counter { get; }

        bool IsReceiver { get; }
        bool IsConnected { set; get; }

        LifedThread ConnectThread { set; get; }

        void Setup ( LifedThread connector, ClientType type, string ip_r = "" );
        void ClientLifeCheck ();
        void Disconnect ();
    }

    public static class NetworkUnit {
        public const int KB = 1000, MB = 1000 * KB;
        public const int INTERVAL = 2;
        public const int DEFAULT_PORT_S = 8000;
        public const int DEFAULT_PORT_R = 8001;


        public static string GetLocalIPAddress () {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList) {
                if (ip.AddressFamily != AddressFamily.InterNetwork) {
                    continue;
                }
                Debug.Log("ip.ToString()=>" + ip.ToString());
                return ip.ToString();
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }

}
