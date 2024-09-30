using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Rnd = UnityEngine.Random;
using KModkit;

public class SymbolPadScript : MonoBehaviour
{
    public KMBombModule Module;
    public KMBombInfo BombInfo;

    public Material[] Colors;
    public KMAudio Audio;
    public TextMesh[] Symbols;
    public KMSelectable[] Buttons;

    public GameObject[] RealButtons;


    private int _moduleId;
    private static int _moduleIdCounter = 1;
    private bool _moduleSolved;
    


    int portCount;
    private List<int> numberList = new List<int>();
    private string[][] theTable = new string[][]{
    new String[]{"☘", "⟅", "⚘", "⛻", "⚗", "☄︎", ":", ";", "▦", "⚙", "⚑", "⚜", "⛼", "⛫", "ↆ", "℧"},
    new String[]{"⚙", "ↆ", "⛫", "℧", ":", "☘", "⚘", "⛼", "⚗", "⟅", "⛻", ";", "⚜", "☄︎", "⚑", "▦"},
    new String[]{"ↆ", "⚘", "⚗", "☄︎", "▦", "⚑", ";", "☘", "⛻", "⟅", ":", "⚙", "℧", "⛼", "⚜", "⛫"},
    new String[]{"⛼", "⚜", "⚗", ":", "℧", ";", "⚑", "⟅", "⛫", "☘", "☄︎", "⛻", "⚙", "⚘", "▦", "ↆ"},
    new String[]{"℧", "⛼", ":", ";", "⚜", "⛫", "☄︎", "⚑", "ↆ", "⛻", "☘", "▦", "⟅", "⚗", "⚙", "⚘"},
    new String[]{"☄︎", "⚑", "⛼", "⚜", "⛻", "▦", "ↆ", "⚙", "⟅", "⚘", "℧", "⛫", ";", "⚗", ":", "☘"},
    new String[]{"⛻", ";", "⚙", "⛫", "ↆ", "℧", "⚗", "⟅", "☘", "☄︎", "⚘", ":", "▦", "⚑", "⛼", "⚜"},
    new String[]{"⚜", ":", "⚘", "▦", "⚙", ";", "⛫", "℧", "⚗", "ↆ", "⟅", "⛼", "☄︎", "☘", "⛻", "⚑"},
    new String[]{"⟅", "⛫", "⛻", "⚑", "☘", "⛼", "▦", "⚘", "☄︎", "⚜", "⚙", "ↆ", ":", ";", "℧", "⚗"}};
    int number;
    int[] arr;
    int progress = 0;

    private string[] PosNames = new string[]{"Top-Left", "Top-Middle", "Top-Right", "Middle-Left", "Center", "Middle-Right", "Bottom-Left", "Bottom-Middle", "Bottom-Right"};

    private void Start()
    {
        _moduleId = _moduleIdCounter++;
        for (int i = 0; i < 9; i++)
        {
            int j = i;
            Buttons[i].OnInteract += delegate () { ButtonPress(j); return false; };
        }
        
        GenerateMod();
    }


    void GenerateMod(){


        //Checking for edge cases
        if (BombInfo.GetPortCount() == 0){
            portCount = 1;
        }
        else{
           portCount = ((BombInfo.GetPortCount() - 1) % 16) + 1; 
        }

        portCount--;

        Debug.LogFormat("[Symbol Pad #{0}] The starting cell is {1}.", _moduleId, portCount + 1);

        arr = Enumerable.Range(0, 9).ToArray().Shuffle();

        Debug.LogFormat("[Symbol Pad #{0}] The order is {1}.", _moduleId, arr.Select(jimbo => PosNames[jimbo]).Join(", "));


        //Generate the 9 different unique numbers
        while (numberList.Count() < 9)
    {
        number = Rnd.Range(0, 16);


        if (!numberList.Contains(number))
        {
            numberList.Add(number);
        }
    }

    //Making the buttons display the symbols



        List<int> orderedNumbers = new List<int>();

        for (int i = 0; i < 16; i++)
        {
            int currentValue = (portCount + i) % 16; // Circular increment

            // If the number exists in the list, add it to the ordered result
            if (numberList.Contains(currentValue))
            {
                orderedNumbers.Add(currentValue);
            }

            // Stop once we've looped back to the start value
            if (currentValue == portCount && i > 0)
            {
                break;
            }
        }

        for (int i = 0; i < 9; i++){
            Symbols[arr[i]].text = theTable[arr[i]][orderedNumbers[i]];
        }
        

    }

