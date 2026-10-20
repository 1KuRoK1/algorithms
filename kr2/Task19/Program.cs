using System;
using System.Diagnostics;

class Program
{
    // Проверка: t[l..r-1] == (y)^k, где y — s[yStart..yStart+b-1] — своя.
    // Принцип: идём по t и сравниваем с y циклически; если хоть один символ
    // не совпал — не степень.
    static bool IsPower(string t, int l, int r, string s, int yStart, int b)
    {
        int len = r - l;
        if (b == 0) return len == 0;
        if (len % b != 0) return false;
        for (int i = 0; i < len; i++)
        {
            if (t[l + i] != s[yStart + (i % b)]) return false;
        }
        return true;
    }

    // Сравнение s[0..a-1] с t[0..a-1] — своя.
    static bool SamePrefix(string s, string t, int a)
    {
        for (int i = 0; i < a; i++)
            if (s[i] != t[i]) return false;
        return true;
    }

    // Сравнение s[n-c..n-1] с t[m-c..m-1] — своя.
    static bool SameSuffix(string s, int n, string t, int m, int c)
    {
        for (int i = 0; i < c; i++)
            if (s[n - c + i] != t[m - c + i]) return false;
        return true;
    }

    static void Main()
    {
        Stopwatch sw = Stopwatch.StartNew();

        string firstLine = Console.ReadLine();
        string[] first = firstLine.Split(new char[] { ' ', '\t' },
                                         StringSplitOptions.RemoveEmptyEntries);
        int n = int.Parse(first[0]);
        int m = int.Parse(first[1]);
        string s = Console.ReadLine().Trim();
        string t = Console.ReadLine().Trim();

        long memStart = GC.GetTotalMemory(false);

        long count = 0;

        // перебираем a = |x| и b = |y|; c = n - a - b
        for (int a = 0; a <= n; a++)
        {
            if (!SamePrefix(s, t, a)) break;    // если префикс не совпал — дальше a не пойдут
            for (int b = 0; b <= n - a; b++)
            {
                int c = n - a - b;

                if (!SameSuffix(s, n, t, m, c)) continue;

                // проверяем, что t[a..m-c-1] = y^k
                int tLen = m - a - c;
                if (tLen < 0) continue;
                if (b == 0)
                {
                    if (tLen == 0) count++;
                }
                else
                {
                    if (tLen % b != 0) continue;
                    if (IsPower(t, a, m - c, s, a, b)) count++;
                }
            }
        }

        sw.Stop();
        long memEnd = GC.GetTotalMemory(false);

        Console.WriteLine(count);
        Console.Error.WriteLine($"TIME_MS={sw.Elapsed.TotalMilliseconds:F2}");
        Console.Error.WriteLine($"MEM_MB={(memEnd - memStart) / 1024.0 / 1024.0:F2}");
    }
}