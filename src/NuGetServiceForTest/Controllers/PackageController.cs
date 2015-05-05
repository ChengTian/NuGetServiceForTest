using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using System.IO;
using Microsoft.Framework.Runtime;

namespace NuGetServiceForTest.Controllers
{
    public class PackageController : Controller
    {
        private readonly string _appBasePath;
        private readonly string _nugetV2ApiService = "https://nuget.org/api/v2";
        private readonly bool _hijacking = true;

        public PackageController(IApplicationEnvironment appEnv)
        {
            _appBasePath = appEnv.ApplicationBasePath;
        }

        [HttpGet]
        [Route("api/v2/FindPackagesById()")]
        public IActionResult FindPackagesById(string id)
        {
            id = id.Trim(new[] { '\'' });

            if (_hijacking)
            {
                //return ServerErrorWithWrongStatusCode();
                var nugetODataTempldatePath = Path.Combine(_appBasePath, "Metadata", "NuGetODataTemplate.xml");
                var xmlString = System.IO.File.ReadAllText(nugetODataTempldatePath)
                    .Replace("SERVICE_HOST_PLACEHOLDER", Request.Host.ToString())
                    .Replace("PACKAGE_VERSION_PLACEHOLDER", "9.9.9")
                    .Replace("PACKAGE_ID_PLACEHOLDER", id);
                return Content(xmlString, "text/xml");
            }
            else
            {
                return Redirect($"{_nugetV2ApiService}/FindPackagesById()?Id='{id}'");
            }

        }

        [HttpGet]
        [Route("api/v2/package/{id}/{version}")]
        public IActionResult GetPackageByIdAndVersion(string id, string version)
        {
            if (_hijacking)
            {
                return ServerErrorWithWrongStatusCode();
            }
            else
            {
                return Redirect($"{_nugetV2ApiService}/package/{id}/{version}");
            }
        }

        private IActionResult ServerErrorWithWrongStatusCode()
        {
            Response.StatusCode = 200;
            var pagePath = Path.Combine(_appBasePath, "Metadata", "ServerError.html");
            return new FilePathResult(pagePath);
        }
    }
}
