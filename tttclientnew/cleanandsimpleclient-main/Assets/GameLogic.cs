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

    //chat
    public Button sendbutton;
    public InputField inputfieldchat;
    public Text currentinputtxt;        //whats being typed in inputfield
    public Text chattxt;                //chat box txt
    public bool sendchat = false;

    void Start()
    {
        NetworkClientProcessing.SetGameLogic(this);
        sendbutton.onClick.AddListener(SendThis);

        create.onClick.AddListener(CreateAccount);
        signin.onClick.AddListener(SiginAccount);
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
