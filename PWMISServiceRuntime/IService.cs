using System;
using System.Collections.Generic;
using System.Text;
using PWMIS.EnterpriseFramework.Service.Basic;
using PWMIS.EnterpriseFramework.Service.Runtime.Principal;
using PWMIS.EnterpriseFramework.Service.Client.Model;

namespace PWMIS.EnterpriseFramework.Service.Runtime
{
    /// <summary>
    /// 基础服务接口
    /// </summary>
    public interface IService
    {

        /// <summary>
        /// 是否继续处理请求，如果不需要服务自动调用服务方法，请在当前方法中做处理，并返回False
        /// </summary>
        /// <param name="context">当前服务上下文</param>
        /// <returns>是否需要继续处理服务请求</returns>
        bool ProcessRequest(IServiceContext context);
        /// <summary>
        /// 请求的服务方法执行完成以后的操作
        /// </summary>
        /// <param name="context"></param>
        void CompleteRequest(IServiceContext context);

        /// <summary>
        /// (发布-订阅模式中)是否已经注销订阅服务
        /// </summary>
        bool IsUnSubscribe { get; }

    }

    /// <summary>
    /// 服务抽象类
    /// </summary>
    public abstract class ServiceBase : IService
    {
        /// <summary>
        /// 当前服务上下文
        /// </summary>
        public IServiceContext CurrentContext { get; set; }

        /// <summary>
        /// (发布-订阅模式中)是否已经注销订阅服务
        /// </summary>
        public bool IsUnSubscribe { get; private set; }

        /// <summary>
        /// 获取全局缓存，根据配置，可以支持分布式的缓存服务器
        /// </summary>
        public ICacheProvider GlobalCache
        {
            get
            {
                return CacheProviderFactory.GetGlobalCacheProvider();
            }
        }
        /// <summary>
        /// 注销订阅的服务，并执行其它清理资源的操作
        /// </summary>
        /// <returns></returns>
        public virtual bool UnSubscribeService()
        {
            if (this.CurrentContext.Session != null)
            {
                this.CurrentContext.Session.Clear();
            }
            this.IsUnSubscribe = true;
            return true;
        }

        /// <summary>
        /// 是否继续处理请求，如果不需要服务自动调用服务方法，请在当前方法中做处理，并返回False，如果需要自定义的处理，请重写该方法
        /// </summary>
        /// <param name="context">当前服务上下文</param>
        public virtual bool ProcessRequest(IServiceContext context)
        {
            this.CurrentContext = context;
            return true;
        }

        /// <summary>
        /// 请求的服务方法执行完成以后的操作
        /// </summary>
        /// <param name="context"></param>
        public virtual void CompleteRequest(IServiceContext context)
        {

        }

        /// <summary>
        /// 发布分布式事件
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public void PublishDistributeEvent(object eventData)
        {
            this.CurrentContext.PublishData(eventData);
        }

        /// <summary>
        /// 在后续的订阅者加入时，服务进行的一些处理并返回给当前订阅者的数据，该数据类型需要与订阅时的数据类型一致。
        /// </summary>
        /// <returns>推送给当前订阅者的数据，如果为空则不会推送</returns>
        public virtual object OnSubsequentSubscribersAdding()
        {
            return null;
        }
    }

   
}
