using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueSettings
{
    [SerializeField]
    public MultipleValidBranchesSelectionMode multipleValidBranchesSelectionMode;

    /* Enums */
    [Serializable]
    public enum MultipleValidBranchesSelectionMode
    {
        FIRST,
        PRIORITY,
        RANDOM
    }

    public bool HideDialoguePaneOnStart = true;

    public TextEffects.TextDisplayMode defaultTextDisplayMode = TextEffects.TextDisplayMode.TYPEWRITER;

    [Header("Typewriter")]
    public float typewriterSpeed = 10f;
    public float typewriterSpeedMultiplier = 2f;
}
