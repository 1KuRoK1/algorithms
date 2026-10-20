using System;
using System.Diagnostics;

class Program
{
    const long MOD = 1000000007L;

    static void Main()
    {
        Stopwatch swTotal = Stopwatch.StartNew();

        string[] parts = Console.ReadLine()
                                .Split(new char[] { ' ', '\t' },
                                       StringSplitOptions.RemoveEmptyEntries);
        int n = int.Parse(parts[0]);
        int a = int.Parse(parts[1]);
        int b = int.Parse(parts[2]);

        long memStart = GC.GetTotalMemory(false);
        Stopwatch swAlgo = Stopwatch.StartNew();

        // f1[i] — число допустимых последовательностей длины i, оканчивающихся на 1;
        // f2[i] — на 2. Переход: добавляем 1/2 ко всем f1[i-1]+f2[i-1],
        // вычитаем те, где уже a единиц подряд (b двоек).
        long[] f1 = new long[n + 1];
        long[] f2 = new long[n + 1];

        if (n >= 1)
        {
            f1[0] = 1; f2[0] = 1;   // «пустая» последовательность
            f1[1] = 1; f2[1] = 1;
        }

        for (int i = 2; i <= n; i++)
        {
            long total = (f1[i - 1] + f2[i - 1]) % MOD;

            long sub1 = 0;
            if (i - 1 - a >= 0) sub1 = f2[i - 1 - a];
            f1[i] = (total - sub1) % MOD;
            if (f1[i] < 0) f1[i] += MOD;

            long sub2 = 0;
            if (i - 1 - b >= 0) sub2 = f1[i - 1 - b];
            f2[i] = (total - sub2) % MOD;
            if (f2[i] < 0) f2[i] += MOD;
        }

        long answer = (f1[n] + f2[n]) % MOD;

        swAlgo.Stop();
        long memEnd = GC.GetTotalMemory(false);
        swTotal.Stop();

        Console.WriteLine(answer);

        Console.Error.WriteLine($"TIME_TOTAL_MS={swTotal.Elapsed.TotalMilliseconds:F2}");
        Console.Error.WriteLine($"TIME_ALGO_MS={swAlgo.Elapsed.TotalMilliseconds:F2}");
        Console.Error.WriteLine($"MEM_MB={(memEnd - memStart) / 1024.0 / 1024.0:F2}");
    }
}