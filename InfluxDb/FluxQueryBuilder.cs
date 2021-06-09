namespace FridgePull.InfluxDb
{
    public class FluxQueryBuilder
    {
        private string _fluxQuery;
        
        public FluxQueryBuilder(string bucket)
        {
            AddBucket(bucket);
        }

        public string GetResult()
        {
            return _fluxQuery;
        }

        public FluxQueryBuilder AddFilter(string key, string value)
        {
            _fluxQuery += $"|> filter(fn: (r) => r[\"{key}\"] == \"{value}\")";
            
            return this;
        }

        public FluxQueryBuilder AddRange(string start, string stop)
        {
            _fluxQuery += $"|> range(start: {start}, stop: {stop})";
            
            return this;
        }
        
        public FluxQueryBuilder AddRange(string start)
        {
            _fluxQuery += $"|> range(start: {start})";
            
            return this;
        }
        
        public FluxQueryBuilder AddGroup(string column)
        {
            _fluxQuery += $"|> group(columns: [\"{column}\"])";
            
            return this;
        }

        public FluxQueryBuilder Last()
        {
            _fluxQuery += "|> last()";

            return this;
        }

        private void AddBucket(string bucket)
        {
            _fluxQuery += $"from(bucket: \"{bucket}\")";
        }
    }
}