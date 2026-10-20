using System;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        Stopwatch swTotal = Stopwatch.StartNew();

        // первая строка: N M
        string firstLine = Console.ReadLine();
        string[] first = firstLine.Split(new char[] { ' ', '\t' },
                                         StringSplitOptions.RemoveEmptyEntries);
        int N = int.Parse(first[0]);
        int M = int.Parse(first[1]);

        // N цветов: по одному в строке или все в одной
        int[] v = new int[N];
        int filled = 0;
        while (filled < N)
        {
            string line = Console.ReadLine();
            if (line == null) break;
            string[] parts = line.Split(new char[] { ' ', '\t' },
                                        StringSplitOptions.RemoveEmptyEntries);
            for (int t = 0; t < parts.Length && filled < N; t++)
                v[filled++] = int.Parse(parts[t]);
        }

        long memStart = GC.GetTotalMemory(false);

        // замер только алгоритма
        Stopwatch swAlgo = Stopwatch.StartNew();

        // Находим максимальный p, при котором v[0..p-1] зеркально совпадает
        // с v[N-p..N-1]. Идём от краёв к центру, пока пары совпадают.
        int maxP = 0;
        while (maxP < N / 2 && v[maxP] == v[N - 1 - maxP])
            maxP++;

        swAlgo.Stop();
        long memEnd = GC.GetTotalMemory(false);
        swTotal.Stop();

        // K = N - p для p = 0..maxP; выводим в порядке убывания K
        for (int p = maxP; p >= 0; p--)
        {
            Console.WriteLine(N - p);
        }

        Console.Error.WriteLine($"TIME_TOTAL_MS={swTotal.Elapsed.TotalMilliseconds:F2}");
        Console.Error.WriteLine($"TIME_ALGO_MS={swAlgo.Elapsed.TotalMilliseconds:F2}");
        Console.Error.WriteLine($"MEM_MB={(memEnd - memStart) / 1024.0 / 1024.0:F2}");
    }
}