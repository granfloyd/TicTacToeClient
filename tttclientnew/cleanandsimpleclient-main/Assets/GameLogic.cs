using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
public class GameLogic : MonoBehaviour
{
    const char sep = ',';
    public GameObject createAccountUI;
    public GameObject roomUI;

    public Text displayusernametxt;
    public Text displayServerMsg;

    //make account
    public Button signin;
    public Button create;
    public InputField inputfieldUsername;
    public Text currentUsername;//whats being typed in inputfield
    public InputField inputfieldPassword;
    public Text currentPassword;//whats being typed in inputfield
    public bool wantsToSignin = false;
    public bool wantsToCreate = false;
    public const string sendingUsername = "";
    public const string sendingPassword = "";

    //chat
    public Button sendbutton;
    public InputField inputfieldchat;
    public Text currentinputtxt;        //whats being typed in inputfield
    public Text chattxt;                //chat box txt
    public bool sendchat = false;

    //room
    public InputField inputfieldRoom;
    public Button createRoomButton;
    public GameObject waitingUI;
    public Button backButton;
    public Text currentRoomtxt;
    public string roomnametxt;

    public GameObject panelPrefab;
    public GameObject TicTacToePrefab;

    public TicTacToe tttRef;
    public GameObject gameStuff;

    void Start()
    {
        NetworkClientProcessing.SetGameLogic(this);
        sendbutton.onClick.AddListener(SendThis);

        create.onClick.AddListener(CreateAccount);
        signin.onClick.AddListener(SiginAccount);

        createRoomButton.onClick.AddListener(CreateRoomUIPanel);
        backButton.onClick.AddListener(ExitRoom);
    }
    public void MakeGame()
    {
        gameStuff = Instantiate(TicTacToePrefab);
        tttRef = gameStuff.GetComponentInChildren<TicTacToe>();
    }
    public bool WhosTurn()
    {
        Debug.Log("Turning turn");
        return tttRef.isMyTurn = true;
    }
    void CreateAccount()
    {
        wantsToCreate = true;
    }

    void SiginAccount()
    {
        wantsToSignin = true;
    }

    void SendThis()
    {
        //1,username,chattxt
        chattxt.text = currentinputtxt.text;
        string chatmsg = ClientToServerSignifiers.ChatMSG.ToString() + sep +
                 displayusernametxt.text + sep + ": " +
                 currentinputtxt.text;
        NetworkClientProcessing.SendMessageToServer(chatmsg, TransportPipeline.ReliableAndInOrder);
    }

    public void CreateRoomUIPanel()
    {
        roomnametxt = currentRoomtxt.text;
        // Instantiate the panel from the prefab
        GameObject panel = Instantiate(panelPrefab);
        Instantiate(waitingUI);
        // Set the name of the panel
        panel.name = "ROOM_" + roomnametxt;

        // Make the panel a child of the canvas
        panel.transform.SetParent(GameObject.Find("ROOM_" + roomnametxt).transform, false);

        // Set the position of the panel
        panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

        // Create a new Text object
        GameObject textObj = new GameObject("RoomNameText");
        textObj.transform.SetParent(panel.transform, false);

        // Add a Text component to the Text object
        Text text = textObj.AddComponent<Text>();

        // Set the text properties
        text.text = roomnametxt;
        text.color = Color.black;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 30;

        // Set the Text object's RectTransform properties
        RectTransform rectTransform = text.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 50);
        rectTransform.anchoredPosition = new Vector2(-5, 385);

        string msg = ClientToServerSignifiers.RoomJoin.ToString() + sep +
                roomnametxt;
        NetworkClientProcessing.SendMessageToServer(msg, TransportPipeline.ReliableAndInOrder);
    }

    private void ExitRoom()
    {
        GameObject needsToGo = GameObject.Find("ROOM_" + roomnametxt);
        Destroy(needsToGo);

        GameObject needsToGo2 = GameObject.FindGameObjectWithTag("Waiting");
        Destroy(needsToGo2);

        string msg = ClientToServerSignifiers.RoomExit.ToString() + sep +
                roomnametxt;
        NetworkClientProcessing.SendMessageToServer(msg, TransportPipeline.ReliableAndInOrder);
    }
    void Update()
    {
        
        //CLIENT->TO->SERVER
        if (wantsToCreate)
        {
            string hashedPassword = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(currentPassword.text)));
            string msg = ClientToServerSignifiers.MakeAccount.ToString() + sep +
                currentUsername.text + sep +
                hashedPassword;
            NetworkClientProcessing.SendMessageToServer(msg, TransportPipeline.ReliableAndInOrder);
            wantsToCreate = false;
        }

        if (wantsToSignin)
        {
            string hashedPassword = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(currentPassword.text)));
            string loginmsg = ClientToServerSignifiers.LoginData.ToString() + sep +
                currentUsername.text + sep +
                hashedPassword;
            NetworkClientProcessing.SendMessageToServer(loginmsg, TransportPipeline.ReliableAndInOrder);
            wantsToSignin = false;
        }
    }


}
