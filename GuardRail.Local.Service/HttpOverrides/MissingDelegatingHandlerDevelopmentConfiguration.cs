using System;
using System.Net.Http;

namespace GuardRail.Local.Service.HttpOverrides;

public sealed class MissingDelegatingHandlerDevelopmentConfiguration(
    string absoluteUri,
    HttpMethod requestMethod)
    : Exception(
        $"The development delegating handler configuration is missing for {requestMethod} {absoluteUri}.");