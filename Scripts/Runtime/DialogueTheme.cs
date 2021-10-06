using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    [Serializable]
    public class DialogueTheme
    {
        [Header("Colors")]
        public Color red = new Color(.90f, .435f, .317f, 1);
        public Color yellow = new Color(.913f, .768f, .415f, 1);
        public Color blue = new Color(.149f, .274f, .325f, 1);

        [Serializable]
        public struct ColorDictionary
        {
            public string name;
            public Color color;
        }

        public List<ColorDictionary> customColors = new List<ColorDictionary>();
        public Dictionary<string, Color> colors;

        [Header("Effects")]
        public AnimationCurve OnLetterAppearAnimationCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
        public AnimationCurve OnLetterAppearOpacity = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
        //[Header("Custom text effects")]
        public List<TextEffect> effects;

        public DialogueTheme()
        {
            colors = new Dictionary<string, Color>()
        {
            {"red", red },
            {"yellow", yellow },
            {"blue", blue }
        };

            foreach (ColorDictionary color in customColors)
            {
                colors.Add(color.name.ToLower(), color.color);
            }

            effects = new List<TextEffect>() {
            new TextEffect("Wave", new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0)), new AnimationCurve(new Keyframe(0, -.5f), new Keyframe(.5f, 1), new Keyframe(1, -.5f)), true)
        };

        }

        public void AddColor(string name, Color color)
        {
            colors.Add(name, color);
        }

        public void ChangeColor(string name, Color newColor)
        {
            string key = name.ToLower();
            colors[key] = newColor;
        }
    }

}