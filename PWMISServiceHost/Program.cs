//是否允许公开使用
#define NotPrivateUse

using System;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using MessagePublisher;
using MessagePublishService;
using PWMIS.EnterpriseFramework.Service.Basic;
using PWMIS.EnterpriseFramework.Service.Runtime;
using PWMIS.EnterpriseFramework.Service.Client.Model;
using PWMIS.EnterpriseFramework.Service.Group;
using System.Xml;
using System.ServiceModel.Description;
using System.Diagnostics;

namespace PWMIS.EnterpriseFramework.Service.Host
{
    class Program
    {
#if(PrivateUse)
        private static readonly string UseDescrition = "当前版本仅供测试，如需要公开使用请购买许可协议";
#endif
        const string LogDirectory = "./Log/";
        private static object sync_obj = new object();
        private static DateTime CurrentDate = new DateTime(1900,1,1);
        private static TextWriter ConsoleOut;
        private static bool EnableConsoleOut = false;//是否允许输出转向
        /// <summary>
        /// 服务宿主地址
        /// </summary>
        public static ServiceHostInfo Host { get; private set; }
        /// <summary>
        /// 远程控制台监听器
        /// </summary>
        public static MessageListener RemoteConsoleListener;

        private static System.Threading.Timer CountTimer ;
        /// <summary>
        /// 监听器统计，每分钟统计一次，如果服务挂机，则统计信息在3分钟后过期
        /// </summary>
        private static void ListenersCountTimer()
        {
            CountTimer = new System.Threading.Timer(new System.Threading.TimerCallback(o =>
            {
                DateTime dt = DateTime.Now;
                int[] arrCount = MessageCenter.Instance.CheckListeners();
                int currCount = arrCount[0];
                //将监听器数量写入全局缓存，供集群调度服务使用
                ICacheProvider cache = CacheProviderFactory.GetGlobalCacheProvider();
                string key = Program.Host.GetUri() + "_HostInfo";

                ServiceHostInfo serviceHostInfo = cache.Get<ServiceHostInfo>(key, () =>
                {
                    ServiceHostInfo info = new ServiceHostInfo();
                    info.RegServerDesc = Program.Host.RegServerDesc;
                    info.RegServerIP = Program.Host.RegServerIP;
                    info.RegServerPort = Program.Host.RegServerPort;
                    info.IsActive = Program.Host.IsActive;
                    info.ServerMappingIP = Program.Host.ServerMappingIP;
                    info.LogDirectory = Program.Host.LogDirectory;
                    info.ListenerCount = currCount;
                    info.ListenerMaxCount = currCount;
                    info.ListenerMaxDateTime = DateTime.Now;
                    info.ActiveConnectCount = arrCount[1];
                    return info;
                },
                new System.Runtime.Caching.CacheItemPolicy() { SlidingExpiration = new TimeSpan(0, 10, 0) }
                );

                bool changed = false;
                int maxCount = serviceHostInfo.ListenerMaxCount;

                if (currCount > maxCount)
                {
                    changed = true;
                    maxCount = currCount;
                    serviceHostInfo.ListenerCount = currCount;
                    serviceHostInfo.ListenerMaxCount = maxCount;
                    serviceHostInfo.ListenerMaxDateTime = DateTime.Now;
                }
                else if (currCount != serviceHostInfo.ListenerCount)
                {
                    changed = true;
                    serviceHostInfo.ListenerCount = currCount;
                }

                if (changed)
                {
                    serviceHostInfo.ActiveConnectCount = arrCount[1];

                    Host.ListenerCount = serviceHostInfo.ListenerCount;
                    Host.ListenerMaxCount = serviceHostInfo.ListenerMaxCount;
                    Host.ListenerMaxDateTime = serviceHostInfo.ListenerMaxDateTime;
                    Host.ActiveConnectCount = serviceHostInfo.ActiveConnectCount;

                    cache.Insert<ServiceHostInfo>(
                           key,
                           serviceHostInfo,
                           new System.Runtime.Caching.CacheItemPolicy() { SlidingExpiration = new TimeSpan(0, 3, 0) }
                           );
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("=========监听器数量统计：当前{0}个,最大{1}个,用时{2} ms ============", currCount, maxCount, DateTime.Now.Subtract(dt).TotalMilliseconds);
                Console.ResetColor();
            }), null, 30000, 60000);
        }

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            if (!System.IO.Directory.Exists(LogDirectory))
            {
                System.IO.Directory.CreateDirectory(LogDirectory);
            }
            Console.WriteLine("Log ok.Log Directory:{0}", LogDirectory);
            /////////////////////////////////////////////////////////////////////////
#if(MONO)
            if (Environment.GetEnvironmentVariable("MONO_STRICT_MS_COMPLIANT") != "yes")
            {
                Environment.SetEnvironmentVariable("MONO_STRICT_MS_COMPLIANT", "yes");
                Console.WriteLine("Setting environment variables \"MONO_STRICT_MS_COMPLIANT\" = Yes!");
            }
            else
            {
                Console.WriteLine("Current environment variables \"MONO_STRICT_MS_COMPLIANT\" = Yes!");
            }
#endif

            ///////////////////////////////////////////////////////////////////////////
            //参数获取设置的服务地址，如果没有，则保留默认的 127.0.0.1:8888
            string ip = System.Configuration.ConfigurationManager.AppSettings["ServerIP"];// "127.0.0.1";
            //string ip = "127.0.0.1";
            IPAddress ipAddr;
            if (args.Length > 0 && IPAddress.TryParse(args[0], out ipAddr))
            {
                ip = ipAddr.ToString();
            }
            Console.WriteLine("Ip config ok.");

            int port = int.Parse( System.Configuration.ConfigurationManager.AppSettings["ServerPort"]);// 8888;
            int tempPort;
            if (args.Length > 1 && int.TryParse(args[1], out tempPort))
            {
                port = tempPort;
            }
            if (args.Length > 2 && args[2].ToLower() == "outlog")
            {
                EnableConsoleOut = true;
            }
            if (args.Length > 3 )
            {
                Console.Title = args[3];
            }

            Console.WriteLine("Address config ok.");
            ////
           
            string uri1 = string.Format("net.tcp://{0}:{1}", ip, port-1);
            NetTcpBinding binding1 = new NetTcpBinding(SecurityMode.None);

            ServiceHost calculatorHost = new ServiceHost(typeof(CalculatorService));
            calculatorHost.AddServiceEndpoint(typeof(ICalculator), binding1, uri1);
            calculatorHost.Opened += delegate
            {
                Console.WriteLine("The Test Service(calculator) has begun to listen");
            };
            calculatorHost.Open();

            ////
            string uri = string.Format("net.tcp://{0}:{1}", ip, port);
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            //Console.WriteLine("binding init 1,ok.");

            binding.MaxBufferSize = int.MaxValue;
            //Console.WriteLine("binding init 2,ok.");

            binding.MaxReceivedMessageSize = int.MaxValue;
            //Console.WriteLine("binding init 3,ok.");
#if(MONO)
            XmlDictionaryReaderQuotas quo = new XmlDictionaryReaderQuotas();
            binding.ReaderQuotas = quo;
            Console.WriteLine("Binding mono init ,ok.");
#endif
            binding.ReaderQuotas.MaxArrayLength = 65536;
            //Console.WriteLine("binding init 4,ok.");

            binding.ReaderQuotas.MaxBytesPerRead = 10 * 1024 * 1024;
            binding.ReaderQuotas.MaxStringContentLength = 10 * 1024 * 1024; //65536;
           
            binding.ReceiveTimeout = TimeSpan.MaxValue;//设置连接自动断开的空闲时长；
            binding.MaxConnections = 500;
            binding.ListenBacklog = 200;
            //Console.WriteLine("binding init 5,ok.");
            binding.TransferMode = TransferMode.Buffered;
            //Console.WriteLine("binding init 6,ok.");
            //请参见 http://msdn.microsoft.com/zh-cn/library/ee767642 进行设置

            ListAllBindingElements(binding);
            Console.WriteLine("Service binding config check all ok.");
            Console.WriteLine("  MaxConnections={0},ListenBacklog={1}", binding.MaxConnections, binding.ListenBacklog);

            ServiceHost host = new ServiceHost(typeof(MessagePublishServiceImpl));
            //设置吞吐量
            ServiceThrottlingBehavior throttlingBehavior = host.Description.Behaviors.Find<ServiceThrottlingBehavior>();
            if (null == throttlingBehavior)
            {
                throttlingBehavior = new ServiceThrottlingBehavior();
                host.Description.Behaviors.Add(throttlingBehavior);
            }
            //throttlingBehavior.MaxConcurrentCalls = 300;
            //throttlingBehavior.MaxConcurrentInstances = 350; //InstanceContextMode.Single 无需设置
            //throttlingBehavior.MaxConcurrentSessions = 500;
            ListServiceBehavior(host);

            host.AddServiceEndpoint(typeof(IMessagePublishService), binding, uri);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("=========PDF.NET.MSF (PWMIS Message Service) Ver {0} =====", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Console.WriteLine("启动消息发布服务……接入地址:{0} ,PID:{1}", uri, Process.GetCurrentProcess().Id);
            Console.ResetColor();
            Console.WriteLine();

            ChangeConsoleOut();
            if (EnableConsoleOut)
            {
                ListAllBindingElements(binding);
                Console.WriteLine("启动消息发布服务……接入地址：{0}", uri);
            }
            Console.WriteLine("检查服务节点... ...");

            ////////////////////向集群中写入节点 ///////////////////////////////////////////////////////////////
            //ServiceHostUri = uri;

            ServiceRegModel model = new ServiceRegModel();
            model.RegServerIP = ip;
            model.RegServerPort = port;
            model.RegServerDesc = string.Format("Server://{0}:{1}", ip, port);
            model.ServerMappingIP = System.Configuration.ConfigurationManager.AppSettings["ServerMappingIP"];

            Host = new ServiceHostInfo();
            Host.RegServerDesc = model.RegServerDesc;
            Host.RegServerIP = model.RegServerIP;
            Host.RegServerPort = model.RegServerPort;
            Host.IsActive = model.IsActive;
            Host.ServerMappingIP = model.ServerMappingIP;

            RegServiceContainer container = new RegServiceContainer();
            container.CurrentContext = new ServiceContext("");
            if (container.RegService(model))
                Console.WriteLine("======注册集群节点成功，服务将以集群模式运行=====================");
            else
                Console.WriteLine("====== 未使用全局缓存，服务将以独立模式运行 =====================");

            ////////////////////////////////////////////////////////////////////////////////////////////////////////
            MessageCenter.Instance.ListenerAdded += new EventHandler<MessageListenerEventArgs>(Instance_ListenerAdded);
            MessageCenter.Instance.ListenerRemoved += new EventHandler<MessageListenerEventArgs>(Instance_ListenerRemoved);
            MessageCenter.Instance.NotifyError += new EventHandler<MessageNotifyErrorEventArgs>(Instance_NotifyError);
            MessageCenter.Instance.ListenerAcceptMessage += new EventHandler<MessageListenerEventArgs>(Instance_ListenerAcceptMessage);
            MessageCenter.Instance.ListenerEventMessage += new EventHandler<MessageListenerEventArgs>(Instance_ListenerEventMessage);
            MessageCenter.Instance.ListenerRequestMessage += new EventHandler<MessageRequestEventArgs>(Instance_ListenerRequestMessage);
            
#if(PrivateUse)
            if (ip.StartsWith("192.168.") || ip.StartsWith("127.0.0.1")) //测试，仅限于局域网使用
            {
                host.Open();

                Console.WriteLine("服务正在运行");
                EnterMessageInputMode();

                Console.WriteLine("正在关闭服务……");
                host.Close();

                Console.WriteLine("服务已关闭。");
            }
            else
            {
                Console.WriteLine("服务已关闭，{0}。", UseDescrition);
            }

#else
            host.Open();

            Console.WriteLine("服务正在运行……");
            ListenersCountTimer();//监听器数量统计
            Console.WriteLine("监听器数量统计功能已开启！");
            EnterMessageInputMode();

            Console.WriteLine("正在关闭服务……");
            host.Close();
            calculatorHost.Close();

            Console.WriteLine("服务已关闭。");
            Console.WriteLine("请按任意键退出...");

#endif

            host = null;
            Console.ReadLine();
        }

       

        /// <summary>
        /// 改变控制台的输出到日志文件中
        /// </summary>
        static void ChangeConsoleOut()
        {
            if (EnableConsoleOut)
            {
                if (CurrentDate != DateTime.Today)
                {
                    lock (sync_obj)
                    {
                        if (CurrentDate != DateTime.Today)
                        {
                            CurrentDate = DateTime.Today;
                            if (ConsoleOut != null)
                                ConsoleOut.Close();

                            string fileName = LogDirectory + CurrentDate.ToString("yyyy-MM-dd") + ".txt";
                            //备份日志文件
                            if (File.Exists(fileName))
                            {
                                string backFileName = LogDirectory + "back-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt";
                                File.Move(fileName, backFileName);
                            }

                            //StreamWriter standardOutput = new StreamWriter(Console.OpenStandardOutput(), Encoding.Default);
                            //standardOutput.AutoFlush = true;
                            //Console.SetOut(standardOutput);
                            Console.WriteLine("已经将控制台输出转向到文件：{0}", fileName);

                            ConsoleOut = File.CreateText(fileName);
                            
                            Console.SetOut(ConsoleOut);
                            ((StreamWriter)ConsoleOut).AutoFlush = true;
                            Console.WriteLine("----服务控制台 日志文件输出------");
                            ConsoleOut.Flush();
                        }
                    }
                }
                //每10秒输出一次缓冲区
                //if (DateTime.Now.Second % 10 == 0)
                //    ConsoleOut.Flush();
            }

        }

        /// <summary>
        /// 消息发送模式；（后续可以实现命令行服务诊断功能）
        /// </summary>
        static void EnterMessageInputMode()
        {
            Console.WriteLine("请输入要发送的消息，按回车键发送。输入 @exit 回车结束操作并关闭服务，输入@help获取控制台命令帮助。");
			string line;
            do
            {
                Console.Write(">>");
                line = Console.ReadLine().Trim();

                if (line == "@help")
                {
                    Console.WriteLine(" MSF Host Console Command:");
                    Console.WriteLine(" @pid:当前进程ID");
                    Console.WriteLine(" @srv_url:服务接入地址");
                    Console.WriteLine(" @>>:控制台输出到文件");
                    Console.WriteLine(" @exit:关闭服务");
                    continue;
                }
                else if ("@pid" == line)
                {
                    Console.WriteLine("当前进程ID：{0}", Process.GetCurrentProcess().Id);
                    continue;
                }
                else if ("@srv_url" == line)
                {
                    Console.WriteLine("服务接入地址：{0}", Host.RegServerDesc);
                    continue;
                }
                else if ("@>>" == line)
                {
                    Console.WriteLine("执行本命令，所有控制台输出都将立即输出到MSF系统日志文件，如果需要重新在控制台查看消息，只有重启本服务。");
                    Console.Write("确认输出转向吗？(Y/N)");
                    line = Console.ReadLine().Trim();
                    if (line == "Y")
                    {
                        EnableConsoleOut = true;
                        ChangeConsoleOut();
                    }
                    else
                    {
                        Console.WriteLine("已经取消执行控制台输出转向。");
                    }
                    continue;
                }
                else if ("@exit" == line)
                {
                    break;
                }
                if (string.Empty == line)
                {
                    Console.WriteLine("[{0}]不能发送空消息！", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    continue;
                }
                int count= MessageCenter.Instance.NotifyMessage(line);
                if (count == 0)
                    Console.WriteLine("没有客户订阅文本消息。");
                else
                    Console.WriteLine("[{0}]发送成功！订阅人数：{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),count);

            } while (true);
        }

        static void Instance_NotifyError(object sender, MessageNotifyErrorEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[{0}]消息发送失败！--IP:{1}; Port:{2}; Error:{3}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), e.Listener.FromIP, e.Listener.FromPort, e.Error.Message);
            Console.WriteLine("移除无效监听器……");
            Console.ResetColor();
            MessageCenter.Instance.RemoveListener(e.Listener);
        }

        static void Instance_ListenerRemoved(object sender, MessageListenerEventArgs e)
        {
            string text= string.Format("[{0}]取消订阅-- From: {1}:{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), e.Listener.FromIP, e.Listener.FromPort);
            Console.WriteLine(text);
            PublisherFactory.Instance.RemoveMessageListener(e.Listener);
            WriteLogFile("MSFListenerLog" + DateTime.Now.ToString("yyyyMMdd") + ".txt", text);
        }

        static void Instance_ListenerAdded(object sender, MessageListenerEventArgs e)
        {
            string text = string.Format("[{0}]订阅消息-- From: {1}:{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), e.Listener.FromIP, e.Listener.FromPort);
            Console.WriteLine(text);
            WriteLogFile("MSFListenerLog"+ DateTime.Now.ToString("yyyyMMdd") + ".txt", text);
        }

        static void Instance_ListenerAcceptMessage(object sender, MessageListenerEventArgs e)
        {
            ChangeConsoleOut();

#if(PrivateUse)
            string ip= e.Listener.FromIP;
            if (ip.StartsWith("192.168.") || ip.StartsWith("127.0.0.1")) //测试，仅限于局域网使用
            {
                ip = string.Empty;
            }
            else
            {
                Console.WriteLine("错误，{0}", UseDescrition);
                return;
            }
#endif
            //下面整个处理过程应该放到一个动态实例对象的方法中,否则,多线程问题难以避免
            SubscriberInfo subInfo = new SubscriberInfo(e.Listener);
            MessageProcesser processer = new MessageProcesser(subInfo, e.Listener.FromMessage);
            processer.ServiceErrorEvent += new EventHandler<ServiceErrorEventArgs>(Processer_ServiceErrorEvent);
            //Console.WriteLine("process message begin.");
            try
            {
                processer.Process();
            }
            catch (Exception ex)
            {
                Processer_ServiceErrorEvent(processer, new ServiceErrorEventArgs(ex));
            }
            //Console.WriteLine("process message end.");
        }

        static void Instance_ListenerRequestMessage(object sender, MessageRequestEventArgs e)
        {
            MessageProcesser processer = new MessageProcesser();
            try
            {
                processer.Execute(e);
            }
            catch (Exception ex)
            {
                Processer_ServiceErrorEvent(processer, new ServiceErrorEventArgs(ex));
            }
        }

        public static void Processer_ServiceErrorEvent(object sender, ServiceErrorEventArgs e)
        {
            string text = string.Format("[{0}]处理服务的时候发生异常：{1}\r\n错误发生时的异常对象调用堆栈：\r\n{2}", 
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 
                e.ErrorMessageText,
                e.ErrorSource == null ? "NULL" : e.ErrorSource.ToString());
            Console.ForegroundColor = ConsoleColor.Red;
            ConsoleWriteSubText(text, 1000);
            Console.ResetColor();
            WriteLogFile("MSFErrorLog.txt", text);
        }

        static void Instance_ListenerEventMessage(object sender, MessageListenerEventArgs e)
        {
            string text = string.Format("[{0}]监听器事件--From: {1}:{2}\r\n[{3}]", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), e.Listener.FromIP, e.Listener.FromPort, e.MessageText);
            ConsoleWriteSubText(text,1000);
            WriteLogFile("MSFListenerEvent.txt", text);
        }

        static void ListAllBindingElements(Binding binding)
        {
            Console.WriteLine("Service All Binding Elements:----------------------------------");
            BindingElementCollection elements = binding.CreateBindingElements();
            for (int i = 0; i < elements.Count; i++)
            {
                Console.WriteLine("{0}. {1}", i + 1, elements[i].GetType().FullName);
            }
        }

        //ServiceBehavior
        static void ListServiceBehavior(ServiceHost host)
        {
            string msg = "Service Behaviors:---------------------------------------------" + Environment.NewLine;
            int i = 1;
            foreach (IServiceBehavior behavior in host.Description.Behaviors)
            {
                msg += i+", "+ behavior.ToString() + Environment.NewLine;

                if (behavior is ServiceThrottlingBehavior)
                {
                    ServiceThrottlingBehavior serviceThrottlingBehavior = (ServiceThrottlingBehavior)behavior;
                    msg += "     maxConcurrentSessions =   " + serviceThrottlingBehavior.MaxConcurrentSessions.ToString() + Environment.NewLine;
                    msg += "     maxConcurrentCalls =   " + serviceThrottlingBehavior.MaxConcurrentCalls.ToString() + Environment.NewLine;
                    msg += "     maxConcurrentInstances =   " + serviceThrottlingBehavior.MaxConcurrentInstances.ToString() + Environment.NewLine;
                }
                i++;
            }
            Console.WriteLine(msg);
        }

        static void ConsoleWriteSubText(string text, int length)
        {
            Console.WriteLine(text.Length > length ? text.Substring(0, length)+" ...\" \r\n" : text);
        }

        /////////////////////
        static void TestService()
        {
            ServiceRequest request = new ServiceRequest();
            request.ServiceName = "User";
            request.MethodName = "Login";
            request.Parameters = new object[] { "aaa", "123" };

            Console.Write(CallService(request));
            Console.WriteLine();
            Console.WriteLine("---服务方法调用完成--");
        }


        static string CallService(ServiceRequest serviceRequest)
        {
            ServiceContext context = new ServiceContext(serviceRequest);
            serviceRequest = context.Request;
            context.ProcessService();
            return context.Response.AllText;
        }

        

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            string errMsg = "程序发生未处理的异常：\r\n" + e.ExceptionObject.ToString();
            Console.WriteLine(errMsg);
            Console.ResetColor();
            WriteLogFile("MSFErrorLog.txt",errMsg);
        }

        static void WriteLogFile(string fileName,string logMsg)
        {
            try
            {
                string text = string.Format("\r\n------------------------------\r\n{0}",  logMsg);
                System.IO.File.AppendAllText(LogDirectory + fileName, text);
            }
            catch(Exception ex)
            {
                Console.WriteLine("保存日志文件错误：{0}，错误信息：{1}",fileName,ex.Message);
            }
        }
    }
}
