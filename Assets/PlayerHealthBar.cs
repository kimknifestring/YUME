using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealthBar : MonoBehaviour
{
    [Header("UI 연결")]
    public Slider foregroundSlider; 
    public Slider backgroundSlider; 

    [Header("속도 설정")]
    public float updateSpeedSeconds = 0.5f;
    public float delayBeforeUpdate = 1f;

    private Coroutine healthUpdateCoroutine;

    public void HandleHealthChanged(float pct)
    {
        if (foregroundSlider != null)
        {
            foregroundSlider.value = pct;
        }

        if (healthUpdateCoroutine != null)
        {
            StopCoroutine(healthUpdateCoroutine);
        }

        if (gameObject.activeInHierarchy)
        {
            healthUpdateCoroutine = StartCoroutine(UpdateBackgroundBar(pct));
        }
    }

    private IEnumerator UpdateBackgroundBar(float newHealthPct)
    {
        yield return new WaitForSecondsRealtime(delayBeforeUpdate);

        float preChangePct = backgroundSlider.value;
        float elapsed = 0f;

        while (elapsed < updateSpeedSeconds)
        {
            elapsed += Time.unscaledDeltaTime;
            backgroundSlider.value = Mathf.Lerp(preChangePct, newHealthPct, elapsed / updateSpeedSeconds);
            yield return null;
        }

        backgroundSlider.value = newHealthPct;
    }
}