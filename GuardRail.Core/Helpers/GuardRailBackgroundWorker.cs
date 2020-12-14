using System;
using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.Core.Helpers
{
    /// <summary>
    /// A background worker for GuardRail.
    /// </summary>
    public sealed class GuardRailBackgroundWorker : IDisposable
    {
        private readonly string _name;
        private readonly TimeSpan _delay;
        private readonly Func<CancellationToken, Task> _work;
        private readonly CancellationToken _cancellationToken;
        private Task _worker;

        /// <summary>
        /// The name of the worker.
        /// </summary>
        public string Name => _name;
        
        private GuardRailBackgroundWorker(
            string name,
            TimeSpan delay,
            Func<CancellationToken, Task> work,
            CancellationToken cancellationToken)
        {
            _name = name;
            _delay = delay;
            _work = work;
            _cancellationToken = cancellationToken;
        }

        /// <summary>
        /// Creates a worker task.
        /// </summary>
        /// <param name="name">The name of the timer.</param>
        /// <param name="delay">How long between executions to wait.</param>
        /// <param name="work">The work to be done.</param>
        /// <param name="cancellationToken">A CancellationToken.</param>
        /// <returns></returns>
        public static GuardRailBackgroundWorker Create(
            string name,
            TimeSpan delay,
            Func<CancellationToken, Task> work,
            CancellationToken cancellationToken) =>
            new GuardRailBackgroundWorker(
                name,
                delay,
                work,
                cancellationToken);

        /// <summary>
        /// Start the worker.
        /// </summary>
        public void Start()
        {
            _worker = new TaskFactory().StartNew(
                async () =>
                {
                    while (!_cancellationToken.IsCancellationRequested)
                    {
                        await _work(_cancellationToken);
                        await Task.Delay(_delay, _cancellationToken);
                    }
                },
                _cancellationToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        /// <summary>
        /// Stops the worker.
        /// </summary>
        public void Stop() =>
            Dispose();

        /// <inheritdoc />
        public void Dispose() =>
            _worker.Dispose();
    }
}