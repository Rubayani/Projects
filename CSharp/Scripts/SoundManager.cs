using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    private void Awake() => instance = this;



    void Start()
    {
        Application.runInBackground = true;
    }


    #region UI

    [Header("UI")]
    public AudioSource uiAudioSource;
    public float uiAudioVolume = 1f;

    #region PlayUISound

    public void PlayUISound(AudioClip uiSound)
    {

        return;
        uiAudioSource.PlayOneShot(uiSound);
    }
        
    #endregion


    #endregion





    #region Ambient

    [Header("Ambient")]
    public AudioSource ambientSource;
    public float ambientFadeDuration = 2f;
    public float ambientAudioVolume = 1f;
    private List<AudioClip> randomAmbient = new List<AudioClip>();

    public void PlayBackground(LocationData location)
    {
        return;
        if (randomAmbientCourotine != null) StopCoroutine(randomAmbientCourotine);
        if (location.locationAmbience == null) return;

        StartCoroutine(FadeOut());
        randomAmbient = new List<AudioClip>(location.randomAmbiences);
        ambientSource.clip = location.locationAmbience;

        ambientSource.time = Random.Range(0f, ambientSource.clip.length);
        StartCoroutine(FadeIn());
        randomAmbientCourotine = StartCoroutine(PlayRandomAmbient());
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(ambientFadeDuration / 4);
        ambientSource.time = 0f;
        ambientSource.volume = 0f;

        ambientSource.Play();
        float currentTime = 0f;
        while (currentTime < ambientFadeDuration)
        {
            ambientSource.volume = Mathf.Lerp(0f, ambientAudioVolume, currentTime / ambientFadeDuration);
            currentTime += Time.deltaTime;
            yield return null;
        }
        ambientSource.volume = ambientAudioVolume;
    }

    IEnumerator FadeOut()
    {
        float currentTime = 0f;
        while (currentTime < ambientFadeDuration)
        {
            ambientSource.volume = Mathf.Lerp(ambientAudioVolume, 0f, currentTime / ambientFadeDuration);
            currentTime += Time.deltaTime;
            yield return null;
        }
        ambientSource.volume = 0f;
    }


    #region PlayRandomAmbient
    [SerializeField] private AudioSource randomAmbientSource;
    private Coroutine randomAmbientCourotine;
    private AudioClip prevoiusAmbient;

    private IEnumerator PlayRandomAmbient()
    {

        while (randomAmbient.Count != 0)
        {
            yield return new WaitForSeconds(Random.Range(3, ambientSource.clip.length));
            AudioClip currentAmbient = randomAmbient[Random.Range(0, randomAmbient.Count)];
            while (currentAmbient == prevoiusAmbient)
                currentAmbient = randomAmbient[Random.Range(0, randomAmbient.Count)];

            randomAmbientSource.clip = currentAmbient;
            randomAmbientSource.Play();
            prevoiusAmbient = currentAmbient;
        }
    }

    #endregion


    #endregion
}