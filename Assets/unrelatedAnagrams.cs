﻿using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using KModkit;
using UnityEngine;
using System;

public class unrelatedAnagrams : MonoBehaviour
{

    public KMAudio newAudio;
    public KMBombModule module;
    public KMBombInfo info;
    public KMSelectable[] btn;
    public MeshRenderer[] leds;
    public string[] letters;
    public string[] buttonPressOrder;

    public Material off;
    public Material green;
    public Material red;

    private static int _moduleIdCounter = 1;
    private int _moduleId = 0;

    private bool _isSolved = false, _lightsOn = false, _pressOrderSet = false;
    public int[] buttonNumbers = new int[9];
    public string[] buttonStrings = new string[9];
    public bool[] buttonStates = new bool[9];

    int initialTime = 0;
    int pressIndex = 0;
    int serialBobCondition = 0;
    bool bobActive = false;
    bool canInteract = true;

    void Start()
    {
        initialTime = (int)info.GetTime();
        _moduleId = _moduleIdCounter++;
        module.OnActivate += Activate;
    }

    private void Awake()
    {
        for (int i = 0; i < 9; i++)
        {
            int j = i;
            btn[i].OnInteract += delegate ()
            {
                handlePress(j);
                return false;
            };
        }
    }

    void Activate()
    {
        Init();
        _lightsOn = true;
    }

    void Init()
    {
        setupLetters();
        setupButtons();
        setupLEDS();
    }

    void setupLEDS()
    {
        for (int i = 0; i < 9; i++)
        {
            leds[i].material = off;
        }
    }



    void setupLetters()
    {
        letters[0] = "U";
        letters[1] = "N";
        letters[2] = "R";
        letters[3] = "E";
        letters[4] = "L";
        letters[5] = "A";
        letters[6] = "T";
        letters[7] = "E";
        letters[8] = "D";

        for (int i = 0; i < 6; i++)
        {
            string temp = "" + info.GetSerialNumber().ElementAt(i);
            if (letters.Contains(temp))
            {
                serialBobCondition++;
            }
        }
    }


