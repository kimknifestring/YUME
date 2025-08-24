using UnityEngine;
using UnityEngine.UI; // UI 요소를 제어하기 위해 필요
using UnityEngine.SceneManagement; // 씬 관리를 위해 필요
using System.Collections;
using UnityEngine.Playables;

public class SceneFader : MonoBehaviour
{
    [Tooltip("화면을 덮는 흰색 UI 이미지")]
    public Image fadeImage;

    [Tooltip("페이드 효과에 걸리는 시간(초)")]
    public float fadeDuration = 2.0f;

    [Tooltip("페이드 아웃 후 불러올 씬의 이름")]
    public string sceneToLoad;
    [Tooltip("컷씬의 이름")]
    public PlayableDirector myTimeLine;

    // 페이드 효과를 시작하는 함수. 다른 스크립트나 버튼에서 이 함수를 호출하면 됩니다.
    public void StartFadeOut()
    {
        myTimeLine.Play();
        StartCoroutine(FadeOutCoroutine());
    }

    // 화면을 점점 하얗게 만드는 코루틴
    private IEnumerator FadeOutCoroutine()
    {
        float timer = 0f;
        Color startColor = fadeImage.color;
        startColor.a = 0f; // 시작은 완전히 투명하게
        fadeImage.color = startColor;
        fadeImage.gameObject.SetActive(true); // 이미지를 활성화

        // 지정된 시간(fadeDuration) 동안 루프를 실행
        while (timer < fadeDuration)
        {
            // 시간에 따라 알파(투명도) 값을 0에서 1로 점차 증가
            float newAlpha = Mathf.Lerp(0, 1, timer / fadeDuration);

            Color newColor = fadeImage.color;
            newColor.a = newAlpha;
            fadeImage.color = newColor;

            timer += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 루프가 끝난 후 알파 값을 확실하게 1로 설정 (완전한 흰색)
        Color finalColor = fadeImage.color;
        finalColor.a = 1f;
        fadeImage.color = finalColor;

        // 지정된 씬을 불러옴
        SceneManager.LoadScene(sceneToLoad);
    }
}