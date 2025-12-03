using System.Collections.Concurrent;

namespace ConsolidacaoVendas.Log
{
    public class LogTracker
    {
        private readonly ConcurrentQueue<string> logs = new();

        public void Add(string message)
        {
            logs.Enqueue($" {DateTime.Now} - {message}");
        }

        public string[] GetAll()
        {
            return logs.ToArray();
        }
    }
}
