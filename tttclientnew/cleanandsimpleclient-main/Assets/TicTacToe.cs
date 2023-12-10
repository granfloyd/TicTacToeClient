using UnityEngine;
using UnityEngine.UI;

public class TicTacToe : MonoBehaviour
{
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

    public int input = 0;
    public bool isMyTurn = false;
    public bool isWinConditionMet = false;

    void Start()
    {
        //make sure waiting ui is not on screen when game is loaded
        GameObject needsToGo2 = GameObject.FindGameObjectWithTag("Waiting");
        Destroy(needsToGo2);

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
        if (!isMyTurn)
            return;
        isMyTurn = false;
        string msg = ClientToServerSignifiers.DisplayMove.ToString() + ',' +
            txt.name;
        NetworkClientProcessing.SendMessageToServer(msg, TransportPipeline.ReliableAndInOrder);
        

        input += 1;
        if (input == 5)
        {
            string msgloser = ClientToServerSignifiers.Loser.ToString();
            NetworkClientProcessing.SendMessageToServer(msgloser, TransportPipeline.ReliableAndInOrder);
            ResetGame();
        }
    }

    bool CheckWinCondition()
    {
        // Check rows
        if (CheckThree(txtx1y1.text, txtx2y1.text, txtx3y1.text)) return SetWinCondition(true);
        if (CheckThree(txtx1y2.text, txtx2y2.text, txtx3y2.text)) return SetWinCondition(true);
        if (CheckThree(txtx1y3.text, txtx2y3.text, txtx3y3.text)) return SetWinCondition(true);

        // Check columns
        if (CheckThree(txtx1y1.text, txtx1y2.text, txtx1y3.text)) return SetWinCondition(true);
        if (CheckThree(txtx2y1.text, txtx2y2.text, txtx2y3.text)) return SetWinCondition(true);
        if (CheckThree(txtx3y1.text, txtx3y2.text, txtx3y3.text)) return SetWinCondition(true);

        // Check diagonals
        if (CheckThree(txtx1y1.text, txtx2y2.text, txtx3y3.text)) return SetWinCondition(true);
        if (CheckThree(txtx1y3.text, txtx2y2.text, txtx3y1.text)) return SetWinCondition(true);

        return false;
    }

    bool CheckThree(string a, string b, string c)
    {
        // Checks if 3 strings  are ==   
        return !string.IsNullOrEmpty(a) && a == b && b == c;
    }

    bool SetWinCondition(bool b)
    {
        isWinConditionMet = b;
        return b;
    }

    public void ResetTxt()
    {
        string s = "";
        txtx1y1.text = s;
        txtx2y1.text = s;
        txtx3y1.text = s;

        txtx1y2.text = s;
        txtx2y2.text = s;
        txtx3y2.text = s;

        txtx1y3.text = s;
        txtx2y3.text = s;
        txtx3y3.text = s;
    }

    public void ResetGame()
    {
        // Reset everything
        ResetTxt();
        input = 0;
        isWinConditionMet = false;
        isMyTurn = false;
    }

    private void Update()
    {
        if (CheckWinCondition())
        {
            Debug.Log("Game Over! We have a winner!");

            string msgwinner = ClientToServerSignifiers.Winner.ToString();
            NetworkClientProcessing.SendMessageToServer(msgwinner, TransportPipeline.ReliableAndInOrder);

            ResetGame();
        }
    }
}

