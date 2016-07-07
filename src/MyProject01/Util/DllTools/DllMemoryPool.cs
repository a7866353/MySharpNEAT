using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Util.DllTools
{
    class MemoryObject
    {
        public int Size;
        public IntPtr Ptr;
    }
    class DllMemoryPool
    {
        private List<MemoryObject> _emptyList;
        private int _emptyListCount;
        private int _size;

        // For debug
        private long _bufferMaxCount;
        private long _bufferCount;
        public int Size
        {
            get { return _size; }
        }

        public DllMemoryPool(int size)
        {
            _bufferCount = _bufferMaxCount = 0;
            _emptyListCount = 0;
            _size = size;
            _emptyList = new List<MemoryObject>();
        }
        public MemoryObject Get()
        {
            MemoryObject res;
            lock (_emptyList)
            {
                if (_emptyList.Count > 0)
                {
                    res = _emptyList[0];
                    _emptyList.RemoveAt(0);
                }
                else
                {
                    res = new MemoryObject();
                    res.Size = _size;
                    res.Ptr = Marshal.AllocHGlobal(res.Size);
                }
                _bufferCount++;
                if (_bufferCount > _bufferMaxCount)
                    _bufferMaxCount = _bufferCount;
            }
            return res;
        }
        public void Free(MemoryObject obj)
        {
            lock (_emptyList)
            {
                _emptyList.Add(obj);
            }
        }
    }

    class DllMemoryPoolCtrl
    {
        private List<DllMemoryPool> _poolList;

        public DllMemoryPoolCtrl()
        {
            _poolList = new List<DllMemoryPool>();
        }

        public MemoryObject Get(int size)
        {
            MemoryObject res = null;
            foreach (DllMemoryPool pool in _poolList)
            {
                if (pool.Size == size)
                {
                    res = pool.Get();
                    break;
                }
            }
            if(res == null)
            {
                DllMemoryPool pool = new DllMemoryPool(size);
                _poolList.Add(pool);
                res = pool.Get();
            }

            return res;
        }

        public void Free(MemoryObject obj)
        {
            foreach (DllMemoryPool pool in _poolList)
            {
                if (pool.Size == obj.Size)
                {
                    pool.Free(obj);
                    break;
                }
            }
        }
    }
}
