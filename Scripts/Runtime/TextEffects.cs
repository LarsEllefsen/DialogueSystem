using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using static DialogueSystem.DialogueHandler;

namespace DialogueSystem
{
    public class TextEffects : MonoBehaviour
    {
        DialogueTheme theme;
        DialogueSettings settings;
        DialogueCallbackActions callbackActions;

        /*Public member variables */
        public bool IsAnimating { get; private set; }
        public bool speedUp { get; private set; }
        public bool Paused { get; private set; }

        /*Control struct */
        class CharacterInfo
        {
            public bool processed;

            public float elapsedTime;
            public float totalDuration;

            public CharacterInfo(float duration)
            {
                totalDuration = duration;
                elapsedTime = 0;
                processed = false;

            }
        }

        /* Private member variables */
        private TMP_Text _textComponent;
        private TMP_TextInfo _textInfo;
        private float deltatime = 0f;
        private Dictionary<int, Color> colorIndices = new Dictionary<int, Color>();
        private List<int> waveIndices = new List<int>();
        private Dictionary<int, CharacterInfo> characterMap = new Dictionary<int, CharacterInfo>();
        private Dictionary<string, List<int>> effectIndices = new Dictionary<string, List<int>>();

        public enum TextDisplayMode
        {
            INSTANT = 0,
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

        public void Pause(bool toggle)
        {
            Paused = toggle;
        }

        public void SetIndices(int start, int end, Effect effect)
        {
            switch (effect)
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
                    colorIndices.Add(i, theme.colors[color]);
                }
            }
        }

        public void SetEffectIndices(string effectName, int start, int end)
        {
            if (!effectIndices.ContainsKey(effectName))
            {
                List<int> indices = new List<int>();
                for (int i = start; i < end; ++i)
                {
                    indices.Add(i);
                }
                effectIndices.Add(effectName, indices);
            }
            else
            {
                List<int> indices;
                if (effectIndices.TryGetValue(effectName, out indices))
                {
                    for (int i = start; i < end; ++i)
                    {
                        indices.Add(i);
                    }
                    effectIndices[effectName] = indices;
                }
            }
        }

        public void ClearAllIndices()
        {
            colorIndices.Clear();
            waveIndices.Clear();
            characterMap.Clear();
            effectIndices.Clear();
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
            if (IsAnimating)
            {
                _textComponent.maxVisibleCharacters = _textComponent.textInfo.characterCount;
                IsAnimating = false;
            }
        }

        public void SpeedUp(bool toggle)
        {
            if (IsAnimating)
            {
                speedUp = toggle;
            }
        }

        public void Typewriter(string text, Action<DialogueEventType> callback, float? typewriterSpeedOverride = null)
        {
            _textComponent.maxVisibleCharacters = 0;
            _textComponent.text = text;
            StartCoroutine(TypewriterEffect(callback, typewriterSpeedOverride));
        }

        public IEnumerator TypewriterEffect(Action<DialogueEventType> callback, float? speedOverride)
        {
            _textComponent.ForceMeshUpdate();
            IsAnimating = true;
            float typeWriterWaitTime;
            for (int i = 0; i < _textComponent.textInfo.characterCount; ++i)
            {
                if (!IsAnimating) break;
                while (Paused) yield return new WaitForEndOfFrame();
                _textComponent.maxVisibleCharacters += 1;
                if (speedOverride != null)
                {
                    typeWriterWaitTime = speedOverride.Value;

                }
                else
                {
                    typeWriterWaitTime = speedUp ? DialogueUtilities.DecreasingFunction(settings.typewriterSpeed * settings.typewriterSpeedMultiplier) : DialogueUtilities.DecreasingFunction(settings.typewriterSpeed);
                }
                yield return new WaitForSeconds(typeWriterWaitTime);
            }

            callback(DialogueEventType.OnTextEnd);
            IsAnimating = false;
            yield return null;
        }


        private void ForEachCharacter(int charIndex, TMP_TextInfo textInfo)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[charIndex];
            if (!charInfo.isVisible)
            {
                return;
            }

            Vector3[] verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
            Color32[] vertexColors = textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;

            OnCharacterAppear(charIndex, charInfo, verts, vertexColors);

