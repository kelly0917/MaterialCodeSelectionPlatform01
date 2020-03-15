using System;
using log4net;
using Microsoft.Extensions.Configuration;
using Novell.Directory.Ldap;

namespace MaterialCodeSelectionPlatform.Web.Utilities
{
    public class LDAPUtil
    {
        private static ILog log = LogHelper.GetLogger<Object>();
        public static string Domain;//域名称
        public static string Host;//域服务器地址
        public static string BaseDC;//根据上面的域服务器地址，每个点拆分为一个DC，例如上面的apac.contoso.com，拆分后就是DC=apac,DC=contoso,DC=com
        public static int Port = 389;//域服务器端口，一般默认就是389
        public static string DomainAdminUser;//域管理员账号用户名，如果只是验证登录用户，不对域做修改，可以就是登录用户名
        public static string DomainAdminPassword;//域管理员账号密码，如果只是验证登录用户，不对域做修改，可以就是登录用户的密码
        public static void Register(IConfigurationRoot configuration)
        {
            Domain = configuration["Domain"];//域名称
            Host = configuration["Host"];//域服务器地址
            BaseDC = configuration["BaseDC"];//根据上面的域服务器地址，每个点拆分为一个DC，例如上面的apac.contoso.com，拆分后就是DC=apac,DC=contoso,DC=com
            Port = int.Parse(configuration["ADPort"]); ;//域服务器端口，一般默认就是389
            DomainAdminUser = configuration["DomainAdminUser"];//域管理员账号用户名，如果只是验证登录用户，不对域做修改，可以就是登录用户名
            DomainAdminPassword = configuration["DomainAdminPassword"];//域管理员账号密码，如果只是验证登录用户，不对域做修改，可以就是登录用户的密码
        }



        public static bool Validate(string username, string password)
        {
            try
            {
                log.Debug($"域用户名为：{username} 开始认证登录");
                using (var conn = new LdapConnection())
                {
                    conn.Connect(Host, Port);
                    log.Debug("域服务端连接完成！");
                    conn.Bind(Domain + "\\" + DomainAdminUser, DomainAdminPassword);//这里用户名或密码错误会抛出异常LdapException
                    log.Debug("域用户管理员账号密码验证完成！");
                    var entities =
                        conn.Search(BaseDC, LdapConnection.ScopeSub,
                            $"sAMAccountName={username}",//注意一个多的空格都不能打，否则查不出来
                            new string[] { "sAMAccountName", "cn", "mail" }, false);
                    log.Debug("用户查找完成！");
                    string userDn = null;
                    while (entities.HasMore())
                    {
                        var entity = entities.Next();
                        var sAMAccountName = entity.GetAttribute("sAMAccountName")?.StringValue;
                       
                        //If you need to Case insensitive, please modify the below code.
                        if (sAMAccountName != null && sAMAccountName.Trim().Equals(username,StringComparison.OrdinalIgnoreCase))
                        {
                            userDn = entity.Dn;
                            break;
                        }
                    }
                    log.Debug("while循环完成！");
                    if (string.IsNullOrWhiteSpace(userDn))
                    {
                        log.Debug("userDn为空！");
                        return false;
                    }
                    conn.Bind(userDn, password);//这里用户名或密码错误会抛出异常LdapException
                    // LdapAttribute passwordAttr = new LdapAttribute("userPassword", password);
                    // var compareResult = conn.Compare(userDn, passwordAttr);
                    conn.Disconnect();
                    return true;
                }
            }
            catch (LdapException ldapEx)
            {
                string message = ldapEx.Message;
                log.Error(ldapEx);
                return false;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }

    }
}