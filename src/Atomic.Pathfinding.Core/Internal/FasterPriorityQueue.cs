using System;
using System.Runtime.CompilerServices;
using Atomic.Pathfinding.Core.Terrain;

namespace Atomic.Pathfinding.Core.Internal
{
    public ref struct FasterPriorityQueue
    {
        public int Count { get; private set; }

        private readonly PriorityQueueItem[] _collection;

        public FasterPriorityQueue(int maxNodes)
        {
            Count = 0;
            _collection = new PriorityQueueItem[maxNodes];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enqueue(ref PriorityQueueItem item, float priority)
        {
            item.SetPriority(priority);
            Count++;
            _collection[Count] = item;
            item.SetQueueIndex(Count);
            CascadeUp(ref item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PriorityQueueItem Dequeue()
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
            formerLastNode.SetQueueIndex(1);
            _collection[Count] = default;
            Count--;

            CascadeDown(formerLastNode);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdatePriority(ref PriorityQueueItem item, float priority)
        {
            item.SetPriority(priority);
            OnItemUpdated(ref item);
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
        private void CascadeUp(ref PriorityQueueItem item)
        {
            //aka Heapify-up
            int parentIndex;

            if (item.QueueIndex > 1)
            {
                parentIndex = item.QueueIndex >> 1;
                var parentNode = _collection[parentIndex];

                if (HasHigherOrEqualPriority(ref parentNode, ref item))
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

                if (HasHigherOrEqualPriority(ref parentNode, ref item))
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

            if (HasHigherPriority(ref childLeft, ref item))
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

                if (HasHigherPriority(ref childLeft, ref childRight))
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

                if (HasHigherPriority(ref childRight, ref item))
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

                if (HasHigherPriority(ref childLeft, ref item))
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

                    if (HasHigherPriority(ref childLeft, ref childRight))
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

                    if (HasHigherPriority(ref childRight, ref item))
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
        private bool HasHigherPriority(ref PriorityQueueItem higher, ref PriorityQueueItem lower) => higher.Priority < lower.Priority;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasHigherOrEqualPriority(ref PriorityQueueItem higher, ref PriorityQueueItem lower) => higher.Priority <= lower.Priority;
        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnItemUpdated(ref PriorityQueueItem node)
        {
            //Bubble the updated node up or down as appropriate
            var parentIndex = node.QueueIndex >> 1;

            if (parentIndex > 0 && HasHigherPriority(ref node, ref _collection[parentIndex]))
                CascadeUp(ref node);
            else
                CascadeDown(node);
        }
    }

    public struct PriorityQueueItem : IEquatable<PriorityQueueItem>
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

        public static bool operator ==(PriorityQueueItem left, PriorityQueueItem right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PriorityQueueItem left, PriorityQueueItem right)
        {
            return !(left == right);
        }

        public bool Equals(PriorityQueueItem other)
        {
            return QueueIndex == other.QueueIndex && CellIndex == other.CellIndex && IsInitiated == other.IsInitiated;
        }

        public override bool Equals(object obj)
        {
            return obj is PriorityQueueItem other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = QueueIndex;
                hashCode = (hashCode * 397) ^ CellIndex;
                hashCode = (hashCode * 397) ^ IsInitiated.GetHashCode();
                return hashCode;
            }
        }
    }
}