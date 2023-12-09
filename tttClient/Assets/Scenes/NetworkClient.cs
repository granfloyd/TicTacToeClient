using UnityEngine;
using UnityEngine.Assertions;
using Unity.Collections;
using Unity.Networking.Transport;
using System.Text;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using JetBrains.Annotations;

public class NetworkClient : MonoBehaviour
{
    NetworkDriver networkDriver;
    NetworkConnection networkConnection;
    NetworkPipeline reliableAndInOrderPipeline;
    NetworkPipeline nonReliableNotInOrderedPipeline;
    const ushort NetworkPort = 9001;
    const string IPAddress = "10.0.0.153";//"";
    public Game gameRef;
    public Other otherRef;
   
    public GameObject gamePrefab;
    public GameObject gameStuff;
    void Start()
    {
        otherRef = GameObject.Find("Cube").GetComponent<Other>();

        if (NetworkClientProcessing.GetNetworkedClient() == null)
        {
            DontDestroyOnLoad(this.gameObject);
            NetworkClientProcessing.SetNetworkedClient(this);
            Connect();
        }
        else
        {
            Debug.Log("Singleton-ish architecture violation detected, investigate where NetworkClient.cs Start() is being called.  Are you creating a second instance of the NetworkClient game object or has NetworkClient.cs been attached to more than one game object?");
            Destroy(this.gameObject);
        }
    }

    public void OnDestroy()
    {
        networkConnection.Disconnect(networkDriver);
        networkConnection = default(NetworkConnection);
        networkDriver.Dispose();
    }
    //makes tic tac toe
    public void MakeGame()
    {
        gameStuff = Instantiate(gamePrefab, transform.position, Quaternion.identity);
        gameRef = gameStuff.GetComponentInChildren<Game>();
    }
    public bool WhosTurn()
    {
        return gameRef.isMyTurn = true;
    }
    public void Restarted()
    { 
    }
         
    void Update()
    {
        ////create account
        //if (otherRef.sendit)
        //{
        //    SendMessageToServer("MAKE_ACCOUNT" + "," + otherRef.currentUsername + "," + otherRef.currentPassword, TransportPipeline.ReliableAndInOrder);
        //    otherRef.sendit = false;
        //}

        //sigin in
        //if (otherRef.senditt)
        //{
        //    SendMessageToServer("LOGIN_DATA" + "," + otherRef.currentUsername + "," + otherRef.currentPassword, TransportPipeline.ReliableAndInOrder);
        //    otherRef.senditt = false;
        //}
        if (otherRef.displayusernametxt.text != "")
        {
            otherRef.createUI.SetActive(false);
            otherRef.mainUI.SetActive(true);
        }
        networkDriver.ScheduleUpdate().Complete();

        #region Check for client to server connection

        if (!networkConnection.IsCreated)
        {
            Debug.Log("Client is unable to connect to server");
            return;
        }

        #endregion

        #region Manage Network Events

        NetworkEvent.Type networkEventType;
        DataStreamReader streamReader;
        NetworkPipeline pipelineUsedToSendEvent;

        while (PopNetworkEventAndCheckForData(out networkEventType, out streamReader, out pipelineUsedToSendEvent))
        {
            TransportPipeline pipelineUsed = TransportPipeline.NotIdentified;
            if (pipelineUsedToSendEvent == reliableAndInOrderPipeline)
                pipelineUsed = TransportPipeline.ReliableAndInOrder;
            else if (pipelineUsedToSendEvent == nonReliableNotInOrderedPipeline)
                pipelineUsed = TransportPipeline.FireAndForget;

            switch (networkEventType)
            {
                case NetworkEvent.Type.Connect:
                    NetworkClientProcessing.ConnectionEvent();
                    break;
                case NetworkEvent.Type.Data:
                    int sizeOfDataBuffer = streamReader.ReadInt();
                    NativeArray<byte> buffer = new NativeArray<byte>(sizeOfDataBuffer, Allocator.Persistent);
                    streamReader.ReadBytes(buffer);
                    byte[] byteBuffer = buffer.ToArray();
                    string msg = Encoding.Unicode.GetString(byteBuffer);
                    NetworkClientProcessing.ReceivedMessageFromServer(msg, pipelineUsed);
                    buffer.Dispose();
                    break;
                case NetworkEvent.Type.Disconnect:
                    NetworkClientProcessing.DisconnectionEvent();
                    networkConnection = default(NetworkConnection);
                    break;
            }
        }

        #endregion
    }
    private bool PopNetworkEventAndCheckForData(out NetworkEvent.Type networkEventType, out DataStreamReader streamReader, out NetworkPipeline pipelineUsedToSendEvent)
    {
        networkEventType = networkConnection.PopEvent(networkDriver, out streamReader, out pipelineUsedToSendEvent);

        if (networkEventType == NetworkEvent.Type.Empty)
            return false;
        return true;
    }

    
    
