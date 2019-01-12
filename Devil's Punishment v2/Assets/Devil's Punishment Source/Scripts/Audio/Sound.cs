using UnityEngine.Audio;
using UnityEngine;
using System;

[Serializable]
/// <summary>
/// An audio sound
/// </summary>
public class Sound
{
    public string Name;
    public AudioClip AudioClip;
    public AudioSource audioSource;
}
