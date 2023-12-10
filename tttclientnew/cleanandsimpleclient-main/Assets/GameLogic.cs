using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameLogic : MonoBehaviour
{
    const char sep = ',';
    public Text displayusernametxt;
    public InputField inputfieldchat;
    public Button sendbutton;
    public Text currentinputtxt;
    public Text chattxt;
    public bool sendchat = false;
    void Start()
    {
        NetworkClientProcessing.SetGameLogic(this);
        sendbutton.onClick.AddListener(SendThis);
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
    void Update()
    {
    }


}
