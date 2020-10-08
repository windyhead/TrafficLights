namespace Lights
{
    using UnityEngine;

    public enum Traffic
    {
        Stop,
        Go,
        GoLeft,
        GoRight,
        Attention
    }

    [System.Serializable]
    public class TrafficState
    {
        [SerializeField] private Traffic _defaultState = default;
        [SerializeField] private string _defaultMessage = default;
        [SerializeField] private float _defaultTime = default;

        private const float DefaultStateTime = 5;
        
        private Traffic _state;
        private string _message;
        private float _time;

        public Traffic State
        {
            get => _state;
            protected set => _state = value;
        }

        public string Message
        {
            get => _message;
           protected set => _message = value;
        }

        public float Time
        {
            get => _time;
            set => _time = value > 0 ? value : DefaultStateTime;
        }

        public void Init()
        {
            State = _defaultState;
            Message = _defaultMessage;
            Time = _defaultTime;
        }
    }
}