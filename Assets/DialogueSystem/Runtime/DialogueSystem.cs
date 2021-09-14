using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem instance { get; private set; }

    /* Private members */
    private DialogueHandler _handler;

    /* Public members */
    #region Public members
    [Header("File path")]
    public string resourceFolderName = "DialogueAssets";
    public string path = "";
    public string dialogueFileName;

    [Header("UI")]
    public GameObject dialoguePane;
    public GameObject dialogueTextGameObject;

    [Header("Theme")]
    [SerializeField]
    public DialogueTheme theme;

    [Header("Settings")]
    [SerializeField]
    public DialogueSettings settings;
    #endregion

    #region Custom functions
    public Func<Condition, bool> CustomTestCondition
    {

        set
        {
            _handler.CustomTestCondition = value;
        }
    }
    #endregion

    #region Callback functions
    public DialogueCallbackActions dialogueCallbackActions;
    #endregion

    public List<GameEventFlag> flags
    {
        get
        {
            return _handler.gameEventFlags;
        }

        set
        {
            _handler.gameEventFlags = value;
        }
    }

    /*State Enum*/
    enum State { 
        Idle,
        AnimatingLine,
    }

    #region Readonly members
    public bool running { get; private set; }
    public bool isAnimating { get { return ui.isAnimating; } }
    public bool speedUp { get { return ui.textEffects.speedUp; } }
    public int currentLine 
    {
        get
        {
            return _handler.currentLine;
        }
        
    }
    public DialogueUI ui { get; private set; }
    #endregion

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        ui = new DialogueUI(dialogueTextGameObject, dialoguePane, theme, settings, dialogueCallbackActions);

        if (settings != null)
        {
            if (settings.HideDialoguePaneOnStart) ui.ShowDialoguePane(false);
        }

        if (_handler == null)
        {
            _handler = new DialogueHandler(ui, settings, dialogueCallbackActions);
        }

    }

    public void SetTheme(DialogueTheme theme)
    {
        ui.SetTheme(theme);
    }

    public void SetCallbackActions(DialogueCallbackActions callbackActions)
    {
        this.dialogueCallbackActions = callbackActions;
        ui.SetCallbackActions(callbackActions);

    }

    public void SetFlags(List<GameEventFlag> flags)
    {
        _handler.gameEventFlags = flags;
    }

    public void SetResourceFolderName(string resourceFolderName)
    {
        this.resourceFolderName = resourceFolderName;
    }
    
    public void SetPath(string path)
    {
        this.path = path;
    }
    
    public void SetDialogueFileName(string dialogueFileName)
    {
        this.dialogueFileName = dialogueFileName;
    }

    public void StartDialogue(TextEffects.TextDisplayMode? textAnimation = null)
    {
        if (running) return;

        if (path.Substring(path.Length - 1) != "/") path += "/";

        string pathToFile = $"{resourceFolderName}/{path}{dialogueFileName}";
        if(_handler.LoadDialogue(pathToFile))
        {
            running = true;
            TextEffects.TextDisplayMode mode = textAnimation == null ? settings.defaultTextDisplayMode : (TextEffects.TextDisplayMode)textAnimation;
            _handler.DisplayLine(0, mode);
        } 
    }

    #region Typewriter Interactions
    public void ToggleSpeedUp(bool toggle)
    {
        ui.textEffects.SpeedUp(toggle);
    }

    public void EndLine()
    {
        ui.textEffects.DisplayWholeText();
    }
    #endregion

    public bool NextLine(TextEffects.TextDisplayMode? textAnimation = null)
    {
        TextEffects.TextDisplayMode mode = textAnimation == null ? settings.defaultTextDisplayMode : (TextEffects.TextDisplayMode)textAnimation;

        if (_handler.currentLine + 1 < _handler.currentBranch.Lines.Length )
        {
            _handler.DisplayLine(currentLine + 1, mode);
            return true;
        } 
        else
        {
            return false;
        }
    }

    public void PreviousLine()
    {

    }

    public void DisplayLine(int line)
    {

    }

    public void StartDialogue(string dialogueFileName, TextEffects.TextDisplayMode? textAnimation = null)
    {
        SetDialogueFileName(dialogueFileName);
        StartDialogue(textAnimation);
    }

    public void StartDialogue(string dialogueFileName, string path, TextEffects.TextDisplayMode? textAnimation = null)
    {
        SetDialogueFileName(dialogueFileName);
        SetPath(path);
        StartDialogue(textAnimation);
    }

    #region
    private void CheckHealth()
    {
        if(flags == null && _handler.CustomTestCondition == null)
        {
            Debug.LogWarning("Game Event Flags are not set, and no custom flag function is provided.");
        }
    }
    #endregion

}
