using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using MaterialCodeSelectionPlatform.Domain;
using MaterialCodeSelectionPlatform.Domain.Entities;
using SqlSugar;

namespace MaterialCodeSelectionPlatform.Data
{
    public partial class UserDaoImpl
    {
   
        /// <summary>
        /// 用户查询
        /// </summary>
        /// <returns></returns>
        public async Task<List<User>> GetDataList(UserSearchCondition searchCondition)
        {
            var query = Db.Queryable<User>();
            if (!string.IsNullOrEmpty(searchCondition.Name))
            {
                query = query.Where(c => c.LoginName.Contains(searchCondition.Name) || c.Name.Contains(searchCondition.Name));
            }

            if (searchCondition.Role != -1)
            {
                query = query.Where(c => c.Role == searchCondition.Role);
            }

            var total = 0;

            var data = await query.ToPageListAsync(searchCondition.Page.PageNo, searchCondition.Page.PageSize, total);
            searchCondition.Page.RecordCount = data.Value;
            return data.Key;
        }


        /// <summary>
        /// 验证页面元素
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public async Task<bool> VerifLoginNameAsync(string loginName, string id)
        {
            if (string.IsNullOrEmpty(loginName))
            {
                return await Task.FromResult(true);
            }
            //新增验证唯一
            if (string.IsNullOrEmpty(id))
            {
                var model = await DbContext.AsQueryable().Where(c => c.LoginName == loginName).FirstAsync();
                if (model != null)
                {
                    return await Task.FromResult(false);
                }
                else
                {
                    return await Task.FromResult(true);
                }
            }
            else//修改验证唯一
            {
                var model = await DbContext.AsQueryable().Where(c => c.LoginName == loginName && c.Id !=id).FirstAsync();
                if (model != null)
                {
                    return await Task.FromResult(false);
                }
                else
                {
                    return await Task.FromResult(true);
                }
            }


        }

        /// <summary>
        /// 保存用户分配的项目
        /// </summary>
        /// <param name="projects"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> SaveUserProjects(List<string> projects, string id, string userId)
        {
            var list = await Db.Queryable<UserProjectMap>().Where(c => c.UserId == id).Select(c=>c.Id).ToListAsync();
            await Db.Deleteable<UserProjectMap>().In(list).ExecuteCommandAsync();

            if (projects.Count == 0)
            {
                return 0;
            }
            List<UserProjectMap> userProjectMaps = new List<UserProjectMap>();
            foreach (var projectId in projects)
            {
                UserProjectMap userProjectMap = new UserProjectMap();
                userProjectMap.Id = Guid.NewGuid().ToString();
                userProjectMap.UserId = id;
                userProjectMap.Flag = 0;
                userProjectMap.ProjectId = projectId;
                userProjectMap.Status = 0;
                userProjectMap.CreateUserId = userId;
                userProjectMap.CreateTime =DateTime.Now;
                userProjectMap.LastModifyTime =DateTime.Now;
                userProjectMap.LastModifyUserId = userId;
                userProjectMaps.Add(userProjectMap);
            }
            return await Db.Insertable<UserProjectMap>(userProjectMaps).ExecuteCommandAsync();

        }

        /// <summary>
        /// 获取待分配的项目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<Project>> GetLeftProjects(string id)
        {

            var ids = await Db.Queryable<Project, UserProjectMap>((p, u) => new object[]
            {
                JoinType.Inner,p.Id ==u.ProjectId
            }).Where((p, u) => u.UserId == id).Select((p, u) => p.Id).ToListAsync();

            var idsStr = string.Join(",", ids);
            var list = await Db.Queryable<Project>().Where(c => !SqlFunc.Contains(idsStr, c.Id)).ToListAsync();
            return list;

        }


        /// <summary>
        /// 获取已分配的项目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<Project>> GetRightProjects(string id)
        {
            var query = Db.Queryable<Project, UserProjectMap>((p, u) => new object[]
            {
                JoinType.Inner,p.Id ==u.ProjectId
            }).Where((p, u) => u.UserId == id && p.Status == 0).Select((p, u) => p);

            return await query.ToListAsync();
        }

        /// <summary>
        /// 根据用户名密码获取
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public async Task<User> GetByUserNamePwd(string userName, string pwd)
        {
            return await DbContext.AsQueryable().Where(c => c.LoginName == userName && c.Password == pwd).FirstAsync();

        }
    }
}
