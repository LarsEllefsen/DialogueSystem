using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleTriggerDialogue : MonoBehaviour
{
    public string folderName;
    public string fileName;

    DialogueSystem dialogueSystem;
    SampleFlags flags;

    public DialogueTheme alternativeTheme;

    private void Start()
    {
        dialogueSystem = DialogueSystem.instance;
        flags = GetComponent<SampleFlags>();

        dialogueSystem.flags = flags.gameEventFlags;
        dialogueSystem.dialogueCallbackActions.OnBranchEnd += OnBranchEndTest;
        //dialogueSystem.CustomTestCondition = CustomTestCondition;
    }

    private void Update()
    {
        if(Input.GetKeyDown("space"))
        {
            if(dialogueSystem.running)
            {
                if (dialogueSystem.isAnimating)
                {
                    Debug.Log("Is animating and trying to end line");
                    dialogueSystem.EndLine();
                }
                else dialogueSystem.NextLine();

            } 
            else
            {
                dialogueSystem.StartDialogue(fileName, folderName);
            }
        }

        if(Input.GetKeyDown(KeyCode.T))
        {
            dialogueSystem.settings.typewriterSpeed = 1f;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {

        }

    }

    public bool CustomTestCondition(Condition condition)
    {
        if(condition.Flag == "X")
        {
            return true;
        }
        return false;
    }

    public void OnLineEndTest()
    {
        Debug.Log("I AM DONE!");
    }

    public void OnBranchEndTest(Branch branch)
    {
        Debug.Log(branch.Lines);
    }

}
