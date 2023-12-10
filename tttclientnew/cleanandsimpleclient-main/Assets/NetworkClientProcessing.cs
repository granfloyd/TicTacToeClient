using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class NetworkClientProcessing
{
    const char sep = ',';
    #region Send and Receive Data Functions
    static public void ReceivedMessageFromServer(string msg, TransportPipeline pipeline)
    {
        Debug.Log("Network msg received =  " + msg + ", from pipeline = " + pipeline);

        string[] csv = msg.Split(sep);
        int signifier = int.Parse(csv[0]);

        //SERVER->TO->CLIENT
        if (signifier == ServerToClientSignifiers.Debug) //display server msgs
        {
            gameLogic.displayServerMsg.text = csv[1];
        }
        else if (signifier == ServerToClientSignifiers.ChatMSG)  //display chat msg to client  
        {
            string chatusername;
            string chattext;
            chatusername = csv[1];
            chattext = csv[2];
            gameLogic.chattxt.text = chatusername + chattext;
        }
        else if (signifier == ServerToClientSignifiers.LoginData) //display logined user
        {
            gameLogic.displayusernametxt.text = csv[1];
            gameLogic.createAccountUI.SetActive(false);
            gameLogic.roomUI.SetActive(true);
        }
        else if (signifier == ServerToClientSignifiers.CreateGame) //makes the game
        {
            //make sure this waiting ui is not on screen when game is loaded
            GameObject needsToGo2 = GameObject.FindGameObjectWithTag("Waiting");
            Destroy(needsToGo2);
        }
        else if (signifier == ServerToClientSignifiers.WhosTurn) //tells the user its there turn
        {

        }
    }

    private static void Destroy(GameObject needsToGo2)
    {
        throw new NotImplementedException();
    }

    static public void SendMessageToServer(string msg, TransportPipeline pipeline)
    {
        networkClient.SendMessageToServer(msg, pipeline);        
    }

    #endregion

    #region Connection Related Functions and Events
    static public void ConnectionEvent()
    {
        Debug.Log("Network Connection Event!");
    }
    static public void DisconnectionEvent()
    {
        Debug.Log("Network Disconnection Event!");
    }
    static public bool IsConnectedToServer()
    {
        return networkClient.IsConnected();
    }
    static public void ConnectToServer()
    {
        networkClient.Connect();
    }
    static public void DisconnectFromServer()
    {
        networkClient.Disconnect();
    }

    #endregion

    #region Setup
    static NetworkClient networkClient;
    static GameLogic gameLogic;

    static public void SetNetworkedClient(NetworkClient NetworkClient)
    {
        networkClient = NetworkClient;
    }
    static public NetworkClient GetNetworkedClient()
    {
        return networkClient;
    }
    static public void SetGameLogic(GameLogic GameLogic)
    {
        gameLogic = GameLogic;
    }

    #endregion

}

#region Protocol Signifiers
static public class ClientToServerSignifiers
{
    public const int ChatMSG = 1;
    public const int MakeAccount = 2;
    public const int LoginData = 3;
    public const int CreateGame = 4;
    public const int WhosTurn = 5;
    public const int DisplayMove = 6;
    public const int Restart = 7;
    public const int RoomJoin = 11;
    public const int RoomExit = 12;
    public const int Winner = 21;
    public const int Loser = 22;
}

static public class ServerToClientSignifiers
{
    public const int ChatMSG = 1;
    public const int MakeAccount = 2;
    public const int LoginData = 3;
    public const int CreateGame = 4;
    public const int WhosTurn = 5;
    public const int DisplayMove = 6;
    public const int Restart = 7;
    public const int RoomJoin = 11;
    public const int RoomExit = 12;
    public const int Winner = 21;
    public const int Loser = 22;
    public const int Debug = 69;
}

#endregion

