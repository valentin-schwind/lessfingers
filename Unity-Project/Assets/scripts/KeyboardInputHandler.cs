﻿using UnityEngine;
using System.Collections;
using System.Text;

public class KeyboardInputHandler : MonoBehaviour
{
    /*
        simple script to make the keyboard input visible on a textfield
    */
    private StringBuilder stringBuilder;
    public UnityEngine.UI.Text TextOut;
    public string Contents;



    void Awake()
    {
        stringBuilder = new StringBuilder();
    }

    public void ClearStringBuilder()
    {
        stringBuilder = new StringBuilder();
        TextOut.text = "";
        while (stringBuilder.Length > 0)
        {
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
        }

    }

    public void WriteInputOnText(string texttoadd)
    {
        /*
        Reads the users Input and writes it onto the text field of the keyboard-Object
        the input commes from the StraysButtonSkript, which gives the text of each button.
        */
        if (texttoadd == "<---")// == KeyCode.Backspace)
        {
            if (stringBuilder.Length > 0) stringBuilder.Remove(stringBuilder.Length - 1, 1);
        }
        else if (texttoadd == "SPACE")//== KeyCode.Space)
        {
            stringBuilder.Append(" ");
        }
        else
        {
            stringBuilder.Append(texttoadd);
        }
        Contents = stringBuilder.ToString();

        if (TextOut != null){
            TextOut.text = Contents;
        }
    }
}
