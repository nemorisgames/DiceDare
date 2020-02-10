using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;
    public AudioSource source1;
    public AudioSource source2;
    private int activeSource = 0;
    private AudioSource [] source;

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
}
