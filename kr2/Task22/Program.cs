using System;
using System.Diagnostics;
using System.Text;      // только для чтения строк; StringBuilder для накопления НЕ используем

class Program
{
    // Распаковка запакованной строки — своя рекурсивная реализация.
    // Принцип: идём по строке; букву добавляем как есть; при цифре d читаем
    // '[', рекурсивно распаковываем содержимое до ']', затем повторяем
    // результат d раз; на верхнем уровне endChar = '\0'.
    static string Unpack(string s, ref int pos, char endChar)
    {
        StringBuilder result = new StringBuilder();   // временно для аккумуляции
        while (pos < s.Length && s[pos] != endChar)
        {
            char c = s[pos];
            if (c >= '1' && c <= '9')
            {
                int count = c - '0';
                pos++;                   // пропускаем цифру
                // ожидаем '['
                if (pos < s.Length && s[pos] == '[') pos++;
                string inner = Unpack(s, ref pos, ']');
                if (pos < s.Length && s[pos] == ']') pos++;   // пропускаем ']'
                for (int k = 0; k < count; k++) result.Append(inner);
            }
            else
            {
                result.Append(c);
                pos++;
            }
        }
        return result.ToString();
    }

    static void Main()
    {
        Stopwatch swTotal = Stopwatch.StartNew();

        int n = int.Parse(Console.ReadLine().Trim());
        string[] packed = new string[n];
        for (int i = 0; i < n; i++)
            packed[i] = Console.ReadLine().Trim();

        long memStart = GC.GetTotalMemory(false);
        Stopwatch swAlgo = Stopwatch.StartNew();

        // распаковка первой строки — она станет текущим общим префиксом
        int p0 = 0;
        string common = Unpack(packed[0], ref p0, '\0');
        int prefixLen = common.Length;

        // для каждой следующей — распаковываем и усекаем common до общего префикса
        for (int i = 1; i < n && prefixLen > 0; i++)
        {
            int p = 0;
            string cur = Unpack(packed[i], ref p, '\0');

            int limit = Math.Min(prefixLen, cur.Length);
            int k = 0;
            while (k < limit && common[k] == cur[k]) k++;
            prefixLen = k;
        }

        swAlgo.Stop();
        long memEnd = GC.GetTotalMemory(false);
        swTotal.Stop();

        // вывод общего префикса
        for (int i = 0; i < prefixLen; i++) Console.Write(common[i]);
        Console.WriteLine();

        Console.Error.WriteLine($"TIME_TOTAL_MS={swTotal.Elapsed.TotalMilliseconds:F2}");
        Console.Error.WriteLine($"TIME_ALGO_MS={swAlgo.Elapsed.TotalMilliseconds:F2}");
        Console.Error.WriteLine($"MEM_MB={(memEnd - memStart) / 1024.0 / 1024.0:F2}");
    }
}