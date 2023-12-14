using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

static public class NetworkClientProcessing
{
    const char sep = ',';
    const string accountexitsmsg = "Account Exists";
    const string accountmademsg = "Account Created";
    private const string defaultUsername = "DefaultUser";
    private const string welcomeMsg = "Welcome ";
    private const string wrongLoginInfo = "Invalid username or password ";

    #region Send and Receive Data Functions
    static public void ReceivedMessageFromServer(string msg, TransportPipeline pipeline)
    {
        Debug.Log("Network msg received =  " + msg + ", from pipeline = " + pipeline);

        string[] csv = msg.Split(sep);
        int signifier = int.Parse(csv[0]);

        //SERVER->TO->CLIENT
        if (signifier == ServerToClientSignifiers.AccountExists) //display server msgs
        {
            gameLogic.displayServerMsg.text = accountexitsmsg;
        }
        else if (signifier == ServerToClientSignifiers.AccountMade) //display server msgs
        {
            gameLogic.displayServerMsg.text = accountmademsg;
        }
        else if (signifier == ServerToClientSignifiers.WrongPasswordOrUsername) //display server msgs
        {
            gameLogic.displayServerMsg.text = wrongLoginInfo;
        }
        else if (signifier == ServerToClientSignifiers.GlobalChatMSG)  //display chat msg to client  
        {
            string chatusername;
            string chattext;
            chatusername = csv[1];
            if(chatusername == "")
            {
                chatusername = defaultUsername;
            }
            chattext = csv[2];
            gameLogic.chattxt.text = chatusername + chattext;
        }
        else if (signifier == ServerToClientSignifiers.LoginData) //display logined user
        {
            gameLogic.displayusernametxt.text = gameLogic.currentUsername.text;
            gameLogic.displayServerMsg.text = welcomeMsg + gameLogic.currentUsername.text;
            gameLogic.createAccountUI.SetActive(false);
            gameLogic.roomUI.SetActive(true);
        }
        else if (signifier == ServerToClientSignifiers.CreateGame) //makes the game
        {
            gameLogic.roomUI.SetActive(false);
            gameLogic.MakeGame();
        }
        else if (signifier == ServerToClientSignifiers.RoomSpectate) //makes the game
        {
            gameLogic.roomUI.SetActive(false);
            gameLogic.MakeGame();
            Debug.Log("spectating");
        }
        else if (signifier == ServerToClientSignifiers.DisplayMove) //tells the user its there turn
        {
            if (csv.Length < 3)
            {
                Debug.LogError("something went wrong: " + msg);
                return;
            }
            string buttonName = csv[1];
            string newText = csv[2];

            Text buttonText = GameObject.Find(buttonName).GetComponent<Text>();
            if (buttonText == null)
            {
                Debug.LogError("Text not found: " + buttonName);
                return;
            }
            buttonText.text = newText;
        }
        else if (signifier == ServerToClientSignifiers.CurrentTurn) //tells the user its there turn
        {
            gameLogic.WhosTurn();
        }
        else if (signifier == ServerToClientSignifiers.ClearedBoard) //tells the user its there turn
        {
            gameLogic.ResetThisGame();
        }
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
    public const int ChatMSG = 10; //sends server chat msg 

    public const int MakeAccount = 20;   //sends server client login data for new account
    public const int LoginData = 21;     //sends server client login data only for made accounts

    public const int SendMove = 30;      //send move to server
    public const int ClearBoard = 31;        //new

    public const int RoomJoin = 40;
    public const int RoomSpectate = 41;
    public const int RoomExit = 42;

}

static public class ServerToClientSignifiers
{
    public const int GlobalChatMSG = 10;

    public const int LoginData = 20;
    public const int AccountExists = 21;
    public const int AccountMade = 22;
    public const int WrongPasswordOrUsername = 23;

    public const int CreateGame = 30;
    public const int CurrentTurn = 31;
    public const int DisplayMove = 32;
    public const int ClearedBoard = 33;

    public const int RoomSpectate = 40;

}

#endregion

