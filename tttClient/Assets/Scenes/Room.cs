using System.Collections;
using System.Collections.Generic;
using UnityEditor.XR;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    public InputField input;
    public Button createRoomButton;
    public GameObject waitingUI;
    public Button backButton;
    public Text currentRoomtxt;
    public string roomnametxt;
    //const int MaxNumberOfClientConnections = 4;
    public int NetworkPort;

    public NetworkClient networkClient;

    private List<string> roomNames = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        networkClient = GetComponent<NetworkClient>();
        // Add listeners to the buttons
        createRoomButton.onClick.AddListener(() => CreateRoom(input.text, NetworkPort));
        //backButton.onClick.AddListener();
    }

    public void CreateOrJoinRoom(string name, int port)
    {

        if (IsRoomExists(name))
        {
            JoinRoom(name);
        }
        else
        {
            Debug.Log("Error");
        }
    }

    public void CreateRoom(string name, int port)
    {
        Debug.Log("Creating room: " + name + " with port: " + port);
        roomNames.Add(name);
    }
    public bool IsRoomExists(string name)
    {
        return roomNames.Contains(name);
    }

    public void JoinRoom(string name)
    {

    }
}



