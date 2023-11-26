using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public NetworkClient clientRef;
    public Button x1y1;
    public Button x2y1;
    public Button x3y1;
    public Text txtx1y1;
    public Text txtx2y1;
    public Text txtx3y1;
    public char x = 'x';
    public char o = 'o';
    public int input = 1;
    //public Button x1y2;
    //public Button x2y2;
    //public Button x3y2;
    //public TextMeshPro txtx1y2;
    //public TextMeshPro txtx2y2;
    //public TextMeshPro txtx3y2;

    //public Button x1y3;
    //public Button x2y3;
    //public Button x3y3;
    //public TextMeshPro txtx1y3;
    //public TextMeshPro txtx2y3;
    //public TextMeshPro txtx3y3;
    // Start is called before the first frame update
    void Start()
    {
        x1y1.onClick.AddListener(() => AddInput(txtx1y1));
        x2y1.onClick.AddListener(() => AddInput(txtx2y1));
        x3y1.onClick.AddListener(() => AddInput(txtx3y1));
    }
    public bool IsEven(int num)
    {
        return num % 2 == 0;
    }
    public bool IsOdd(int num)
    {
        return num % 2 != 0;
    }
    public void AddInput(Text txt)
    {
        string msg;
        if (IsOdd(input))
        {
            txt.text = x.ToString();
            msg = $"MOVE,{txt.name},{x}";
            input += 1;
        }
        else
        {
            txt.text = o.ToString();
            msg = $"MOVE,{txt.name},{o}";
        }
        clientRef.SendMessageToServer(msg);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
