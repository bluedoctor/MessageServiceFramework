﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using PWMIS.EnterpriseFramework.Service.Basic;
using TranstarAuction.Service.Runtime.Principal;

using MessageSubscriber;
using PWMIS.EnterpriseFramework.Common.Encrypt;
using PWMIS.EnterpriseFramework.Service.Runtime.Principal;
using PWMIS.EnterpriseFramework.Service.Client.Model;
using PWMIS.EnterpriseFramework.Service.Client;
using PWMIS.EnterpriseFramework.IOC;



namespace TranstarAuction.Service.Runtime
{
    /// <summary>
    /// 服务上下文
    /// </summary>
    public class ServiceContext : IServiceContext
    {
        #region 私有变量定义
        private ServiceResponse _response;
        private IServiceSession _session;
        private string _errorMessage = "";
        private bool hasError;

        private static readonly string NoneData1 = DataConverter.Encrypt8bitString( "[]");
        private static readonly string NoneData2 = DataConverter.Encrypt8bitString("null");

        #endregion

        #region 错误处理
        public event EventHandler<ServiceErrorEventArgs> ServiceErrorEvent;

        private void OnServiceError(Exception ex)
        {
            if (this.ServiceErrorEvent != null)
                this.ServiceErrorEvent(this, new ServiceErrorEventArgs(ex));
        }

        private void OnServiceError(Exception ex,string errorMessage)
        {
            if (this.ServiceErrorEvent != null)
                this.ServiceErrorEvent(this, new ServiceErrorEventArgs(ex, errorMessage));
        }

        private void OnServiceError(string errorMessage)
        {
            if (this.ServiceErrorEvent != null)
                this.ServiceErrorEvent(this, new ServiceErrorEventArgs(errorMessage));
        }

        #endregion

