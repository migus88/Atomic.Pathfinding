using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Atomic.Pathfinding.Core.Internal
{
    public class FastPriorityQueue<T> where T : class, IPriorityProvider
    {
        public int Count { get; private set; }

        private readonly T[] _collection;
        private double? _lowestPriority = null;

        public FastPriorityQueue(int maxNodes = 10000)
        {
            Count = 0;
            _collection = new T[maxNodes];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enqueue(T item, double priority)
        {
            item.Priority = priority;
            Count++;
            _collection[Count] = item;
            item.QueueIndex = Count;
            CascadeUp(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Dequeue()
        {
            var result = _collection[1];

            if (Count == 1)
            {
                _collection[1] = null;
                Count = 0;

                return result;
            }

            var formerLastNode = _collection[Count];
            _collection[1] = formerLastNode;
            formerLastNode.QueueIndex = 1;
            _collection[Count] = null;
            Count--;

            CascadeDown(formerLastNode);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdatePriority(T item, double priority)
        {
            item.Priority = priority;
            OnItemUpdated(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item)
        {
            return _collection[item.QueueIndex] == item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Array.Clear(_collection, 1, Count);
            Count = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CascadeUp(T item)
        {
            //aka Heapify-up
            int parent;

            if (item.QueueIndex > 1)
            {
                parent = item.QueueIndex >> 1;
                var parentNode = _collection[parent];

                if (HasHigherOrEqualPriority(parentNode, item))
                    return;

                //Node has lower priority value, so move parent down the heap to make room
                _collection[item.QueueIndex] = parentNode;
                parentNode.QueueIndex = item.QueueIndex;

                item.QueueIndex = parent;
            }
            else
            {
                return;
            }

            while (parent > 1)
            {
                parent >>= 1;
                var parentNode = _collection[parent];

                if (HasHigherOrEqualPriority(parentNode, item))
                    break;

                //Node has lower priority value, so move parent down the heap to make room
                _collection[item.QueueIndex] = parentNode;
                parentNode.QueueIndex = item.QueueIndex;

                item.QueueIndex = parent;
            }

            _collection[item.QueueIndex] = item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CascadeDown(T item)
        {
            //aka Heapify-down
            var finalQueueIndex = item.QueueIndex;
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
                    item.QueueIndex = childLeftIndex;
                    childLeft.QueueIndex = finalQueueIndex;
                    _collection[finalQueueIndex] = childLeft;
                    _collection[childLeftIndex] = item;

                    return;
                }

                // Check if the left-child is higher-priority than the right-child
                var childRight = _collection[childRightIndex];

                if (HasHigherPriority(childLeft, childRight))
                {
                    // left is highest, move it up and continue
                    childLeft.QueueIndex = finalQueueIndex;
                    _collection[finalQueueIndex] = childLeft;
                    finalQueueIndex = childLeftIndex;
                }
                else
                {
                    // right is even higher, move it up and continue
                    childRight.QueueIndex = finalQueueIndex;
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
                    childRight.QueueIndex = finalQueueIndex;
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
                    item.QueueIndex = finalQueueIndex;
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
                        item.QueueIndex = childLeftIndex;
                        childLeft.QueueIndex = finalQueueIndex;
                        _collection[finalQueueIndex] = childLeft;
                        _collection[childLeftIndex] = item;

                        break;
                    }

                    // Check if the left-child is higher-priority than the right-child
                    var childRight = _collection[childRightIndex];

                    if (HasHigherPriority(childLeft, childRight))
                    {
                        // left is highest, move it up and continue
                        childLeft.QueueIndex = finalQueueIndex;
                        _collection[finalQueueIndex] = childLeft;
                        finalQueueIndex = childLeftIndex;
                    }
                    else
                    {
                        // right is even higher, move it up and continue
                        childRight.QueueIndex = finalQueueIndex;
                        _collection[finalQueueIndex] = childRight;
                        finalQueueIndex = childRightIndex;
                    }
                }
                // Not swapping with left-child, does right-child exist?
                else if (childRightIndex > Count)
                {
                    item.QueueIndex = finalQueueIndex;
                    _collection[finalQueueIndex] = item;

                    break;
                }
                else
                {
                    // Check if the right-child is higher-priority than the current node
                    var childRight = _collection[childRightIndex];

                    if (HasHigherPriority(childRight, item))
                    {
                        childRight.QueueIndex = finalQueueIndex;
                        _collection[finalQueueIndex] = childRight;
                        finalQueueIndex = childRightIndex;
                    }
                    // Neither child is higher-priority than current, so finish and stop.
                    else
                    {
                        item.QueueIndex = finalQueueIndex;
                        _collection[finalQueueIndex] = item;

                        break;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasHigherPriority(T higher, T lower) => higher.Priority < lower.Priority;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasHigherOrEqualPriority(T higher, T lower) => higher.Priority <= lower.Priority;
        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnItemUpdated(T node)
        {
            //Bubble the updated node up or down as appropriate
            var parentIndex = node.QueueIndex >> 1;

            if (parentIndex > 0 && HasHigherPriority(node, _collection[parentIndex]))
                CascadeUp(node);
            else
                CascadeDown(node);
        }
    }


    public interface IPriorityProvider
    {
        double Priority { get; set; }
        int QueueIndex { get; set; }
    }
}