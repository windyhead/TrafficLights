namespace Lights
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class TrafficController : MonoBehaviour
    {
        public Action<string> OnStateChanged;
        public Action<float> OnTimerChanged;

        public bool useBlinkLites = false;
        public bool useAttentionBox = true;
        public bool useLeftBox = false;
        public bool useRightBox = false;
        
        [Header("Default Traffic States")] [SerializeField]
        private TrafficState _stopState = default;

        [SerializeField] private TrafficState _attentionState = default;
        [SerializeField] private TrafficState _goState = default;
        [SerializeField] private TrafficState _goLeftState = default;
        [SerializeField] private TrafficState _goRightState = default;

        [Header("Assets")] [SerializeField] private LightBox _stopBox = default;
        [SerializeField] private LightBox _attentionBox = default;
        [SerializeField] private LightBox _middleGoBox = default;
        [SerializeField] private LightBox _leftGoBox = default;
        [SerializeField] private LightBox _rightGoBox = default;
        [SerializeField] private TrafficPanel _trafficPanel = default;

        private const float BlinkLength = 2f;
        private const float BlinkInterval = 0.5f;
        private readonly Dictionary<Traffic, TrafficState> _trafficStates = new Dictionary<Traffic, TrafficState>();
        private LightBox[] _lightBoxes;
        private Traffic _currentState;
        private float _timer = 0;
        private bool _isSimulationStarted;

        private void Awake()
        {
            _isSimulationStarted = false;
            AddStates();
            SetLightBoxes();
            DisableLightBoxes();
            _trafficPanel.Init(this, _trafficStates[Traffic.Stop].time, _trafficStates[Traffic.Attention].time,
                _trafficStates[Traffic.Go].time, _trafficStates[Traffic.GoLeft].time,
                _trafficStates[Traffic.GoRight].time, useBlinkLites, useAttentionBox, useLeftBox, useRightBox);
        }

        private void Update()
        {
            if (!_isSimulationStarted)
                return;
            
            _timer += Time.deltaTime;
            
            OnTimerChanged?.Invoke(_trafficStates[_currentState].time - _timer);
            
            if (_timer >= _trafficStates[_currentState].time)
            {
                _timer = 0;
                SwitchState();
            }
        }

        public void StartSimulation()
        {
            SetState(Traffic.Stop);
            _isSimulationStarted = true;
        }

        public void StopSimulation()
        {
            _isSimulationStarted = false;
            ResetTimer();
            DisableLightBoxes();
        }

        public bool IsSimulationStarted()
        {
            return _isSimulationStarted;
        }

        public void ChangeTimers(Traffic state, float newTime)
        {
            _trafficStates[state].time = newTime;
        }

        private void SetLightBoxes()
        {
            if (_stopBox == null || _attentionBox == null || _middleGoBox == null || _leftGoBox == null ||
                _rightGoBox == null)
                Debug.LogError($"some light box was not defined in inspector, please fix it!");

            _lightBoxes = new LightBox[5] {_stopBox, _attentionBox, _middleGoBox, _leftGoBox, _rightGoBox};
        }

        private void AddStates()
        {
            _trafficStates.Add(Traffic.Attention, _attentionState);
            _trafficStates.Add(Traffic.Stop, _stopState);
            _trafficStates.Add(Traffic.Go, _goState);
            _trafficStates.Add(Traffic.GoLeft, _goLeftState);
            _trafficStates.Add(Traffic.GoRight, _goRightState);
        }

        private void SwitchState()
        {
            DisableLightBoxes();
            switch (_currentState)
            {
                case Traffic.Go:
                    if (useLeftBox)
                        SetState(Traffic.GoLeft);
                    else if (useRightBox)
                        SetState(Traffic.GoRight);
                    else if (useAttentionBox)
                        SetState(Traffic.Attention);
                    else
                        SetState(Traffic.Stop);
                    break;

                case Traffic.GoLeft:
                    if (useRightBox)
                        SetState(Traffic.GoRight);
                    else if (useAttentionBox)
                        SetState(Traffic.Attention);
                    else
                        SetState(Traffic.Stop);
                    break;

                case Traffic.GoRight:
                    SetState(useAttentionBox ? Traffic.Attention : Traffic.Stop);
                    break;

                case Traffic.Attention:
                    SetState(Traffic.Stop);
                    break;
                
                default:
                    SetState(++_currentState);
                    break;
            }
        }

        private void SetState(Traffic state)
        {
            _currentState = state;
            OnStateChanged?.Invoke(_trafficStates[_currentState].message);
            SwitchLightBox();
            Debug.Log(_trafficStates[_currentState].message);
        }

        private void SwitchLightBox()
        {
            float blinkTimer = _trafficStates[_currentState].time - BlinkLength;

            switch (_currentState)
            {
                case Traffic.Stop:
                    _stopBox.EnableLight(useBlinkLites, BlinkLength, blinkTimer, BlinkInterval);
                    break;
                case Traffic.Attention:
                    _attentionBox.EnableLight(useBlinkLites, BlinkLength, blinkTimer, BlinkInterval);
                    break;
                case Traffic.Go:
                    _middleGoBox.EnableLight(useBlinkLites, BlinkLength, blinkTimer, BlinkInterval);
                    break;
                case Traffic.GoLeft:
                    _leftGoBox.EnableLight(useBlinkLites, BlinkLength, blinkTimer, BlinkInterval);
                    break;
                case Traffic.GoRight:
                    _rightGoBox.EnableLight(useBlinkLites, BlinkLength, blinkTimer, BlinkInterval);
                    break;
                default:
                    Debug.LogError($"current traffic state is undefined, please fix it!");
                    break;
            }
        }

        private void DisableLightBoxes()
        {
            foreach (var box in _lightBoxes)
            {
                if(useBlinkLites)
                    box.StopAllCoroutines();
                box.DisableLight();
            }
        }

        private void ResetTimer() => _timer = 0;
    }
}