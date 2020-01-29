using System.Collections.Generic;
using System.Threading.Tasks;
using MaterialCodeSelectionPlatform.Domain;
using MaterialCodeSelectionPlatform.Domain.Entities;

namespace MaterialCodeSelectionPlatform.Service
{
    public partial class UserServiceImpl
    {
        /// <summary>
        /// 用户查询列表
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="role"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<List<User>> GetDataList(UserSearchCondition searchCondition)
        {
            return await _UserDao.GetDataList(searchCondition);
        }


        /// <summary>
        /// 验证页面元素
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public async Task<bool> VerifLoginNameAsync(string loginName, string id)
        {
            return await _UserDao.VerifLoginNameAsync(loginName,id);
        }

        /// <summary>
        /// 保存用户分配的项目
        /// </summary>
        /// <param name="projects"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> SaveUserProjects(List<string> projects, string id, string userId)
        {
            return await _UserDao.SaveUserProjects(projects, id,userId);
        }



        /// <summary>
        /// 获取待分配的项目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<Project>> GetLeftProjects(string id)
        {
            return await _UserDao.GetLeftProjects( id);
        }


        /// <summary>
        /// 获取已分配的项目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<Project>> GetRightProjects(string id)
        {
            return await _UserDao.GetRightProjects(id);
        }

        /// <summary>
        /// 根据用户名密码获取
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public async Task<User> GetByUserNamePwd(string userName, string pwd)
        {
            return await _UserDao.GetByUserNamePwd(userName,pwd);
        }
    }
}