using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace DialogueSystem
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager instance { get; private set; }

        /* Private members */
        private DialogueHandler _handler;

        /* Public members */
        #region Public members
        [Header("File path")]
        public string resourceFolderName = "DialogueAssets";
        public string path = "";
        public string dialogueFileName;

        [Header("Graph")]
        public DialogueGraph dialogueGraph;

        [Header("UI")]
        public GameObject dialoguePane;
        public GameObject dialogueTextGameObject;

        [Header("Theme")]
        [SerializeField]
        public DialogueTheme theme;

        [Header("Settings")]
        [SerializeField]
        public DialogueSettings settings;
        public DialogueDictionary dictionary;
        #endregion

        #region Custom functions
        public Func<DialogueConditional, bool> CustomTestCondition
        {

            set
            {
                _handler.CustomTestCondition = value;
            }
        }

        public Func<DialogueChoices, bool> OnChoiceDraw { get { return _handler.OnChoiceDraw; } set { _handler.OnChoiceDraw += value; } }
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

        public List<DialogueGameState> gameStateVariables
        {
            get
            {
                return _handler.gameStateVariables;
            }

            private set
            {
                _handler.gameStateVariables = value;
            }
        }

        /*State Enum*/
        public enum State
        {
            NotRunning,
            Idle,
            Running,
            Animating,
            AwaitingChoice,
            Waiting,
            Paused,
        }

        #region Readonly members
        public bool IsRunning { get { return _handler.IsRunning; } }
        public bool isAnimating { get { return Ui.isAnimating; } }
        public bool speedUp { get { return Ui.textEffects.speedUp; } }
        public BaseNode CurrentNode
        {
            get
            {
                return _handler.CurrentNode;
            }

        }
        public BranchNode CurrentBranch
        {
            get
            {
                return _handler.CurrentBranch;
            }

        }
        public DialogueGraph CurrentGraph
        {
            get
            {
                return _handler.CurrentGraph;
            }

        }
        public DialogueUI Ui { get; private set; }
        public DialogueState CurrentState { get { return _handler.CurrentState; } }
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

            Ui = new DialogueUI(dialogueTextGameObject, dialoguePane, theme, settings, dialogueCallbackActions);

            if (settings != null)
            {
                if (settings.HideDialoguePaneOnStart) Ui.ShowDialoguePane(false);
            }

            if (_handler == null)
            {
                _handler = new DialogueHandler(Ui, settings, dialogueCallbackActions, dictionary, this);
            }

        }

        public void SetTheme(DialogueTheme theme)
        {
            Ui.SetTheme(theme);
        }

        public void SetCallbackActions(DialogueCallbackActions callbackActions)
        {
            this.dialogueCallbackActions = callbackActions;
            Ui.SetCallbackActions(callbackActions);

        }
        public void RegisterEventHandler(Action<DialogueEvent> eventHandler)
        {
            dialogueCallbackActions.EventHandler += eventHandler;
        }

        public void DeregisterEventHandler(Action<DialogueEvent> eventHandler)
        {
            dialogueCallbackActions.EventHandler -= eventHandler;
        }

        public void SetFlags(List<GameEventFlag> flags)
        {
            _handler.gameEventFlags = flags;
        }

        public void SetDialogGameState(List<DialogueGameState> gameState)
        {
            this.gameStateVariables = gameState;
        }

        public void SetDialogueFileName(string dialogueFileName)
        {
            this.dialogueFileName = dialogueFileName;
        }

        public void StartDialogue()
        {
            if (IsRunning)
            {
                Debug.LogWarning("The dialogue system is already running.");
                return;
            }

            if (dialogueGraph == null)
            {
                Debug.LogError("No dialogue graph is set.");
            }
            _handler.ExecuteGraph(dialogueGraph);

        }

        #region Typewriter Interactions
        public void ToggleSpeedUp(bool toggle)
        {
            Ui.textEffects.SpeedUp(toggle);
        }

        public void EndLine()
        {
            Ui.textEffects.DisplayWholeText();
        }
        #endregion

        public void AdvanceDialogue()
        {

            if (CurrentState == DialogueState.AwaitingChoice)
            {
                Debug.LogWarning("Dialogue is currently awaiting a dialogue choice. Use SelectDialogueChoice to advance the dialogue.");
                return;
            }

            if (CurrentState == DialogueState.Waiting)
            {
                Debug.LogWarning("Dialogue system is currently waiting.");
                return;
            }

            if (CurrentState != DialogueState.Running && CurrentState != DialogueState.Idle && CurrentState != DialogueState.Paused)
            {
                Debug.LogWarning("Dialogue system is currenly not running.");
                return;
            }


            _handler.TraverseGraph();


        }

        public void SelectDialogueChoice(int choiceNum)
        {
            //TODO: Figure out how we should handle this.
            Debug.Log(choiceNum);
            if (CurrentState != DialogueState.AwaitingChoice)
            {
                Debug.LogWarning("The dialogue system is not awaiting a player choice. Current node is " + _handler.CurrentNode.NodeType);
                return;
            }
            _handler.HandleChoiceNode(_handler.CurrentNode as ChoiceNode, choiceNum);
        }

        public void SelectDialogueChoice(string choiceName)
        {
            if (CurrentState != DialogueState.AwaitingChoice)
            {
                Debug.LogWarning("The dialogue system is not awaiting a player choice. Current node is " + _handler.CurrentNode.NodeType);
                return;
            }

            ChoiceNode choiceNode = _handler.CurrentNode as ChoiceNode;
            DialogueChoice selectedChoice = choiceNode.GetChoices().Choices.Find(x => x.choiceName == choiceName);
            if (selectedChoice == null)
            {
                Debug.LogError($"No choice with name {choiceName} found.");
                return;
            }

            _handler.HandleChoiceNode(_handler.CurrentNode as ChoiceNode, selectedChoice.choiceNumber);
        }

        public void Pause(bool toggle)
        {
            _handler.Pause(toggle);
        }

        #region
        private void CheckHealth()
        {
            if (flags == null && _handler.CustomTestCondition == null)
            {
                Debug.LogWarning("Game Event Flags are not set, and no custom flag function is provided.");
            }
        }
        #endregion

    }
}
