using System.Collections.Generic;

public class MinHeap
{
    private List<Cell> heap = new List<Cell>();

    public int Count => heap.Count;

    public void Add(Cell c)
    {
        heap.Add(c);
        HeapifyUp(heap.Count - 1);
    }

    public Cell PopMin()
    {
        if (heap.Count == 0) return null;

        Cell min = heap[0];
        heap[0] = heap[heap.Count - 1];
        heap.RemoveAt(heap.Count - 1);

        HeapifyDown(0);

        return min;
    }

    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parent = (index - 1) / 2;
            if (heap[index].fCost >= heap[parent].fCost)
                break;

            (heap[index], heap[parent]) = (heap[parent], heap[index]);
            index = parent;
        }
    }

    private void HeapifyDown(int index)
    {
        int lastIndex = heap.Count - 1;

        while (true)
        {
            int left = index * 2 + 1;
            int right = index * 2 + 2;
            int smallest = index;

            if (left <= lastIndex && heap[left].fCost < heap[smallest].fCost)
                smallest = left;
            if (right <= lastIndex && heap[right].fCost < heap[smallest].fCost)
                smallest = right;

            if (smallest == index)
                break;

            (heap[index], heap[smallest]) = (heap[smallest], heap[index]);
            index = smallest;
        }
    }

    public bool Contains(Cell c) => heap.Contains(c);
}