    void ButtonPress(int borg){
        Buttons[borg].AddInteractionPunch(0.5f);
        if (_moduleSolved){
            return;
        }
        if (arr[progress] == borg){
            progress++;
            RealButtons[borg].GetComponent<MeshRenderer>().material = Colors[1];
            if (progress == 9){
                Debug.LogFormat("[Symbol Pad #{0}] Module Solved!", _moduleId);
                Module.HandlePass();
                _moduleSolved = true;
                StartCoroutine(SolveAnimation());
            }
        }
        else{
            progress = 0;
            for (int i = 0; i < 9; i++){
                RealButtons[i].GetComponent<MeshRenderer>().material = Colors[0];
            }
            Debug.LogFormat("[Symbol Pad #{0}] Incorrectly pressed the {1} button. Strike!", _moduleId, PosNames[borg]);
            Module.HandleStrike();
        }
    }

    IEnumerator SolveAnimation(){

        var time = 0.4f;

        //The Setup

        RealButtons[0].GetComponent<MeshRenderer>().material = Colors[4];
        yield return new WaitForSeconds(time);
        RealButtons[0].GetComponent<MeshRenderer>().material = Colors[3];
        RealButtons[1].GetComponent<MeshRenderer>().material = Colors[4];
        RealButtons[3].GetComponent<MeshRenderer>().material = Colors[4];
        yield return new WaitForSeconds(time);
        RealButtons[0].GetComponent<MeshRenderer>().material = Colors[2];
        RealButtons[1].GetComponent<MeshRenderer>().material = Colors[3];
        RealButtons[3].GetComponent<MeshRenderer>().material = Colors[3];
        RealButtons[2].GetComponent<MeshRenderer>().material = Colors[4];
        RealButtons[4].GetComponent<MeshRenderer>().material = Colors[4];
        RealButtons[6].GetComponent<MeshRenderer>().material = Colors[4];
        yield return new WaitForSeconds(time);
        RealButtons[0].GetComponent<MeshRenderer>().material = Colors[1];
        RealButtons[1].GetComponent<MeshRenderer>().material = Colors[2];
        RealButtons[3].GetComponent<MeshRenderer>().material = Colors[2];
        RealButtons[2].GetComponent<MeshRenderer>().material = Colors[3];
        RealButtons[4].GetComponent<MeshRenderer>().material = Colors[3];
        RealButtons[6].GetComponent<MeshRenderer>().material = Colors[3];
        RealButtons[5].GetComponent<MeshRenderer>().material = Colors[4];
        RealButtons[7].GetComponent<MeshRenderer>().material = Colors[4];
        yield return new WaitForSeconds(time);
        RealButtons[0].GetComponent<MeshRenderer>().material = Colors[0];
        RealButtons[1].GetComponent<MeshRenderer>().material = Colors[1];
        RealButtons[3].GetComponent<MeshRenderer>().material = Colors[1];
        RealButtons[2].GetComponent<MeshRenderer>().material = Colors[2];
        RealButtons[4].GetComponent<MeshRenderer>().material = Colors[2];
        RealButtons[6].GetComponent<MeshRenderer>().material = Colors[2];
        RealButtons[5].GetComponent<MeshRenderer>().material = Colors[3];
        RealButtons[7].GetComponent<MeshRenderer>().material = Colors[3];
        RealButtons[8].GetComponent<MeshRenderer>().material = Colors[4];
        yield return new WaitForSeconds(time);


        //The loop
        while (true){
        RealButtons[0].GetComponent<MeshRenderer>().material = Colors[4];
        RealButtons[1].GetComponent<MeshRenderer>().material = Colors[0];
        RealButtons[3].GetComponent<MeshRenderer>().material = Colors[0];
        RealButtons[2].GetComponent<MeshRenderer>().material = Colors[1];
        RealButtons[4].GetComponent<MeshRenderer>().material = Colors[1];
        RealButtons[6].GetComponent<MeshRenderer>().material = Colors[1];
        RealButtons[5].GetComponent<MeshRenderer>().material = Colors[2];
        RealButtons[7].GetComponent<MeshRenderer>().material = Colors[2];
        RealButtons[8].GetComponent<MeshRenderer>().material = Colors[3];
        yield return new WaitForSeconds(time);
        RealButtons[0].GetComponent<MeshRenderer>().material = Colors[3];
        RealButtons[1].GetComponent<MeshRenderer>().material = Colors[4];
        RealButtons[3].GetComponent<MeshRenderer>().material = Colors[4];
        RealButtons[2].GetComponent<MeshRenderer>().material = Colors[0];
        RealButtons[4].GetComponent<MeshRenderer>().material = Colors[0];
        RealButtons[6].GetComponent<MeshRenderer>().material = Colors[0];
        RealButtons[5].GetComponent<MeshRenderer>().material = Colors[1];
        RealButtons[7].GetComponent<MeshRenderer>().material = Colors[1];
        RealButtons[8].GetComponent<MeshRenderer>().material = Colors[2];
        yield return new WaitForSeconds(time);
        RealButtons[0].GetComponent<MeshRenderer>().material = Colors[2];
        RealButtons[1].GetComponent<MeshRenderer>().material = Colors[3];
        RealButtons[3].GetComponent<MeshRenderer>().material = Colors[3];
        RealButtons[2].GetComponent<MeshRenderer>().material = Colors[4];
        RealButtons[4].GetComponent<MeshRenderer>().material = Colors[4];
        RealButtons[6].GetComponent<MeshRenderer>().material = Colors[4];
        RealButtons[5].GetComponent<MeshRenderer>().material = Colors[0];
        RealButtons[7].GetComponent<MeshRenderer>().material = Colors[0];
        RealButtons[8].GetComponent<MeshRenderer>().material = Colors[1];
        yield return new WaitForSeconds(time);
        RealButtons[0].GetComponent<MeshRenderer>().material = Colors[1];
        RealButtons[1].GetComponent<MeshRenderer>().material = Colors[2];
        RealButtons[3].GetComponent<MeshRenderer>().material = Colors[2];
        RealButtons[2].GetComponent<MeshRenderer>().material = Colors[3];
        RealButtons[4].GetComponent<MeshRenderer>().material = Colors[3];
        RealButtons[6].GetComponent<MeshRenderer>().material = Colors[3];
        RealButtons[5].GetComponent<MeshRenderer>().material = Colors[4];
        RealButtons[7].GetComponent<MeshRenderer>().material = Colors[4];
        RealButtons[8].GetComponent<MeshRenderer>().material = Colors[0];
        yield return new WaitForSeconds(time);
        RealButtons[0].GetComponent<MeshRenderer>().material = Colors[0];
        RealButtons[1].GetComponent<MeshRenderer>().material = Colors[1];
        RealButtons[3].GetComponent<MeshRenderer>().material = Colors[1];
        RealButtons[2].GetComponent<MeshRenderer>().material = Colors[2];
        RealButtons[4].GetComponent<MeshRenderer>().material = Colors[2];
        RealButtons[6].GetComponent<MeshRenderer>().material = Colors[2];
        RealButtons[5].GetComponent<MeshRenderer>().material = Colors[3];
        RealButtons[7].GetComponent<MeshRenderer>().material = Colors[3];
        RealButtons[8].GetComponent<MeshRenderer>().material = Colors[4];
        yield return new WaitForSeconds(time);
        }
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = "!{0} press tl tm tr ml mm mr bl bm br [Press the buttons in those positions.]";
#pragma warning restore 414

    private IEnumerator ProcessTwitchCommand(string command){
        command = command.ToLowerInvariant().Trim();
        if (!command.StartsWith("press ")){
            yield break;
        }
        command = command.Substring(6);

        var lonelySandwichShop = new string[]{"tl", "tm", "tr", "ml", "mm", "mr", "bl", "bm", "br"};

        var gusFring = command.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

        var blananaCremePie = new List<int>();

        if (gusFring.Length != 9){
            yield break;
        }

        for (int i = 0; i < 9; i++){
            int how = Array.IndexOf(lonelySandwichShop, gusFring[i]);
            if (how == -1){
                yield break;
            }
            blananaCremePie.Add(how);
        }

        if (blananaCremePie.Distinct().Count() != 9){
            yield break;
        }
        yield return null;

        for (int k = 0; k < 9; k++){
            Buttons[blananaCremePie[k]].OnInteract();
            yield return new WaitForSeconds(0.099f);
        }
    }

    private IEnumerator TwitchHandleForcedSolve(){
        while (!_moduleSolved){
            Buttons[arr[progress]].OnInteract();
            yield return new WaitForSeconds(0.099f);
        }
    }



}
