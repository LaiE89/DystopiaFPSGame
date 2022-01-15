using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CutSceneController : MonoBehaviour {
    // Declare any public variables that you want to be able 
    // to access throughout your scene
    [SerializeField] public GameObject loadingScreen; 
    [SerializeField] public Slider slider;
    [SerializeField] public TextMeshProUGUI progressText;
    [SerializeField] public ParticleSystem bloodParticles;
    [SerializeField] public ParticleSystem groundParticles;

    [Header("Singletons")]
    public SoundController soundController;
    public static CutSceneController Instance { get; private set; } // static singleton

    void Awake() {
        SceneController.sceneIndex = SceneManager.GetActiveScene().buildIndex;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Singleton Stuff
        if (Instance == null) { 
            Instance = this;
        }else { 
            Destroy(gameObject);
        }

        // Cache references to all desired variables
        soundController = FindObjectOfType<SoundController>();
    }
}