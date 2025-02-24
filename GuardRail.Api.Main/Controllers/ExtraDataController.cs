using GuardRail.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace GuardRail.Api.Main.Controllers;

[ApiController]
[Route("[controller]")]
public class ExtraDataController(
    ILogger<AccessPointController> logger)
    : GhControllerBase<AccessPointController>(
        logger);