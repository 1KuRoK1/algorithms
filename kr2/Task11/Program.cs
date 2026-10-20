using System;
using System.Diagnostics;

class Program
{
    // Сортировка слиянием для long[] — своя реализация.
    // Принцип: делим пополам, рекурсивно сортируем, сливаем через буфер.
    static void MergeSort(long[] a, int left, int right, long[] buffer)
    {
        if (right - left <= 1) return;
        int mid = (left + right) / 2;
        MergeSort(a, left, mid, buffer);
        MergeSort(a, mid, right, buffer);
        int i = left, j = mid, k = left;
        while (i < mid && j < right)
        {
            if (a[i] <= a[j]) buffer[k++] = a[i++];
            else              buffer[k++] = a[j++];
        }
        while (i < mid)   buffer[k++] = a[i++];
        while (j < right) buffer[k++] = a[j++];
        for (int t = left; t < right; t++) a[t] = buffer[t];
    }

    // Бинарный поиск в отсортированном long[] — своя реализация.
    // Принцип: сравниваем с серединой и отбрасываем половину, где элемента быть не может.
    static int BinarySearch(long[] a, int n, long x)
    {
        int lo = 0, hi = n - 1;
        while (lo <= hi)
        {
            int mid = (lo + hi) / 2;
            if (a[mid] == x) return mid;
            if (a[mid] < x) lo = mid + 1;
            else             hi = mid - 1;
        }
        return -1;
    }

    // Индексная max-куча по nextPos — своя реализация.
    // Принцип: в корне лежит элемент с самым дальним следующим использованием;
    // posInHeap даёт O(1) доступ к позиции элемента в куче для уменьшения ключа.
    class IndexedMaxHeap
    {
        public int[] item;
        public int[] nextPos;
        public int[] posInHeap;
        public int size;

        public void Init(int capacity, int uniqueCount)
        {
            item      = new int[capacity + 2];
            nextPos   = new int[capacity + 2];
            posInHeap = new int[uniqueCount];
            for (int i = 0; i < uniqueCount; i++) posInHeap[i] = -1;
            size = 0;
        }

        void Swap(int i, int j)
        {
            int ti = item[i];    item[i]    = item[j];    item[j]    = ti;
            int tn = nextPos[i]; nextPos[i] = nextPos[j]; nextPos[j] = tn;
            posInHeap[item[i]] = i;
            posInHeap[item[j]] = j;
        }

        void SiftUp(int i)
        {
            while (i > 1)
            {
                int p = i / 2;
                if (nextPos[p] < nextPos[i]) { Swap(p, i); i = p; }
                else break;
            }
        }

        void SiftDown(int i)
        {
            while (true)
            {
                int l = 2 * i, r = 2 * i + 1, biggest = i;
                if (l <= size && nextPos[l] > nextPos[biggest]) biggest = l;
                if (r <= size && nextPos[r] > nextPos[biggest]) biggest = r;
                if (biggest == i) break;
                Swap(i, biggest);
                i = biggest;
            }
        }

        public void Insert(int ci, int np)
        {
            size++;
            item[size]    = ci;
            nextPos[size] = np;
            posInHeap[ci] = size;
            SiftUp(size);
        }

        public int ExtractMax()
        {
            int top = item[1];
            posInHeap[top] = -1;
            if (size == 1) { size = 0; return top; }
            item[1]    = item[size];
            nextPos[1] = nextPos[size];
            posInHeap[item[1]] = 1;
            size--;
            SiftDown(1);
            return top;
        }

        public void DecreaseKey(int ci, int np)
        {
            int i = posInHeap[ci];
            nextPos[i] = np;
            SiftUp(i);
        }
    }

    static void Main()
    {
        Stopwatch sw = Stopwatch.StartNew();

        // первая строка: N M
        string firstLine = Console.ReadLine();
        string[] first = firstLine.Split(new char[] { ' ', '\t' },
                                         StringSplitOptions.RemoveEmptyEntries);
        int N = int.Parse(first[0]);
        int M = int.Parse(first[1]);

        // M запросов: по одному в строке или все в одной
        long[] R = new long[M];
        int filled = 0;
        while (filled < M)
        {
            string line = Console.ReadLine();
            if (line == null) break;
            string[] parts = line.Split(new char[] { ' ', '\t' },
                                        StringSplitOptions.RemoveEmptyEntries);
            for (int t = 0; t < parts.Length && filled < M; t++)
                R[filled++] = long.Parse(parts[t]);
        }

        long memStart = GC.GetTotalMemory(false);

        // сжатие координат
        long[] sorted = new long[M];
        for (int i = 0; i < M; i++) sorted[i] = R[i];
        long[] buf = new long[M];
        MergeSort(sorted, 0, M, buf);

        int K = 0;
        for (int i = 0; i < M; i++)
        {
            if (i == 0 || sorted[i] != sorted[i - 1])
            {
                sorted[K] = sorted[i];
                K++;
            }
        }

        int[] comp = new int[M];
        for (int i = 0; i < M; i++)
            comp[i] = BinarySearch(sorted, K, R[i]);

        // next[i] = позиция следующего вхождения того же значения
        int[] next = new int[M];
        int[] lastSeen = new int[K];
        for (int i = 0; i < K; i++) lastSeen[i] = int.MaxValue;
        for (int i = M - 1; i >= 0; i--)
        {
            int ci = comp[i];
            next[i] = lastSeen[ci];
            lastSeen[ci] = i;
        }

        // алгоритм Белади: при промахе с полным кешем вытесняем тот,
        // чьё следующее использование наступит позже всех
        bool[] inCache = new bool[K];
        IndexedMaxHeap heap = new IndexedMaxHeap();
        heap.Init(Math.Max(N, M) + 2, K);

        int misses = 0;
        int cacheSize = 0;

        for (int i = 0; i < M; i++)
        {
            int ci = comp[i];
            if (inCache[ci])
            {
                heap.DecreaseKey(ci, next[i]);
            }
            else
            {
                misses++;
                if (cacheSize < N)
                {
                    inCache[ci] = true;
                    heap.Insert(ci, next[i]);
                    cacheSize++;
                }
                else
                {
                    int evicted = heap.ExtractMax();
                    inCache[evicted] = false;
                    inCache[ci] = true;
                    heap.Insert(ci, next[i]);
                }
            }
        }

        sw.Stop();
        long memEnd = GC.GetTotalMemory(false);

        Console.WriteLine(misses);
        Console.Error.WriteLine($"TIME_MS={sw.Elapsed.TotalMilliseconds:F2}");
        Console.Error.WriteLine($"MEM_MB={(memEnd - memStart) / 1024.0 / 1024.0:F2}");
    }
}