﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class StreamVideo : MonoBehaviour {

    // Hier wird das Video in den GameObject geladen.
    public RawImage rawImage;
    public VideoPlayer videoPlayer;

	// Use this for initialization
	void Start () {
        //StartCoroutine(PlayVideo());
	}
	
	IEnumerator PlayVideo()
    {
        WaitForSeconds wait = new WaitForSeconds(1);
        videoPlayer.url = Application.dataPath + "/Video/" + CrossSceneInformation.video;
        videoPlayer.Prepare();
        wait = new WaitForSeconds(1);
        while (!videoPlayer.isPrepared)
        {
            yield return wait;
            break;
        }
        rawImage.texture = videoPlayer.texture;
        videoPlayer.Play();
    }

    void Update()
    {
        // Hier sind die Kommandos fürs abspielen gesetzt, hier auf "P"(Play) gesetzt.
        if (Input.GetKeyDown(KeyCode.P) && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 5)
        {
            StartCoroutine(PlayVideo());
        }
    }
}
