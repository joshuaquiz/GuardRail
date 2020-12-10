using GuardRail.Core;
using GuardRail.Hq.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GuardRail.Hq.Api.Controllers
{
    [Route("api/[controller]")]
    public class InstallController : ControllerBase
    {
        private readonly IRemoteFileStorage _remoteFileStorage;

        public InstallController(IRemoteFileStorage remoteFileStorage)
        {
            _remoteFileStorage = remoteFileStorage;
        }

        /// <summary>
        /// GET api/install/latest
        /// Returns the latest version payload.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public InstallConfiguration GetLatestVersion()
        {
            _remoteFileStorage.GetFileAsync(
                )
            return
        }
    }
}