using System.Collections.Generic;

namespace TabulaRasa.Core.Objects
{
    public class TraitSet
    {
        private Entity _owner;
        private readonly Dictionary<string, string> _traits;
        public string this[string key]
        {
            get
            {
                return GetValue(key);
            }
            set
            {
                SetValue(key, value);
            }
        }

        public TraitSet(Entity owner)
        {
            _owner = owner;
            _traits = new Dictionary<string, string>();
        }

        string GetValue(string key)
        {
            return _traits.TryGetValue(key, out var val) ? val : null;
        }

        void SetValue(string key, string value)
        {
            if (_traits.ContainsKey(key))
            {
                if (string.IsNullOrWhiteSpace(value))
                    _traits.Remove(key);
                else
                    _traits[key] = value;
            }
            else
                _traits.Add(key, value);
        }
    }
}