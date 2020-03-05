using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;
    public AudioSource source1;
    public AudioSource source2;
    public AudioSource sourceFX;
    private int activeSource = 0;
    private AudioSource [] source;
    public AudioMixer audioMixer;

    void Awake(){
        if(Instance != null)
            Destroy(this.gameObject);
        else
            Instance = this;
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        source = new AudioSource[]{source1,source2};
        source1.mute = PlayerPrefs.GetInt("bgmMute") == 1;
        source2.mute = PlayerPrefs.GetInt("bgmMute") == 1;
        sourceFX.mute = PlayerPrefs.GetInt("sfxMute") == 1;
    }

    public bool SFXMuted(){
        return sourceFX.mute;
    }

    public bool BGMMuted(){
        return (source1.mute && source2.mute);
    }

    public void Play(AudioClip clip, float volume = 1f, float pitch = 1f){
        bool b = source[activeSource].isPlaying;
        if(b){
            if(source[activeSource].clip == clip)
                return;
            StartCoroutine(FadeOut(source[activeSource]));
            activeSource = (activeSource + 1) % 2;
        }
        source[activeSource].clip = clip;
        source[activeSource].pitch = pitch;
        if(b)
            StartCoroutine(FadeIn(source[activeSource],volume));
        else{
            source[activeSource].volume = volume;
            source[activeSource].Play();
        }   
    }

    public void PlayFX(AudioClip clip, float volume = 1f, float pitch = -1f){
        float aux = sourceFX.pitch;
        if(pitch == -1f){
            aux = sourceFX.pitch;
		    sourceFX.pitch = Random.Range(0.8f,1.2f);
        }
        else{
            sourceFX.pitch = pitch;
        }
		sourceFX.PlayOneShot(clip,volume);
		sourceFX.pitch = aux;
    }

    public void MuteBGM(bool b){
        source1.mute = b;
        source2.mute = b;
    }

    IEnumerator FadeOut(AudioSource source){
        while(source.volume > 0){
            source.volume -= 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        source.Stop();
    }

    IEnumerator FadeIn(AudioSource source, float target){
        source.volume = 0;
        source.Play();
        while(source.volume < target){
            source.volume += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        source.volume = target;
    }

    public void MuteSFX(bool b){
        sourceFX.mute = b;
    }
}
