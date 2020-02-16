using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T>
{
    //priority queue represented as a list
    List<T> pQueue;
    //comparison type between two objects
    public delegate bool Comparator(T t1, T t2);
    Comparator comp;
    public PriorityQueue(Comparator comp)
    {
        pQueue = new List<T>();
        this.comp = comp;
    }
    public List<T> getList()
    {
        return pQueue;
    }
    public void Add(T item)
    {
        pQueue.Add(item);
        moveUp();
    }
    public int Count
    {
        get{return pQueue.Count;}
    }
    public bool isEmpty
    {
        get{return pQueue.Count == 0;}
    }
    public T pop()
    {
        T front = pQueue[0];
        int last = pQueue.Count-1;
        pQueue[0] = pQueue[last];
        pQueue.RemoveAt(last); //constant time since item is last in list
        moveDown();
        return front;
    }
    //moves the last node up in O(logn) time
    void moveUp()
    {
        int currentIndex = pQueue.Count-1;
        while(currentIndex > 0)
        {
            int parentIndex = getParent(currentIndex);
            if (comp(pQueue[currentIndex], pQueue[parentIndex]))
            {
                T temp = pQueue[parentIndex];
                pQueue[parentIndex] = pQueue[currentIndex];
                pQueue[currentIndex] = temp;
            }
            else
            {
                return;
            }
            currentIndex = parentIndex;
        }
    }
    //moves the node at the top down in O(logn) time
    void moveDown()
    {
        int currentIndex = 0;
        int previousIndex = -1;
        while (currentIndex != previousIndex)
        {
            previousIndex = currentIndex;
            int leftIndex = getLeftChild(currentIndex);
            int rightIndex = getRightChild(currentIndex);
            if(leftIndex >= pQueue.Count)
            {
                return;
            }
            T current = pQueue[currentIndex];
            T left = pQueue[leftIndex];
            
            if(rightIndex >= pQueue.Count)
            {
                if (comp(current,left))
                {
                    return;
                }
                else
                {
                    pQueue[currentIndex] = left;
                    pQueue[leftIndex] = current;
                    currentIndex = leftIndex;
                }
            }
            else
            {
                T right = pQueue[rightIndex];
                if (comp(current,left) && comp(current,right))
                {
                    return;
                }
                else if(comp(left, current) && comp(left, right))
                {
                    pQueue[currentIndex] = left;
                    pQueue[leftIndex] = current;
                    currentIndex = leftIndex;
                }
                else if(comp(right, current) && comp(right,left))
                {
                    pQueue[currentIndex] = right;
                    pQueue[rightIndex] = current;
                    currentIndex = rightIndex;
                }
            }
        }

    }
    int getParent(int index)
    {
        return (index-1)/2;
    }
    int getLeftChild(int index)
    {
        return (index+1)*2-1;
    }
    int getRightChild(int index)
    {
        return (index+1)*2;
    }
}
