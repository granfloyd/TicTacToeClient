using UnityEngine;
using UnityEngine.Assertions;
using Unity.Collections;
using Unity.Networking.Transport;
using System.Text;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Numerics;

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
    void Start()
    {
        GameObject game = GameObject.Find("gamestuff");
        if (game != null)
            gameRef = game.GetComponent<Game>();
        //gameRef = GameObject.Find("gamestuff").GetComponent<Game>();
        otherRef = GameObject.Find("Cube").GetComponent<Other>();

        networkDriver = NetworkDriver.Create();
        reliableAndInOrderPipeline = networkDriver.CreatePipeline(typeof(FragmentationPipelineStage), typeof(ReliableSequencedPipelineStage));
        nonReliableNotInOrderedPipeline = networkDriver.CreatePipeline(typeof(FragmentationPipelineStage));
        networkConnection = default(NetworkConnection);
        NetworkEndPoint endpoint = NetworkEndPoint.Parse(IPAddress, NetworkPort, NetworkFamily.Ipv4);
        networkConnection = networkDriver.Connect(endpoint);
    }

    public void OnDestroy()
    {
        networkConnection.Disconnect(networkDriver);
        networkConnection = default(NetworkConnection);
        networkDriver.Dispose();
    }

    void Update()
    {
        #region Check Input and Send Msg
        //create account
        if(otherRef.sendit)
        {
            SendMessageToServer("MAKE_ACCOUNT" + "," + otherRef.currentUsername + "," + otherRef.currentPassword, TransportPipeline.ReliableAndInOrder);
            otherRef.sendit = false;
        }
        //sigin in
        if (otherRef.senditt)
        {
            SendMessageToServer("LOGIN_DATA" + "," + otherRef.currentUsername + "," + otherRef.currentPassword ,TransportPipeline.ReliableAndInOrder);
            otherRef.senditt = false;
        }
        if(otherRef.displayusernametxt.text != "")
        {
            otherRef.createUI.SetActive(false);
            otherRef.mainUI.SetActive(true);
        }
        

        #endregion

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
            if (pipelineUsedToSendEvent == reliableAndInOrderPipeline)
                Debug.Log("Network event from: reliableAndInOrderPipeline");
            else if (pipelineUsedToSendEvent == nonReliableNotInOrderedPipeline)
                Debug.Log("Network event from: nonReliableNotInOrderedPipeline");

            switch (networkEventType)
            {
                case NetworkEvent.Type.Connect:
                    Debug.Log("We are now connected to the server");
                    break;
                case NetworkEvent.Type.Data:
                    int sizeOfDataBuffer = streamReader.ReadInt();
                    NativeArray<byte> buffer = new NativeArray<byte>(sizeOfDataBuffer, Allocator.Persistent);
                    streamReader.ReadBytes(buffer);
                    byte[] byteBuffer = buffer.ToArray();
                    string msg = Encoding.Unicode.GetString(byteBuffer);
                    ProcessReceivedMsg(msg);
                    buffer.Dispose();
                    break;
                case NetworkEvent.Type.Disconnect:
                    Debug.Log("Client has disconnected from server");
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

    private void ProcessReceivedMsg(string msg)
    {
        Debug.Log("Msg received = " + msg);

        // If the server sends a "YOUR_TURN" message, it's this client's turn
        if (msg.StartsWith("LOGIN_SUCCESSFUL"))
        {
            string[] msgParts = msg.Split(',');
            otherRef.displayusernametxt.text = msgParts[1];
            otherRef.createUI.SetActive(false);
            otherRef.mainUI.SetActive(true);
            return;
        }
        if (msg.StartsWith("GIMME_YOUR_INFO"))
        {
            string infomsg = otherRef.displayusernametxt.text;
            SendMessageToServer("GET_USERNAME," + infomsg, TransportPipeline.ReliableAndInOrder);
            return;
        }
        if (msg.StartsWith("CREATING_GAME"))
        {
            Instantiate(gamePrefab, transform.position, Quaternion.identity);
            return;
        }

        if (msg.StartsWith("YOUR_TURN"))
        {
            gameRef.isMyTurn = true;
            Debug.Log("SUP");
            return;
        }
        // If the server sends a "MOVE" message, update the button text
        if (msg.StartsWith("MOVE"))
        {
            string[] msgParts = msg.Split(',');
            if (msgParts.Length < 3)
            {
                Debug.LogError("Invalid MOVE message: " + msg);
                return;
            }

            string buttonName = msgParts[1];
            string newText = msgParts[2];

            Text buttonText = GameObject.Find(buttonName).GetComponent<Text>();
            if (buttonText == null)
            {
                Debug.LogError("Text not found: " + buttonName);
                return;
            }

            buttonText.text = newText;
        }
        if (msg.StartsWith("RESET"))
        {
            string doneMsg = "RESET_COMPLETE";
            gameRef.ResetGame();
            SendMessageToServer(doneMsg, TransportPipeline.ReliableAndInOrder);

        }
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

    internal void JoinRoom(string name)
    {
        throw new NotImplementedException();
    }


}

public enum TransportPipeline
{
    NotIdentified,
    ReliableAndInOrder,
    FireAndForget
}
