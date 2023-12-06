using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Other : MonoBehaviour
{

    public NetworkClient clientRef;
    public GameObject createUI;
    public GameObject mainUI;
    public Button signin;
    public Button create;

    public Button swaggybutton;
    public Button smallbutton;
    public Button sadbutton;
    public Button dispairbutton;

    public Button onebutton;
    public Button twobutton;
    public Button threebutton;

    public Button back;

    public Button done;
    public Text createusernametxt;
    public Text createpasswordtxt;

    public Text displayusernametxt;

    public string currentUsername;
    public string currentPassword;

    public bool sendit = false;
    public bool senditt = false;

    public InputField inputfieldchat;
    public Button sendbutton;

    public Text currentinputtxt;

    public Text chattxt;

    public bool sendchat = false;
    // Start is called before the first frame update
    void Start()
    {
        clientRef = GameObject.Find("important").GetComponent<NetworkClient>();
        create.onClick.AddListener(CreateAccount);
        signin.onClick.AddListener(SiginAccount);

        swaggybutton.onClick.AddListener(() => AddThis("swaggy"));
        smallbutton.onClick.AddListener(() => AddThis("small"));
        sadbutton.onClick.AddListener(() => AddThis("sad"));
        dispairbutton.onClick.AddListener(() => AddThis("dispair"));

        onebutton.onClick.AddListener(() => AddThis2("1"));
        twobutton.onClick.AddListener(() => AddThis2("2"));
        threebutton.onClick.AddListener(() => AddThis2("3"));

        back.onClick.AddListener(DeleteThis);

        sendbutton.onClick.AddListener(SendThis);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void AddThis(string txt)
    {
        createusernametxt.text += txt;
    }

    void AddThis2(string txt)
    {
        createpasswordtxt.text += txt;
    }
    void DeleteThis()
    {
        createusernametxt.text = null;
        createpasswordtxt.text = null;
        currentUsername = null;
        currentPassword = null;
    }
    void CreateAccount()
    {
        currentUsername = createusernametxt.text;
        currentPassword = createpasswordtxt.text;
        sendit = true;
    }
    void SiginAccount()
    {
        currentUsername = createusernametxt.text;
        currentPassword = createpasswordtxt.text;
        senditt = true;
    }
    void SendThis()
    {
        chattxt.text = currentinputtxt.text;
        Debug.Log("yup");
        string chatmsg = "CHAT_MSG," + displayusernametxt.text + "," + ": " + currentinputtxt.text;
        clientRef.SendMessageToServer(chatmsg, TransportPipeline.ReliableAndInOrder);
    }
}

