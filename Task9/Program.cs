using System;
using System.Diagnostics;

class Program
{
    // Прямой перебор шага — своя реализация.
    // Принцип: для каждого возможного шага d между соседними оставленными
    // деревьями считаем, сколько вариантов поставить первое дерево так,
    // чтобы последнее не вышло за границу; суммируем по всем d.
    static void Main()
    {
        // --- замер: старт ---
        Stopwatch sw = Stopwatch.StartNew();

        string[] parts = Console.ReadLine()
                                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        int n = int.Parse(parts[0]);
        int m = int.Parse(parts[1]);

        // фиксируем память после ввода
        long memStart = GC.GetTotalMemory(false);

        long answer;

        if (m == 0)
        {
            // вырубить всё — ровно один способ
            answer = 1;
        }
        else if (m == 1)
        {
            // одно дерево — любое из n
            answer = n;
        }
        else
        {
            // m >= 2: перебираем шаг d между соседними оставленными
            int maxD = (n - 1) / (m - 1);
            long sum = 0;
            for (int d = 1; d <= maxD; d++)
            {
                // первое дерево можно поставить в (n - (m-1)*d) позиций
                sum += n - (m - 1) * d;
            }
            answer = sum;
        }

        // --- замер: финиш ---
        sw.Stop();
        long memEnd = GC.GetTotalMemory(false);

        Console.WriteLine(answer);

        // диагностика в stderr — не попадает в стандартный вывод
        Console.Error.WriteLine($"TIME_MS={sw.Elapsed.TotalMilliseconds:F2}");
        Console.Error.WriteLine($"MEM_MB={(memEnd - memStart) / 1024.0 / 1024.0:F2}");
    }
}