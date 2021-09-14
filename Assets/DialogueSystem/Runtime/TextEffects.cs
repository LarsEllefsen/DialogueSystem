using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using static DialogueUtilities;

public class TextEffects: MonoBehaviour
{
    private TMP_Text _textComponent;
    public List<int> waveIndices = new List<int>();

    DialogueTheme theme;
    DialogueSettings settings;
    DialogueCallbackActions callbackActions;

    private Dictionary<int, Color> colorIndices = new Dictionary<int, Color>();


    private float deltatime = 0f;
    public bool isAnimating { get; private set; }
    public bool speedUp { get; private set; }

    private Action currentCallback;

    public enum TextDisplayMode
    {
        NONE = 0,
        TYPEWRITER = 1
    }

    public enum Effect
    {
        WAVE = 0,
    }

    public void SetTheme(DialogueTheme theme)
    {
        this.theme = theme;
    }

    public void SetCallbackActions(DialogueCallbackActions callbackActions)
    {
        this.callbackActions = callbackActions;
    }

    public void SetIndices(int start, int end, Effect effect)
    {
        switch(effect)
        {
            case Effect.WAVE:
                for (int i = start; i < end; ++i)
                {
                    waveIndices.Add(i);
                }
                break;
            default:
                break;
        }
    }

    public void SetIndex(int i)
    {
        waveIndices.Add(i);
    }

    public void SetColorIndices(int start, int end, string color)
    {
        
        if (theme.colors.ContainsKey(color.ToLower()))
        {
            for (int i = start; i < end; ++i)
            {
                colorIndices.Add( i, theme.colors[color] );
            }
        }
    }


    public void ClearAllIndices()
    {
        colorIndices.Clear();
        waveIndices.Clear();
    }

    public void Init(DialogueTheme theme, DialogueSettings settings, DialogueCallbackActions callbackActions = null)
    {
        _textComponent = GetComponent<TMP_Text>();
        this.theme = theme;
        this.settings = settings;
        this.callbackActions = callbackActions;
    }

    public void DisplayWholeText()
    {
        if(isAnimating)
        {
            _textComponent.maxVisibleCharacters = _textComponent.textInfo.characterCount;
            //currentCallback();
            isAnimating = false;
        }
    }

    public void SpeedUp(bool toggle)
    {
        if(isAnimating)
        {
            speedUp = toggle;
        }
    }

    public void Test()
    {
        Debug.Log(callbackActions.OnBranchEnd.GetInvocationList().Length);
    }

    public void Typewriter(string text, Action callback)
    {
        _textComponent.maxVisibleCharacters = 0;
        _textComponent.text = text;
        currentCallback = callback;
        StartCoroutine(TypewriterEffect(callback));
    }

    public IEnumerator TypewriterEffect(Action callback)
    {
        _textComponent.ForceMeshUpdate();
        isAnimating = true;
        for (int i = 0; i<_textComponent.textInfo.characterCount; ++i)
        {
            if (!isAnimating) break;
            _textComponent.maxVisibleCharacters += 1;
            float typeWriterWaitTime = speedUp ? DecreasingFunction(settings.typewriterSpeed * settings.typewriterSpeedMultiplier) : DecreasingFunction(settings.typewriterSpeed);
            yield return new WaitForSeconds(typeWriterWaitTime);
        }
        callback();
        isAnimating = false;
        yield return null;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        var textInfo = _textComponent.textInfo;
        if (textInfo.characterCount > 0)
        {
            _textComponent.ForceMeshUpdate();
            

            deltatime += Time.deltaTime;

            for (int i = 0; i < textInfo.characterCount; ++i)
            {
                var charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible)
                {
                    continue;
                }

                Vector3[] verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
                Color32[] vertexColors = textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;

                if (waveIndices.Contains(i))
                {

                    for (int j = 0; j < 4; ++j)
                    {
                        Vector3 orig = verts[charInfo.vertexIndex + j];
                        verts[charInfo.vertexIndex + j] = orig + new Vector3(0, theme.WaveCurve.Evaluate(deltatime + orig.x * 0.01f) * 10f, 0);
                    }
                }

                if (colorIndices.ContainsKey(i))
                {

                    for (int j = 0; j < 4; ++j)
                    {
                        vertexColors[charInfo.vertexIndex + j] = colorIndices[i];
                    }
                }

            }

            for (int i = 0; i < textInfo.meshInfo.Length; ++i)
            {
                var meshInfo = textInfo.meshInfo[i];
                meshInfo.mesh.vertices = meshInfo.vertices;
                meshInfo.mesh.colors32 = meshInfo.colors32;

                _textComponent.UpdateGeometry(meshInfo.mesh, i);
            }

            //_textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }
    }
}
