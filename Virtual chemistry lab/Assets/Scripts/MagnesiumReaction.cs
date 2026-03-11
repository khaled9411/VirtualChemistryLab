using UnityEngine;
using DG.Tweening;
using System.Collections;

public class MagnesiumReaction : MonoBehaviour
{
    public bool isReacting = false;
    public bool reactionComplete = false;

    public float currentTemperature = 25f;
    public float ignitionTemperature = 650f;
    public float maxTemperature = 3100f;
    public float heatingRate = 50f;

    public ParticleSystem brightFlashParticles;
    public ParticleSystem whiteSmokeParticles;
    public ParticleSystem sparkParticles;
    public ParticleSystem initialFireParticles;

    public Light reactionLight;
    public float maxLightIntensity = 10f;

    public Renderer magnesiumRenderer;
    public Color originalColor = new Color(0.75f, 0.75f, 0.75f);
    public Color burningColor = new Color(1f, 0.95f, 0.8f);
    public Color mgoColor = Color.white;

    public ReactionUI reactionUI;
    public ParticleManager particleManager;

    private float reactionProgress = 0f;
    private Sequence temperatureSequence;

    public System.Action OnIgnition;
    public System.Action OnReactionComplete;
    public System.Action<float> OnTemperatureChanged;
    public System.Action<float> OnProgressChanged;

    void Start()
    {
        InitializeMagnesium();
    }

    void Update()
    {
        if (isReacting && !reactionComplete)
        {
            UpdateReaction();
        }
    }

    void InitializeMagnesium()
    {
        if (magnesiumRenderer != null)
            magnesiumRenderer.material.color = originalColor;

        if (reactionLight != null)
        {
            reactionLight.intensity = 0f;
            reactionLight.color = new Color(1f, 0.95f, 0.8f);
            reactionLight.gameObject.SetActive(false);
        }

        currentTemperature = 25f;
    }

    public void ApplyHeat(float heatAmount)
    {
        if (reactionComplete) return;

        currentTemperature += heatAmount * Time.deltaTime * heatingRate;
        currentTemperature = Mathf.Clamp(currentTemperature, 25f, maxTemperature);

        OnTemperatureChanged?.Invoke(currentTemperature);

        if (!isReacting && currentTemperature >= ignitionTemperature)
        {
            StartReaction();
        }
    }

    void StartReaction()
    {
        if (isReacting) return;
        isReacting = true;

        OnIgnition?.Invoke();

        ActivateLighting();
        ActivateParticles();

        if (reactionUI != null)
            reactionUI.ShowEquation();

        DOTween.To(() => currentTemperature, x =>
        {
            currentTemperature = x;
            OnTemperatureChanged?.Invoke(currentTemperature);
        }, maxTemperature, 3f).SetEase(Ease.OutQuad);
    }

    void UpdateReaction()
    {
        reactionProgress += Time.deltaTime / 8f;
        reactionProgress = Mathf.Clamp01(reactionProgress);

        OnProgressChanged?.Invoke(reactionProgress);

        if (magnesiumRenderer != null)
        {
            Color currentColor = Color.Lerp(burningColor, mgoColor, reactionProgress);
            magnesiumRenderer.material.color = currentColor;

            float emissionIntensity = Mathf.Sin(reactionProgress * Mathf.PI) * 3f;
            magnesiumRenderer.material.SetColor("_EmissionColor",
                burningColor * emissionIntensity);
        }

        if (reactionLight != null)
        {
            float lightCurve = Mathf.Sin(reactionProgress * Mathf.PI);
            reactionLight.intensity = maxLightIntensity * lightCurve;
        }

        if (reactionProgress >= 1f)
        {
            CompleteReaction();
        }
    }

    void CompleteReaction()
    {
        reactionComplete = true;
        isReacting = false;

        StartCoroutine(StopParticlesGradually());

        if (reactionLight != null)
        {
            DOTween.To(() => reactionLight.intensity, x => reactionLight.intensity = x,
                0f, 2f).OnComplete(() => reactionLight.gameObject.SetActive(false));
        }

        DOTween.To(() => currentTemperature, x =>
        {
            currentTemperature = x;
            OnTemperatureChanged?.Invoke(currentTemperature);
        }, 200f, 5f);

        if (magnesiumRenderer != null)
        {
            magnesiumRenderer.material.DOColor(mgoColor, 1f);
            magnesiumRenderer.material.SetColor("_EmissionColor", Color.black);
        }

        OnReactionComplete?.Invoke();

        if (reactionUI != null)
            reactionUI.ShowCompletionMessage();
    }

    void ActivateLighting()
    {
        if (reactionLight == null) return;

        reactionLight.gameObject.SetActive(true);
        reactionLight.intensity = 0f;
        DOTween.To(() => reactionLight.intensity, x => reactionLight.intensity = x,
            maxLightIntensity, 0.3f).SetEase(Ease.OutFlash);
    }

    void ActivateParticles()
    {
        if (brightFlashParticles != null) brightFlashParticles.Play();
        if (whiteSmokeParticles != null) whiteSmokeParticles.Play();
        if (sparkParticles != null) sparkParticles.Play();
        if (initialFireParticles != null) initialFireParticles.Stop();
    }

    IEnumerator StopParticlesGradually()
    {
        yield return new WaitForSeconds(1f);
        if (sparkParticles != null) sparkParticles.Stop();

        yield return new WaitForSeconds(1f);
        if (brightFlashParticles != null) brightFlashParticles.Stop();

        yield return new WaitForSeconds(2f);
        if (whiteSmokeParticles != null) whiteSmokeParticles.Stop();
    }

    public void ResetReaction()
    {
        isReacting = false;
        reactionComplete = false;
        reactionProgress = 0f;
        currentTemperature = 25f;

        DOTween.Kill(this);
        InitializeMagnesium();

        if (reactionUI != null)
            reactionUI.HideAll();
    }
}