    void setupButtons()
    {
        buttonNumbers[0] = UnityEngine.Random.Range(0, 9);
        buttonNumbers[1] = UnityEngine.Random.Range(0, 9);
        do
        {
            buttonNumbers[1] = UnityEngine.Random.Range(0, 9);
        } while (buttonNumbers[1] == buttonNumbers[0]);
        buttonNumbers[2] = UnityEngine.Random.Range(0, 9);
        do
        {
            buttonNumbers[2] = UnityEngine.Random.Range(0, 9);
        } while (buttonNumbers[2] == buttonNumbers[0] || buttonNumbers[2] == buttonNumbers[1]);
        buttonNumbers[3] = UnityEngine.Random.Range(0, 9);
        do
        {
            buttonNumbers[3] = UnityEngine.Random.Range(0, 9);
        } while (buttonNumbers[3] == buttonNumbers[0] || buttonNumbers[3] == buttonNumbers[1] || buttonNumbers[3] == buttonNumbers[2]);
        buttonNumbers[4] = UnityEngine.Random.Range(0, 9);
        do
        {
            buttonNumbers[4] = UnityEngine.Random.Range(0, 9);
        } while (buttonNumbers[4] == buttonNumbers[0] || buttonNumbers[4] == buttonNumbers[1] || buttonNumbers[4] == buttonNumbers[2] || buttonNumbers[4] == buttonNumbers[3]);
        buttonNumbers[5] = UnityEngine.Random.Range(0, 9);
        do
        {
            buttonNumbers[5] = UnityEngine.Random.Range(0, 9);
        } while (buttonNumbers[5] == buttonNumbers[0] || buttonNumbers[5] == buttonNumbers[1] || buttonNumbers[5] == buttonNumbers[2] || buttonNumbers[5] == buttonNumbers[3] || buttonNumbers[5] == buttonNumbers[4]);
        buttonNumbers[6] = UnityEngine.Random.Range(0, 9);
        do
        {
            buttonNumbers[6] = UnityEngine.Random.Range(0, 9);
        } while (buttonNumbers[6] == buttonNumbers[0] || buttonNumbers[6] == buttonNumbers[1] || buttonNumbers[6] == buttonNumbers[2] || buttonNumbers[6] == buttonNumbers[3] || buttonNumbers[6] == buttonNumbers[4] || buttonNumbers[6] == buttonNumbers[5]);
        buttonNumbers[7] = UnityEngine.Random.Range(0, 9);
        do
        {
            buttonNumbers[7] = UnityEngine.Random.Range(0, 9);
        } while (buttonNumbers[7] == buttonNumbers[0] || buttonNumbers[7] == buttonNumbers[1] || buttonNumbers[7] == buttonNumbers[2] || buttonNumbers[7] == buttonNumbers[3] || buttonNumbers[7] == buttonNumbers[4] || buttonNumbers[7] == buttonNumbers[5] || buttonNumbers[7] == buttonNumbers[6]);
        buttonNumbers[8] = UnityEngine.Random.Range(0, 9);
        do
        {
            buttonNumbers[8] = UnityEngine.Random.Range(0, 9);
        } while (buttonNumbers[8] == buttonNumbers[0] || buttonNumbers[8] == buttonNumbers[1] || buttonNumbers[8] == buttonNumbers[2] || buttonNumbers[8] == buttonNumbers[3] || buttonNumbers[8] == buttonNumbers[4] || buttonNumbers[8] == buttonNumbers[5] || buttonNumbers[8] == buttonNumbers[6] || buttonNumbers[8] == buttonNumbers[7]);


        for (int i = 0; i < 9; i++)
        {
            buttonStrings[i] = letters[buttonNumbers[i]];
            buttonStates[i] = false;
            btn[i].GetComponentInChildren<TextMesh>().text = buttonStrings[i];
        }


    }
    void setupPressOrder(bool log)
    {

        bobActive = false;
        if (info.GetOffIndicators().ToList().Contains("BOB") && serialBobCondition > 1)
        {
            bobActive = true;
            buttonPressOrder = new[] {"U", "N", "R", "E", "L", "A", "T", "E", "D"};
            if (log)
                Debug.LogFormat("[Unrelated Anagrams #{0}] Unlit BOB found, serial number: {1}. Sequence: UNRELATED", _moduleId, info.GetSerialNumber());
        }
        else if (info.GetOnIndicators().ToList().Count() > 2)
        {
            buttonPressOrder = new[] {"U", "N", "D", "E", "R", "T", "A", "L", "E"};
            if (log)
                Debug.LogFormat("[Unrelated Anagrams #{0}] Lit indicator count: {1}. Initial sequence: UNDERTALE", _moduleId, info.GetOnIndicators().ToList().Count());
        }
        else if (info.GetOffIndicators().ToList().Count() > 2)
        {
            buttonPressOrder = new[] {"D", "E", "L", "T", "A", "R", "U", "N", "E"};
            if (log)
                Debug.LogFormat("[Unrelated Anagrams #{0}] Unlit indicator count: {1}. Initial sequence: DELTARUNE", _moduleId, info.GetOffIndicators().ToList().Count());
        }
        else if (info.GetSolvedModuleNames().Count() == 8)
        {
            buttonPressOrder = new[] {"N", "U", "D", "E", "A", "L", "E", "R", "T"};
            if (log)
                Debug.LogFormat("[Unrelated Anagrams #{0}] Solved module count: 8. Initial sequence: NUDE ALERT", _moduleId);
        }
        else if (info.GetModuleNames().Count() < 6)
        {
            buttonPressOrder = new[] {"A", "N", "T", "D", "U", "E", "L", "E", "R"};
            if (log)
                Debug.LogFormat("[Unrelated Anagrams #{0}] Module count: {1}. Initial sequence: ANT DUELER", _moduleId, info.GetModuleNames().Count());
        }
        else if (info.GetModuleNames().Count() - info.GetSolvableModuleNames().Count() > 1 || info.GetTime() <= 60)
        {
            buttonPressOrder = new[] {"U", "L", "T", "R", "A", "N", "E", "E", "D"};
            if (log)
                Debug.LogFormat("[Unrelated Anagrams #{0}] Needy module count: {1}, time left: {2}. Initial sequence: ULTRA NEED", _moduleId, info.GetModuleNames().Count() - info.GetSolvableModuleNames().Count(), info.GetTime());
        }
        else if (initialTime >= 600)
        {
            buttonPressOrder = new[] {"E", "L", "D", "E", "R", "A", "U", "N", "T"};
            if (log)
                Debug.LogFormat("[Unrelated Anagrams #{0}] Initial time: {1}. Initial sequence: ELDER AUNT", _moduleId, initialTime);
        }
        else if (info.GetPortCount(Port.PS2) + info.GetPortCount(Port.StereoRCA) * 2 + info.GetPortCount(Port.ComponentVideo) * 3 + info.GetPortCount(Port.CompositeVideo) > 2)
        {
            buttonPressOrder = new[] {"N", "U", "T", "L", "E", "A", "D", "E", "R"};
            if (log)
                Debug.LogFormat("[Unrelated Anagrams #{0}] PS/2 ports: {1}, Stereo RCA ports: {2}, Component Video ports: {3}, Composite Video ports: {4}. Initial sequence: NUT LEADER", _moduleId, info.GetPortCount(Port.PS2), info.GetPortCount(Port.StereoRCA), info.GetPortCount(Port.ComponentVideo), info.GetPortCount(Port.CompositeVideo));
        }
        else if (info.GetSerialNumber().Contains("D") || info.GetSerialNumber().Contains("E"))
        {
            buttonPressOrder = new[] {"N", "E", "U", "T", "R", "A", "L", "E", "D"};
            if (log)
                Debug.LogFormat("[Unrelated Anagrams #{0}] Serial number: {1}. Initial sequence: NEUTRAL ED", _moduleId, info.GetSerialNumber());
        }
        else
        {
            buttonPressOrder = new[] {"U", "N", "R", "E", "L", "A", "T", "E", "D"};
            if (log)
                Debug.LogFormat("[Unrelated Anagrams #{0}] None of the conditions apply. Initial sequence: UNRELATED", _moduleId);
        }

        if (!bobActive)
        {
            if (info.GetBatteryCount(Battery.AA) > 0 && log)
            {
                Debug.LogFormat("[Unrelated Anagrams #{0}] AA Battery count: {1}. Right shifting sequence {1} times.", _moduleId, info.GetBatteryCount(Battery.AA));
            }
            for (int aa = 0; aa < info.GetBatteryCount(Battery.AA); aa++)
            {
                string lastButton = buttonPressOrder[8];
                for (int i = 8; i > 0; i--)
                {
                    buttonPressOrder[i] = buttonPressOrder[i - 1];
                }
                buttonPressOrder[0] = lastButton;
            }
            if (info.GetBatteryCount(Battery.D) > 0 && log)
            {
                Debug.LogFormat("[Unrelated Anagrams #{0}] D Battery count: {1}. Left shifting sequence {1} times.", _moduleId, info.GetBatteryCount(Battery.D));
            }
            for (int d = 0; d < info.GetBatteryCount(Battery.D); d++)
            {
                string firstButton = buttonPressOrder[0];
                for (int i = 0; i < 8; i++)
                {
                    buttonPressOrder[i] = buttonPressOrder[i + 1];
                }
                buttonPressOrder[8] = firstButton;
            }
            if (info.GetPortCount() % 2 == 1)
            {
                if (log)
                    Debug.LogFormat("[Unrelated Anagrams #{0}] Port count is odd ({1}). Reversing sequence.", _moduleId, info.GetPortCount());
                buttonPressOrder = new[] {buttonPressOrder[8], buttonPressOrder[7], buttonPressOrder[6], buttonPressOrder[5], buttonPressOrder[4], buttonPressOrder[3], buttonPressOrder[2], buttonPressOrder[1], buttonPressOrder[0]};
            }
            if (log)
                Debug.LogFormat("[Unrelated Anagrams #{0}] Sequence: {1}{2}{3}{4}{5}{6}{7}{8}{9}", _moduleId, buttonPressOrder[0], buttonPressOrder[1], buttonPressOrder[2], buttonPressOrder[3], buttonPressOrder[4], buttonPressOrder[5], buttonPressOrder[6], buttonPressOrder[7], buttonPressOrder[8]);
        }
    }
    void resetGame()
    {
        //setupButtons();
        _pressOrderSet = false;
        setupLEDS();
    }
    void handlePress(int btnIndex)
    {
        newAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, btn[btnIndex].transform);
        btn[btnIndex].AddInteractionPunch(.5f);
        if (!_lightsOn || _isSolved || !canInteract) return;

