using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Manager for all of the sound fx and music
/// </summary>
public class SoundManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the sound manager
    /// </summary>
    public static SoundManager instance;

    [Header("Audio")]
    [SerializeField]
    [Tooltip("Collection of sounds")]
    private SoundGroup[] soundGroups;

    private Dictionary<string, SoundGroup> soundGroupDictionary;

    private void Awake()
    {
        //Maintain singleton instance
        if (instance == null)
            instance = this;
        //Ensure there is only one instance of the SoundManager
        else if (instance != this)
            Destroy(gameObject);
        //Persist between scenes
        //DontDestroyOnLoad(gameObject);
        soundGroupDictionary = new Dictionary<string, SoundGroup>(soundGroups.Length);
        foreach (SoundGroup group in soundGroups)
        {
            soundGroupDictionary.Add(group.Name, group);
            group.Initialize(this);
        }
    }

    /// <summary>
    /// Play a sound
    /// </summary>
    /// <param name="soundName">The name of the sound</param>
    /// <param name="loop">Whether to loop the sound (optional)</param>
    /// <param name="volume">The volume of the sound (optional)</param>
    /// <param name="pitch">The pitch of the sound (optional)</param>
    public void PlaySound(string soundGroupName, string soundName, bool loop = false, float volume = 1, float pitch = 1)
    {
        SoundGroup group = null;
        if (soundGroupDictionary.TryGetValue(soundGroupName, out group))
        {
            Sound sound = group.GetSound(soundName);
            if (sound == null)
            {
                Debug.LogError("Error: Sound could not be played. Sound not found.");
                return;
            }

            sound.audioSource.clip = sound.AudioClip;
            sound.audioSource.loop = loop;
            sound.audioSource.volume = volume;
            sound.audioSource.pitch = pitch;
            sound.audioSource.Play();
        }
        else
            Debug.LogError("Error: Sound could not be played. SoundGroup not found.");
    }

    /// <summary>
    /// Play a sound
    /// </summary>
    /// <param name="soundName">The name of the sound</param>
    /// <param name="loop">Whether to loop the sound (optional)</param>
    /// <param name="volume">The volume of the sound (optional)</param>
    /// <param name="pitch">The pitch of the sound (optional)</param>
    public void PlaySound(string soundGroupName, int soundIndex, bool loop = false, float volume = 1, float pitch = 1)
    {
        SoundGroup group = null;
        if (soundGroupDictionary.TryGetValue(soundGroupName, out group))
        {
            Sound sound = group.GetSounds()[soundIndex];
            if (sound == null)
            {
                Debug.LogError("Error: Sound could not be played. Sound not found.");
                return;
            }
            sound.audioSource.loop = loop;
            sound.audioSource.volume = volume;
            sound.audioSource.pitch = pitch;
            sound.audioSource.Play();
        }
        else
            Debug.LogError("Error: Sound could not be played. SoundGroup not found.");
    }

    /// <summary>
    /// Gets the sound group
    /// </summary>
    /// <returns>The sound group</returns>
    /// <param name="soundGroupName">Sound group name</param>
    public SoundGroup GetSoundGroup(string soundGroupName)
    {
        SoundGroup group = null;
        if (!soundGroupDictionary.TryGetValue(soundGroupName, out group))
            Debug.LogError("Error: SoundGroup not found.");
        return group;
    }

    /// <summary>
    /// Stops a sound from playing
    /// </summary>
    /// <param name="soundName">The name of the sound</param>
    public void StopSound(string soundGroupName, string soundName)
    {
        SoundGroup group = null;
        if (soundGroupDictionary.TryGetValue(soundGroupName, out group))
        {
            Sound sound = group.GetSound(soundName);
            if (sound == null)
            {
                Debug.LogError("Error: Sound could not be stopped. Sound not found.");
                return;
            }
            sound.audioSource.Stop();
        }
        else
            Debug.LogError("Error: Sound could not be stopped. SoundGroup not found.");
    }


}
