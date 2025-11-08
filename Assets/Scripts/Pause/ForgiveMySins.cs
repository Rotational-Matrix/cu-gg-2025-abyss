using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomNumInput : MonoBehaviour
{
    // This is poorly named
    // I am crashing out because I've been helping people with AP work
    // and the api for the unity text input isn't readable.
    // so I have created an abomination
    [SerializeField] private TMPro.TMP_Text prevValueText;
    [SerializeField] private TMPro.TMP_Text minMaxText;
    [SerializeField] private TMPro.TMP_Text displayText;
    private int max = 99;
    private readonly int min = 0;
    private char[] logicalCharArr = new char[2];
    private int nextNumPos = 0;
    private readonly string colour = "red";
    private Action<int> callback;


    /* REALLY IMPORTANT DETAIL:
     *  - AVPopup (like AYSPopup) has a very dynamic callback
     *      - AVPopup (and AYS...) will pop itself from the menu PRIOR executing its callback
     */

    

    public void InitNumInput(Action<int> callback, string valueName, string prevValue)
    {
        InitNumInput(callback, valueName, prevValue, 99, 99.ToString());
    }

    public void InitNumInput(Action<int> callback, string valueName, string prevValue, int max, string maxText)
    {
        this.callback = callback;
        for (int i = 0; i < logicalCharArr.Length; i++)
            logicalCharArr[i] = '_';
        nextNumPos = 0;
        SetPrevValText(valueName, prevValue);
        this.max = max;
        minMaxText.text = "MIN: " + min.ToString() + ", MAX: " + maxText;
    }

    public void InitNumInput(Action<int> callback)
    {
        InitNumInput(callback, null, null);
    }





    public void HandleKeyCode(KeyCode keyCode)
    {
        //I ignore the bool I made this to return
        GetNextChar(keyCode);
    }

    private void SetPrevValText(string valueName, string prevValue)
    {
        if (string.IsNullOrEmpty(valueName) || string.IsNullOrEmpty(prevValue))
            prevValueText.text = "";
        else
            prevValueText.text = "CURRENT " + valueName + " VALUE: " + prevValue;
    }


    private bool GetNextChar(KeyCode keyCode)
    {
        string toStr = keyCode.ToString();
        char inputChar = toStr[toStr.Length - 1];
        // this is an evil manoeuvre to bypass the fact that
        // the ToString of a key like 5 is "alpha5", not "5" 
        if (keyCode == KeyCode.Backspace)
            return AttemptBackspace();
        else if (char.IsDigit(inputChar))
            return AttemptNumber(inputChar);
        else if (keyCode == KeyCode.Return)
            return AttemptReturn();
        else if (keyCode == KeyCode.Escape)
            return AttemptEscape();
        else
            return false;
    }

    private bool AttemptBackspace()
    {
        if (nextNumPos > 0)
        {
            nextNumPos--;
            logicalCharArr[nextNumPos] = '_';
            UpdateText();
            return true;
        }
        return false;
    }

    private bool AttemptNumber(char inputChar)
    {
        if(nextNumPos < logicalCharArr.Length)
        {
            logicalCharArr[nextNumPos] = inputChar;
            nextNumPos++;
            UpdateText();
            return true;
        }
        else 
            return false;
    }

    private bool AttemptReturn()
    {
        if (nextNumPos == logicalCharArr.Length)
        {
            string bucketStr = "";
            for (int i = 0; i < logicalCharArr.Length; i++)
                bucketStr += logicalCharArr[i];
            StateManager.ExitTopMenu(); //exits SetVal
            callback(Constrain(int.Parse(bucketStr))); //generally, most dynamics exit and then callback
            return true;
        }
        return false;
    }

    private bool AttemptEscape()
    {
        StateManager.ExitTopMenu(); //exits SetVal
        return true;
    }

    private void UpdateText()
    {
        string bucketStr = "";
        for(int i = 0; i < logicalCharArr.Length; i++)
        {
            if (i == nextNumPos)
                bucketStr += "<color=\"" + colour + "\">" + logicalCharArr[i] + "</color>";
            else
                bucketStr += logicalCharArr[i];
        }
        displayText.text = bucketStr;
    }
    private int Constrain(int val)
    {
        if (val > max)
            return max;
        else if (val < min)
            return min;
        else
            return val;
    }


    private void Awake()
    {
        for(int i = 0; i < logicalCharArr.Length; i++)
            logicalCharArr[i] = '_';
    }




}
