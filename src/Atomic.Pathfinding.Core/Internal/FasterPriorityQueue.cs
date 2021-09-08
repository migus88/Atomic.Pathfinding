using System;
using System.Runtime.CompilerServices;

namespace Atomic.Pathfinding.Core.Internal
{
    public ref struct FasterPriorityQueue
    {
        public int Count { get; private set; }

        private readonly PriorityQueueItem[] _collection;

        public FasterPriorityQueue(int maxNodes)
        {
            Count = 0;
            _collection = new PriorityQueueItem[maxNodes + 1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enqueue(PriorityQueueItem item, float priority)
        {
            item.SetPriority(priority);
            Count++;
            _collection[Count] = item;
            item.SetQueueIndex(Count);
            CascadeUp(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PriorityQueueItem Dequeue()
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
            formerLastNode.SetQueueIndex(1);
            _collection[Count] = null;
            Count--;

            CascadeDown(formerLastNode);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdatePriority(PriorityQueueItem item, float priority)
        {
            item.SetPriority(priority);
            OnItemUpdated(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(PriorityQueueItem item)
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
        private void CascadeUp(PriorityQueueItem item)
        {
            //aka Heapify-up
            int parentIndex;

            if (item.QueueIndex > 1)
            {
                parentIndex = item.QueueIndex >> 1;
                var parentNode = _collection[parentIndex];

                if (HasHigherOrEqualPriority(parentNode, item))
                    return;

                //Node has lower priority value, so move parent down the heap to make room
                _collection[item.QueueIndex] = parentNode;
                parentNode.SetQueueIndex(item.QueueIndex);

                item.SetQueueIndex(parentIndex);
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
                _collection[item.QueueIndex] = parentNode;
                parentNode.SetQueueIndex(item.QueueIndex);

                item.SetQueueIndex(parentIndex);
            }

            _collection[item.QueueIndex] = item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CascadeDown(PriorityQueueItem item)
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
                    item.SetQueueIndex(childLeftIndex);
                    childLeft.SetQueueIndex(finalQueueIndex);
                    _collection[finalQueueIndex] = childLeft;
                    _collection[childLeftIndex] = item;

                    return;
                }

                // Check if the left-child is higher-priority than the right-child
                var childRight = _collection[childRightIndex];

                if (HasHigherPriority(childLeft, childRight))
                {
                    // left is highest, move it up and continue
                    childLeft.SetQueueIndex(finalQueueIndex);
                    _collection[finalQueueIndex] = childLeft;
                    finalQueueIndex = childLeftIndex;
                }
                else
                {
                    // right is even higher, move it up and continue
                    childRight.SetQueueIndex(finalQueueIndex);
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
                    childRight.SetQueueIndex(finalQueueIndex);
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
                    item.SetQueueIndex(finalQueueIndex);
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
                        item.SetQueueIndex(childLeftIndex);
                        childLeft.SetQueueIndex(finalQueueIndex);
                        _collection[finalQueueIndex] = childLeft;
                        _collection[childLeftIndex] = item;

                        break;
                    }

                    // Check if the left-child is higher-priority than the right-child
                    var childRight = _collection[childRightIndex];

                    if (HasHigherPriority(childLeft, childRight))
                    {
                        // left is highest, move it up and continue
                        childLeft.SetQueueIndex(finalQueueIndex);
                        _collection[finalQueueIndex] = childLeft;
                        finalQueueIndex = childLeftIndex;
                    }
                    else
                    {
                        // right is even higher, move it up and continue
                        childRight.SetQueueIndex(finalQueueIndex);
                        _collection[finalQueueIndex] = childRight;
                        finalQueueIndex = childRightIndex;
                    }
                }
                // Not swapping with left-child, does right-child exist?
                else if (childRightIndex > Count)
                {
                    item.SetQueueIndex(finalQueueIndex);
                    _collection[finalQueueIndex] = item;

                    break;
                }
                else
                {
                    // Check if the right-child is higher-priority than the current node
                    var childRight = _collection[childRightIndex];

                    if (HasHigherPriority(childRight, item))
                    {
                        childRight.SetQueueIndex(finalQueueIndex);
                        _collection[finalQueueIndex] = childRight;
                        finalQueueIndex = childRightIndex;
                    }
                    // Neither child is higher-priority than current, so finish and stop.
                    else
                    {
                        item.SetQueueIndex(finalQueueIndex);
                        _collection[finalQueueIndex] = item;

                        break;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasHigherPriority(PriorityQueueItem higher, PriorityQueueItem lower) =>
            higher.Priority < lower.Priority;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasHigherOrEqualPriority(PriorityQueueItem higher, PriorityQueueItem lower) =>
            higher.Priority <= lower.Priority;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnItemUpdated(PriorityQueueItem node)
        {
            //Bubble the updated node up or down as appropriate
            var parentIndex = node.QueueIndex >> 1;

            if (parentIndex > 0 && HasHigherPriority(node, _collection[parentIndex]))
                CascadeUp(node);
            else
                CascadeDown(node);
        }
    }

    public class PriorityQueueItem
    {
        public int CellIndex { get; private set; }
        public int QueueIndex { get; private set; }
        public float Priority { get; private set; }
        public bool IsInitiated { get; private set; }

        public PriorityQueueItem(int cellIndex)
        {
            CellIndex = cellIndex;
            QueueIndex = 0;
            Priority = 0;
            IsInitiated = true;
        }

        public void SetQueueIndex(int index)
        {
            IsInitiated = true;
            QueueIndex = index;
        }

        public void SetPriority(float priority)
        {
            IsInitiated = true;
            Priority = priority;
        }

        public void SetCellIndex(int index)
        {
            IsInitiated = true;
            CellIndex = index;
        }
    }
}