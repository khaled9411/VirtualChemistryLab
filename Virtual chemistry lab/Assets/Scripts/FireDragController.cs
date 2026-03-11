using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class FireDragController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool isDraggable = true;
    public float returnSpeed = 0.5f;
    public bool returnToOrigin = true;

    public float detectionRadius = 0.8f;
    public LayerMask magnesiumLayer;
    public string magnesiumTag = "Magnesium";

    public float dragScaleMultiplier = 1.3f;
    public float scaleAnimDuration = 0.2f;

    public AudioClip fireCrackleSound;
    public AudioClip ignitionSound;
    [Range(0f, 1f)] public float soundVolume = 0.8f;

    private Vector3 originalPosition;
    private Vector3 originalScale;
    private bool isDragging = false;
    private Camera mainCamera;
    private Canvas parentCanvas;
    private RectTransform rectTransform;
    private MagnesiumReaction nearbyMagnesium;
    private AudioSource audioSource;
    private bool hasIgnited = false;

    private bool is3DMode = false;
    private Plane dragPlane;

    void Awake()
    {
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        is3DMode = (rectTransform == null);
    }

    void Start()
    {
        originalPosition = transform.position;
        originalScale = transform.localScale;

        if (fireCrackleSound != null)
        {
            audioSource.clip = fireCrackleSound;
            audioSource.loop = true;
            audioSource.volume = soundVolume * 0.5f;
            audioSource.Play();
        }

        IdleFireAnimation();
    }

    void Update()
    {
        if (isDragging)
        {
            CheckMagnesiumProximity();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDraggable || hasIgnited) return;

        isDragging = true;
        DOTween.Kill(transform, true);

        transform.DOScale(originalScale * dragScaleMultiplier, scaleAnimDuration)
            .SetEase(Ease.OutBack);

        if (rectTransform != null)
            rectTransform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable || hasIgnited) return;

        if (is3DMode)
        {
            Ray ray = mainCamera.ScreenPointToRay(eventData.position);
            if (dragPlane.Raycast(ray, out float distance))
            {
                transform.position = ray.GetPoint(distance);
            }
        }
        else
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                eventData.position,
                parentCanvas.worldCamera,
                out Vector2 localPoint))
            {
                transform.position = parentCanvas.transform.TransformPoint(localPoint);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;

        isDragging = false;

        transform.DOScale(originalScale, scaleAnimDuration).SetEase(Ease.OutBounce);

        if (nearbyMagnesium != null && !hasIgnited)
        {
            TriggerIgnition();
        }
        else if (returnToOrigin)
        {
            transform.DOMove(originalPosition, returnSpeed)
                .SetEase(Ease.InOutSine)
                .OnComplete(IdleFireAnimation);
        }
    }

    void CheckMagnesiumProximity()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, magnesiumLayer);

        if (hits.Length > 0)
        {
            MagnesiumReaction mg = hits[0].GetComponent<MagnesiumReaction>();
            if (mg != null && !mg.reactionComplete)
            {
                nearbyMagnesium = mg;
                mg.ApplyHeat(1f);

                HighlightFire(true);
            }
        }
        else
        {
            nearbyMagnesium = null;
            HighlightFire(false);

            Collider2D hit2D = Physics2D.OverlapCircle(transform.position,
                detectionRadius, magnesiumLayer);
            if (hit2D != null && hit2D.CompareTag(magnesiumTag))
            {
                MagnesiumReaction mg = hit2D.GetComponent<MagnesiumReaction>();
                if (mg != null && !mg.reactionComplete)
                {
                    nearbyMagnesium = mg;
                    mg.ApplyHeat(1f);
                    HighlightFire(true);
                }
            }
        }
    }

    void TriggerIgnition()
    {
        hasIgnited = true;
        isDraggable = false;

        if (ignitionSound != null)
            audioSource.PlayOneShot(ignitionSound, soundVolume);

        transform.DOMove(nearbyMagnesium.transform.position, 0.3f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                transform.DOScale(Vector3.zero, 0.2f)
                    .OnComplete(() => gameObject.SetActive(false));
            });
    }

    void IdleFireAnimation()
    {
        if (!isDragging)
        {
            transform.DOScale(originalScale * 1.05f, 0.8f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);

            transform.DOMoveY(originalPosition.y + 0.05f, 1f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }

    void HighlightFire(bool highlight)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color targetColor = highlight ?
                new Color(1f, 0.8f, 0.2f) : Color.white;
            sr.DOColor(targetColor, 0.2f);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0f, 0.4f);
        Gizmos.DrawSphere(transform.position, detectionRadius);
    }

    public void SetupFor3D(float dragHeight = 0f)
    {
        is3DMode = true;
        dragPlane = new Plane(Vector3.up, new Vector3(0, dragHeight, 0));
    }
}