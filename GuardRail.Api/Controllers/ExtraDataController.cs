using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GuardRail.Api.Controllers;

[ApiController]
[UserAccessTokenAuthorization]
[Route("[controller]")]
public sealed class ExtraDataController(
    ILogger<AccessPointController> logger)
    : GhControllerBase<AccessPointController>(
        logger);