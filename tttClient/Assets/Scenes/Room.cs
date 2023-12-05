using System.Collections;
using System.Collections.Generic;
//using UnityEditor.XR;
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

    public NetworkClient networkClient;

    //private List<string> roomNames = new List<string>();

    public GameObject panelPrefab;
 
    // Start is called before the first frame update
    void Start()
    {
        networkClient = GameObject.Find("important").GetComponent<NetworkClient>();
        // Add listeners to the buttons
        createRoomButton.onClick.AddListener(CreateUIPanel);
        backButton.onClick.AddListener(Exit);
    }



    public void CreateUIPanel()
    {
        roomnametxt = currentRoomtxt.text;
        // Instantiate the panel from the prefab
        GameObject panel = Instantiate(panelPrefab);

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
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        // Set the Text object's RectTransform properties
        RectTransform rectTransform = text.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 50);
        rectTransform.anchoredPosition = new Vector2(0, 0);
        networkClient.SendMessageToServer("ROOM_" + "," + roomnametxt, TransportPipeline.ReliableAndInOrder);
    }

    private void Exit()
    {
        networkClient.SendMessageToServer("ROOM_EXIT" + roomnametxt, TransportPipeline.ReliableAndInOrder);
    }

}



