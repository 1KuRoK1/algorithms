using System;
using System.Diagnostics;

class Program
{
    // Метод Кадане для 1D-массива — своя реализация.
    // Принцип: идём по массиву, поддерживаем cur (лучшая сумма, заканчивающаяся
    // здесь) и best (глобальный максимум); если cur < 0 — начинаем заново.
    static long MaxSubarray(long[] a, int m, long bestSoFar)
    {
        long cur = 0;
        long best = bestSoFar;
        for (int i = 0; i < m; i++)
        {
            cur += a[i];
            if (cur > best) best = cur;
            if (cur < 0) cur = 0;
        }
        return best;
    }

    static void Main()
    {
        Stopwatch swTotal = Stopwatch.StartNew();

        string[] first = Console.ReadLine()
                                .Split(new char[] { ' ', '\t' },
                                       StringSplitOptions.RemoveEmptyEntries);
        int N = int.Parse(first[0]);
        int M = int.Parse(first[1]);

        long[][] C = new long[N][];
        for (int i = 0; i < N; i++)
        {
            C[i] = new long[M];
            int filled = 0;
            while (filled < M)
            {
                string line = Console.ReadLine();
                if (line == null) break;
                string[] parts = line.Split(new char[] { ' ', '\t' },
                                            StringSplitOptions.RemoveEmptyEntries);
                for (int t = 0; t < parts.Length && filled < M; t++)
                    C[i][filled++] = long.Parse(parts[t]);
            }
        }

        long memStart = GC.GetTotalMemory(false);
        Stopwatch swAlgo = Stopwatch.StartNew();

        long best = long.MinValue;
        long[] colSum = new long[M];

        for (int top = 0; top < N; top++)
        {
            for (int j = 0; j < M; j++) colSum[j] = 0;

            for (int bottom = top; bottom < N; bottom++)
            {
                for (int j = 0; j < M; j++) colSum[j] += C[bottom][j];
                best = MaxSubarray(colSum, M, best);
            }
        }

        swAlgo.Stop();
        long memEnd = GC.GetTotalMemory(false);
        swTotal.Stop();

        Console.WriteLine(best);

        Console.Error.WriteLine($"TIME_TOTAL_MS={swTotal.Elapsed.TotalMilliseconds:F2}");
        Console.Error.WriteLine($"TIME_ALGO_MS={swAlgo.Elapsed.TotalMilliseconds:F2}");
        Console.Error.WriteLine($"MEM_MB={(memEnd - memStart) / 1024.0 / 1024.0:F2}");
    }
}