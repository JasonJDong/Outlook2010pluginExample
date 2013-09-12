using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Newegg.Outlook.JIRA.Add_in
{
    public class SimpleCache<TValue> : Dictionary<String, TValue>
    {
        public void AddCache(String key, TValue cache)
        {
            if (!ContainsKey(key))
            {
                Add(key, cache);
            }
            else
            {
                this[key] = cache;
            }
        }

        public TValue GetCache(String key)
        {
            TValue v;
            if (TryGetValue(key,out v))
            {
                return v;
            }
            return default(TValue);
        }

        public void RemoveCache(String key)
        {
            Remove(key);
        }

        public void ClearCache()
        {
            Clear();
        }
    }
}
