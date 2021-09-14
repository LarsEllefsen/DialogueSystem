using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Globalization;

[RequireComponent(typeof(AudioSource))]
public class DialogueAudio : MonoBehaviour
{
    public AudioClip testClip;
    public AudioClip clip;

    //public ExifToolWrapper wrapper = new ExifToolWrapper();

    public List<VoiceCuts> voiceCuts = new List<VoiceCuts>();

    public AudioSource src;

    private int current;
    private float delay;

    void Awake()
    {
        src = GetComponent<AudioSource>();
    }

    public void SetClip(AudioClip clip)
    {
        this.clip = clip;
        src.clip = clip;
        //PlaySlice(1f, 1.9f, .9f);
    }

    public void SetDelay(float delay)
    {
        this.delay = delay;
    }

    public void TestPlay()
    {
        current = 0;
        //PopulateMetadata();
        Debug.Log(voiceCuts[0].startTime);
        PlaySlice(voiceCuts[current].startTime, voiceCuts[current].endTime, voiceCuts[current].pitch, voiceCuts[current].reverse);

    }

    private AudioClip GetAudioSlice(AudioClip originalClip, float start, float end)
    {
        int frequency = originalClip.frequency;
        Debug.Log(frequency);
        float timeLength = end - start;
        int samplesLength = (int)(frequency * timeLength) * 2;
        AudioClip slice = AudioClip.Create(originalClip.name + "-sub", samplesLength, originalClip.channels, frequency, false);

        /* Create a temporary buffer for the samples */
        float[] data = new float[samplesLength];
        /* Get the data from the original clip */
        originalClip.GetData(data, (int)(frequency * start));
        /* Transfer the data to the new clip */
        slice.SetData(data, 0);

        return slice;
        
    }

    private void PlaySlice(float fromSeconds, float toSeconds, float pitch, bool reverse)
    {
        src.time = fromSeconds;
        src.pitch = reverse ? pitch : -pitch;
        src.Play();
        double endTime = AudioSettings.dspTime + (toSeconds - fromSeconds);
        src.SetScheduledEndTime(endTime);
        StartCoroutine(NotifyOnAudioEnd(endTime, OnAudioSliceDone));
    }

    public void OnAudioSliceDone()
    {
        if (current + 1 < voiceCuts.Count)
        {
            current += 1;
            PlaySlice(voiceCuts[current].startTime, voiceCuts[current].endTime, voiceCuts[current].pitch, voiceCuts[current].reverse);
        }
        else
        {
            Debug.Log("Im all done");
            current = 0;
            PlaySlice(voiceCuts[current].startTime, voiceCuts[current].endTime, voiceCuts[current].pitch, voiceCuts[current].reverse);
        }
    }

    IEnumerator NotifyOnAudioEnd(double endTime, Action callback)
    {
        //yield return new WaitForSeconds(endTime);
        while (AudioSettings.dspTime < endTime)
        {
            yield return new WaitForEndOfFrame();
        }
        
        callback();
    }

   /* public void PopulateMetadata()
    {
        wrapper.GetCuePoints("E:/Software/test_speech.wav");
        cuts = new VoiceCuts[wrapper.Count];
        Debug.Log("metadata done");
        foreach (ExifTagItem i in wrapper)
        {
            VoiceCuts cut = new VoiceCuts();
            cut.startTime = float.Parse(i.start, CultureInfo.InvariantCulture);
            cut.endTime = float.Parse(i.end, CultureInfo.InvariantCulture);
            cut.pitch = 1f;
            cut.reverse = false;
            voiceCuts.Add(cut);
            //Debug.Log("Group: " + i.group + "; Name: " + i.name + "; Start: " + i.start + "; End: " + i.end);
        }
    }*/
}
