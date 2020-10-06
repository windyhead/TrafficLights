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
        [SerializeField] private Traffic _traffic = default;
        [SerializeField] private Image _lamp = default;
        private Color _definedColor = default;
        private TrafficController _controller;
        public void Init(TrafficController controller)
        {
            _controller = controller;
            
            switch (_traffic)
            {
                case Traffic.Stop:
                    SetLampColor(Color.red);
                    break;
                case Traffic.Attention:
                    SetLampColor(Color.yellow);
                    break;
                case Traffic.Go:
                    SetLampColor(Color.green);
                    break;
                case Traffic.GoRight:
                    SetLampColor(Color.green);
                    break;
                case Traffic.GoLeft:
                    SetLampColor(Color.green);
                    break;
                default:
                    Debug.LogError($"light box: {name} color was not defined in inspector, please fix it!");
                    SetLampColor(Color.gray);
                    break;
            }

            _controller.OnLightEnabled += EnableLight;
            _controller.OnLightDisabled += DisableLight;
        }
        

        public void EnableLight(Traffic state,bool blink, float blinkLength, float blinkTimer, float blinkInterval)
        {
            if(state!=_traffic)
                return;
            
            _lamp.color = _definedColor;
            if (blink)
                StartCoroutine(Blink(blinkLength, blinkTimer, blinkInterval));
        }
        
        ~LightBox()
        {
            _controller.OnLightEnabled -= EnableLight;
            _controller.OnLightDisabled -= DisableLight;
            Debug.Log("Destroyed");
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
                DisableLight(_traffic);
            }
        }

        public void DisableLight(Traffic state)
        {
            if (state == _traffic)
            _lamp.color = Color.gray;
        }
        
        private void SetLampColor(Color color) => _definedColor = color;
    }
}