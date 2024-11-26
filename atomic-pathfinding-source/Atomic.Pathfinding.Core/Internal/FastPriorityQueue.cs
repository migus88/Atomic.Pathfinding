using System;
using System.Runtime.CompilerServices;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Interfaces;

namespace Atomic.Pathfinding.Core.Internal
{
    public unsafe class FastPriorityQueue<T>  where T : unmanaged, ICell
    {
        public int Count { get; private set; }

        private readonly IntPtr[] _collection;

        public FastPriorityQueue(int maxNodes)
        {
            Count = 0;
            _collection = new IntPtr[maxNodes + 1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enqueue(T* item, float priority)
        {
            item->ScoreF = priority;
            Count++;
            _collection[Count] = (IntPtr)item;
            item->QueueIndex = Count;
            CascadeUp(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T* Dequeue()
        {
            var result = _collection[1];

            if (Count == 1)
            {
                _collection[1] = default;
                Count = 0;

                return (T*)result;
            }

            var formerLastNode = (T*)_collection[Count];
            _collection[1] = (IntPtr)formerLastNode;
            formerLastNode->QueueIndex = 1;
            _collection[Count] = default;
            Count--;

            CascadeDown(formerLastNode);
            return (T*)result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T* item)
        {
            return _collection[item->QueueIndex] == (IntPtr)item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Array.Clear(_collection, 1, Count);
            Count = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CascadeUp(T* item)
        {
            //aka Heapify-up
            int parentIndex;

            if (item->QueueIndex > 1)
            {
                parentIndex = item->QueueIndex >> 1;
                var parentNode = _collection[parentIndex];

                if (HasHigherOrEqualPriority((T*)parentNode, item))
                    return;

                //Node has lower priority value, so move parent down the heap to make room
                _collection[item->QueueIndex] = parentNode;
                ((T*)parentNode)->QueueIndex = item->QueueIndex;

                item->QueueIndex = parentIndex;
            }
            else
            {
                return;
            }

            while (parentIndex > 1)
            {
                parentIndex >>= 1;
                var parentNode = _collection[parentIndex];

                if (HasHigherOrEqualPriority((T*)parentNode, item))
                    break;

                //Node has lower priority value, so move parent down the heap to make room
                _collection[item->QueueIndex] = parentNode;
                ((T*)parentNode)->QueueIndex = item->QueueIndex;

                item->QueueIndex = parentIndex;
            }

            _collection[item->QueueIndex] = (IntPtr)item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CascadeDown(T* item)
        {
            //aka Heapify-down
            var finalQueueIndex = item->QueueIndex;
            var childLeftIndex = 2 * finalQueueIndex;

            // If leaf node, we're done
            if (childLeftIndex > Count) return;

            // Check if the left-child is higher-priority than the current node
            var childRightIndex = childLeftIndex + 1;
            var childLeft = _collection[childLeftIndex];

            if (HasHigherPriority((T*)childLeft, item))
            {
                // Check if there is a right child. If not, swap and finish.
                if (childRightIndex > Count)
                {
                    item->QueueIndex = childLeftIndex;
                    ((T*)childLeft)->QueueIndex = finalQueueIndex;
                    _collection[finalQueueIndex] = childLeft;
                    _collection[childLeftIndex] = (IntPtr)item;

                    return;
                }

                // Check if the left-child is higher-priority than the right-child
                var childRight = _collection[childRightIndex];

                if (HasHigherPriority((T*)childLeft, (T*)childRight))
                {
                    // left is highest, move it up and continue
                    ((T*)childLeft)->QueueIndex = finalQueueIndex;
                    _collection[finalQueueIndex] = childLeft;
                    finalQueueIndex = childLeftIndex;
                }
                else
                {
                    // right is even higher, move it up and continue
                    ((T*)childRight)->QueueIndex = finalQueueIndex;
                    _collection[finalQueueIndex] = childRight;
                    finalQueueIndex = childRightIndex;
                }
            }
            // Not swapping with left-child, does right-child exist?
            else if (childRightIndex > Count)
            {
                return;
            }
            else
            {
                // Check if the right-child is higher-priority than the current node
                var childRight = _collection[childRightIndex];

                if (HasHigherPriority((T*)childRight, item))
                {
                    ((T*)childRight)->QueueIndex = finalQueueIndex;
                    _collection[finalQueueIndex] = childRight;
                    finalQueueIndex = childRightIndex;
                }
                // Neither child is higher-priority than current, so finish and stop.
                else
                {
                    return;
                }
            }

            while (true)
            {
                childLeftIndex = 2 * finalQueueIndex;

                // If leaf node, we're done
                if (childLeftIndex > Count)
                {
                    item->QueueIndex = finalQueueIndex;
                    _collection[finalQueueIndex] = (IntPtr)item;

                    break;
                }

                // Check if the left-child is higher-priority than the current node
                childRightIndex = childLeftIndex + 1;
                childLeft = _collection[childLeftIndex];

                if (HasHigherPriority((T*)childLeft, item))
                {
                    // Check if there is a right child. If not, swap and finish.
                    if (childRightIndex > Count)
                    {
                        item->QueueIndex = childLeftIndex;
                        ((T*)childLeft)->QueueIndex = finalQueueIndex;
                        _collection[finalQueueIndex] = childLeft;
                        _collection[childLeftIndex] = (IntPtr)item;

                        break;
                    }

                    // Check if the left-child is higher-priority than the right-child
                    var childRight = _collection[childRightIndex];

                    if (HasHigherPriority((T*)childLeft, (T*)childRight))
                    {
                        // left is highest, move it up and continue
                        ((T*)childLeft)->QueueIndex = finalQueueIndex;
                        _collection[finalQueueIndex] = childLeft;
                        finalQueueIndex = childLeftIndex;
                    }
                    else
                    {
                        // right is even higher, move it up and continue
                        ((T*)childRight)->QueueIndex = finalQueueIndex;
                        _collection[finalQueueIndex] = childRight;
                        finalQueueIndex = childRightIndex;
                    }
                }
                // Not swapping with left-child, does right-child exist?
                else if (childRightIndex > Count)
                {
                    item->QueueIndex = finalQueueIndex;
                    _collection[finalQueueIndex] = (IntPtr)item;

                    break;
                }
                else
                {
                    // Check if the right-child is higher-priority than the current node
                    var childRight = _collection[childRightIndex];

                    if (HasHigherPriority((T*)childRight, item))
                    {
                        ((T*)childRight)->QueueIndex = finalQueueIndex;
                        _collection[finalQueueIndex] = childRight;
                        finalQueueIndex = childRightIndex;
                    }
                    // Neither child is higher-priority than current, so finish and stop.
                    else
                    {
                        item->QueueIndex = finalQueueIndex;
                        _collection[finalQueueIndex] = (IntPtr)item;

                        break;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasHigherPriority(T* higher, T* lower) =>
            higher->ScoreF < lower->ScoreF;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasHigherOrEqualPriority(T* higher, T* lower) =>
            higher->ScoreF <= lower->ScoreF;
    }
}