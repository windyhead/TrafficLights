namespace Lights
{
    using System;
    using UnityEngine;

    public class TrafficController : MonoBehaviour
    {
        public Action<string> OnStateChanged;
        public Action<float> OnTimerChanged;
        public Action<Traffic, bool, float, float, float> OnLightEnabled;
        public Action<Traffic> OnLightDisabled;

        public bool useBlinkLites = false;
        public bool useAttentionBox = true;
        public bool useLeftBox = false;
        public bool useRightBox = false;

        [Header("Assets")] 
        [SerializeField] private TrafficStatesData _trafficStatesData = default;
        [SerializeField] private LightBox[] _lightBoxes = default;
        [SerializeField] private TrafficPanel _trafficPanel = default;

        private const float BlinkLength = 2f;
        private const float BlinkInterval = 0.5f;

        private Traffic _currentState;
        private float _timer = 0;
        private bool _isSimulationStarted;

        private void Awake()
        {
            _isSimulationStarted = false;
            _trafficStatesData.CreateStatesDictionary();
            SetLightBoxes();
            DisableLightBoxes();
            _trafficPanel.Init(this, _trafficStatesData.GetState(Traffic.Stop).Time,
                _trafficStatesData.GetState(Traffic.Attention).Time,
                _trafficStatesData.GetState(Traffic.Go).Time, _trafficStatesData.GetState(Traffic.GoLeft).Time,
                _trafficStatesData.GetState(Traffic.GoRight).Time, useBlinkLites, useAttentionBox, useLeftBox,
                useRightBox);
        }

        private void Update()
        {
            if (!_isSimulationStarted)
                return;

            _timer += Time.deltaTime;

            OnTimerChanged?.Invoke(_trafficStatesData.GetState(_currentState).Time - _timer);

            if (_timer >= _trafficStatesData.GetState(_currentState).Time)
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

        public void ChangeTimers(Traffic state, float newTime) => 
            _trafficStatesData.GetState(state).Time = newTime;

        private void SetLightBoxes()
        {
            foreach (var box in _lightBoxes)
                box.Init(this);
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
            OnStateChanged?.Invoke(_trafficStatesData.GetState(_currentState).Message);
            SwitchLightBox();
            Debug.Log(_trafficStatesData.GetState(_currentState).Message);
        }

        private void SwitchLightBox()
        {
            float blinkTimer = _trafficStatesData.GetState(_currentState).Time - BlinkLength;
            OnLightEnabled?.Invoke(_currentState, useBlinkLites, BlinkLength, blinkTimer, BlinkInterval);
        }

        private void DisableLightBoxes() => OnLightDisabled?.Invoke(_currentState);

        private void ResetTimer() => _timer = 0;
    }
}