using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CustomNumInput : MonoBehaviour
{
    /* NOTE: [Cu] wrote this while using the very accursed legacy 'KeyCode' system
     *  - now, with the actual InputSystem, there are better ways of doing this.
     *      - onTextInput and KeyControlDIsplayName are far better for this.
     */
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
        UpdateText();
        nextNumPos = 0;
        SetPrevValText(valueName, prevValue);
        this.max = max;
        minMaxText.text = "MIN: " + min.ToString() + ", MAX: " + maxText;
    }

    public void InitNumInput(Action<int> callback)
    {
        InitNumInput(callback, null, null);
    }





    public void HandleKey(Key key)
    {
        //I ignore the bool I made this to return
        GetNextChar(key);
    }

    private void SetPrevValText(string valueName, string prevValue)
    {
        if (string.IsNullOrEmpty(valueName) || string.IsNullOrEmpty(prevValue))
            prevValueText.text = "";
        else
            prevValueText.text = "CURRENT " + valueName + " VALUE: " + prevValue;
    }


    private bool GetNextChar(Key key)
    {
        string keyDisplayName = ProtoInputHandler.CurrentKeyboard[key].displayName;
        //Note that the DisplayName of the number keys is in fact just a string of the number
        if (key == Key.Backspace)
            return AttemptBackspace();
        else if (char.IsDigit(keyDisplayName[0]))
            return AttemptNumber(keyDisplayName[0]);
        else if (key == Key.Enter)
            return AttemptReturn();
        else if (key == Key.Escape)
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
