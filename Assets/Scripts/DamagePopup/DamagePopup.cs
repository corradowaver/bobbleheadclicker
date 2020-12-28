/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using Core.Math;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private static Transform _popupPrefab = null;

    public static Transform PopupPrefab
    {
        get
        {
            if (_popupPrefab == null)
            {
                _popupPrefab = Resources.Load<Transform>("popup");
            }

            return _popupPrefab;
        }
    }

    // Create a Damage Popup
    public static DamagePopup Create(Vector3 position, BigNumber damageAmount, bool isCriticalHit) {
        Transform damagePopupTransform = Instantiate(PopupPrefab, position, Quaternion.identity);

        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damageAmount, isCriticalHit);

        return damagePopup;
    }

    private static int sortingOrder;

    private const float DISAPPEAR_TIMER_MAX = 1f;

    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;
    private Vector3 moveVector;
    private int _direction = 1;
    private void Awake() {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    public void Setup(BigNumber damageAmount, bool isCriticalHit)
    {
        _direction = Random.Range(0, 2) == 0 ? -1 : 1;
        textMesh.SetText(damageAmount.ToString());
        if (!isCriticalHit) {
            // Normal hit
            textMesh.fontSize = 36;
            ColorUtility.TryParseHtmlString("#FFC500",out textColor); 
        } else {
            // Critical hit
            textMesh.fontSize = 45;
            ColorUtility.TryParseHtmlString("#FF2B00",out textColor);
        }
        textMesh.color = textColor;
        disappearTimer = DISAPPEAR_TIMER_MAX;

        sortingOrder++;
        textMesh.sortingOrder = sortingOrder;

        moveVector = new Vector3(.7f* _direction, 1 * Random.value) * 60f;
    }

    private void Update() {
        transform.position += moveVector  * Time.deltaTime;
        moveVector -= moveVector * 8f * Time.deltaTime;

        if (disappearTimer > DISAPPEAR_TIMER_MAX * .5f) {
            float increaseScaleAmount = 1f;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
        } else {
            float decreaseScaleAmount = 1f;
            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
        }

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0) {
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0) {
                Destroy(gameObject);
            }
        }
    }

}
