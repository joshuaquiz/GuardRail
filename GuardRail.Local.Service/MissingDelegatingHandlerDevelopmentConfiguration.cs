using System;
using System.Net.Http;

namespace GuardRail.Local.Service;

public sealed class MissingDelegatingHandlerDevelopmentConfiguration(
    string absoluteUri,
    HttpMethod requestMethod)
    : Exception(
        $"The development delegating handler configuration is missing for {requestMethod} {absoluteUri}.");