using System.Collections.Concurrent;

namespace ChatApp.Api.Services
{
    public class ConnectionTracker
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> _map = new();

        public void AddConnection(string userId, string connectionId)
        {
            var conns = _map.GetOrAdd(userId, _ => new ConcurrentDictionary<string, byte>());
            conns[connectionId] = 0;
        }

        public void RemoveConnection(string userId, string connectionId)
        {
            if (_map.TryGetValue(userId, out var conns))
            {
                conns.TryRemove(connectionId, out _);
                if (conns.IsEmpty)
                {
                    _map.TryRemove(userId, out _);
                }
            }
        }

        public string[] GetConnections(string userId)
        {
            if (_map.TryGetValue(userId, out var conns))
            {
                return conns.Keys.ToArray();
            }
            return Array.Empty<string>();
        }

        public bool HasConnections(string userId) => _map.ContainsKey(userId);
    }
}
