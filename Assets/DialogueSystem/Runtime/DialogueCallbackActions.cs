using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DialogueCallbackActions
{
    /* Actions */
    public Action OnLineEnd;
    public Action<Branch> OnBranchEnd;
    public Action OnDialogueEnd;

    /*Events*/
    public UnityEvent OnLineEndEvents;
    public UnityEvent OnBranchEndEvents;
    public UnityEvent OnDialogueEndEvents;
}
