using UnityEngine;
using System.Collections;
using TMPro;
using Utilities.SimpleSingleton;

namespace Utilities.Popper
{
/// <summary>
/// Just a TMP text that moves up hence "Pop-up", used in Bellark.
/// Not pooled at the moment.
/// </summary>
public class PopupManager : Singleton<PopupManager>
{
    public GameObject damagePopupPrefab;
    public GameObject attackPopupPrefab;
    public GameObject snowBunnyPopupPrefab;
    public GameObject madmansPopupPrefab;
    [SerializeField] Transform popupParent;

    public void ShowPopup(string popupMessage, GameObject popupPrefab, Vector3 position, float moveUp = 1f, float duration = 1.5f)
    {
        StartCoroutine(ShowPopupRoutine(popupMessage, popupPrefab, position, moveUp, duration));
    }

    IEnumerator ShowPopupRoutine(string popupMessage, GameObject popupPrefab, Vector3 position, float moveUp = 1f, float duration = 1f)
    {
        GameObject popup = Instantiate(popupPrefab, position, Quaternion.LookRotation(Camera.main.transform.forward));
        TextMeshProUGUI text = popup.GetComponentInChildren<TextMeshProUGUI>();
        text.text = popupMessage;

        Vector3 startPos = popup.transform.position;
        Vector3 endPos = startPos + new Vector3(0, moveUp, 0);

        float elapsed = 0f;
        Color startColor = text.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Move upward
            popup.transform.position = Vector3.Lerp(startPos, endPos, t);

            // Fade out
            Color newColor = startColor;
            newColor.a = Mathf.Lerp(1f, 0f, t);
            text.color = newColor;

            yield return null;
        }
        Destroy(popup);
    }
}
}