using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using Org.BouncyCastle.Crypto;

namespace Craft.Net.Server
{
    public class MinecraftClient
    {
        private const int BufferSize = 1024;
        
        #region Fields
        
        public Socket Socket;
        public string Username, Hostname;
        public Queue<Packet> SendQueue;
        public bool IsDisconnected;
        public bool IsLoggedIn;

        internal BufferedBlockCipher Decrypter;
        internal int RecieveBufferIndex;
        internal byte[] RecieveBuffer;
        internal string AuthenticationHash;
        internal bool EncryptionEnabled;
        
        #endregion
        
        public MinecraftClient(Socket Socket)
        {
            this.Socket = Socket;
            this.RecieveBuffer = new byte[1024];
            this.RecieveBufferIndex = 0;
            this.SendQueue = new Queue<Packet>();
            this.IsDisconnected = false;
            this.IsLoggedIn = false;
            this.EncryptionEnabled = false;
        }

        public void SendPacket(Packet packet)
        {
            this.SendQueue.Enqueue(packet);
        }
    }
}

