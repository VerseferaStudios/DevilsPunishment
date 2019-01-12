using UnityEngine.Audio;
using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
/// <summary>
/// A group of sounds
/// </summary>
public class SoundGroup
{
    private SoundManager soundManager;

    public string Name;

    public AudioSource audioSource;
    public AudioMixerGroup mixerGroup;

    [SerializeField]
    [Tooltip("Group of sounds")]
    public Sound[] sounds;



	//Collection of sounds for easy access
    public Dictionary<string, Sound> soundDictionary;

    public void Initialize(SoundManager soundManager)
    {
        this.soundManager = soundManager;
        soundDictionary = new Dictionary<string, Sound>(sounds.Length);
        if (audioSource != null && mixerGroup != null)
            audioSource.outputAudioMixerGroup = mixerGroup;

        foreach (Sound sound in sounds)
        {
            if (audioSource != null)
                sound.audioSource = audioSource;
            else if (sound.audioSource == null)
            {
                sound.audioSource = soundManager.gameObject.AddComponent<AudioSource>();
                sound.audioSource.outputAudioMixerGroup = mixerGroup;
                sound.audioSource.clip = sound.AudioClip;
            }
            if (!soundDictionary.ContainsKey(sound.Name))
                soundDictionary.Add(sound.Name, sound);
            else
                Debug.LogError("Error: You can not have two sounds with the same name.");
        }
    }

    /// <summary>
    /// Get the sound by name
    /// </summary>
    /// <param name="soundName">The name of the sound</param>
    /// <returns>Returns the sound if found</returns>
    public Sound GetSound(string soundName)
    {
        Sound sound;
        if (soundDictionary.TryGetValue(soundName, out sound))
        {
            return sound;
        }
        else
            Debug.LogError("Error: Sound could not be returned. Sound not found.");
        return null;
    }

	public Sound[] GetSounds() {
		return sounds;
	}

}
