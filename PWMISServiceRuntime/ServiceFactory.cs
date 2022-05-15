using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PWMIS.EnterpriseFramework.Service.Basic;
using PWMIS.EnterpriseFramework.IOC;

namespace PWMIS.EnterpriseFramework.Service.Runtime
{
    /// <summary>
    /// 服务工厂。【2022-5-15 服务支持Actor模式调用】
    /// </summary>
    public class ServiceFactory
    {
        /// <summary>
        /// 根据服务名称获取服务对象实例
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static IService GetService(string serviceName)
        {
            return Unity.Instance.GetInstance<IService>(serviceName);
        }

        /// <summary>
        /// 根据服务上下文创建服务对象实例。如果是推送模式，则从缓存获取服务对象。
        /// </summary>
        /// <param name="context">当前服务上下文</param>
        /// <returns></returns>
        public static IService GetService(IServiceContext context)
        {
            ServiceRequest request = context.Request;
            if (context.User!=null && context.User.UserData!=null && context.User.UserData.Contains("$ActorInstanceId="))
            {
                //处理Actor模式的服务实例 2022.5.15
                int startIndex = context.User.UserData.IndexOf("$ActorInstanceId=");
                int endIndex = context.User.UserData.IndexOf('$', startIndex + 1);
                if (endIndex == -1) endIndex = context.User.UserData.Length - 1;
                string actorString = context.User.UserData.Substring(startIndex+1, endIndex - startIndex );
                //示例：actorString="ActorInstanceId=aaaa;ActiveLife=10"
                string[] arr1 = actorString.Split(';');
                string[] arr2 = arr1[0].Split('=');
                if (arr2[0] == "ActorInstanceId")
                {
                    string actorId = arr2[1];
                    IService service = null;
                    if (request.RequestModel == RequestModel.GetService)
                    {
                        string strActiveLife = arr1[1].Split('=')[1];
                        //请求相应模式，服务对象生命周期由Proxy对象指定
                        if (!int.TryParse(strActiveLife, out int activeLife))
                            activeLife = 20;
                        service= context.Cache.Get<IService>(actorId, () => 
                         {
                             var temp= Unity.Instance.GetInstance<IService>(request.ServiceName);
                             temp.ActorInstanceId = actorId;
                             return temp;
                         },
                         new System.Runtime.Caching.CacheItemPolicy()
                         {
                             AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(activeLife)
                         });
                    }
                    else
                    {
                        //推送模式，服务对象生命周期由服务对象自己管理
                        service= context.Cache.Get<IService>(actorId, () => {
                           return  context.Cache.Get<IService>(request.ServiceUrl, () => {
                               var temp = Unity.Instance.GetInstance<IService>(request.ServiceName);
                               temp.ActorInstanceId = actorId;
                               return temp;
                           });
                        });
                        if (service!=null && context.Cache.Get<IService>(request.ServiceUrl) == null)
                        {
                            context.Cache.Insert<IService>(request.ServiceUrl, service);
                        }
                    }
                    return service;
                }
            }
           
            if (request.RequestModel == RequestModel.GetService)
            {
                return Unity.Instance.GetInstance<IService>(request.ServiceName);
            }
            else
            {
                return context.Cache.Get<IService>(request.ServiceUrl, () => {
                    return Unity.Instance.GetInstance<IService>(request.ServiceName);
                });
            }
        }

        /// <summary>
        /// 移除缓存的服务对象实例
        /// </summary>
        /// <param name="context">当前服务上下文</param>
        public static void RemoveServiceObject(IServiceContext context)
        {
            var service = context.Cache.Get<IService>(context.Request.ServiceUrl);
            if (service != null && service.ActorInstanceId != null)
            {
                context.Cache.Remove(service.ActorInstanceId);
            }
            context.Cache.Remove(context.Request.ServiceUrl);
        }
    }

    /*
     * 示例：在服务层，需要按照下面的方式定义具体的服务类 
     * 
    public class TestCalculatorService : IService
    {
        public int Add(int a, int b)
        {
            //模拟服务器延时
            System.Threading.Thread.Sleep(5000);
            return a + b;
        }

        public void ProcessRequest(IServiceContext context)
        {

        }
    }
     * */
}
