using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class DialogueHandler
{
    /* Readonly */
    public Dialogue dialogue { get; private set; }
    public TextAsset asset { get; private set; }
    public int currentLine { get; private set; }
    public bool isAnimating { get { return ui.isAnimating; } }

    public Branch currentBranch { get; private set; }
    public List<Branch> currentValidBranches { get; private set; }

    /*
     * Settings
    */
    public DialogueSettings settings;
    public DialogueCallbackActions callbackActions;

    //Voice stuff
    public DialogueAudio audio;
    public AudioClip voiceClip;
    public VoiceCuts[] voiceCuts;
    public float voiceCutDelay = .5f;

    StringBuilder str = new StringBuilder();

    public List<GameEventFlag> gameEventFlags { get; set; }
    private DialogueUI ui;

    /* Higher order functions? */
    public Func<Condition, bool> CustomTestCondition { get; set; }

    public DialogueHandler(DialogueUI ui, DialogueSettings settings = null, DialogueCallbackActions callbackActions = null)
    {
        this.ui = ui;
        this.settings = settings ?? new DialogueSettings();
        this.callbackActions = callbackActions ?? new DialogueCallbackActions();
    }


    public bool LoadDialogue(string pathToResourceFile)
    {
        if(pathToResourceFile != null && pathToResourceFile != "")
        {
            asset = Resources.Load<TextAsset>(pathToResourceFile);
            if(asset != null)
            {
                dialogue = JsonUtility.FromJson<Dialogue>(asset.ToString());
                GetPossibleBranches();
                SetCurrentBranch();
                return true;
            }
            Debug.LogError($"dialogue file at {pathToResourceFile} could not be found.");
            return false;
        }
        return false;
    }

    public void SetUI(DialogueUI ui)
    {
        this.ui = ui;
    }

    public void SetCallbackActions(DialogueCallbackActions callbackActions)
    {
        this.callbackActions = callbackActions;
    }

    private void GetPossibleBranches()
    {
        List<Branch> validBranches = new List<Branch>();
        foreach (Branch branch in dialogue.Branches)
        {
            List<bool> evaluations = new List<bool>();
            foreach(Condition condition in branch.Conditions)
            {
                bool eval;
                if(CustomTestCondition != null)
                {
                    eval = CustomTestCondition(condition);
                } else
                {
                    eval = TestCondition(condition);
                }
                evaluations.Add(eval);
            }

            if (evaluations.TrueForAll(x => x == true)) validBranches.Add(branch);
        }
        currentValidBranches = validBranches;
    }

    private void SetCurrentBranch()
    {
        if(currentValidBranches.Count > 0)
        {
            switch(settings.multipleValidBranchesSelectionMode)
            {
                case DialogueSettings.MultipleValidBranchesSelectionMode.FIRST:
                    currentBranch = currentValidBranches[0];
                    break;
                case DialogueSettings.MultipleValidBranchesSelectionMode.PRIORITY:
                    currentBranch = currentValidBranches[0];
                    break;
                case DialogueSettings.MultipleValidBranchesSelectionMode.RANDOM:
                    int randInt = Random.Range(0, currentValidBranches.Count - 1);
                    currentBranch = currentValidBranches[randInt];
                    break;
                default:
                    break;
            }
        }
    }

    private bool TestCondition(Condition condition)
    {
        if(gameEventFlags != null)
        {
            GameEventFlag flag = gameEventFlags.Find(x => x.name.ToLower() == condition.Flag.ToLower());
            if(flag != null)
            {
                return flag.active == condition.MustBe;
            } else
            {
                return false;
            }
        } 
        else
        {
            Debug.LogError("Game event flags are not set!");
            return false;
        }
    }

    public void DisplayLine(int num, TextEffects.TextDisplayMode textAnimation)
    {
        currentLine = num;
        string processedLine = ProcessText(currentBranch.Lines[num].Text);
        ui.SetDialogueText(processedLine, textAnimation, InvokeCallbacks);
    }

    private void InvokeCallbacks()
    {
        if(callbackActions != null)
        {
            if(currentLine == currentBranch.Lines.Length - 1)
            {
                callbackActions.OnBranchEnd.Invoke(currentBranch);
                callbackActions.OnBranchEndEvents.Invoke();
            }

            // On dialogue end??
        }
    }

    string ProcessText(string line)
    {
        str.Clear();
        ui.Reset();

        List<TextCommand> AllCmdObjects = new List<TextCommand>();
        List<int> CmdObjectIndex = line.AllIndexesOf("{");

        int index = 0;
        for (int i = 0; i < line.Length; ++i)
        {
            if (CmdObjectIndex.Contains(i))
            {
                int cmdEndIndex = line.IndexOf('}', i);
                string substring = line.Substring(i, cmdEndIndex - i + 1);
                TextCommand cmd = JsonUtility.FromJson<TextCommand>(substring);
                switch (cmd.effect)
                {
                    case "wave":

                        ui.textEffects.SetIndices(index, index + cmd.text.Length, TextEffects.Effect.WAVE);
                        break;
                    default:
                        break;
                }
                if (cmd.color != null)
                {
                    ui.textEffects.SetColorIndices(index, index + cmd.text.Length, cmd.color);
                }
                str.Append(cmd.text);
                i = cmdEndIndex;
                index += cmd.text.Length;
            }
            else
            {
                str.Append(line[i]);
                index++;
            }
        }

        return str.ToString();
    }

    //public override void Interact()
    //{
    //    if (!isInteracting)
    //    {
    //        RotateTowardsPlayer();
    //        ui.ToggleDialoguePanel(true);
    //        isInteracting = true;
    //        timeCount = 0;
    //        isRotatingToPlayer = true;
    //        sm.SetDialogueCamera(transform);

    //        foreach (Branch branch in dialogue.Branches)
    //        {
    //            GameEventFlag flag = evt.flags.Find(x => x.name == branch.Condition);
    //            if (flag != null && flag.active)
    //            {
    //                numLines = branch.Lines.Length;
    //                currentBranch = branch;
    //                DisplayLine(0);
    //                /*string line = branch.Lines[0];
    //                string processedLine = ProcessText(line);
    //                ui.SetDialogueText(processedLine, TextEffects.TextDisplayMode.TYPEWRITER);
    //                StartCoroutine(DisplayText(processedLine));
    //                ignoreFlags.Add(flag);*/
    //            }
    //        }
    //    }
    //    else
    //    {
    //        if (isAnimating)
    //        {
    //            Debug.Log("Im currently animating a line, lets display the whole shebang!");
    //            ui.SkipText();
    //            return;
    //        }
    //        else
    //        {
    //            if (currentLine + 1 < numLines)
    //            {
    //                currentLine++;
    //                Debug.Log("Im currently animating a line, lets display the whole shebang!");
    //                DisplayLine(currentLine++);
    //            }
    //            else
    //            {
    //                EndInteraction();
    //            }
    //        }
    //    }
    //}

    //public void EndInteraction()
    //{
    //    sm.EndInteraction();
    //    isInteracting = false;
    //    ui.ToggleDialoguePanel(false);
    //}

    //void textFinishedCallback()
    //{
    //    isAnimating = false;
    //}

    //private void RotateTowardsPlayer()
    //{
    //    Vector3 dir = (sm.mTransform.position - transform.position).normalized;
    //    timeCount += 0.05f ;
    //    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), timeCount);//Quaternion.LookRotation(dir);

    //    if(timeCount >= 0.98)
    //    {
    //        isRotatingToPlayer = false;
    //        timeCount = 0f;
    //    }
    //}

    //private void Update()
    //{
    //    if(isRotatingToPlayer)
    //    {
    //        RotateTowardsPlayer();
    //    }
    //}

}

[Serializable]
public class VoiceCuts
{
    public float startTime = 0f;
    public float endTime = 1f;
    public float pitch = 1f;
    public bool reverse = false;
}

[Serializable]
public class TextCommand
{
    public string effect;
    public string color;
    public string text;
}

public class TestEvents
{
    public bool Testi = true;
    public Dictionary<string, bool> testy;
    public List<GameEventFlag> flags = new List<GameEventFlag>();

    public TestEvents()
    {
        testy = new Dictionary<string, bool>();
        testy.Add("testo", true);
        flags.Add(new GameEventFlag("B", true));
    }
}

public class GameEventFlag
{
    public string name;
    public bool active;
    public GameEventFlag(string name, bool active)
    {
        this.name = name;
        this.active = active;
    }
}

public class JsonHelper
{
    public static T[] getJsonArray<T>(string json)
    {
        string newJson = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}

