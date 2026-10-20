using System;
using System.Diagnostics;
using System.Text;

// Своя структура для участника.
// Никаких встроенных контейнеров (List<T>, SortedSet и т.п.) не используем.
struct Participant
{
    public string Login;
    public long P;
    public long F;
}

class Program
{
    // Сравнение «кто выше» — своя реализация.
    // Принцип: больше задач — выше; при равенстве — меньше штраф;
    // при равенстве обоих — логин лексикографически меньше.
    static bool Higher(Participant a, Participant b)
    {
        if (a.P != b.P) return a.P > b.P;
        if (a.F != b.F) return a.F < b.F;
        return string.CompareOrdinal(a.Login, b.Login) < 0;
    }

    // In-place quicksort по схеме Хоара — своя реализация.
    // Принцип: выбираем опорный (средний) элемент, двумя указателями
    // с концов сдвигаем меньшие влево, большие вправо, меняем местами
    // нарушающие пары; затем рекурсивно сортируем обе части.
    // Дополнительной памяти O(n) не используется — только стек рекурсии.
    static void QuickSort(Participant[] a, int left, int right)
    {
        int i = left, j = right;
        Participant pivot = a[(left + right) / 2];

        while (i <= j)
        {
            // двигаем i вправо, пока a[i] должен идти выше pivot
            while (Higher(a[i], pivot)) i++;
            // двигаем j влево, пока a[j] должен идти ниже pivot
            while (Higher(pivot, a[j])) j--;

            if (i <= j)
            {
                Participant tmp = a[i];
                a[i] = a[j];
                a[j] = tmp;
                i++;
                j--;
            }
        }

        if (left < j)  QuickSort(a, left, j);
        if (i < right) QuickSort(a, i, right);
    }

    static void Main()
    {
        // --- замер: старт ---
        Stopwatch sw = Stopwatch.StartNew();

        int n = int.Parse(Console.ReadLine().Trim());
        Participant[] a = new Participant[n];

        for (int i = 0; i < n; i++)
        {
            string[] parts = Console.ReadLine()
                                    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            a[i].Login = parts[0];
            a[i].P = long.Parse(parts[1]);
            a[i].F = long.Parse(parts[2]);
        }

        long memStart = GC.GetTotalMemory(false);

        QuickSort(a, 0, n - 1);

        sw.Stop();
        long memEnd = GC.GetTotalMemory(false);

        // Вывод через StringBuilder — быстрее, чем n вызовов WriteLine
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < n; i++)
        {
            sb.Append(a[i].Login).Append('\n');
        }
        Console.Write(sb.ToString());

        Console.Error.WriteLine($"TIME_MS={sw.Elapsed.TotalMilliseconds:F2}");
        Console.Error.WriteLine($"MEM_MB={(memEnd - memStart) / 1024.0 / 1024.0:F2}");
    }
}