using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using log4net;
using MaterialCodeSelectionPlatform.Domain;
using MaterialCodeSelectionPlatform.Service;
using MaterialCodeSelectionPlatform.Utilities;
using MaterialCodeSelectionPlatform.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MaterialCodeSelectionPlatform.ManagerWeb
{
    public class BaseController<S,T> : Controller where T:IDataEntity,new()
    where S:IEntityService<T>
    {
        private ILog log = LogHelper.GetLogger<Object>();

        protected S Service { get; set; }

        protected T Entity { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme).Result.Succeeded)
            {
                context.HttpContext.Response.Redirect("/Login/Index");
            }
            base.OnActionExecuting(context);
        }
        /// <summary>
        /// 应用程序跟目录
        /// </summary>
        protected static string appBasePath = AppDomain.CurrentDomain.BaseDirectory;
        
        /// <summary>
        /// 转换为layui table所需要的数据格式
        /// </summary>
        /// <param name="result"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        protected IActionResult ConvertListResult(object result, DataPage page, IsoDateTimeConverter timeFormat=null)
        {
            ListResultModel resultModel = new ListResultModel();
            resultModel.count = page.RecordCount;
            resultModel.data = result;
            if(timeFormat==null)
            {
                timeFormat = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            }
           
            return Content(JsonConvert.SerializeObject(resultModel, timeFormat));
        }

        /// <summary>
        /// 转换正确的结果格式
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected IActionResult ConvertSuccessResult(object result)
        {
            DataResult dataResult = new DataResult();
            dataResult.Data = result;
            dataResult.Success = true;

            return Content(JsonConvert.SerializeObject(dataResult));
        }

        /// <summary>
        /// 转换正确的结果格式
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected IActionResult ConvertSuccessResult(object result, string message)
        {
            DataResult dataResult = new DataResult();
            dataResult.Data = result;
            dataResult.Success = true;
            dataResult.Message = message;
            return Content(JsonConvert.SerializeObject(dataResult));
        }

        /// <summary>
        /// 转换为错误的结果格式
        /// </summary>
        /// <param name="result"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        protected IActionResult ConvertErrorResult(object result, Exception ex)
        {
            DataResult dataResult = new DataResult();
            dataResult.Exception = ex;
            dataResult.Success = false;
            return Content(JsonConvert.SerializeObject(dataResult));
        }


        /// <summary>
        /// 转换为错误的结果格式
        /// </summary>
        /// <param name="result"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected JsonResult ConvertFailResult(object result, string msg)
        {
            DataResult dataResult = new DataResult();
            dataResult.Message = msg;
            dataResult.Success = false;
            return Json(dataResult);
        }

       
        protected string newId
        {
            get { return Guid.NewGuid().ToString(); }
        }

        /// <summary>
        /// 语言类型
        /// </summary>
        protected int LanguageType
        {
            get
            {
                return HttpContext.Session.GetString("LanguageType") == null
                    ? 0
                    : int.Parse(HttpContext.Session.GetString("LanguageType"));
            }
        }


        protected string UserId
        {
            get { return HttpContext.Session.GetString("UserId")??Guid.NewGuid().ToString(); }
        }

        protected string UserName
        {
            get { return HttpContext.Session.GetString("UserName") ?? string.Empty; ; }
        }

        protected string UserTrueName
        {
            get { return HttpContext.Session.GetString("LoginName") ?? string.Empty; ; }
        }

        /// <summary>
        /// 组装搜索条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <returns></returns>
        protected T SetSCondition<T>(DataPage page) where T : SConditionBase, new()
        {
            T t = new T();
            t.Page = page;
            return t;
        }

        /// <summary>
        /// 保存文件，放回文件路径
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        protected string SaveFile(IFormFile file)
        {
            var folderPath = System.IO.Path.Combine(appBasePath, "Uploader", DateTime.Now.ToString("yyyyMMdd"));
            if (!System.IO.Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileName = file.FileName;
            var ext = Path.GetExtension(fileName);

            var newFileName = fileName.Replace(ext, DateTime.Now.ToString("yyyyMMddHHmmss") + ext);

            var fullPath = Path.Combine(folderPath , newFileName);
            using (FileStream fileStream = System.IO.File.Create(fullPath))
            {
                file.CopyTo(fileStream);
                fileStream.Flush();
                fileStream.Close();
            }
            return fullPath;
        }

        /// <summary>
        /// 文件流的方式下载
        /// </summary>
        /// <returns></returns>
        public IActionResult DownLoad(string file)
        {
            var addrUrl = file;
            var stream = System.IO.File.OpenRead(addrUrl);
            string fileExt = Path.GetExtension(file);
            //获取文件的ContentType
            var provider = new FileExtensionContentTypeProvider();
            var memi = provider.Mappings[fileExt];
            return File(stream, memi, Path.GetFileName(addrUrl));
        }

        /// <summary>
        /// 新增或修改数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<IActionResult> SaveOrUpdate(T entity)
        {
            //新增
            if (entity.Id.IsNullOrEmpty())
            {
                entity.Id = newId;
                entity.CreateTime =DateTime.Now;
                entity.CreateUserId = UserId;
                entity.LastModifyTime = DateTime.Now;
                entity.LastModifyUserId = UserId;
                await Service.SaveAsync(entity);
            }
            else//修改
            {
                var model = await Service.GetAsync(entity.Id);
                if (model == null)
                {
                    return ConvertFailResult(null, "数据已经被删除");
                }

                entity.CreateTime = model.CreateTime;
                entity.CreateUserId = model.CreateUserId;
                entity.LastModifyUserId = UserId;
                entity.LastModifyTime =DateTime.Now;

                await Service.UpdateAsync(entity);

            }
            return ConvertSuccessResult("success");
        }

        /// <summary>
        /// 通用根据Id获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<IActionResult> GetById(string id)
        {
            var model = await Service.GetAsync(id);
            return ConvertSuccessResult(model);
        }

        /// <summary>
        /// 根据Id删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<IActionResult> DeleteById(string id)
        {
            var result = await Service.DeleteByIdAsync(id);
            return ConvertSuccessResult(result);
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<IActionResult> SaveStatus(string id, int status)
        {
            Entity = await Service.GetAsync(id);
            Entity.Status = status;
            var result = await Service.UpdateAsync(Entity);
            return ConvertSuccessResult(result);
        }



    }
}
