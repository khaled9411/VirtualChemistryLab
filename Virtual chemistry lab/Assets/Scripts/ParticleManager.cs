using UnityEngine;
using DG.Tweening;

public class ParticleManager : MonoBehaviour
{
    [Header("Particle Systems")]
    public ParticleSystem brightFlash;
    public ParticleSystem whiteSmokeStream;
    public ParticleSystem sparks;
    public ParticleSystem heatDistortion;
    public MagnesiumReaction reactionSystem;
    public ReactionUI uiSystem;

    void Start()
    {
        if (reactionSystem != null)
        {
            reactionSystem.OnIgnition += HandleIgnition;
            reactionSystem.OnReactionComplete += HandleCompletion;
            reactionSystem.OnProgressChanged += uiSystem.UpdateProgress;
        }
    }

    void HandleIgnition()
    {

        if (brightFlash != null) brightFlash.Play();
        if (sparks != null) sparks.Play();

        DOVirtual.DelayedCall(0.5f, () =>
        {
            if (whiteSmokeStream != null) whiteSmokeStream.Play();
            if (heatDistortion != null) heatDistortion.Play();
        });
    }

    void HandleCompletion()
    {

        DOVirtual.DelayedCall(1f, () => { if (sparks != null) sparks.Stop(); });
        DOVirtual.DelayedCall(2f, () => { if (brightFlash != null) brightFlash.Stop(); });
        DOVirtual.DelayedCall(4f, () =>
        {
            if (whiteSmokeStream != null) whiteSmokeStream.Stop();
            if (heatDistortion != null) heatDistortion.Stop();
        });
    }

    void OnDestroy()
    {
        if (reactionSystem != null)
        {
            reactionSystem.OnIgnition -= HandleIgnition;
            reactionSystem.OnReactionComplete -= HandleCompletion;
        }
    }
}