        if (!_pressOrderSet)
        {
            setupPressOrder(true);
            _pressOrderSet = true;
        }

        if (buttonPressOrder[pressIndex] == buttonStrings[btnIndex] && !buttonStates[btnIndex])
        {
            pressIndex++;
            buttonStates[btnIndex] = true;
            leds[btnIndex].material = green;
            Debug.LogFormat("[Unrelated Anagrams #{0}] Correct button pressed. Input received: button {1} at index {2}.",_moduleId, buttonStrings[btnIndex], btnIndex);
        } else
        {
            module.HandleStrike();
            Debug.LogFormat("[Unrelated Anagrams #{0}] Incorrect button pressed. Expected: {1}. Received: {2}.",_moduleId, buttonPressOrder[pressIndex], buttonStrings[btnIndex]);
            //Debug.LogFormat("[Unrelated Anagrams #{0}] If you feel that this strike is an error, please contact AAces as soon as possible so we can get this error sorted out. Have a copy of this log file handy. Discord: AAces#0908", _moduleId);
            pressIndex = 0;
            for (int i = 0; i < 9; i++)
            {
                leds[i].material = red;
            }
            StartCoroutine(RedLights());
            return;
        }
        if (pressIndex == 9)
        {
            module.HandlePass();
            newAudio.PlaySoundAtTransform(UnityEngine.Random.Range(0,100) != 0 ? "solve" : "ohyes", transform);
            _isSolved = true;
            Debug.LogFormat("[Unrelated Anagrams #{0}] Module solved!", _moduleId);
        }
    }
    IEnumerator RedLights()
    {
        canInteract = false;
        for (int i = 0; i < 9; i++)
        {
            leds[i].material = red;
            buttonStates[i] = false;
        }
        yield return new WaitForSeconds(2.0f);
        resetGame();
        canInteract = true;
    }

    private List<int> AllIndexesOf(string str, string searchstring)
    {
        List<int> temp = new List<int>();
        for (int i = 0; i < searchstring.Length; i++)
            if (searchstring[i] == str.ToCharArray()[0])
                temp.Add(i);
        return temp;
    }

    #pragma warning disable 414
    private string TwitchHelpMessage = "Use !{0} press RAD TUNE to press the buttons with the respective letters. You can also use numbers, the keys are numbered in reading order starting from 1";
    #pragma warning restore 414
    public KMSelectable[] ProcessTwitchCommand(string command)
    {
        command = command.Trim().ToUpperInvariant();
        if (!command.StartsWith("PRESS")) return null;

        command = command.Substring(6);
        for (int i = 0; i < command.Length; i++)
            if (!command[i].EqualsAny('U', 'N', 'D', 'E', 'R', 'T', 'A', 'L', '1', '2', '3', '4', '5', '6', '7', '8', '9')) return null;
        List<int> ButtonsPressed = new List<int> { };
        List<KMSelectable> Buttons = new List<KMSelectable> { };
        if (Regex.IsMatch(command, "[UNDERTAL]"))
        {
            foreach (Match buttonIndexString in Regex.Matches(command, "[UNDERTAL]"))
            {
                int buttonIndex = 0;
                while (buttonIndexString.Value != buttonStrings[buttonIndex] || ButtonsPressed.Contains(buttonIndex) || buttonStates[buttonIndex])
                {
                    if (++buttonIndex == 9) break;
                }

                if (buttonIndex >= 0 && buttonIndex < 9)
                {
                    ButtonsPressed.Add(buttonIndex);
                    Buttons.Add(btn[buttonIndex]);
                }
            }

            if (Buttons.Count == 0) return null;
            else return Buttons.ToArray();
        } else
        {
            foreach (Match buttonIndexString in Regex.Matches(command, "[1-9]"))
            {
                int buttonIndex;
                if (!int.TryParse(buttonIndexString.Value, out buttonIndex))
                {
                    continue;
                }

                buttonIndex--;

                if (buttonIndex >= 0 && buttonIndex < 9)
                {
                    if (ButtonsPressed.Contains(buttonIndex))
                        continue;

                    ButtonsPressed.Add(buttonIndex);
                    Buttons.Add(btn[buttonIndex]);
                }
            }

            if (Buttons.Count == 0) return null;
            else return Buttons.ToArray();
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        setupPressOrder(false);
        int start = pressIndex;
        for (int i = start; i < 9; i++)
        {
            List<int> temp = AllIndexesOf(buttonPressOrder[i], buttonStrings.Join(""));
            if (buttonPressOrder[i] == "E")
            {
                for (int j = 0; j < 2; j++)
                {
                    if (!buttonStates[temp[j]])
                    {
                        btn[temp[j]].OnInteract();
                        yield return new WaitForSeconds(0.1f);
                        break;
                    }
                }
            }
            else
            {
                btn[temp[0]].OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
