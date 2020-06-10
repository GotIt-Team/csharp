using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GotIt.BLL.Managers;
using GotIt.BLL.ViewModels;
using GotIt.Common.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GotIt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        private readonly SystemManager _feedbackManager;
        private readonly RequestAttributes _requestAttributes;

        public SystemController(RequestAttributes requestAttributes, SystemManager feedbackManager)
        {
            _requestAttributes = requestAttributes;
            _feedbackManager = feedbackManager;
        }

        [HttpPost]
        [Route("feedback")]
        public Result<bool> Feedback(FeedbackViewModel feedbackViewModel)
        {
            return _feedbackManager.AddFeedback(_requestAttributes.Id, feedbackViewModel);
        }
        [HttpPost]
        [Route("contact-us")]
        public Result<object> ContactUs()
        {
            throw new NotImplementedException();
        }
    }
}
