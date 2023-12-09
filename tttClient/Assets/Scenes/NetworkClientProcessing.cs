using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;
static public class NetworkClientProcessing
{
    #region Send and Receive Data Functions
    const char sep = ',';
    static public void ReceivedMessageFromServer(string msg, TransportPipeline pipeline)
    {
        Debug.Log("Network msg received =  " + msg + ", from pipeline = " + pipeline);

        string[] csv = msg.Split(sep);
        int signifier = int.Parse(csv[0]);
        Other otherRef = GameObject.Find("Cube").GetComponent<Other>();
        Room roomRef = GameObject.Find("MainUI").GetComponent<Room>();


        //SERVER->TO->CLIENT
        if (signifier == ServerToClientSignifiers.ChatMSG)
        {
            string chatusername;
            string chattext;
            chatusername = csv[1];
            chattext = csv[2];
            otherRef.chattxt.text = chatusername + chattext;
        }
        else if (signifier == ServerToClientSignifiers.MakeAccount)
        {

        }
        else if (signifier == ServerToClientSignifiers.LoginData)
        {
            otherRef.displayusernametxt.text = csv[1];
            otherRef.createUI.SetActive(false);
            otherRef.mainUI.SetActive(true);
        }
        else if (signifier == ServerToClientSignifiers.CreateGame)// makes the game...
        {
            networkClient.MakeGame();
        }
        else if (signifier == ServerToClientSignifiers.WhosTurn)// its this players turn 
        {
            networkClient.WhosTurn();
        }
        else if (signifier == ServerToClientSignifiers.DisplayMove)// update the board
        {
            if (csv.Length < 3)
            {
                Debug.LogError("Invalid MOVE message: " + msg);
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
        else if (signifier == ServerToClientSignifiers.Restart)// restarted
        {
            networkClient.Restarted();//    might have to maek a boolean
        }


        
    }
    static public void SendMessageToServer(string msg, TransportPipeline pipeline)
    {
        string[] csv = msg.Split(sep);
        int signifier = int.Parse(csv[0]);
        Other otherRef = GameObject.Find("Cube").GetComponent<Other>();
        Room roomRef = GameObject.Find("MainUI").GetComponent<Room>();

        //networkClient.SendMessageToServer(msg, pipeline);
        //CLIENT->TO->SERVER 
        if (signifier == ClientToServerSignifiers.ChatMSG)
        {
            string chatmsg = ClientToServerSignifiers.ChatMSG +
                otherRef.displayusernametxt.text + sep + ": " +
                otherRef.currentinputtxt.text;
            SendMessageToServer(chatmsg, TransportPipeline.ReliableAndInOrder);
        }
        else if (signifier == ClientToServerSignifiers.MakeAccount) //send username and password
        {
            if (otherRef.sendit)//if create account is pressed 
            {
                SendMessageToServer(ClientToServerSignifiers.MakeAccount + sep +
                    otherRef.currentUsername + sep +
                    otherRef.currentPassword,
                    TransportPipeline.ReliableAndInOrder);
                otherRef.sendit = false;    //set button back to false after sending the 2 strings 
            }
        }
        else if (signifier == ClientToServerSignifiers.LoginData) //send username and password
        {
            if (otherRef.senditt)//if sign in is pressed 
            {
                SendMessageToServer(ClientToServerSignifiers.LoginData + sep +
                    otherRef.currentUsername + sep +
                    otherRef.currentPassword,
                    TransportPipeline.ReliableAndInOrder);
                otherRef.senditt = false;   //set button back to false after sending the 2 strings 
            }
        }
        else if (signifier == ClientToServerSignifiers.RoomJoin) //send username and password
        {
            SendMessageToServer(ClientToServerSignifiers.RoomJoin + sep + roomRef.roomnametxt, TransportPipeline.ReliableAndInOrder);
        }
        else if (signifier == ClientToServerSignifiers.RoomExit) //send username and password
        {
            SendMessageToServer(ClientToServerSignifiers.RoomExit + sep + roomRef.roomnametxt, TransportPipeline.ReliableAndInOrder);
        }
        else if (signifier == ClientToServerSignifiers.Loser) //send username and password
        {

        }
        else if (signifier == ClientToServerSignifiers.Winner) //send username and password
        {

        }
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

    static public void SetNetworkedClient(NetworkClient NetworkClient)
    {
        networkClient = NetworkClient;
    }
    static public NetworkClient GetNetworkedClient()
    {
        return networkClient;
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
}


#endregion
