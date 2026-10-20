using System;
using System.Diagnostics;

class Program
{
    // Сортировка слиянием — своя реализация.
    // Принцип: делим массив пополам, рекурсивно сортируем каждую половину,
    // затем сливаем две отсортированные половины в один массив через буфер.
    static void MergeSort(int[] a, int left, int right, int[] buffer)
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

    // Бинарный поиск — своя реализация.
    // Принцип: на отсортированном массиве сравниваем target с серединой,
    // отбрасываем ту половину, где target быть не может, пока не сузим
    // до позиции вставки; затем сравниваем с соседями и берём ближайшего.
    static int FindClosest(int[] a, int n, long target)
    {
        int lo = 0, hi = n - 1;
        while (lo < hi)
        {
            int mid = (lo + hi) / 2;
            if (a[mid] < target) lo = mid + 1;
            else                 hi = mid;
        }
        long best = a[lo];
        if (lo > 0)
        {
            long cand = a[lo - 1];
            if (Math.Abs(cand - target) < Math.Abs(best - target)) best = cand;
        }
        if (lo + 1 < n)
        {
            long cand = a[lo + 1];
            if (Math.Abs(cand - target) < Math.Abs(best - target)) best = cand;
        }
        return (int)best;
    }

    // Перебор подмножеств рекурсией — своя реализация.
    // Принцип: для каждого камня две ветви — «не берём» и «берём»;
    // на листе рекурсии записываем накопленную сумму в массив.
    static int[] AllSubsetSums(int[] w, int start, int count)
    {
        int[] sums = new int[1 << count];
        FillSums(w, start, count, 0, 0, sums, 0);
        return sums;
    }

    static int FillSums(int[] w, int start, int count,
                        int idx, int curSum, int[] sums, int pos)
    {
        if (idx == count)
        {
            sums[pos] = curSum;
            return pos + 1;
        }
        pos = FillSums(w, start, count, idx + 1, curSum, sums, pos);
        pos = FillSums(w, start, count, idx + 1, curSum + w[start + idx], sums, pos);
        return pos;
    }

    static void Main()
    {
        // --- замер: старт ---
        Stopwatch sw = Stopwatch.StartNew();

        // ---------- Ввод ----------
        int n = int.Parse(Console.ReadLine().Trim());
        int[] w = new int[n];
        string[] parts = Console.ReadLine()
                                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        long total = 0;
        for (int i = 0; i < n; i++)
        {
            w[i] = int.Parse(parts[i]);
            total += w[i];
        }

        // фиксируем память после ввода (чтобы замер шёл «по алгоритму»)
        long memStart = GC.GetTotalMemory(false);

        // ---------- Meet-in-the-middle ----------
        // Meet-in-the-middle — схема решения.
        // Принцип: делим камни на две половины, перебираем все суммы
        // подмножеств каждой половины; правую сортируем, а для каждой
        // левой суммы бинарным поиском ищем ближайшую правую так,
        // чтобы вес куч был как можно ближе к половине общего веса.
        int half = n / 2;
        int[] leftW  = new int[half];
        int[] rightW = new int[n - half];
        for (int i = 0; i < half; i++) leftW[i]      = w[i];
        for (int i = half; i < n; i++) rightW[i - half] = w[i];

        int[] leftSums  = AllSubsetSums(leftW,  0, half);
        int[] rightSums = AllSubsetSums(rightW, 0, n - half);

        int[] buffer = new int[rightSums.Length];
        MergeSort(rightSums, 0, rightSums.Length, buffer);

        long bestDiff = long.MaxValue;
        for (int i = 0; i < leftSums.Length; i++)
        {
            long s1 = leftSums[i];
            long target = total / 2 - s1;
            int s2 = FindClosest(rightSums, rightSums.Length, target);
            long diff = Math.Abs(total - 2 * (s1 + s2));
            if (diff < bestDiff) bestDiff = diff;
        }

        // --- замер: финиш ---
        sw.Stop();
        long memEnd = GC.GetTotalMemory(false);

        // ---------- Вывод ----------
        Console.WriteLine(bestDiff);

        // диагностика в stderr — не мешает стандартному выводу
        Console.Error.WriteLine($"TIME_MS={sw.Elapsed.TotalMilliseconds:F2}");
        Console.Error.WriteLine($"MEM_MB={(memEnd - memStart) / 1024.0 / 1024.0:F2}");
    }
}