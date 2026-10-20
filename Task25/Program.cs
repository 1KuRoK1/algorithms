using System;
using System.Diagnostics;

class Program
{
    // Проверка свойства кучи одним проходом — своя реализация.
    // Принцип: для каждого родителя (индексы 0..n/2-1) проверяем, что он
    // не больше своих детей (2i+1 и 2i+2); если хоть раз нарушено — NO.
    static void Main()
    {
        // --- замер: старт ---
        Stopwatch sw = Stopwatch.StartNew();

        int n = int.Parse(Console.ReadLine().Trim());
        int[] a = new int[n];
        string[] parts = Console.ReadLine()
                                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < n; i++)
            a[i] = int.Parse(parts[i]);

        long memStart = GC.GetTotalMemory(false);

        bool isHeap = true;
        // родители — с 0 по n/2 - 1 (у остальных нет детей)
        for (int i = 0; i < n / 2; i++)
        {
            int left  = 2 * i + 1;
            int right = 2 * i + 2;

            if (left < n && a[i] > a[left])  { isHeap = false; break; }
            if (right < n && a[i] > a[right]) { isHeap = false; break; }
        }

        sw.Stop();
        long memEnd = GC.GetTotalMemory(false);

        Console.WriteLine(isHeap ? "YES" : "NO");

        Console.Error.WriteLine($"TIME_MS={sw.Elapsed.TotalMilliseconds:F2}");
        Console.Error.WriteLine($"MEM_MB={(memEnd - memStart) / 1024.0 / 1024.0:F2}");
    }
}