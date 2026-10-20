using System;
using System.Diagnostics;

class Program
{
    // Поразрядная сортировка (LSD, radix sort по 8 бит) — своя реализация.
    // Принцип: проходим по числу 4 раза, каждый раз раскладываем элементы
    // по 256 корзинам согласно очередному байту, затем собираем обратно;
    // после 4 проходов массив отсортирован.
    static void RadixSort(uint[] a)
    {
        int n = a.Length;
        uint[] buffer = new uint[n];        // буфер для пересыпания
        int[] count = new int[256];         // счётчики по байту

        for (int shift = 0; shift < 32; shift += 8)
        {
            // обнуляем счётчики
            for (int i = 0; i < 256; i++) count[i] = 0;

            // считаем, сколько чисел имеют каждый байт
            for (int i = 0; i < n; i++)
                count[(a[i] >> shift) & 0xFF]++;

            // префиксные суммы — позиции начала корзин
            int sum = 0;
            for (int i = 0; i < 256; i++)
            {
                int c = count[i];
                count[i] = sum;
                sum += c;
            }

            // раскладываем по корзинам
            for (int i = 0; i < n; i++)
            {
                int b = (int)((a[i] >> shift) & 0xFF);
                buffer[count[b]++] = a[i];
            }

            // возвращаем обратно
            uint[] tmp = a;
            a = buffer;
            buffer = tmp;
        }

        // после чётного числа проходов (4) данные могут оказаться в buffer —
        // копируем обратно в исходный массив
        if (ReferenceEquals(a, buffer) == false)
        {
            // a и buffer поменялись местами чётное число раз — a указывает
            // на исходный массив, данные в нём; ничего делать не нужно.
            // На случай нечётного числа проходов (здесь 4 — чётное) —
            // этот блок для надёжности.
        }

        // На случай, если данные остались в buffer (нечётное число проходов) —
        // копируем. В нашем случае 4 прохода — чётное, поэтому данные в a.
        // Но чтобы не зависеть от чётности, явно проверим:
        // если buffer хранит результат — перекопируем.
    }

    static void Main()
    {
        // --- замер: старт ---
        Stopwatch sw = Stopwatch.StartNew();

        string[] parts = Console.ReadLine()
                                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        int N = int.Parse(parts[0]);
        uint K = uint.Parse(parts[1]);
        uint M = uint.Parse(parts[2]);
        uint L = uint.Parse(parts[3]);

        long memStart = GC.GetTotalMemory(false);

        // ---------- Генерация последовательности ----------
        uint[] a = new uint[N];
        a[0] = K;
        const ulong MOD32 = 0xFFFFFFFFUL;   // 2^32 - 1
        for (int i = 0; i < N - 1; i++)
        {
            ulong x = ((ulong)a[i] * M) % MOD32;
            a[i + 1] = (uint)(x % L);
        }

        // ---------- Сортировка ----------
        RadixSort(a);

        // ---------- Сумма по нечётным позициям ----------
        // 1-й, 3-й, 5-й … → индексы 0, 2, 4, …
        ulong sum = 0;
        for (int i = 0; i < N; i += 2)
            sum += a[i];
        ulong answer = sum % L;

        // --- замер: финиш ---
        sw.Stop();
        long memEnd = GC.GetTotalMemory(false);

        Console.WriteLine(answer);

        Console.Error.WriteLine($"TIME_MS={sw.Elapsed.TotalMilliseconds:F2}");
        Console.Error.WriteLine($"MEM_MB={(memEnd - memStart) / 1024.0 / 1024.0:F2}");
    }
}