using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleTriggerDialogue : MonoBehaviour
{
    public string folderName;
    public string fileName;

    public string testDict = "First initial value";

    DialogueSystem dialogueSystem;
    SampleFlags flags;
    List<DialogueGameState> gameStateVariables = new List<DialogueGameState>();

    public DialogueTheme alternativeTheme;
    public DialogueGraph graph;

    public SampleUIHandler UiHandler;

    public Transform testCube;


    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        dialogueSystem = DialogueSystem.instance;
        flags = GetComponent<SampleFlags>();

        dialogueSystem.flags = flags.gameEventFlags;
        dialogueSystem.dialogueCallbackActions.OnNodeLeave += OnNodeLeave;
        dialogueSystem.dialogueCallbackActions.OnNodeEnter += OnNodeEnter;

        //dialogueSystem.CustomTestCondition = CustomTestCondition;

        //NodeParser parser = new NodeParser();
        //parser.GenerateGraphFromScript(graph);

        gameStateVariables.Add(new DialogueGameState(10f, "floatExample"));

        dialogueSystem.SetDialogGameState(gameStateVariables);
        dialogueSystem.OnChoiceDraw += OnChoiceDraw;

        dialogueSystem.dictionary.AddEntry("test", GetDictionaryValue);
        dialogueSystem.dictionary.AddEntry("test2", "static value");
        testDict = "Bing Bong Hansen";

        dialogueSystem.RegisterEventHandler(TestEventHandler);
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (dialogueSystem.IsRunning)
            {
                if (dialogueSystem.isAnimating)
                {
                    dialogueSystem.EndLine();
                }
                else
                {
                    dialogueSystem.AdvanceDialogue();
                }

            }
            else
            {
                dialogueSystem.StartDialogue();
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            dialogueSystem.settings.typewriterSpeed = 1f;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            dialogueSystem.settings.textDisplayMode = TextEffects.TextDisplayMode.TYPEWRITER;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if(dialogueSystem.CurrentState == DialogueSystem.State.Paused)
            {
                dialogueSystem.Pause(false);
            } else
            {
                dialogueSystem.Pause(true);
            }
        }

    }

    public bool CustomTestCondition(Condition condition)
    {
        if (condition.Flag == "X")
        {
            return true;
        }
        return false;
    }

    public void OnLineStartCallback(TextNode node)
    {
        string test = node.TryGetMetadataByKey("speaker", dialogueSystem.dictionary);
        if(test != null)
        {
            UiHandler.SetSpeakerName(test);
        } else
        {
            UiHandler.DisableNamePlate();
        }


    }

    public void OnNodeLeave(BaseNode node)
    {
        Debug.Log("left " + node.name);
    }

    public void OnNodeEnter(BaseNode node)
    {
        Debug.Log("Entered " + node.name);
    }

    public void OnLineEndCallback(TextNode node)
    {
        //UiHandler.DisableNamePlate();
    }

    public bool OnChoiceDraw(DialogueChoices node)
    {
        UiHandler.RenderDialogueChoices(node, dialogueSystem);
        return true;
    }

    public string GetDictionaryValue()
    {
        return testDict;
    }

    public void TestEventHandler(DialogueEvent myEvent)
    {
        if (myEvent != null)
        {
            if (myEvent.eventName == "rotateCube")
            {
                dialogueSystem.ui.ShowDialoguePane(false);
                StartCoroutine(Rotate(myEvent.floatParameter));
            }

            if (myEvent.eventName == "yourEvent")
            {
                
            }
        }
    }

    IEnumerator Rotate(float duration)
    {
        float startRotation = testCube.eulerAngles.y;
        float endRotation = startRotation + 360.0f;
        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float yRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360.0f;
            testCube.eulerAngles = new Vector3(testCube.eulerAngles.x, yRotation, testCube.eulerAngles.z);
            yield return null;
        }
        dialogueSystem.AdvanceDialogue();
    }

}
