using UnityEngine;
using UnityEngine.UI; // UI ��Ҹ� �����ϱ� ���� �ʿ�
using UnityEngine.SceneManagement; // �� ������ ���� �ʿ�
using System.Collections;
using UnityEngine.Playables;

public class SceneFader : MonoBehaviour
{
    [Tooltip("ȭ���� ���� ��� UI �̹���")]
    public Image fadeImage;

    [Tooltip("���̵� ȿ���� �ɸ��� �ð�(��)")]
    public float fadeDuration = 2.0f;

    [Tooltip("���̵� �ƿ� �� �ҷ��� ���� �̸�")]
    public string sceneToLoad;
    [Tooltip("�ƾ��� �̸�")]
    public PlayableDirector myTimeLine;

    // ���̵� ȿ���� �����ϴ� �Լ�. �ٸ� ��ũ��Ʈ�� ��ư���� �� �Լ��� ȣ���ϸ� �˴ϴ�.
    public void StartFadeOut()
    {
        myTimeLine.Play();
        StartCoroutine(FadeOutCoroutine());
    }

    // ȭ���� ���� �Ͼ�� ����� �ڷ�ƾ
    private IEnumerator FadeOutCoroutine()
    {
        float timer = 0f;
        Color startColor = fadeImage.color;
        startColor.a = 0f; // ������ ������ �����ϰ�
        fadeImage.color = startColor;
        fadeImage.gameObject.SetActive(true); // �̹����� Ȱ��ȭ

        // ������ �ð�(fadeDuration) ���� ������ ����
        while (timer < fadeDuration)
        {
            // �ð��� ���� ����(����) ���� 0���� 1�� ���� ����
            float newAlpha = Mathf.Lerp(0, 1, timer / fadeDuration);

            Color newColor = fadeImage.color;
            newColor.a = newAlpha;
            fadeImage.color = newColor;

            timer += Time.deltaTime;
            yield return null; // ���� �����ӱ��� ���
        }

        // ������ ���� �� ���� ���� Ȯ���ϰ� 1�� ���� (������ ���)
        Color finalColor = fadeImage.color;
        finalColor.a = 1f;
        fadeImage.color = finalColor;

        // ������ ���� �ҷ���
        SceneManager.LoadScene(sceneToLoad);
    }
}