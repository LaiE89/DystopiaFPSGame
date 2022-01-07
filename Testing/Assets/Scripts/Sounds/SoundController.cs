using System;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine;

public class SoundController : MonoBehaviour {
    public Sound[] sounds;

    void Awake() {
        foreach(Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume; 
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Play();
    }

    public void Stop(string sound, float fadeTime) {
        Sound s = Array.Find(sounds, item => item.name == sound);
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
        foreach (Sound s in sounds) {
            if (!s.music) {
                s.source.Stop();
            }
        }
    }
}