            foreach (string effect in effectIndices.Keys)
            {
                List<int> indices;
                if (effectIndices.TryGetValue(effect, out indices))
                {
                    if (indices.Contains(charIndex))
                    {
                        TextEffect fx = theme.effects.Find(x => x.name == effect);


                        for (int j = 0; j < 4; ++j)
                        {
                            Vector3 orig = verts[charInfo.vertexIndex + j];
                            float evaluatedXPosition = fx.XPosAnimationCurve.Evaluate(deltatime + (fx.OffsetLettersX ? orig.y : orig.z) * 0.01f) * 10f;
                            float evaluatedYPosition = fx.YPosAnimationCurve.Evaluate(deltatime + (fx.OffsetLettersY ? orig.x : orig.z) * 0.01f) * 10f;
                            verts[charInfo.vertexIndex + j] = orig + new Vector3(evaluatedXPosition, evaluatedYPosition, 0);

                            // Scale
                            Vector3[] vertices = new Vector3[] { verts[charInfo.vertexIndex], verts[charInfo.vertexIndex + 1], verts[charInfo.vertexIndex + 2], verts[charInfo.vertexIndex + 3] };
                            Vector3 centerPoint = DialogueUtilities.CalculateCenter(vertices);
                            verts[charInfo.vertexIndex + j] = (verts[charInfo.vertexIndex + j] - centerPoint) * fx.scaleAnimationCurve.Evaluate(deltatime) + centerPoint;

                        }

                        // Rotation
                        Vector3[] v = new Vector3[] { verts[charInfo.vertexIndex], verts[charInfo.vertexIndex + 1], verts[charInfo.vertexIndex + 2], verts[charInfo.vertexIndex + 3] };
                        DialogueUtilities.RotateVertices(fx.rotationAnimationCurve.Evaluate(deltatime), ref v);
                        for (int i = 0; i < v.Length; i++)
                        {
                            verts[charInfo.vertexIndex + i] = v[i];
                        }
                    }
                }
            }

            if (colorIndices.ContainsKey(charIndex))
            {

                for (int j = 0; j < 4; ++j)
                {
                    vertexColors[charInfo.vertexIndex + j] = colorIndices[charIndex];
                }
            }
            //DebugExtension.DebugPoint(verts[charInfo.vertexIndex + j], Color.red);
            Debug.DrawLine(verts[charInfo.vertexIndex], verts[charInfo.vertexIndex + 1], Color.red);
            Debug.DrawLine(verts[charInfo.vertexIndex + 1], verts[charInfo.vertexIndex + 2], Color.red);
            Debug.DrawLine(verts[charInfo.vertexIndex + 2], verts[charInfo.vertexIndex + 3], Color.red);
            Debug.DrawLine(verts[charInfo.vertexIndex + 3], verts[charInfo.vertexIndex], Color.red);

        }

        private void OnCharacterAppear(int charIndex, TMP_CharacterInfo charInfo, Vector3[] verts, Color32[] vertexColors)
        {
            int numKeys = theme.OnLetterAppearAnimationCurve.length;
            float positionDuration = theme.OnLetterAppearAnimationCurve.keys[numKeys - 1].time;
            float colorDuration = theme.OnLetterAppearOpacity.keys[numKeys - 1].time;

            if (!characterMap.ContainsKey(charIndex))
            {
                CharacterInfo characterInfo = new CharacterInfo(Mathf.Max(positionDuration, colorDuration));
                characterMap.Add(charIndex, characterInfo);
                callbackActions.OnCharacterAppear?.Invoke('c');
            }

            CharacterInfo currentCharacter;
            if (characterMap.TryGetValue(charIndex, out currentCharacter))
            {
                if (currentCharacter.processed) return;

                currentCharacter.elapsedTime += Time.deltaTime;

                if (currentCharacter.elapsedTime < currentCharacter.totalDuration)
                {
                    for (int j = 0; j < 4; ++j)
                    {
                        Vector3 orig = verts[charInfo.vertexIndex + j];
                        float curveValue = theme.OnLetterAppearAnimationCurve.Evaluate(currentCharacter.elapsedTime);
                        verts[charInfo.vertexIndex + j] = orig + new Vector3(0, curveValue * 10, 0);

                        Color32 charColor = vertexColors[charInfo.vertexIndex + j];
                        float opacity = DialogueUtilities.FloatToByte(theme.OnLetterAppearOpacity.Evaluate(currentCharacter.elapsedTime));
                        charColor.a = (byte)opacity;
                        vertexColors[charInfo.vertexIndex + j] = charColor;
                    }
                }
                else
                {
                    currentCharacter.processed = true;
                }
            }
            else
            {
                Debug.LogWarning("No character found in char map");
            }


        }

        void LateUpdate()
        {

            if (!Paused)
            {
                _textInfo = _textComponent.textInfo;
                if (_textInfo.characterCount > 0)
                {
                    _textComponent.ForceMeshUpdate();

                    deltatime += Time.deltaTime;

                    for (int i = 0; i < _textInfo.characterCount; ++i)
                    {
                        ForEachCharacter(i, _textInfo);

                    }

                    for (int i = 0; i < _textInfo.meshInfo.Length; ++i)
                    {
                        var meshInfo = _textInfo.meshInfo[i];
                        meshInfo.mesh.vertices = meshInfo.vertices;
                        meshInfo.mesh.colors32 = meshInfo.colors32;

                        _textComponent.UpdateGeometry(meshInfo.mesh, i);
                    }
                }
            }
        }
    }
}
