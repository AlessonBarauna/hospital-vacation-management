namespace HospitalVacationManagement.Application.System;

public sealed record VersionResponse(
    string Application,
    string Environment,
    string Version,
    DateTime ServerTimeUtc);