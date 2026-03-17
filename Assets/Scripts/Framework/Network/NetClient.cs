using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UIElements;

public class NetClient
{
    private TcpClient _Client;
    private NetworkStream _TcpStream;

    private const int BufferSize = 1024 * 64;
    private byte[] _Buffer = new byte[BufferSize];

    private MemoryStream _MemStream;
    private BinaryReader _BinaryReader;

    public NetClient()
    {
        _MemStream = new MemoryStream();
        _BinaryReader = new BinaryReader(_MemStream);
    }

    public void OnConnectServer(string host, int port)
    {
        try
        {
            IPAddress[] addresses = Dns.GetHostAddresses(host);

            if(addresses.Length == 0)
            {
                Debug.Log("服务端地址不可用");
                return;
            }

            if (addresses[0].AddressFamily == AddressFamily.InterNetworkV6)
            {
                _Client = new TcpClient(AddressFamily.InterNetworkV6);
            }
            else
            {
                _Client = new TcpClient(AddressFamily.InterNetwork);
            }

            _Client.SendTimeout = 1000;
            _Client.ReceiveTimeout = 1000;

            _Client.NoDelay = true;
            _Client.BeginConnect(host, port, OnConnect, null);
        }
        catch(Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
    private void OnConnect(IAsyncResult result)
    {
        if(_Client == null || !_Client.Connected)
        {
            Debug.LogError("服务链接失败");
            return;
        }

        Manager.Net.OnNetConnected();

        _TcpStream = _Client.GetStream();
        _TcpStream.BeginRead(_Buffer, 0, BufferSize, OnRead, null);
    }

    private void OnRead(IAsyncResult result)
    {
        try
        {
            if (_Client == null || _TcpStream == null)
            {
                return;
            }

            int length = _TcpStream.EndRead(result);

            if(length < 1)
            {
                OnDisConnected();
                return;
            }
            RecieveData(length);
            lock(_TcpStream)
            {
                Array.Clear(_Buffer, 0, _Buffer.Length);
                _TcpStream.BeginRead(_Buffer, 0, BufferSize, OnRead, null);
            }
        }
        catch(Exception e)
        {
            Debug.LogError(e.Message);
            OnDisConnected();
        }
    }
    /// <summary>
    /// 数据解析
    /// </summary>
    private void RecieveData(int length)
    {
        _MemStream.Seek(0, SeekOrigin.End);
        _MemStream.Write(_Buffer, 0, length);
        _MemStream.Seek(0, SeekOrigin.Begin);

        while(RemainingBytesLength() >= 8)
        {
            int msgId = _BinaryReader.ReadInt32();
            int msgLen = _BinaryReader.ReadInt32();

            if(RemainingBytesLength() >= msgLen)
            {
                byte[] data = _BinaryReader.ReadBytes(msgLen);

                string msg = System.Text.Encoding.UTF8.GetString(data);

                //TODO:转存
                Manager.Net.Recieve(msgId, msg);
            }
            else
            {
                _MemStream.Position = _MemStream.Position - 8;
                break;
            }
        }

        byte[] remain = _BinaryReader.ReadBytes(RemainingBytesLength());

        _MemStream.SetLength(0);
        _MemStream.Write(remain, 0, remain.Length);
    }
    private int RemainingBytesLength()
    {
        int length = (int)(_MemStream.Length - _MemStream.Position);
        return length;
    }

    public void SendMessage(int msgID, string msg)
    {
        using(MemoryStream ms = new MemoryStream())
        {
            ms.Position = 0;
            BinaryWriter writer = new BinaryWriter(ms);

            byte[] data = System.Text.Encoding.UTF8.GetBytes(msg);

            writer.Write(msgID);

            writer.Write((int)data.Length);

            writer.Write(data);

            writer.Flush();

            if(_Client != null && _Client.Connected)
            {
                byte[] sendData = ms.ToArray();

                _TcpStream.BeginWrite(sendData,0, sendData.Length, OnEndSend, null);
            }
            else
            {
                Debug.LogError("未连接到服务器");
            }
        }
    }

    private void OnEndSend(IAsyncResult ar)
    {
        try
        {
            _TcpStream.EndWrite(ar);
        }
        catch(Exception e)
        {
            OnDisConnected();
            Debug.LogError(e.Message);
        }
    }
    private void OnDisConnected()
    {
        if(_Client != null && _Client.Connected )
        {
            _Client.Close();
            _Client = null;

            _TcpStream.Close();
            _TcpStream = null;
        }
        Manager.Net.OnDisConnected();
    }
}
