using System;
using System.Runtime.CompilerServices;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Interfaces;

namespace Atomic.Pathfinding.Core.Internal
{
    internal unsafe class FastPriorityQueue : IDisposable
    {
        public int Count;

        private readonly Cell*[] _collection;

        public FastPriorityQueue(int maxNodes)
        {
            Count = 0;
            _collection = new Cell*[maxNodes + 1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enqueue(Cell* item, float priority)
        {
            item->ScoreF = priority;
            Count++;
            _collection[Count] = item;
            item->QueueIndex = Count;
            CascadeUp(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Cell* Dequeue()
        {
            var result = _collection[1];

            if (Count == 1)
            {
                _collection[1] = default;
                Count = 0;

                return result;
            }

            var formerLastNode = _collection[Count];
            _collection[1] = formerLastNode;
            formerLastNode->QueueIndex = 1;
            _collection[Count] = default;
            Count--;

            CascadeDown(formerLastNode);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Cell* item)
        {
            return _collection[item->QueueIndex] == item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Array.Clear(_collection, 1, Count);
            Count = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CascadeUp(Cell* item)
        {
            //aka Heapify-up
            int parentIndex;

            if (item->QueueIndex > 1)
            {
                parentIndex = item->QueueIndex >> 1;
                var parentNode = _collection[parentIndex];

                if (HasHigherOrEqualPriority(parentNode, item))
                    return;

                //Node has lower priority value, so move parent down the heap to make room
                _collection[item->QueueIndex] = parentNode;
                (parentNode)->QueueIndex = item->QueueIndex;

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

                if (HasHigherOrEqualPriority(parentNode, item))
                    break;

                //Node has lower priority value, so move parent down the heap to make room
                _collection[item->QueueIndex] = parentNode;
                (parentNode)->QueueIndex = item->QueueIndex;

                item->QueueIndex = parentIndex;
            }

            _collection[item->QueueIndex] = item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CascadeDown(Cell* item)
        {
            //aka Heapify-down
            var finalQueueIndex = item->QueueIndex;
            var childLeftIndex = 2 * finalQueueIndex;

            // If leaf node, we're done
            if (childLeftIndex > Count) return;

            // Check if the left-child is higher-priority than the current node
            var childRightIndex = childLeftIndex + 1;
            var childLeft = _collection[childLeftIndex];

            if (HasHigherPriority(childLeft, item))
            {
                // Check if there is a right child. If not, swap and finish.
                if (childRightIndex > Count)
                {
                    item->QueueIndex = childLeftIndex;
                    (childLeft)->QueueIndex = finalQueueIndex;
                    _collection[finalQueueIndex] = childLeft;
                    _collection[childLeftIndex] = item;

                    return;
                }

                // Check if the left-child is higher-priority than the right-child
                var childRight = _collection[childRightIndex];

                if (HasHigherPriority(childLeft, childRight))
                {
                    // left is highest, move it up and continue
                    (childLeft)->QueueIndex = finalQueueIndex;
                    _collection[finalQueueIndex] = childLeft;
                    finalQueueIndex = childLeftIndex;
                }
                else
                {
                    // right is even higher, move it up and continue
                    (childRight)->QueueIndex = finalQueueIndex;
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

                if (HasHigherPriority(childRight, item))
                {
                    (childRight)->QueueIndex = finalQueueIndex;
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
                    _collection[finalQueueIndex] = item;

                    break;
                }

                // Check if the left-child is higher-priority than the current node
                childRightIndex = childLeftIndex + 1;
                childLeft = _collection[childLeftIndex];

                if (HasHigherPriority(childLeft, item))
                {
                    // Check if there is a right child. If not, swap and finish.
                    if (childRightIndex > Count)
                    {
                        item->QueueIndex = childLeftIndex;
                        (childLeft)->QueueIndex = finalQueueIndex;
                        _collection[finalQueueIndex] = childLeft;
                        _collection[childLeftIndex] = item;

                        break;
                    }

                    // Check if the left-child is higher-priority than the right-child
                    var childRight = _collection[childRightIndex];

                    if (HasHigherPriority(childLeft, childRight))
                    {
                        // left is highest, move it up and continue
                        (childLeft)->QueueIndex = finalQueueIndex;
                        _collection[finalQueueIndex] = childLeft;
                        finalQueueIndex = childLeftIndex;
                    }
                    else
                    {
                        // right is even higher, move it up and continue
                        (childRight)->QueueIndex = finalQueueIndex;
                        _collection[finalQueueIndex] = childRight;
                        finalQueueIndex = childRightIndex;
                    }
                }
                // Not swapping with left-child, does right-child exist?
                else if (childRightIndex > Count)
                {
                    item->QueueIndex = finalQueueIndex;
                    _collection[finalQueueIndex] = item;

                    break;
                }
                else
                {
                    // Check if the right-child is higher-priority than the current node
                    var childRight = _collection[childRightIndex];

                    if (HasHigherPriority(childRight, item))
                    {
                        (childRight)->QueueIndex = finalQueueIndex;
                        _collection[finalQueueIndex] = childRight;
                        finalQueueIndex = childRightIndex;
                    }
                    // Neither child is higher-priority than current, so finish and stop.
                    else
                    {
                        item->QueueIndex = finalQueueIndex;
                        _collection[finalQueueIndex] = item;

                        break;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasHigherPriority(Cell* higher, Cell* lower) =>
            higher->ScoreF < lower->ScoreF;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasHigherOrEqualPriority(Cell* higher, Cell* lower) =>
            higher->ScoreF <= lower->ScoreF;

        public void Dispose()
        {
            
        }
    }
}