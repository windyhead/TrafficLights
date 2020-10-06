namespace Lights
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Traffic Data", menuName = "Traffic Data", order = 51)]
    public class TrafficStatesData : ScriptableObject
    {
        [SerializeField] private List<TrafficState> _trafficStates = new List<TrafficState>() ; 
        private Dictionary<Traffic, TrafficState> _trafficDictionary = new Dictionary<Traffic, TrafficState>();

        public void CreateStatesDictionary()
        {
            foreach (var state in _trafficStates) 
                _trafficDictionary.Add(state._state,state);
        }

        public TrafficState GetState(Traffic traffic)
        {
            return _trafficDictionary[traffic];
        }
    }
}
