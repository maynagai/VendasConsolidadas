namespace ConsolidacaoVendas.Log
{
    public class ProgressTracker
    {
        private long total = 0;
        private long processed = 0;

        public void SetTotal(long total) => Interlocked.Exchange(ref total, total);
        public void IncrementProcessed(long count = 1) => Interlocked.Add(ref processed, count);

        public int PercentComplete
        {
            get
            {
                var t = Interlocked.Read(ref total);
                if (t == 0) return 0;
                var p = (double)Interlocked.Read(ref processed) / t * 100.0;
                return (int)p;
            }
        }

        public long Total => Interlocked.Read(ref total);
        public long Processed => Interlocked.Read(ref processed);
    }
}

