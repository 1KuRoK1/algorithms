using System;
using System.Diagnostics;
using System.Globalization;

class Program
{
    struct Sheet
    {
        public double A;
        public double B;
        public int Index;       // исходный номер (1-based)
    }

    // Сортировка слиянием для Sheet[] — своя реализация.
    // Принцип: делим пополам, рекурсивно сортируем, сливаем через буфер;
    // сравнение — по убыванию (A - B), при равенстве — по возрастанию Index.
    static void MergeSort(Sheet[] s, int left, int right, Sheet[] buffer)
    {
        if (right - left <= 1) return;
        int mid = (left + right) / 2;
        MergeSort(s, left, mid, buffer);
        MergeSort(s, mid, right, buffer);

        int i = left, j = mid, k = left;
        while (i < mid && j < right)
        {
            if (Better(s[i], s[j])) buffer[k++] = s[i++];
            else                    buffer[k++] = s[j++];
        }
        while (i < mid)   buffer[k++] = s[i++];
        while (j < right) buffer[k++] = s[j++];
        for (int t = left; t < right; t++) s[t] = buffer[t];
    }

    // Своё сравнение: i должен идти раньше j, если A_i - B_i > A_j - B_j;
    // при равенстве — меньший исходный номер идёт раньше (стабильность).
    static bool Better(Sheet x, Sheet y)
    {
        double dx = x.A - x.B;
        double dy = y.A - y.B;
        if (dx != dy) return dx > dy;
        return x.Index < y.Index;
    }

    static void Main()
    {
        Stopwatch swTotal = Stopwatch.StartNew();

        int N = int.Parse(Console.ReadLine().Trim());
        Sheet[] sheets = new Sheet[N];
        for (int i = 0; i < N; i++)
        {
            string[] parts = Console.ReadLine()
                                    .Split(new char[] { ' ', '\t' },
                                           StringSplitOptions.RemoveEmptyEntries);
            sheets[i].A = double.Parse(parts[0], CultureInfo.InvariantCulture);
            sheets[i].B = double.Parse(parts[1], CultureInfo.InvariantCulture);
            sheets[i].Index = i + 1;
        }

        long memStart = GC.GetTotalMemory(false);
        Stopwatch swAlgo = Stopwatch.StartNew();

        // сортируем по убыванию (A - B)
        Sheet[] buf = new Sheet[N];
        MergeSort(sheets, 0, N, buf);

        // считаем время растворения: минимум по k от 0 до N
        // max(сумма A первых k, сумма B последних N-k)
        double[] prefixA = new double[N + 1];
        double[] suffixB = new double[N + 2];
        for (int i = 0; i < N; i++) prefixA[i + 1] = prefixA[i] + sheets[i].A;
        for (int i = N - 1; i >= 0; i--) suffixB[i] = suffixB[i + 1] + sheets[i].B;

        double best = double.MinValue;
        for (int k = 0; k <= N; k++)
        {
            double t = Math.Max(prefixA[k], suffixB[k]);
            if (t > best) best = t;
        }

        swAlgo.Stop();
        long memEnd = GC.GetTotalMemory(false);
        swTotal.Stop();

        // вывод: время и порядок листов
        Console.WriteLine(best.ToString("F3", CultureInfo.InvariantCulture));
        for (int i = 0; i < N; i++)
        {
            if (i > 0) Console.Write(' ');
            Console.Write(sheets[i].Index);
        }
        Console.WriteLine();

        Console.Error.WriteLine($"TIME_TOTAL_MS={swTotal.Elapsed.TotalMilliseconds:F2}");
        Console.Error.WriteLine($"TIME_ALGO_MS={swAlgo.Elapsed.TotalMilliseconds:F2}");
        Console.Error.WriteLine($"MEM_MB={(memEnd - memStart) / 1024.0 / 1024.0:F2}");
    }
}