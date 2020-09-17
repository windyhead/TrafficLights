using System;
using System.Globalization;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;

namespace Lights
{
    using UnityEngine;
    using UnityEngine.UI;

    public class TrafficPanel : MonoBehaviour
    {
        [SerializeField] private Button _startSimulationButton = default;

        [Header("Texts")] 
        [SerializeField] private Text _startSimulationTitle = default;
        [SerializeField] private Text _trafficMessage = default;
        [SerializeField] private Text _timer = default;

        [Header("Input")] 
        [SerializeField] private InputField _stopTimer = default;
        [SerializeField] private InputField _attentionTimer = default;
        [SerializeField] private InputField _goTimer = default;
        [SerializeField] private InputField _goLeftTimer = default;
        [SerializeField] private InputField _goRightTimer = default;

        [Header("Toggles")] 
        [SerializeField] private Toggle _blinkToggle = default;
        [SerializeField] private Toggle _attentionToggle = default;
        [SerializeField] private Toggle _goLeftToggle = default;
        [SerializeField] private Toggle _goRightToggle = default;

        [Header("Switched Light Boxes")] 
        [SerializeField] private GameObject _attentionBox = default;
        [SerializeField] private GameObject _goLeftBox = default;
        [SerializeField] private GameObject _goRightBox = default;

        private const string StartSimulationText = "Включить симуляцию";
        private const string StopSimulationText = "Выключить симуляцию";

        private TrafficController _controller;
        private Toggle[] _options;
        private InputField[] _timers;

        public void Init(TrafficController controller, float stopTime, float attentionTime, float goTime,
            float goLeftTime, float goRightTime, bool blink, bool attention, bool goLeft, bool goRight)
        {
            _controller = controller;

            _startSimulationTitle.text = StartSimulationText;
            _stopTimer.text = stopTime.ToString();
            _attentionTimer.text = attentionTime.ToString();
            _goTimer.text = goTime.ToString();
            _goLeftTimer.text = goLeftTime.ToString();
            _goRightTimer.text = goRightTime.ToString();

            _blinkToggle.isOn = blink;
            _attentionToggle.isOn = attention;
            _goLeftToggle.isOn = goLeft;
            _goRightToggle.isOn = goRight;

            _startSimulationButton.onClick.AddListener(SwitchSimulation);

            SetTimers();
            SetToggles();
            
            OnBlinkToggleChanged(blink);
            OnAttentionToggleChanged(attention);
            OnGoLeftToggleChanged(goLeft);
            OnOnGoRightToggleChanged(goRight);

            _controller.OnStateChanged += ChangeMessage;
            _controller.OnTimerChanged += ChangeStateTimer;

            _options = new Toggle[4] {_blinkToggle, _attentionToggle, _goLeftToggle, _goRightToggle};
        }

        private void SetToggles()
        {
            _blinkToggle.onValueChanged.AddListener(OnBlinkToggleChanged);
            _attentionToggle.onValueChanged.AddListener(OnAttentionToggleChanged);
            _goLeftToggle.onValueChanged.AddListener(OnGoLeftToggleChanged);
            _goRightToggle.onValueChanged.AddListener(OnOnGoRightToggleChanged);
        }

        private void SetTimers()
        {
            _stopTimer.onEndEdit.AddListener(OnStopTimerChanged);
            _attentionTimer.onEndEdit.AddListener(OnAttentionTimerChanged);
            _goTimer.onEndEdit.AddListener(OnGoTimerChanged);
            _goLeftTimer.onEndEdit.AddListener(OnGoLeftTimerChanged);
            _goRightTimer.onEndEdit.AddListener(OnGoRightTimerChanged);
        }

        private void SwitchSimulation()
        {
            if (_controller.IsSimulationStarted())
            {
                _controller.StopSimulation();
                ChangeMessage(String.Empty);
                ChangeStateTimer();
            }
            else
                _controller.StartSimulation();

            EnableOptions(!_controller.IsSimulationStarted());
            _startSimulationTitle.text = _controller.IsSimulationStarted() ? StopSimulationText : StartSimulationText;
        }

        private void ChangeMessage(string text) => _trafficMessage.text = text;

        private void ChangeStateTimer(float time = 100) =>
            _timer.text = time == 100 ? string.Empty : Math.Round(time, 1).ToString();

        private void OnStopTimerChanged(string time)
        {
            _controller.ChangeTimers(Traffic.Stop, int.Parse(time));
        }

        private void OnAttentionTimerChanged(string time)
        {
            _controller.ChangeTimers(Traffic.Attention, int.Parse(time));
        }

        private void OnGoTimerChanged(string time)
        {
            _controller.ChangeTimers(Traffic.Go, int.Parse(time));
        }

        private void OnGoLeftTimerChanged(string time)
        {
            _controller.ChangeTimers(Traffic.GoLeft, int.Parse(time));
        }

        private void OnGoRightTimerChanged(string time)
        {
            _controller.ChangeTimers(Traffic.GoRight, int.Parse(time));
        }

        private void OnBlinkToggleChanged(bool isEnabled)
        {
            _controller.useBlinkLites = isEnabled;
        }

        private void OnAttentionToggleChanged(bool isEnabled)
        {
            _controller.useAttentionBox = isEnabled;
            EnableLightBox(Traffic.Attention, isEnabled);
        }

        private void OnGoLeftToggleChanged(bool isEnabled)
        {
            _controller.useLeftBox = isEnabled;
            EnableLightBox(Traffic.GoLeft, isEnabled);
        }

        private void OnOnGoRightToggleChanged(bool isEnabled)
        {
            _controller.useRightBox = isEnabled;
            EnableLightBox(Traffic.GoRight, isEnabled);
        }

        private void EnableLightBox(Traffic boxType, bool isEnabled)
        {
            switch (boxType)
            {
                case Traffic.Attention:
                    _attentionBox.SetActive(isEnabled);
                    _attentionTimer.interactable = isEnabled;
                    break;
                case Traffic.GoLeft:
                    _goLeftBox.SetActive(isEnabled);
                    _goLeftTimer.interactable = isEnabled;
                    break;
                case Traffic.GoRight:
                    _goRightBox.SetActive(isEnabled);
                    _goRightTimer.interactable = isEnabled;
                    break;
                default:
                    Debug.LogError($"trying to switch wrong box type:{boxType}!");
                    break;
            }
        }

        private void EnableOptions(bool isEnabled)
        {
            foreach (var option in _options)
                option.interactable = isEnabled;
        }
    }
}