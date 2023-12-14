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
    const int accountExitsid = 1;
    const int accountMadeid = 2;
    const int welcomeMsgID = 3;
    const int wrongLoginInfoid = 4;

    #region Send and Receive Data Functions
    static public void ReceivedMessageFromServer(string msg, TransportPipeline pipeline)
    {
        Debug.Log("Network msg received =  " + msg + ", from pipeline = " + pipeline);

        string[] csv = msg.Split(sep);
        int signifier = int.Parse(csv[0]);

        //SERVER->TO->CLIENT
        if (signifier == ServerToClientSignifiers.AccountExists) //display server msgs
        {
            if (csv[1] == accountExitsid.ToString())//31,1
            {
                gameLogic.displayServerMsg.text = accountexitsmsg;
            }
            else
            {
                Debug.Log("Something went wrong");
            }
        }
        else if (signifier == ServerToClientSignifiers.AccountMade) //display server msgs
        {
            if (csv[1] == accountMadeid.ToString())//32,2
            {
                gameLogic.displayServerMsg.text = accountmademsg;
            }
            else
            {
                Debug.Log("Something went wrong");
            }

        }
        else if (signifier == ServerToClientSignifiers.WelcomeMSG) //display server msgs
        {
            if (csv[1] == welcomeMsgID.ToString())//32,3
            {
                gameLogic.displayServerMsg.text = welcomeMsg;
            }
            else
            {
                Debug.Log("Something went wrong");
            }

        }
        else if (signifier == ServerToClientSignifiers.WrongPasswordOrUsername) //display server msgs
        {
            if (csv[1] == wrongLoginInfoid.ToString())//32,4
            {
                gameLogic.displayServerMsg.text = wrongLoginInfo;
            }
            else
            {
                Debug.Log("Something went wrong");
            }

        }
        else if (signifier == ServerToClientSignifiers.ChatMSG)  //display chat msg to client  
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
            gameLogic.displayusernametxt.text = csv[1];
            gameLogic.createAccountUI.SetActive(false);
            gameLogic.roomUI.SetActive(true);
        }
        else if (signifier == ServerToClientSignifiers.CreateGame) //makes the game
        {
            gameLogic.roomUI.SetActive(false);
            gameLogic.MakeGame();
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
        else if (signifier == ServerToClientSignifiers.WhosTurn) //tells the user its there turn
        {
            gameLogic.WhosTurn();
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
    public const int ChatMSG = 1;
    public const int MakeAccount = 2;
    public const int LoginData = 3;
    public const int AccountExists = 31;
    public const int AccountMade = 32;
    public const int WelcomeMSG = 33;
    public const int WrongPasswordOrUsername = 34;
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
    public const int AccountExists = 31;
    public const int AccountMade = 32;
    public const int WelcomeMSG = 33;
    public const int WrongPasswordOrUsername = 34;
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

