using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public NetworkClient clientRef;
    public Button x1y1;
    public Button x2y1;
    public Button x3y1;
    public Text txtx1y1;
    public Text txtx2y1;
    public Text txtx3y1;

    public Button x1y2;
    public Button x2y2;
    public Button x3y2;
    public Text txtx1y2;
    public Text txtx2y2;
    public Text txtx3y2;

    public Button x1y3;
    public Button x2y3;
    public Button x3y3;
    public Text txtx1y3;
    public Text txtx2y3;
    public Text txtx3y3;

    public bool isMyTurn = false;

    // Start is called before the first frame update
    void Start()
    {
        //y1
        x1y1.onClick.AddListener(() => AddInput(txtx1y1));
        x2y1.onClick.AddListener(() => AddInput(txtx2y1));
        x3y1.onClick.AddListener(() => AddInput(txtx3y1));

        //y2
        x1y2.onClick.AddListener(() => AddInput(txtx1y2));
        x2y2.onClick.AddListener(() => AddInput(txtx2y2));
        x3y2.onClick.AddListener(() => AddInput(txtx3y2));

        //y3
        x1y3.onClick.AddListener(() => AddInput(txtx1y3));
        x2y3.onClick.AddListener(() => AddInput(txtx2y3));
        x3y3.onClick.AddListener(() => AddInput(txtx3y3));
    }

    public void AddInput(Text txt)
    {
        // If it's not this client's turn, don't do anything
        if (!isMyTurn)
            return;

        string msg = $"MOVE,{txt.name}";
        clientRef.SendMessageToServer(msg);

        // After making a move, it's no longer this client's turn
        isMyTurn = false;

        
    }
    bool CheckWinCondition()
    {
        // Check rows
        if (CheckThree(txtx1y1.text, txtx2y1.text, txtx3y1.text)) return true;
        if (CheckThree(txtx1y2.text, txtx2y2.text, txtx3y2.text)) return true;
        if (CheckThree(txtx1y3.text, txtx2y3.text, txtx3y3.text)) return true;

        // Check columns
        if (CheckThree(txtx1y1.text, txtx1y2.text, txtx1y3.text)) return true;
        if (CheckThree(txtx2y1.text, txtx2y2.text, txtx2y3.text)) return true;
        if (CheckThree(txtx3y1.text, txtx3y2.text, txtx3y3.text)) return true;

        // Check diagonals
        if (CheckThree(txtx1y1.text, txtx2y2.text, txtx3y3.text)) return true;
        if (CheckThree(txtx1y3.text, txtx2y2.text, txtx3y1.text)) return true;

        return false;
    }
    bool CheckThree(string a, string b, string c)
    {
        return !string.IsNullOrEmpty(a) && a == b && b == c;
    }
    // Update is called once per frame
    void Update()
    {
        // Check for a win condition after each move
        if (CheckWinCondition())
        {
            Debug.Log("Game Over! We have a winner!");
            StartCoroutine(ReloadSceneAfterDelay(5));
        }
    }
    IEnumerator ReloadSceneAfterDelay(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
