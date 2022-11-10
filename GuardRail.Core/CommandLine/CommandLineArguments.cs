using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GuardRail.Core.CommandLine;

/// <summary>
/// Command line arguments.
/// </summary>
public sealed class CommandLineArguments : IList<CommandLineArgument>
{
    private readonly IList<CommandLineArgument> _backingDataStore;

    private CommandLineArguments(IEnumerable<string> args) =>
        _backingDataStore = args.Select(CommandLineArgument.Parse).ToList();

    /// <summary>
    /// Creates a CommandLineArguments from the strings passed in.
    /// </summary>
    /// <param name="args">The arguments to parse.</param>
    public static CommandLineArguments Create(IEnumerable<string> args) =>
        new(args);

    /// <summary>
    /// Checks to see if the CommandLineArgumentType was defined.
    /// </summary>
    /// <param name="commandLineArgumentType">The type to check for.</param>
    /// <returns>bool</returns>
    public bool ContainsKey(CommandLineArgumentType commandLineArgumentType) =>
        _backingDataStore.Any(x => x.Type == commandLineArgumentType);

    /// <summary>
    /// Tries to get the CommandLineArgumentType for the type passed in.
    /// Returns null if no value is found.
    /// </summary>
    /// <param name="commandLineArgumentType">The type to check for.</param>
    /// <returns>CommandLineArgument</returns>
    public CommandLineArgument TryGetByKey(CommandLineArgumentType commandLineArgumentType) =>
        _backingDataStore.FirstOrDefault(x => x.Type == commandLineArgumentType);

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