using UnityEngine;

namespace DialogueSystem
{
    [System.Serializable]
    public class TextEffect
    {
        public string name;

        public AnimationCurve YPosAnimationCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
        public AnimationCurve XPosAnimationCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));

        public TextEffect(string name, AnimationCurve xPosCurve, AnimationCurve yPosCurve, bool loop = false)
        {
            this.name = name;
            XPosAnimationCurve = xPosCurve;
            YPosAnimationCurve = yPosCurve;
            if (loop)
            {
                XPosAnimationCurve.postWrapMode = WrapMode.Loop;
                YPosAnimationCurve.postWrapMode = WrapMode.Loop;
            }
        }
    }
}
