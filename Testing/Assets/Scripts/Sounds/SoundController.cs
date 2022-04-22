using System;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine;

public class SoundController : MonoBehaviour {
    public AudioMixerGroup master;
    public Sound[] specialSounds;
    public AudioSource[] allSounds;

    void Awake() {
        foreach(Sound s in specialSounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume; 
            s.source.pitch = s.pitch;
            s.source.spatialBlend = s.spatialBlend;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.group;
        }
    }

    void Start() {
        allSounds = FindObjectsOfType<AudioSource>() as AudioSource[];
    }

    public AudioSource GetSound(string sound) {
        Sound s = Array.Find(specialSounds, item => item.name == sound);
        if (s == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return null;
        }
        return s.source;
    }

    public void Play(string name) {
        Sound s = Array.Find(specialSounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Play();
    }

    public void PlayOneShot(string name) {
        Sound s = Array.Find(specialSounds, sound => sound.name == name);
        if (s == null) {
            // Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        //s.source.Play();
        s.source.PlayOneShot(s.source.clip);
    }

    public void PlayClipAtPoint(string name, Vector3 position, float pitch, float volume) {
        Sound s = Array.Find(specialSounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        var sound = PlayClipAt(s.source.clip, position, pitch);
        sound.pitch = pitch;
        sound.volume = volume;
        sound.spatialBlend = 1;
        sound.outputAudioMixerGroup = master;
    }

    public AudioSource PlayClipAt(AudioClip clip, Vector3 pos, float pitch) {
        var tempGO = new GameObject("TempAudio"); // create the temp object
        tempGO.transform.position = pos; // set its position
        var aSource = tempGO.AddComponent<AudioSource>(); // add an audio source
        aSource.clip = clip; // define the clip
        // set other aSource properties here, if desired
        aSource.Play(); // start the sound
        Destroy(tempGO, clip.length / Math.Abs(pitch)); // destroy object after clip duration
        return aSource; // return the AudioSource reference
    }

    public void Stop(string sound, float fadeTime) {
        Sound s = Array.Find(specialSounds, item => item.name == sound);
        if (s == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        //s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        //s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
        StartCoroutine(FadeOut(s, fadeTime));
    }

    public IEnumerator FadeOut(Sound s, float fadeTime) {
        float startVolume = s.volume;
 
        while (s.source.volume > 0) {
            s.source.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
        s.source.Stop();
        s.source.volume = startVolume;
    }

    public void StopAll() {
        foreach (AudioSource s in allSounds) {
            if (s != null) {
                s.Stop();
            }
        }
    }

    public void PauseAll() {
        foreach (AudioSource s in allSounds) {
            if (s != null) {
                s.Pause();
            }
        }
    }
}
