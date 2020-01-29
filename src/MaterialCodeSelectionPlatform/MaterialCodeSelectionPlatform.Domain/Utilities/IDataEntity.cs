using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using SqlSugar;
using CommodityCodeSelectionPlatform.Domain.Entities;

namespace CommodityCodeSelectionPlatform.Domain
{
    public abstract class IDataEntity : IStringIdEntity, IIdEntity<string>
    {
        [SugarColumn(IsPrimaryKey = true)]
        public virtual string Id { get; set; }
        public virtual long Flag { get; set; }

        public virtual int Status { get; set; }

        public virtual string CreateUserId { get; set; }
        public virtual DateTime CreateTime { get; set; }
        public virtual string LastModifyUserId { get; set; }
        public virtual DateTime LastModifyTime { get; set; }

        public T Clone<T>() where T : new()
        {
            var propertys = this.GetType().GetProperties();
            T t = new T();
            foreach (var propertyInfo in propertys)
            {
                var pname = propertyInfo.Name;
                var value = propertyInfo.GetValue(this);
                if (value != null && t.GetType().GetProperties().Select(c => c.Name).Contains(pname))
                {
                    t.GetType().GetProperty(pname).SetValue(t, value);
                }
            }
            return t;
        }


      


        /// <summary>
        /// 对象赋值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="to">返回结果的对象，新对象</param>
        /// <param name="so">源对象</param>
        /// <returns></returns>
        public T SetValue<T>(T to, T so) where T : new()
        {
            var propertys = so.GetType().GetProperties();
            foreach (var propertyInfo in propertys)
            {
                var pname = propertyInfo.Name;
                var value = propertyInfo.GetValue(so);
                if (value != null && to.GetType().GetProperties().Select(c => c.Name).Contains(pname)
                    && pname != "Version"
                    //&& pname != "Flag"
                    && pname != "Status"
                    && pname != "CreateUserId"
                    && pname != "LastModifyTime"
                    && pname != "CreateTime"
                    && pname != "LastModifyUserId")
                {
                    to.GetType().GetProperty(pname).SetValue(to, value);
                }
            }
            return to;
        }

        /// <summary>
        /// 对象赋值
        /// </summary>
        /// <typeparam name="T">目标对象</typeparam>
        /// <param name="to">返回结果的对象，新对象</param>
        /// <param name="so">源对象</param>
        /// <returns></returns>
        public T SetValueDTO<T, S>(T to, S so)
            where T : new() where S : new()
        {
            to = new T();
            var propertys = so.GetType().GetProperties();
            foreach (var propertyInfo in propertys)
            {
                var pname = propertyInfo.Name;
                var value = propertyInfo.GetValue(so);
                if (value != null && to.GetType().GetProperties().Select(c => c.Name).Contains(pname))
                {
                    to.GetType().GetProperty(pname).SetValue(to, value);
                }
            }
            return to;
        }
    }
}
