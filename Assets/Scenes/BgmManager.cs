using UnityEngine;
using System.Collections;

public class BgmManager : MonoBehaviour
{
    // ���� �� �ϳ��� �����ϵ��� ����� �̱��� ����
    public static BgmManager instance;

    private AudioSource audioSource;
    private Coroutine currentFade = null; // ���� �������� ���̵� �ڷ�ƾ�� ����

    void Awake()
    {
        // ���� �̹� BgmManager�� �ִ��� Ȯ��
        if (instance == null)
        {
            // ���ٸ�, �� �ڽ��� instance�� ����
            instance = this;
            // ���� ��ȯ�Ǿ �� ������Ʈ�� �ı����� �ʵ��� ����
            DontDestroyOnLoad(gameObject);

            // AudioSource ������Ʈ�� �߰��ϰ� �⺻ ����
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true; // BGM�� ���� �ݺ� ���
        }
        else if (instance != this)
        {
            // �̹� �����Ѵٸ�, ���� ���� �� �ڽ��� �ı�
            Destroy(gameObject);
        }
    }

    // ���ο� BGM�� ���̵� ���ϸ� ����ϴ� �Լ�
    public void Play(AudioClip bgmClip, float fadeDuration = 1.0f)
    {
        // �̹� �ٸ� ���̵尡 ���� ���̶�� ����
        if (currentFade != null)
        {
            StopCoroutine(currentFade);
        }
        // ���ο� ���̵� �ڷ�ƾ ����
        currentFade = StartCoroutine(FadeAndPlayCoroutine(bgmClip, fadeDuration));
    }

    // ���� BGM�� ���̵� �ƿ��ϸ� �����ϴ� �Լ�
    public void Stop(float fadeDuration = 1.0f)
    {
        if (currentFade != null)
        {
            StopCoroutine(currentFade);
        }
        // ���̵� �ƿ� �ڷ�ƾ ����
        currentFade = StartCoroutine(FadeOutCoroutine(fadeDuration));
    }

    // �ε巯�� ��ȯ�� ���� �ڷ�ƾ (���̵� �ƿ� -> Ŭ�� ��ü -> ���̵� ��)
    private IEnumerator FadeAndPlayCoroutine(AudioClip newClip, float duration)
    {
        // 1. ���� �������� 0���� ���̵� �ƿ�
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

        // 2. ���ο� ����� Ŭ������ ��ü�ϰ� ���
        audioSource.clip = newClip;
        audioSource.Play();

        // 3. 0���� ��ǥ ����(1)���� ���̵� ��
        timer = 0f;
        while (timer < duration)
        {
            audioSource.volume = Mathf.Lerp(0f, 1f, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = 1f;
        currentFade = null; // �ڷ�ƾ �Ϸ�
    }

    // ���̵� �ƿ��� �����ϴ� �ڷ�ƾ
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
        currentFade = null; // �ڷ�ƾ �Ϸ�
    }
}