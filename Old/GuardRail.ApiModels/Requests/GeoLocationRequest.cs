using System;

namespace GuardRail.ApiModels.Requests;

public sealed record GeoLocationRequest(
    Guid DoorId,
    double Latitude,
    double Longitude);