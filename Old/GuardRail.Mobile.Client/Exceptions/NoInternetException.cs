using System;

namespace GuardRail.Mobile.Client.Exceptions;

public sealed class NoInternetException()
    : Exception("Check your network connection and try again.");