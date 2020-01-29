using System.Collections.Generic;
using System.Threading.Tasks;
using MaterialCodeSelectionPlatform.Domain;
using MaterialCodeSelectionPlatform.Domain.Entities;

namespace MaterialCodeSelectionPlatform.Service
{
    public partial interface IUserService
    {
        /// <summary>
        /// 用户查询
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="role"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        Task<List<User>> GetDataList(UserSearchCondition searchCondition);


        /// <summary>
        /// 验证页面元素
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        Task<bool> VerifLoginNameAsync(string loginName, string id);


        /// <summary>
        /// 保存用户分配的项目
        /// </summary>
        /// <param name="projects"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> SaveUserProjects(List<string> projects, string id, string userId);



        /// <summary>
        /// 获取待分配的项目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<Project>> GetLeftProjects(string id);


        /// <summary>
        /// 获取已分配的项目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<Project>> GetRightProjects(string id);

        /// <summary>
        /// 根据用户名密码获取
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        Task<User> GetByUserNamePwd(string userName, string pwd);
    }
}