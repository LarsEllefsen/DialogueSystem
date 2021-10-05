using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueSystem;

public class SampleTriggerDialogue : MonoBehaviour
{
    public string folderName;
    public string fileName;

    public string testDict = "First initial value";

    DialogueManager dialogueManager;
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
        dialogueManager = DialogueManager.instance;
        flags = GetComponent<SampleFlags>();

        dialogueManager.flags = flags.gameEventFlags;
        dialogueManager.dialogueCallbackActions.OnNodeLeave += OnNodeLeave;
        dialogueManager.dialogueCallbackActions.OnNodeEnter += OnNodeEnter;

        //dialogueSystem.CustomTestCondition = CustomTestCondition;

        //NodeParser parser = new NodeParser();
        //parser.GenerateGraphFromScript(graph);

        gameStateVariables.Add(new DialogueGameState(10f, "floatExample"));

        dialogueManager.SetDialogGameState(gameStateVariables);
        dialogueManager.OnChoiceDraw += OnChoiceDraw;

        dialogueManager.dictionary.AddEntry("test", GetDictionaryValue);
        dialogueManager.dictionary.AddEntry("test2", "static value");
        testDict = "Bing Bong Hansen";

        dialogueManager.RegisterEventHandler(TestEventHandler);
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (dialogueManager.IsRunning)
            {
                if (dialogueManager.isAnimating)
                {
                    dialogueManager.EndLine();
                }
                else
                {
                    dialogueManager.AdvanceDialogue();
                }

            }
            else
            {
                dialogueManager.StartDialogue();
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            dialogueManager.settings.typewriterSpeed = 1f;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            dialogueManager.settings.textDisplayMode = TextEffects.TextDisplayMode.TYPEWRITER;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if(dialogueManager.CurrentState == DialogueState.Paused)
            {
                dialogueManager.Pause(false);
            } else
            {
                dialogueManager.Pause(true);
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
        string test = node.TryGetMetadataByKey("speaker", dialogueManager.dictionary);
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
        UiHandler.RenderDialogueChoices(node, dialogueManager);
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
                dialogueManager.ui.ShowDialoguePane(false);
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
        dialogueManager.AdvanceDialogue();
    }

}
