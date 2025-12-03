namespace ConsolidacaoVendas.Log
{
    public class ProgressTracker
    {
        private long _total;
        private long _processed;

        public void SetTotal(long total)
            => Interlocked.Exchange(ref _total, total);

        public void IncrementProcessed(long count = 1)
            => Interlocked.Add(ref _processed, count);

        public int PercentComplete
        {
            get
            {
                var t = Interlocked.Read(ref _total);
                if (t == 0) return 0;

                var p = (double)Interlocked.Read(ref _processed) / t * 100.0;
                return (int)p;
            }
        }

        public long Total => Interlocked.Read(ref _total);
        public long Processed => Interlocked.Read(ref _processed);
    }

}

