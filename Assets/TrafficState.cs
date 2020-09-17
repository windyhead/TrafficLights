namespace Lights
{
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
        public Traffic _state;
        public float time ;
        public string message;
    }
}