        #region 公开的属性
        /// <summary>
        /// 没有结果记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool NoResultRecord(string input)
        {
            if (string.IsNullOrEmpty(input) || input == NoneData1 || input == NoneData2)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 获取服务请求对象
        /// </summary>
        public ServiceRequest Request { get; set; }
        /// <summary>
        /// 获取或者设置服务响应对象
        /// </summary>
        public ServiceResponse Response
        {
            get
            {
                if (_response == null)
                    _response = GetNewResponse();
                return _response;
            }
            set
            {
                _response = value;
            }
        }

        public ServiceResponse GetNewResponse()
        {
            return new ServiceResponse(new System.IO.MemoryStream());
        }

        /// <summary>
        /// 获取或者设置会话状态。如果服务方法指定了需要使用会话状态，则会自动调用该属性。
        /// </summary>
        public IServiceSession Session {
            get {
                if (_session == null && this.SessionRequired)
                {
                    if (string.IsNullOrEmpty(this.SessionID))
                        throw new InvalidOperationException("会话标示不能为空！");
                    _session = SessionContainer.Instance.GetSession(this.SessionID);
                }
                return _session;
            }
            set {
                _session = value;
            }
        }

        /// <summary>
        /// 当前服务上下文运行时的会话标示
        /// </summary>
        public string SessionID { get; set; }

        /// <summary>
        /// 系统缓存
        /// </summary>
        public ICacheProvider Cache
        {
            get
            {
                return CacheProviderFactory.GetCacheProvider();
            }
        }

        /// <summary>
        /// 服务是否必须要求依赖于会话状态，该属性需要在IService接口方法里面指定
        /// </summary>
        public bool SessionRequired { get; set; }
        /// <summary>
        /// 获取服务关联的用户对象（该对象在应用程序中，用户登录成功以后，调用ServiceAuthentication 对象设置 ）
        /// </summary>
        public ServiceIdentity User
        {
            get
            {
                ServiceAuthentication auth = new ServiceAuthentication(this);
                return auth.GetIdentity();
            }
        }
        /// <summary>
        /// 服务所在的宿主
        /// </summary>
        public ServiceHostInfo Host { get; set; }

        bool _parallelExecute = true;
        /// <summary>
        /// 指定服务是否可以并发执行,默认为并行
        /// </summary>
        public bool ParallelExecute { 
            get {
                return _parallelExecute;
            }
            set {
                _parallelExecute = value;
            }
        }

        int _batchInterval = 1000;
        /// <summary>
        /// 每一批次的执行间隔时间，单位是毫秒，如果小于等于零，则不执行等待。默认为1秒
        /// </summary>
        public int BatchInterval
        {
            get
            {
                return _batchInterval;
            }
            set
            {
                _batchInterval = value;
            }
        }

        /// <summary>
        /// 是否向客户端发送空的结果，例如空的列表记录，或者结果为 NULL 的对象 
        /// </summary>
        public bool SendEmptyResult { get; set; }

        /// <summary>
        /// 获取（客户端）消息函数
        /// </summary>
        public Func<string,string> GetMessageFun;

        /// <summary>
        /// 预先获取（客户端）消息函数
        /// </summary>
        public Func<string, string> PreGetMessageFun;

        //private TResult innerCallBackFunction<T,TResult>(T para,Func<string, string> function)
        //{
        //    MessageConverter<T> convertPara = new MessageConverter<T>();
        //    string strPara = convertPara.Serialize(para);

        //    string strResult = function(strPara);

        //    MessageConverter<TResult> converter = new MessageConverter<TResult>(strResult);
        //    return converter.Result;
        //}

        /// <summary>
        /// 回调客户端的函数
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="para">参数</param>
        /// <returns>结果</returns>
        public TResult CallBackFunction<T, TResult>(T para)
        {
            MessageConverter<T> convertPara = new MessageConverter<T>();
            string strPara = convertPara.Serialize(para);

            string strResult = GetMessageFun(strPara);

            MessageConverter<TResult> converter = new MessageConverter<TResult>(strResult);
            return converter.Result;
        }

        /// <summary>
        /// 预先回调客户端的函数
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="para">参数</param>
        /// <returns>结果</returns>
        public TResult PreCallBackFunction<T, TResult>(T para)
        {
            MessageConverter<T> convertPara = new MessageConverter<T>();
            string strPara = convertPara.Serialize(para);

            string strResult = PreGetMessageFun(strPara);

            MessageConverter<TResult> converter = new MessageConverter<TResult>(strResult);
            return converter.Result;
        }
        /// <summary>
        /// 执行的批次号
        /// </summary>
        public int BatchIndex { get; set; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 以一个服务请求对象初始化本类
        /// </summary>
        /// <param name="request"></param>
        public ServiceContext(ServiceRequest request)
        {
            this.Request = request;
        }

        /// <summary>
        ///  以一个服务地址初始化本类，该操作将得到服务请求对象
        /// </summary>
        /// <param name="serviceUrl"></param>
        public ServiceContext(string serviceUrl)
        {
            ServiceRequest request = new ServiceRequest();
            //这里需要对复杂参数进行预处理
            request.ServiceUrl = serviceUrl;
            if (request.MethodParameters != null)
            {
                this.hasError =! InitRequestParameters(request, out _errorMessage);
            }
            else
            {
                request.Parameters = new object[] { };
            }
            this.Request = request;
        }

        private bool InitRequestParameters(ServiceRequest request,out string errorMessage)
        {
            List<IocProvider> providers = null;

            foreach (var item in request.MethodParameters)
            {
                if (item.ParameterType == null)
                {
                    if (providers == null)
                    {
                        providers = Unity.Instance.GetProviders("ServiceModel");
                        if (providers == null)
                        {
                            //throw new InvalidOperationException("IOC配置文件中IOC节点没有指定的名称ServiceModel");
                            errorMessage = "IOC配置文件中IOC节点没有指定的名称ServiceModel";
                            return false;
                        }
                        
                    }

                    IocProvider provider = providers.FirstOrDefault(i => i.FullClassName == item.ParameterTypeName);
                    if (provider != null)
                    {
                        item.ParameterType = Unity.Instance.GetProviderType(provider);
                        item.ParameterValue = ParameterParse.GetObject(item);
                    }
                    else
                    {
                        //处理泛型列表
                        if (item.ParameterTypeName.StartsWith("System.Collections.Generic.List`1[["))
                        {
                            string tempTypeName = item.ParameterTypeName.Replace("System.Collections.Generic.List`1[[", "");
                            tempTypeName = tempTypeName.Substring(0, tempTypeName.IndexOf(','));
                            provider = providers.FirstOrDefault(i => i.FullClassName == tempTypeName);
                            if (provider == null)
                            {
                                //throw new InvalidOperationException("IOC配置文件的名称ServiceModel下面没有定义当前类型：" + tempTypeName);
                                errorMessage = "IOC配置文件的名称ServiceModel下面没有定义当前类型：" + tempTypeName;
                                return false;
                            }


                            //生成基本类型
                            Type tempType = Unity.Instance.GetProviderType(provider);
                            Type generic = typeof(List<>);
                            Type[] typeArgs = { tempType };
                            Type constructed = generic.MakeGenericType(typeArgs);

                            item.ParameterType = constructed;
                            item.ParameterValue = ParameterParse.GetObject(item);
                        }
                        else
                        {
                            //throw new InvalidOperationException("系统不能处理当前类型的参数：" + item.ParameterTypeName);
                            errorMessage = "系统不能处理当前类型的参数：" + item.ParameterTypeName;
                            return false;
                        }
                    }

                }
            }//end for

            request.Parameters = request.MethodParameters.Select(o => o.ParameterValue).ToArray();
            errorMessage = "";
            return true;
        }

        #endregion

        #region 处理服务请求

        /// <summary>
        /// 处理服务请求，并将服务执行结果写入基础流，供远程调用方使用
        /// </summary>
        public void ProcessService()
        {
            if (this.hasError)
            {
                this.OnServiceError(new InvalidOperationException( _errorMessage));
                this.hasError = false;
                return;
            }
            IService service = ServiceFactory.GetService(this.Request.ServiceName);
            if (service == null)
            {
                this.OnServiceError(new InvalidOperationException("服务名称未注册，请检查服务配置。服务名：" + this.Request.ServiceName));
                return;
            }
            //发布模式，可能会共用一个服务上下文，所以需要重新设置 Response 对象
            //if(this.Request.RequestModel== RequestModel.Publish)
            //    this.Response = this.GetNewResponse();

            try
            {
                if (service.ProcessRequest(this))
                {
                    //执行上面的方法后，可以获得是否需要会话状态
                    //如果服务没有做自定义处理，则执行默认的服务行为，调用服务方法
                    if (service.IsUnSubscribe)
                    {
                        this.Response.Write("");
                    }
                    else
                    {
                        object result = this.ExecuteService(service);
                        if (result != null)
                        {
                            Type tempType= result.GetType();
                           
                            if (tempType == typeof(string))
                            {
                                this.Response.Write((string)result);
                            }
                            else if (tempType == typeof(DateTime)) //由于服务器和客户端时间格式可能不相同，统一用Json方式序列化
                            {
                                this.Response.WriteJsonString(result);
                            }
                             else if (tempType == typeof(byte[]))
                            {
                                this.Response.Write((byte[])result);
                            }
                            else if (tempType.IsValueType)
                            {
                                this.Response.Write(result.ToString());
                            }
                            else
                            {
                                this.Response.WriteJsonString(result);
                            }
                            this.Response.ResultType = tempType;
                        }
                        else
                        {
                            this.Response.Write("");
                        }
                    }
                    
                    service.CompleteRequest(this);
                    this.Response.End();
                }
            }
            catch (Exception ex)
            {
                this.Response.Write("");
                this.Response.End();
                this.OnServiceError(ex, string.Format(
                    "执行服务方法错误，源错误信息：{0}，\r\n请求的Uri:{1}，\r\n{2}:{3},{4}\r\n",
                    ex.Message,
                    this.Request.ServiceUrl,
                    this.Request.ClientIP,
                    this.Request.ClientPort,
                    this.Request.ClientIdentity )
                    );
            }
        }

        /// <summary>
        /// 
        /// 
        /// 处理服务请求，并将服务执行结果写入基础流，供远程调用放使用。如果服务需要会话，将创建会话状态对象
        /// </summary>
        /// <param name="sessionId">会话标示</param>
        public void ProcessService(string sessionId)
        {
            this.SessionID = sessionId;
            ProcessService();
        }

        /// <summary>
        /// 直接执行服务请求的方法
        /// </summary>
        /// <returns></returns>
        public object ExecuteService()
        {
            IService service = ServiceFactory.GetService(this.Request.ServiceName);
            return ExecuteService(service);
        }
        #endregion

        #region 动态方法执行
        /// <summary>
        /// 执行服务
        /// </summary>
        /// <returns></returns>
        private object ExecuteService(IService service)
        {
            return DynamicServiceCall(service, this.Request.MethodName, this.Request.Parameters);
        }

        private static object DynamicServiceCall(IService service, string methodName, string parameterString)
        {
            object[] parameters = ParameterParse.GetParameters(parameterString);
            return DynamicServiceCall(service, methodName, parameters);
        }

        private static object DynamicServiceCall(IService service, string methodName, object[] parameters)
        {
            MethodInfo methodInfo = service.GetType().GetMethod(methodName);
            if (methodInfo == null)
                throw new InvalidOperationException("当前方法调用无效，请确认方法名称正确并且具有公开的访问属性。");

            DynamicMethodExecutor executor = new DynamicMethodExecutor(methodInfo);
            object result = executor.Execute(service, parameters);

            return result;
        }
        #endregion

    }
}
