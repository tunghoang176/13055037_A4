using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {

    public static MusicManager instance;

    public bool isMute = false;
    public float volume = 1;

    [SerializeField]
    private AudioClip acBackground = null;
    private AudioSource audioSource = null;

    private AudioSource audioSource2 = null;
    private AudioSource audioSource3 = null;

    [Header("Audio Clip")]
    [SerializeField]
    private AudioClip acEatPellet = null;
    [SerializeField]
    private AudioClip acEatPowerPill = null;
    [SerializeField]
    private AudioClip acEatCherry = null;
    [SerializeField]
    private AudioClip acEatGhost = null;
    [SerializeField]
    private AudioClip acReady = null;
    [SerializeField]
    private AudioClip acScared = null;
    [SerializeField]
    private AudioClip acWalk = null;

    private void Awake()
    {
        MakeSingleInstance();

        audioSource = GetComponent<AudioSource>();
        LoadSystem();
    }

    private void MakeSingleInstance()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlayOneShotAs2(AudioClip audioClip, float speed = 1)
    {
        if (audioSource2 == null)
        {
            audioSource2 = this.gameObject.AddComponent<AudioSource>();
            audioSource2.volume = volume;
            audioSource2.mute = isMute;
        }

        audioSource2.pitch = speed;
        audioSource2.PlayOneShot(audioClip, volume);
    }

    public void PlayOneShot(AudioClip audioClip = null)
    {
        audioSource.PlayOneShot(audioClip, volume);
    }

    public IEnumerator _PlayOneShotAs(AudioClip audioClip = null, float volume = 1)
    {
        AudioSource audioSource2 = this.gameObject.AddComponent<AudioSource>();
        audioSource2.mute = isMute;
        audioSource2.volume = volume;
        audioSource2.PlayOneShot(audioClip, volume);

        audioSource.volume = 0;
        yield return new WaitForSeconds(audioClip.length);

        audioSource.volume = this.volume;
        if (audioSource2 != null) Destroy(audioSource2);
    }

    public void PlayLoop(AudioClip audioClip = null)
    {
        audioSource.clip = (audioClip != null) ? audioClip : acBackground;

        audioSource.Play();
    }

    public void LoadSystem()
    {
        audioSource.mute = isMute;
        audioSource.volume = volume;

        LoadAllElement();
    }

    public void LoadAllElement()
    {
        GameObject[] elements;
        elements = GameObject.FindGameObjectsWithTag("SoundElement");

        foreach (GameObject el in elements)
        {
            AudioSource asEl = el.GetComponent<AudioSource>();

            if (asEl != null)
            {
                asEl.mute = isMute;
                asEl.volume *= volume;
            }
        }
    }

    public void ChangeMute()
    {
        isMute = !isMute;
        audioSource.mute = isMute;
        if (audioSource2 != null) audioSource2.mute = isMute;
        if (audioSource3 != null) audioSource3.mute = isMute;
    }

    public void ChangeVolume(float value)
    {
        volume = value;
        audioSource.volume = volume;
        if (audioSource2 != null) audioSource2.volume = volume;
        if (audioSource2 != null) audioSource3.volume = volume;
    }
    public void Ready()
    {
        StartCoroutine(_PlayOneShotAs(acReady, 0.5f));
    }

    public float timeScared = 0;
    private AudioClip oldAc = null;

    private void Update()
    {
        if(timeScared > 0)
        {
            timeScared -= Time.deltaTime;
        }
        else
        {
            timeScared = 0;
            if (oldAc != null && audioSource.clip != oldAc)
            {
                audioSource.clip = oldAc;
                audioSource.Play();
            }
        }
    }
    public void Scared(float time)
    {
        timeScared += time;
        if(audioSource.clip != acScared)
        {
            oldAc = audioSource.clip;
            audioSource.clip = acScared;
            audioSource.Play();
        }
    }

    public void EatPellet()
    {
        PlayOneShot(acEatPellet);
    }

    public void EatePowrPill()
    {
        PlayOneShot(acEatPowerPill);
    }

    public void EatCherry()
    {
        PlayOneShot(acEatCherry);
    }

    public void EatGhost()
    {
        PlayOneShot(acEatGhost);
    }

    public void Walk(bool isPlay)
    {
        if (isPlay)
        {
            if (audioSource3 == null)
            {
                audioSource3 = this.gameObject.AddComponent<AudioSource>();
                audioSource3.clip = acWalk;
                audioSource3.mute = isMute;
                audioSource3.volume = volume;
                audioSource3.loop = true;
                audioSource3.Play();
            }
            else
            {
                if(!audioSource3.isPlaying) audioSource3.Play();
            }
        }
        else
        {
            if (audioSource3 == null || !audioSource3.isPlaying) return;
            audioSource3.Stop();
        }        
    }
}
