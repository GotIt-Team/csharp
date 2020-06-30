using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GotIt.BLL.Managers;
using GotIt.BLL.ViewModels;
using GotIt.Common.Enums;
using GotIt.Common.Helper;
using GotIt.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GotIt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        private readonly SystemManager _systemManager;
        private readonly RequestAttributes _requestAttributes;

        public SystemController(RequestAttributes requestAttributes, SystemManager systemManager)
        {
            _requestAttributes = requestAttributes;
            _systemManager = systemManager;
        }

        [HttpPost]
        [Route("feedback")]
        [Authrization(EUserType.regular)]
        public Result<bool> Feedback([FromBody] FeedbackViewModel feedbackViewModel)
        {
            return _systemManager.AddFeedback(_requestAttributes.Id, feedbackViewModel);
        }

        [HttpPost]
        [Route("contact-us")]
        [Authrization(EUserType.regular, EUserType.organization)]
        public async Task<Result<bool>> ContactUs([FromBody] ContactUsViewModel message)
        {
            return await _systemManager.ContactUs(_requestAttributes.Id, message);
        }

        [HttpPost]
        [Route("upload-image")]
        [Authrization(EUserType.regular, EUserType.organization)]
        public Result<List<string>> UploadImage()
        {
            var files = HttpContext.Request.Form.Files;
            return _systemManager.UploadImages(_requestAttributes.AppBaseUrl, files);
        }
    }
}
