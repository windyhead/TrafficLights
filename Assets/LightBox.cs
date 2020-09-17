namespace Lights
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;

    public enum LampColors
    {
        Red,
        Green,
        Yellow
    }

    public class LightBox : MonoBehaviour
    {
        [SerializeField] private LampColors _color = default;
        [SerializeField] private Image _lamp = default;
        private Color _definedColor = default;

        private void Awake()
        {
            switch (_color)
            {
                case LampColors.Red:
                    SetLampColor(Color.red);
                    break;
                case LampColors.Yellow:
                    SetLampColor(Color.yellow);
                    break;
                case LampColors.Green:
                    SetLampColor(Color.green);
                    break;
                default:
                    Debug.LogError($"light box: {name} color was not defined in inspector, please fix it!");
                    SetLampColor(Color.gray);
                    break;
            }
        }

        public void EnableLight(bool blink, float blinkLength, float blinkTimer, float blinkInterval)
        {
            _lamp.color = _definedColor;
            if (blink)
                StartCoroutine(Blink(blinkLength, blinkTimer, blinkInterval));
        }

        private IEnumerator Blink(float blinkLength, float blinkTimer, float blinkInterval)
        {
            int blinkCount = (int) (blinkLength / blinkInterval);
            yield return new WaitForSeconds(blinkTimer);
            for (int i = 0; i < blinkCount; i++)
            {
                yield return new WaitForSeconds(blinkInterval / 2);
                _lamp.color = _definedColor;
                yield return new WaitForSeconds(blinkInterval / 2);
                DisableLight();
            }
        }

        public void DisableLight() => _lamp.color = Color.gray;
        private void SetLampColor(Color color) => _definedColor = color;
    }
}