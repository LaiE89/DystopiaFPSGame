using System;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine;

public class SoundController : MonoBehaviour {
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

    public void PlayClipAtPoint(string name, Vector3 position) {
        Sound s = Array.Find(specialSounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        AudioSource.PlayClipAtPoint(s.source.clip, position);
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
 
        s.source.Stop ();
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