    public void Connect()
    {
        networkDriver = NetworkDriver.Create();
        reliableAndInOrderPipeline = networkDriver.CreatePipeline(typeof(FragmentationPipelineStage), typeof(ReliableSequencedPipelineStage));
        nonReliableNotInOrderedPipeline = networkDriver.CreatePipeline(typeof(FragmentationPipelineStage));
        networkConnection = default(NetworkConnection);
        NetworkEndPoint endpoint = NetworkEndPoint.Parse(IPAddress, NetworkPort, NetworkFamily.Ipv4);
        networkConnection = networkDriver.Connect(endpoint);
    }

    public bool IsConnected()
    {
        return networkConnection.IsCreated;
    }

    public void Disconnect()
    {
        networkConnection.Disconnect(networkDriver);
        networkConnection = default(NetworkConnection);
    }

    public void SendMessageToServer(string msg, TransportPipeline pipeline)
    {
        NetworkPipeline networkPipeline = reliableAndInOrderPipeline;
        if (pipeline == TransportPipeline.FireAndForget)
            networkPipeline = nonReliableNotInOrderedPipeline;

        byte[] msgAsByteArray = Encoding.Unicode.GetBytes(msg);
        NativeArray<byte> buffer = new NativeArray<byte>(msgAsByteArray, Allocator.Persistent);

        DataStreamWriter streamWriter;
        networkDriver.BeginSend(networkPipeline, networkConnection, out streamWriter);
        streamWriter.WriteInt(buffer.Length);
        streamWriter.WriteBytes(buffer);
        networkDriver.EndSend(streamWriter);

        buffer.Dispose();
    }
}

public enum TransportPipeline
{
    NotIdentified,
    ReliableAndInOrder,
    FireAndForget
}
//private void ProcessReceivedMsg(string msg)
//{
//    Debug.Log("Msg received = " + msg);


//    //if (msg.StartsWith("CHAT_MSG"))
//    //{
//    //    string[] msgParts = msg.Split(',');
//    //    string chatusername;
//    //    string chattext;
//    //    chatusername = msgParts[1];
//    //    chattext = msgParts[2];
//    //    otherRef.chattxt.text = chatusername + chattext;
//    //    return;
//    //}
//    // If the server sends a "YOUR_TURN" message, it's this client's turn
//    //if (msg.StartsWith("LOGIN_SUCCESSFUL"))
//    //{
//    //    string[] msgParts = msg.Split(',');
//    //    otherRef.displayusernametxt.text = msgParts[1];
//    //    otherRef.createUI.SetActive(false);
//    //    otherRef.mainUI.SetActive(true);
//    //    return;
//    //}

//    ////i dont think i use this anymore didnt add
//    //if (msg.StartsWith("GIMME_YOUR_INFO"))
//    //{
//    //    string infomsg = otherRef.displayusernametxt.text;
//    //    SendMessageToServer("GET_USERNAME," + infomsg, TransportPipeline.ReliableAndInOrder);
//    //    return;
//    //}

//    //if (msg.StartsWith("CREATING_GAME"))
//    //{
//    //    gameStuff = Instantiate(gamePrefab, transform.position, Quaternion.identity);
//    //    gameRef = gameStuff.GetComponentInChildren<Game>();            
//    //    return;
//    //}

//    //if (msg.StartsWith("YOUR_TURN"))
//    //{
//    //    gameRef.isMyTurn = true;
//    //    Debug.Log("SUP");
//    //    return;
//    //}
//    // If the server sends a "MOVE" message, update the button text
//    //if (msg.StartsWith("MOVE"))
//    //{
//    //    string[] msgParts = msg.Split(',');
//    //    if (msgParts.Length < 3)
//    //    {
//    //        Debug.LogError("Invalid MOVE message: " + msg);
//    //        return;
//    //    }

//    //    string buttonName = msgParts[1];
//    //    string newText = msgParts[2];

//    //    Text buttonText = GameObject.Find(buttonName).GetComponent<Text>();
//    //    if (buttonText == null)
//    //    {
//    //        Debug.LogError("Text not found: " + buttonName);
//    //        return;
//    //    }

//    //    buttonText.text = newText;
//    //}
//    if (msg.StartsWith("RESET"))
//    {
//        string doneMsg = "RESET_COMPLETE";
//        gameRef.ResetGame();
//        SendMessageToServer(doneMsg, TransportPipeline.ReliableAndInOrder);

//    }
//}