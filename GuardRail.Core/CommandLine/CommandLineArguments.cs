using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GuardRail.Core.CommandLine
{
    /// <summary>
    /// Defined command line arguments.
    /// </summary>
    public sealed class CommandLineArguments : IList<CommandLineArgument>
    {
        private readonly IList<CommandLineArgument> _backingDataStore;

        public CommandLineArguments()
            : this(Enumerable.Empty<string>())
        {

        }

        public CommandLineArguments(IEnumerable<string> args)
        {
            _backingDataStore = args.Select(CommandLineArgument.Parse).ToList();
        }

        /// <inheritdoc />
        public IEnumerator<CommandLineArgument> GetEnumerator() =>
            _backingDataStore.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            _backingDataStore.GetEnumerator();

        /// <inheritdoc />
        public void Add(CommandLineArgument item) =>
            _backingDataStore.Add(item);

        /// <inheritdoc />
        public void Clear() =>
            _backingDataStore.Clear();

        /// <inheritdoc />
        public bool Contains(CommandLineArgument item) =>
            _backingDataStore.Contains(item);

        /// <inheritdoc />
        public void CopyTo(CommandLineArgument[] array, int arrayIndex) =>
            _backingDataStore.CopyTo(array, arrayIndex);

        /// <inheritdoc />
        public bool Remove(CommandLineArgument item) =>
            _backingDataStore.Remove(item);

        /// <inheritdoc />
        public int Count => _backingDataStore.Count;

        /// <inheritdoc />
        public bool IsReadOnly => _backingDataStore.IsReadOnly;

        /// <inheritdoc />
        public int IndexOf(CommandLineArgument item) =>
            _backingDataStore.IndexOf(item);

        /// <inheritdoc />
        public void Insert(int index, CommandLineArgument item) =>
            _backingDataStore.Insert(index, item);

        /// <inheritdoc />
        public void RemoveAt(int index) =>
            _backingDataStore.RemoveAt(index);

        /// <inheritdoc />
        public CommandLineArgument this[int index]
        {
            get => _backingDataStore[index];
            set => _backingDataStore[index] = value;
        }

        /// <inheritdoc />
        public override string ToString() =>
            string.Join(" ", _backingDataStore);
    }
}