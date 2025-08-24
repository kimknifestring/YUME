using UnityEngine;
using System.Collections;

public class BgmManager : MonoBehaviour
{
    // 씬에 단 하나만 존재하도록 만드는 싱글턴 패턴
    public static BgmManager instance;

    private AudioSource audioSource;
    private Coroutine currentFade = null; // 현재 진행중인 페이드 코루틴을 저장

    void Awake()
    {
        // 씬에 이미 BgmManager가 있는지 확인
        if (instance == null)
        {
            // 없다면, 나 자신을 instance로 지정
            instance = this;
            // 씬이 전환되어도 이 오브젝트가 파괴되지 않도록 설정
            DontDestroyOnLoad(gameObject);

            // AudioSource 컴포넌트를 추가하고 기본 설정
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true; // BGM은 보통 반복 재생
        }
        else if (instance != this)
        {
            // 이미 존재한다면, 새로 생긴 나 자신을 파괴
            Destroy(gameObject);
        }
    }

    // 새로운 BGM을 페이드 인하며 재생하는 함수
    public void Play(AudioClip bgmClip, float fadeDuration = 1.0f)
    {
        // 이미 다른 페이드가 진행 중이라면 중지
        if (currentFade != null)
        {
            StopCoroutine(currentFade);
        }
        // 새로운 페이드 코루틴 시작
        currentFade = StartCoroutine(FadeAndPlayCoroutine(bgmClip, fadeDuration));
    }

    // 현재 BGM을 페이드 아웃하며 정지하는 함수
    public void Stop(float fadeDuration = 1.0f)
    {
        if (currentFade != null)
        {
            StopCoroutine(currentFade);
        }
        // 페이드 아웃 코루틴 시작
        currentFade = StartCoroutine(FadeOutCoroutine(fadeDuration));
    }

    // 부드러운 전환을 위한 코루틴 (페이드 아웃 -> 클립 교체 -> 페이드 인)
    private IEnumerator FadeAndPlayCoroutine(AudioClip newClip, float duration)
    {
        // 1. 현재 볼륨에서 0으로 페이드 아웃
        float startVolume = audioSource.volume;
        float timer = 0f;

        while (timer < duration)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = 0f;
        audioSource.Stop();

        // 2. 새로운 오디오 클립으로 교체하고 재생
        audioSource.clip = newClip;
        audioSource.Play();

        // 3. 0에서 목표 볼륨(1)으로 페이드 인
        timer = 0f;
        while (timer < duration)
        {
            audioSource.volume = Mathf.Lerp(0f, 1f, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = 1f;
        currentFade = null; // 코루틴 완료
    }

    // 페이드 아웃만 진행하는 코루틴
    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = audioSource.volume;
        float timer = 0f;

        while (timer < duration)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
        currentFade = null; // 코루틴 완료
    }
}