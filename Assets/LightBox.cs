namespace Lights
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;

    public class LightBox : MonoBehaviour
    {
        [SerializeField] private Traffic _definedState = default;
        [SerializeField] private Image _lamp = default;

        private Color _definedColor = default;
        private TrafficController _controller;

        public void Init(TrafficController controller)
        {
            _controller = controller;

            switch (_definedState)
            {
                case Traffic.Stop:
                    DefineLampColor(Color.red);
                    break;
                case Traffic.Attention:
                    DefineLampColor(Color.yellow);
                    break;
                case Traffic.Go:
                    DefineLampColor(Color.green);
                    break;
                case Traffic.GoRight:
                    DefineLampColor(Color.green);
                    break;
                case Traffic.GoLeft:
                    DefineLampColor(Color.green);
                    break;
                default:
                    Debug.LogError($"light box: {name} color was not defined in inspector, please fix it!");
                    DefineLampColor(Color.gray);
                    break;
            }

            _controller.OnLightEnabled += EnableLight;
            _controller.OnLightDisabled += DisableLight;
        }

        ~LightBox()
        {
            _controller.OnLightEnabled -= EnableLight;
            _controller.OnLightDisabled -= DisableLight;
        }

        private void EnableLight(Traffic state, bool blink, float blinkLength, float blinkTimer, float blinkInterval)
        {
            if (state != _definedState)
                return;

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
                SetLampGray();
            }
        }

        private void DisableLight(Traffic state)
        {
            if (state == _definedState)
            {
                StopAllCoroutines();
                SetLampGray();
            }
        }

        private void DefineLampColor(Color color) => _definedColor = color;

        private void SetLampGray() => _lamp.color = Color.gray;
    }
}