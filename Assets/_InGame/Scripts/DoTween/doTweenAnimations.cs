
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class doTweenAnimations : MonoBehaviour
{
    // Static method to move an object from one position to another
    public static void MoveToPosition(GameObject target, Vector3 endPosition, float duration, Ease easeType = Ease.Linear)
    {
        target.transform.DOMove(endPosition, duration).SetEase(easeType);
    }

    public static void ScaleIn(GameObject target, Vector3 targetScale, float duration, Ease easeType = Ease.OutBack, bool shouldDestroy = false)
    {
        target.transform.DOScale(targetScale, duration).SetEase(easeType).OnComplete(() =>
        {
            if (shouldDestroy)
            {
                GameObject.Destroy(target);
            }
        });
    }
    // Static method to scale out an object (from normal size to small)
    public static void ScaleOut(GameObject target, float duration, Ease easeType = Ease.InBack, bool shouldDestroy = true)
    {
        if (target == null) return;
        target.transform.DOScale(Vector3.zero, duration).SetEase(easeType).OnComplete(() =>
        {
            if (shouldDestroy)
            {
                GameObject.Destroy(target);
            }
        });
    }

    // Static method to rotate an object to a target rotation
    public static void RotateObject(GameObject target, Vector3 targetRotation, float duration, Ease easeType = Ease.OutQuad)
    {
        target.transform.DORotate(targetRotation, duration).SetEase(easeType);
    }

    public static void Fade(GameObject targetObject, float duration, bool fadeIn, bool shouldDestroy = true)
    {
        if (targetObject == null) return;

        Image image = targetObject.GetComponent<Image>();
        if (image == null)
        {
            Debug.LogWarning("Fade method called on an object without an Image component.");
            return;
        }

        targetObject.GetComponent<MonoBehaviour>().StartCoroutine(FadeCoroutine(image, duration, fadeIn, shouldDestroy));
    }

    private static IEnumerator FadeCoroutine(Image image, float duration, bool fadeIn, bool shouldDestroy)
    {
        Color originalColor = image.color;
        float startAlpha = fadeIn ? 0 : originalColor.a;
        float endAlpha = fadeIn ? originalColor.a : 0;

        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, startAlpha);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, endAlpha);

        if (!fadeIn && shouldDestroy)
        {
            GameObject.Destroy(image.gameObject);
        }
    }
    // Static method to shake an object
    public static void ShakeObject(GameObject target, float duration, float strength = 1f, int vibrato = 10, float randomness = 90f, bool fadeOut = true)
    {
        target.transform.DOShakePosition(duration, strength, vibrato, randomness, fadeOut);
    }

    // Static method to punch scale (quick expand and shrink effect)
    public static void PunchScale(GameObject target, Vector3 punch, float duration, int vibrato = 10, float elasticity = 1f)
    {
        target.transform.DOPunchScale(punch, duration, vibrato, elasticity);
    }

    // Static method to change an object's color
    public static void ChangeColor(Material targetMaterial, Color endColor, float duration, Ease easeType = Ease.Linear)
    {
        targetMaterial.DOColor(endColor, duration).SetEase(easeType);
    }
}
