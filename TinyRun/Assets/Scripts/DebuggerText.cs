using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebuggerText : MonoBehaviour {

    public int maxLines = 10;
    
    private Text text;

    // Start is called before the first frame update
    void Start() {
        text = GetComponent<Text>();
    }

    
    public void Log(object obj) {
        string str = obj.ToString();

        text.text = GetLastNLines(text.text + "\n" + str, maxLines);
    }

    public void Clear() {
        text.text = "";
    }


    private int CountLines(string str) {
        return str.Split("\n").Length;
    }

    private string GetLastNLines(string str, int n) {
        string[] ar = str.Split("\n");

        if (n >= ar.Length) {
            return str;
        }

        string result = "";
        for (int i = ar.Length - n - 1; i < ar.Length; i++) {
            result += ar[i];
            if (i < ar.Length - 1) {
                result += "\n";
            }
        }
        return result;
    }
    
}
