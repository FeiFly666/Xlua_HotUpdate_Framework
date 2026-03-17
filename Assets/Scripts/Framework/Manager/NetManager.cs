using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class NetManager : MonoBehaviour
{
    NetClient _NetClient;

    Queue<KeyValuePair<int,string>> _MessageQueue = new Queue<KeyValuePair<int, string>>();

    LuaFunction ReceiveMessage;

    public void Init()
    {
        _NetClient = new NetClient();

        ReceiveMessage = Manager.Lua.luaEnv.Global.Get<LuaFunction>("ReceiveMessage");
    }

    public void SendMessage(int msgID, string msg)
    {
        _NetClient.SendMessage(msgID, msg);
    }

    public void ConnectedToServer(string post, int poer)
    {
        _NetClient.OnConnectServer(post, poer);
    }

    public void OnNetConnected()
    {

    }

    public void OnDisConnected()
    {

    }

    public void Recieve(int msgID, string msg)
    { 
        _MessageQueue.Enqueue(new KeyValuePair<int, string>(msgID, msg));
    }

    private void Update()
    {
        if(_MessageQueue.Count > 0)
        {
            KeyValuePair<int,string> currentMsg = _MessageQueue.Dequeue();

            ReceiveMessage?.Call(currentMsg.Key, currentMsg.Value);
        }
    }
}
