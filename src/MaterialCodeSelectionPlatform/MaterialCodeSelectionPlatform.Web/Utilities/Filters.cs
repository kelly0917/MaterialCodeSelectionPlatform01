using System;
using log4net;
using MaterialCodeSelectionPlatform.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace MaterialCodeSelectionPlatform.Web
{

    /// <summary>
    /// 异常过滤器
    /// </summary>
    public class ExceptionFilter : IExceptionFilter
    {
        private ILog logger = LogHelper.GetLogger<ExceptionFilter>();
        public void OnException(ExceptionContext context)
        {
            logger.Error(
                "过滤器拦截异常" + ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor)
                .DisplayName, context.Exception);


            //context.Result = new ContentResult("<script type='text/javascript'>top.location.href='/User/Index';</script>");

            DataResult dataResult = new DataResult();
            dataResult.Data = null;
            //dataResult.Code = 99;
            //dataResult.Msg = context.Exception.Message;
            context.Result = new ContentResult()
            {
                Content = JsonConvert.SerializeObject(dataResult)
            };
        }



    }
}