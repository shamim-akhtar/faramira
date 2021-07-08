using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PGGE.Patterns;
using UnityEngine.SceneManagement;

public class GameApp : Singleton<GameApp>
{
    public AmbientSound mAmbientSound;
    //public BottomMenu mBottomMenu;
    public AudioClip[] mAudioClips;

    Dictionary<string, AudioClip> sceneAudios = new Dictionary<string, AudioClip>();

    public string JigsawImageFilename = "tall02_9_12";

    void Start()
    {
        SceneManager.LoadScene("_splash_screen");
        //sceneAudios.Add("MainMenu", mAudioClips[0]);
        //sceneAudios.Add("TicTacToe", mAudioClips[1]);
        //sceneAudios.Add("8Puzzle", mAudioClips[2]);
        //sceneAudios.Add("Maze", mAudioClips[3]);
        //sceneAudios.Add("Memory", mAudioClips[4]);
        //sceneAudios.Add("Tetris", mAudioClips[5]);
        //sceneAudios.Add("Quiz", mAudioClips[6]);
    }

    void Update()
    {

    }

    void OnEnable()
    {
        //Debug.Log("OnEnable called");
        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        //Debug.Log("OnDisable");
        //SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    Debug.Log("Loaded: " + scene.name);
    //    if (sceneAudios.ContainsKey(scene.name))
    //    {
    //        AudioClip clip = sceneAudios[scene.name];
    //        if (clip != null)
    //        {
    //            PlaySceneAudio(clip);
    //        }
    //    }
    //}

    //void PlaySceneAudio(AudioClip clip)
    //{
    //    mAmbientSound.Play(clip);
    //}
}
