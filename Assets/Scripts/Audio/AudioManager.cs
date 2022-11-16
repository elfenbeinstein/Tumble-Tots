using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// handles the music of levels and lobby and fades between the two
/// subscribes to the EventListener "AUDIO"
/// </summary>
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioSource levelAS;
    [SerializeField] private AudioSource lobbyAS;

    [SerializeField] private float durationFadeIn;
    [SerializeField] private float durationFadeOut;

    [SerializeField] private float volumeLevel;
    [SerializeField] private float volumeLobby;

    void Awake()
    {
        EventSystem.Instance.AddEventListener("AUDIO", AudioListener);

        // make sure there's only one AudioManager in the scene:
        GameObject[] objs = GameObject.FindGameObjectsWithTag("AudioManager");
        if (objs.Length > 1) Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);

        // get the correct setup for the mixer sliders
        EventSystem.Instance.Fire("AUDIO", "music", PlayerPrefs.GetFloat("MusicVolume", 0.75f));
        EventSystem.Instance.Fire("AUDIO", "sfx", PlayerPrefs.GetFloat("SFXVolume", 0.75f));
    }
    private void OnDestroy()
    {
        EventSystem.Instance.RemoveEventListener("AUDIO", AudioListener);
    }

    private void AudioListener(string eventName, object param)
    {
        if (eventName == "lobby") StartLobby();
        else if (eventName == "level") StartLevel();
        else if (eventName == "sfx")
        {
            mixer.SetFloat("SFX", Mathf.Log10((float)param) * 20);
        }
        else if (eventName == "music")
        {
            mixer.SetFloat("Music", Mathf.Log10((float)param) * 20);
        }
    }

    [ContextMenu("Lobby")]
    private void StartLobby()
    {
        StopAllCoroutines();

        // start lobby music
        if (lobbyAS.isPlaying == false) lobbyAS.Play();
        StartCoroutine(FadeIn(durationFadeIn, volumeLobby, lobbyAS));

        // fade out level music if playing
        if (levelAS.isPlaying != false) LevelStop();
    }

    [ContextMenu("Level")]
    private void StartLevel()
    {
        StopAllCoroutines();

        // start level music
        if (levelAS.isPlaying == false) levelAS.Play();
        StartCoroutine(FadeIn(durationFadeIn, volumeLevel, levelAS));

        // fade out lobby music if playing
        if (lobbyAS.isPlaying != false) LobbyStop();
    }
    
    [ContextMenu("LobbyStop")]
    private void LobbyStop()
    {
        StartCoroutine(FadeIn(durationFadeOut, 0, lobbyAS));
    }

    [ContextMenu("LevelStop")]
    private void LevelStop()
    {
        StartCoroutine(FadeIn(durationFadeOut, 0, levelAS));
    }

    IEnumerator FadeIn (float duration, float targetVolume, AudioSource source)
    {
        float currentTime = 0;
        float startVolume = source.volume;

        while (currentTime <= duration)
        {
            currentTime += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
            yield return null;
        }

        if (targetVolume == 0)
        {
            source.Stop();
            source.volume = 0;
        }
        yield break;
    }


    // only used for debugging
    [ContextMenu("remove player prefs")]
    private void RemovePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
