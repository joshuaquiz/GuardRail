using System.Collections.Generic;

namespace GuardRail.Hardware.GuardRailCustom;

public sealed class NetworkHardwareCache
{
    private readonly Dictionary<string, CustomHardwareSettings> _cache = new Dictionary<string, CustomHardwareSettings>();

    public void Add(
        string key,
        CustomHardwareSettings settings) =>
        _cache[key] = settings;

    public CustomHardwareSettings Get(
        string key) =>
        _cache[key];

    public IReadOnlyCollection<CustomHardwareSettings> GetAll() =>
        _cache.Values;

    public void Remove(
        string key) =>
        _cache.Remove(key);
}