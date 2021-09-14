using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueUI 
{
    private GameObject _paneGameObject;
    private GameObject _textGameObject;
    private TMP_Text _text;

    public TextEffects textEffects { get; private set; }
    public DialogueTheme theme { get; private set; }
    public DialogueSettings settings { get; private set; }
    public DialogueCallbackActions callbackActions { get; private set; }
    
    public bool isAnimating { get { return textEffects.isAnimating; } }


    public DialogueUI(GameObject dialogueText, GameObject dialoguePane = null, DialogueTheme defaultTheme = null, DialogueSettings settings = null, DialogueCallbackActions callbackActions = null)
    {
        if (dialoguePane != null) _paneGameObject = dialoguePane;
        if (dialogueText != null)
        {
            Debug.Log(dialogueText);
            _textGameObject = dialogueText;
            _text = _textGameObject.GetComponent<TMP_Text>();
            if (_text == null)
            {
                Debug.Log("Why");
                _text = _textGameObject.AddComponent<TMP_Text>();
            }
        } else
        {
            Debug.LogError("Dialogue UI is missing dialogue text gameobject");
        }



        this.settings = settings == null ? new DialogueSettings() : settings;
        this.callbackActions = callbackActions;
        theme = defaultTheme == null ? new DialogueTheme() : defaultTheme;

        textEffects = _text.gameObject.AddComponent<TextEffects>();
        textEffects.Init(theme, settings, callbackActions);
    }

    public void SetDialoguePane(GameObject gameObject)
    {
        _paneGameObject = gameObject;
    }

    public void SetDialogueTextGameObject(GameObject gameObject)
    {
        _textGameObject = gameObject;
        _text = _textGameObject.GetComponent<TMP_Text>();
        if (_text == null)
        {
            _text = _textGameObject.AddComponent<TMP_Text>();
        }
    }

    public void Reset()
    {
        textEffects.ClearAllIndices();
    }

    public void SetDialogueText(string text, TextEffects.TextDisplayMode mode, Action callback)
    {
        ShowDialoguePane(true);
        switch (mode)
        {
            case TextEffects.TextDisplayMode.TYPEWRITER:
                textEffects.Typewriter(text, callback);
                break;
            case TextEffects.TextDisplayMode.NONE:
                _text.text = text;
                callback();
                break;
            default:
                _text.text = text;
                break;
        }
    }

    public void SetTheme(DialogueTheme theme)
    {
        this.theme = theme;
        textEffects.SetTheme(theme);
    }

    public void SetCallbackActions(DialogueCallbackActions callbackActions)
    {
        this.callbackActions = callbackActions;
        textEffects.SetCallbackActions(callbackActions);
    }

    public void ShowDialoguePane(bool toggle)
    {
        if(_paneGameObject != null)
        {
            _paneGameObject.SetActive(toggle);
            SetTextGameobjectActive(toggle);
        }
    }

    public void SetTextGameobjectActive(bool toggle)
    {
        _textGameObject.SetActive(toggle);
    }
}
