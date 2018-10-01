using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDiagram
{
    class EntrySorter<T> : IComparer<T> where T : Entry
    {
        private T[] _heap;
        int _hsize;
        int _fsize;
        bool _preserveDifferingCtxSrc;

        private delegate int CompareDelegate(T x, T y);
        private CompareDelegate _compare;
        private const bool NamespaceGoesFirst = true;

        public void SortByStrict()
        {
            _compare = compareStrict;
            _preserveDifferingCtxSrc = true;
            sort();
        }

        public void SortByTag()
        {
            _compare = compareTag;
            _preserveDifferingCtxSrc = false;
            sort();
        }

        public int Compare(T x, T y)
        {
            return _compare(x, y);
        }

        public T[] GetArray()
        {
            T[] ret = new T[_fsize];

            for(int i = 0; i < ret.Length; i++)
            {
                ret[i] = _heap[i + (_heap.Length - _fsize)];
            }

            return ret;
        }

        private void sort()
        {
            _fsize = 0;
            _hsize = _heap.Length;
            reorderAllElements();
            insertAllElementsToSortedList();
        }

        private int compareStrict(T x, T y)
        {
            string ctx1 = null;
            string ctx2 = null;
            int ret;
            
            if (x.Namespace != null)
            {
                ctx1 = x.Namespace;
            }
            else if(x.Class != null)
            {
                ctx1 = x.Class;
            }

            if (y.GenericContext)
            {
                if (ctx1 != null)
                {
                    ret = String.Compare(ctx1 + y.Separator + x.TagName, y.TagName);
                }
                else
                {
                    ret = String.Compare(x.TagName, y.TagName);
                }

                if((ret == 0) && ((x.HasAdjacentClone == -1) || (x.HasAdjacentClone == 1)))
                {
                    ret = x.HasAdjacentClone;
                }

                return ret;
            }

            if(y.Namespace != null)
            {
                ctx2 = y.Namespace;
            }
            else if(y.Class != null)
            {
                ctx2 = y.Class;
            }

            //ret = String.Compare(ctx1, ctx2);

            //if(ret != 0)
            //{
            //    return ret;
            //}

            if((ctx1 == null) && (ctx2 != null))
            {
                return String.Compare(x.TagName, ctx2 + y.Separator + y.TagName);
            }
            else if((ctx1 != null) && (ctx2 == null))
            {
                return String.Compare(ctx1 + x.Separator + x.TagName, y.TagName);
            }
            else if((ctx1 == null) && (ctx2 == null))
            {
                return String.Compare(x.TagName, y.TagName);
            }

            return String.Compare(ctx1 + x.Separator + x.TagName, ctx2 + y.Separator + y.TagName);
        }

        private static int compareTag(T x, T y)
        {
            return String.Compare(x.TagName, y.TagName);
        }

        private void reorderAllElements()
        {
            //Order n (length) * log(n) (average distance from the top)
            for (int i = 1; i < _heap.Length; i++)
            {
                siftUp(i, findParent(i));
            }
        }

        private void insertAllElementsToSortedList()
        {
            int index;
            T temp, m;

            //Order n (hsize) * log(n) (average distance from bottom)
            //(and checks for duplicates; negligible impact (O(n)))
            while (_hsize > 0)
            {
                temp = _heap[0];
                index = _heap.Length - _fsize - 1;

                //Does nothing if _hsize == 1
                _heap[0] = _heap[_hsize - 1];
                _hsize--;
                siftDown(0);

                if(_fsize > 0)
                {
                    m = _heap[index + 1];
                    if (_compare(temp, m) == 0)
                    {
                        if(_preserveDifferingCtxSrc && (m.HasAdjacentClone == 0) && hasDifferingContextSrc(temp, m))
                        {
                            //don't destroy, is different context
                            if(((temp.Namespace != null) && NamespaceGoesFirst) ||
                                ((temp.Namespace == null) && !NamespaceGoesFirst)) //Check ordering
                            {
                                temp.HasAdjacentClone = 2; //Is priority
                                m.HasAdjacentClone = -1;   //Relative position of -1 on the list
                            }
                            else
                            {
                                temp.HasAdjacentClone = 1; //Relative position of +1 on the list
                                m.HasAdjacentClone = 2; //Is priority
                            }
                        } //else destroy
                        else if(m.HasAdjacentClone != 0)
                        {
                            //Prioritize by lower EntryID
                            if(!hasDifferingContextSrc(temp, m) && (m.EntryID > temp.EntryID))
                            {
                                _heap[index + 1] = temp;
                                temp = null;
                            }
                            else if(_heap[index + 2].EntryID > temp.EntryID)
                            {
                                _heap[index + 2] = temp;
                                temp = null;
                            }
                        }
                        else if(m.EntryID > temp.EntryID)
                        {
                            _heap[index + 1] = temp;
                            temp = null;
                        }
                        else
                        {
                            temp = null;
                        }
                    }
                }

                if(temp != null)
                {
                    _heap[index] = temp;
                    _fsize++;
                }
            }
        }

        private void siftUp(int e, int p)
        {
            if(_compare(_heap[e], _heap[p]) > 0)
            {
                T temp = _heap[e];
                _heap[e] = _heap[p];
                _heap[p] = temp;

                if(p != 0)
                {
                    siftUp(p, findParent(p));
                }
            }
        }

        private void siftDown(int e)
        {
            int c1 = (e << 1) + 1;
            int c2 = (e << 1) + 2;

            if(c2 < _hsize)
            {
                if(_compare(_heap[c1], _heap[c2]) > 0) //c1 is greater than c2, use c1
                {
                    if(_compare(_heap[c1], _heap[e]) > 0) //c1 is greater than e, reorder
                    {
                        T temp = _heap[e];
                        _heap[e] = _heap[c1];
                        _heap[c1] = temp;

                        siftDown(c1);
                    }
                }
                else //use c2
                {
                    if (_compare(_heap[c2], _heap[e]) > 0) //c2 is greater than e, reorder
                    {
                        T temp = _heap[e];
                        _heap[e] = _heap[c2];
                        _heap[c2] = temp;

                        siftDown(c2);
                    }
                }
            }
            else if(c1 < _hsize)
            {
                if (_compare(_heap[c1], _heap[e]) > 0) //c1 is greater than e, reorder
                {
                    T temp = _heap[e];
                    _heap[e] = _heap[c1];
                    _heap[c1] = temp;
                }
            }
        }

        private int findParent(int c)
        {
            return ((c & 0x1) != 0) ? //check if odd
                (c >> 1) :          //removes parity bit (c - 1) / 2
                ((c - 2) >> 1);     //else is even (c - 2) / 2
        }

        private bool hasDifferingContextSrc(T x, T y)
        {
            if(x.Namespace != null)
            {
                if(y.Namespace == null)
                {
                    return true;
                }
            }
            else if(y.Namespace != null)
            {
                return true;
            }

            return false;
        }

        public EntrySorter(ICollection<T> entries)
        {
            _compare = compareTag;
            _heap = entries.ToArray();
        }
    }
}
