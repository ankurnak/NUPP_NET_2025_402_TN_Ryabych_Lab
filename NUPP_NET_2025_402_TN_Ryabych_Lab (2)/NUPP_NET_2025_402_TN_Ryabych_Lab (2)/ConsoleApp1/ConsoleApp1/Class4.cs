using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    // Коментар: Клас для демонстрації примітивів синхронізації
    public class SynchronizationExamples
    {
        // 1. Примітив Lock: Об'єкт для блокування
        private readonly object _lockObject = new object();
        private int _lockCounter = 0; // Спільний ресурс, захищений lock

        // 2. Примітив SemaphoreSlim: Обмеження кількості потоків (наприклад, 2 одночасно)
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(2, 2);

        // 3. Примітив AutoResetEvent: Сигналізація між потоками
        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

        // --- Демонстрація Lock ---
        public void DemonstrateLock(int threadId)
        {
            Console.WriteLine($"[Lock Demo] Потік {threadId}: Намагається увійти в критичну секцію...");

            // Коментар: Використання lock для атомарності операції
            lock (_lockObject)
            {
                // Цей блок може виконуватися лише одним потоком одночасно
                int temp = _lockCounter;
                Thread.Sleep(50); // Імітація роботи
                _lockCounter = temp + 1;
                Console.WriteLine($"[Lock Demo] Потік {threadId}: Збільшив лічильник до {_lockCounter}");
            } // Блокування знімається
        }

        // --- Демонстрація Semaphore ---
        public async Task DemonstrateSemaphoreAsync(int threadId)
        {
            Console.WriteLine($"[Semaphore Demo] Потік {threadId}: Очікує на семафор...");

            // Коментар: Очікування на звільнення ресурсу (дозволено лише 2)
            await _semaphore.WaitAsync();

            try
            {
                // Цей блок виконується лише двома потоками одночасно
                Console.WriteLine($"[Semaphore Demo] Потік {threadId}: Увійшов. Виконує роботу (300 мс)...");
                await Task.Delay(300);
            }
            finally
            {
                // Коментар: Звільнення ресурсу (обов'язково в finally)
                _semaphore.Release();
                Console.WriteLine($"[Semaphore Demo] Потік {threadId}: Звільнив семафор.");
            }
        }

        // --- Демонстрація AutoResetEvent ---

        // Потік-очікувач
        public void WaiterThread()
        {
            Console.WriteLine("\n[AutoResetEvent Demo] Waiter: Очікую на сигнал...");
            // Коментар: Блокування до отримання сигналу
            _autoResetEvent.WaitOne();
            Console.WriteLine("[AutoResetEvent Demo] Waiter: Отримав сигнал! Продовжую роботу.");
        }

        // Потік-сигналізатор
        public void SignalerThread()
        {
            Console.WriteLine("[AutoResetEvent Demo] Signaler: Робить підготовчу роботу...");
            Thread.Sleep(1000); // Імітація роботи
            // Коментар: Надсилання сигналу (автоматично скидається після одного WaitOne)
            _autoResetEvent.Set();
            Console.WriteLine("[AutoResetEvent Demo] Signaler: Надіслав сигнал.");
        }
    }
}