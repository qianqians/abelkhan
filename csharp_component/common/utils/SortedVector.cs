using System.Collections.Generic;

namespace abelkhan
{
    public class SortedVector<TKey, TValue>
    {
        private SortedList<TKey, TValue> datas;

        public SortedVector()
        {
            datas = new SortedList<TKey, TValue>();
        }

        public void Add(TKey key, TValue value)
        {
            if (datas.ContainsKey(key))
            {
                datas[key] = value;
            }
            else
            {
                datas.Add(key, value);
            }
        }

        public void Remove(TKey key)
        {
            datas.Remove(key);
        }

        public int Count()
        {
            return datas.Count;
        }

        public class SortedVectorException : System.Exception
        {
            public SortedVectorException(string msg) : base(msg) {}
        }
        public TValue GetValueByIndex(int index)
        {
            var cindex = 0;
            foreach (var i in datas)
            {
                if (cindex == index)
                {
                    return i.Value;
                }

                cindex++;
            }

            throw new SortedVectorException("index out of range!");
        }

        public bool TryGetValueByIndex(int index, out TValue value)
        {
            value = default;

            var cindex = 0;
            foreach (var i in datas)
            {
                if (cindex == index)
                {
                    value = i.Value;
                    return true;
                }

                cindex++;
            }

            return false;
        }
    }
}
