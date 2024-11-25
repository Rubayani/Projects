using UnityEngine;
using TMPro;
using DG.Tweening;

public class DamageText : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public float initialScale = 3f;
    public float popScale = 3.3f;

    [Header("Time Stamps")]
    public float scaleUpStartTime = 0.05f;
    public float scaleDownStartTime = 0.10f;
    public float fadeOutStartTime = 0.9f;

    [Header("Durations")]
    public float popDuration = 0.05f;
    public float shrinkDuration = 0.05f;
    public float fadeDuration = 0.3f;
    public float moveDuration = 0.5f;
    private RectTransform rectTransform;

    void Awake()
    {
        textMesh.outlineWidth = 0.4f;
        textMesh.outlineColor = Color.black;
        textMesh.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, 0.4f);
        rectTransform = GetComponent<RectTransform>();
    }

    public void InitTextDamage(int amount, bool isPlayer, bool isCrit)
    {
        if (amount == 0)
            textMesh.text = "Miss";
        else
        {
            textMesh.text = $"{amount:N0}";
            if (isCrit)
                textMesh.color = Color.red;
        }
        
        float angle = Random.Range(-45f, 45f) + (isPlayer ? 180f : 0f);
        Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right;
        Vector2 targetPosition = rectTransform.anchoredPosition + direction * 50;
        rectTransform.DOAnchorPos(targetPosition, moveDuration).SetEase(Ease.OutExpo);
        StartAnimation();

    }

    private void StartAnimation()
    {
        rectTransform.localScale = Vector3.one * initialScale;
        rectTransform.DOScale(popScale, popDuration).SetDelay(scaleUpStartTime);
        rectTransform.DOScale(1f, shrinkDuration).SetDelay(scaleDownStartTime);
        textMesh.DOFade(0, fadeDuration).SetDelay(fadeOutStartTime).OnComplete(() => End());
    }

    private void End()
    {
        rectTransform.DOKill();
        Destroy(gameObject);
    }
}
