using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource effectAudio, bgmAudio;

    Dictionary<string, AudioClip> soundDictionary = new Dictionary<string, AudioClip>();

    AudioClip nextMusic;

    public SoundClip[] soundClips;
    public AudioClip[] hitClips;
    public AudioClip[] BGMClips;

    private static AudioManager singletonInScene;
    public static AudioManager SingletonInScene
    {
        get
        {
            return singletonInScene;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        singletonInScene = this;
        singletonInScene.effectAudio = transform.Find("EffectAudio").GetComponent<AudioSource>();
        singletonInScene.bgmAudio = transform.Find("BGMAudio").GetComponent<AudioSource>();

        if (soundClips != null) {
            foreach (SoundClip clip in soundClips) {
                soundDictionary.Add(clip.clipNmae, clip.clip);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PauseBGM() {
        bgmAudio.Pause();
    }

    public void ChangeBGM(bool shopping, int curRound) {
        
        
    }

    IEnumerator OnChangingBGM() {
        yield return new WaitForSeconds(1.5f);
        bgmAudio.Play();
    }

    public void PlaySound2D(string _name, float volume)
    {
        if (soundDictionary.ContainsKey(_name))
        {
            effectAudio.PlayOneShot(soundDictionary[_name], volume);
        }
        else Debug.Log("沒有這個音檔");
    }
    public void PlaySound2D(string _name, float volume, float pitch)
    {
        if (soundDictionary.ContainsKey(_name))
        {
            effectAudio.pitch = pitch;
            effectAudio.PlayOneShot(soundDictionary[_name], volume);
        }
        else Debug.Log("沒有這個音檔");
    }
    public void ChangeBGM(int id) {
        switch (id) {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
        }
    }

    public void PlayRandomHit(float volume) {
        int r = Random.Range(0, 100) % hitClips.Length;
        effectAudio.PlayOneShot(hitClips[r], volume);
    }


    IEnumerator ReturnPitch() {

        yield return null;
        effectAudio.pitch = 1.0f;

    }
}


[System.Serializable]
public class SoundClip
{
    public string clipNmae;
    public AudioClip clip;
}