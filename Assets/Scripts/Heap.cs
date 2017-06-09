using System;
using System.Collections;
using System.Collections.Generic;

public class Heap<T> where T : IHeapItem<T>
{
    T[] items;
    int currentItemCount;

    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }

    public bool Contains(T item)
    {
        return Equals(item, items[item.HeapIndex]);
    }

    public int Count
    {
        get
        {
            return currentItemCount;
        }
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    void SortUp(T item)
    {
        while (true)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;
            T parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }
        }
    }

    void SortDown(T item)
    {
        while (true)
        {
            int childLeftIndex = 2 * item.HeapIndex + 1;
            int childRightIndex = childLeftIndex + 1;
            int swapIndex = 0;

            if (childLeftIndex < currentItemCount)
            {
                swapIndex = childLeftIndex;

                if (childRightIndex < currentItemCount && items[childLeftIndex].CompareTo(items[childRightIndex]) < 0)
                {
                    swapIndex = childRightIndex;
                }

                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }       
    }

    void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;

        int bIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemA.HeapIndex;
        itemA.HeapIndex = bIndex;
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}