using System;
using System.Collections.Generic;
using System.Linq;

namespace GuardRail.Hardware.GuardRailCustom;

public sealed class NetworkHardwareCache : IDisposable
{
    private readonly Dictionary<string, CustomHardwareSettings> _cache = new();

    public void AddOrUpdate(
        string key,
        CustomHardwareSettings settings)
    {
        if (_cache.TryGetValue(
                key,
                out var item))
        {
            // TODO: check to see if the items are different. If not then we can return.
            item.Dispose();
            _cache.Remove(key);
        }

        _cache[key] = settings;
    }

    public CustomHardwareSettings? Get(
        string key) =>
        _cache.TryGetValue(
            key,
            out var item)
            ? item
            : _cache.FirstOrDefault().Value;//null;

    public IReadOnlyCollection<CustomHardwareSettings> GetAll() =>
        _cache.Values;

    public void Remove(
        string key) =>
        _cache.Remove(key);

    public void Dispose()
    {
        foreach (var item in _cache)
        {
            item.Value.Dispose();
        }
    }
}