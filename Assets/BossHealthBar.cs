using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossHealthBar : MonoBehaviour
{
    [Header("UI ����")]
    public Slider foregroundSlider;
    public Slider backgroundSlider;

    [Header("�ӵ� ����")]
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

        Debug.Log($"�ִϸ��̼� ����. From: {preChangePct}, To: {newHealthPct}, Duration: {updateSpeedSeconds}");

        if (updateSpeedSeconds <= 0)
        {
            backgroundSlider.value = newHealthPct;
            yield break;
        }

        while (elapsed < updateSpeedSeconds)
        {
            elapsed += Time.unscaledDeltaTime;
            backgroundSlider.value = Mathf.Lerp(preChangePct, newHealthPct, elapsed / updateSpeedSeconds);
            yield return null;
        }

        backgroundSlider.value = newHealthPct;
    }

}