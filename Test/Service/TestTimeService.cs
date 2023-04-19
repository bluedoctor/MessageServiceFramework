using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PWMIS.EnterpriseFramework.Service.Runtime;
using Model;
using System.Threading.Tasks;

namespace ServiceSample
{
    public class TestTimeService:ServiceBase
    {
        TimeCount tc;
        
        public TestTimeService()
        {
            tc = new TimeCount();
        }

        /// <summary>
        /// 供订阅使用的获取服务器时间的方法
        /// </summary>
        /// <returns></returns>
        public TimeCount ServerTime(int index)
        {
            if (base.CurrentContext.User != null)
            {
                Console.WriteLine("username:{0}", base.CurrentContext.User.Name);
            }
            else
            {
                Console.WriteLine("No User.");
            }
            tc.Index = index;
            tc.Execute();
            return tc;
        }

        /// <summary>
        /// 使用事件订阅的模式，进行并发的服务器时间推送测试，达到分布式事件推送的效果。
        /// </summary>
        /// <param name="timeCount"></param>
        /// <returns></returns>
        public ServiceEventSource ParallelTime(int timeCount)
        {
            return new ServiceEventSource(this, 1, () => {
                PublishParallelData(timeCount);
                Console.WriteLine("All Published OK.");
                //推送一个结束时间标志，客户端收到后关闭连接。
                //base.PublishDistributeEvent(new DateTime(2018,1,1));
                //base.CurrentContext.PublishEventSource.DeActive();
            });

        }

        private void PublishParallelData(int timeCount)
        {
            Task[] tasks = new Task[timeCount];
            for (int i = 0; i < timeCount; i++)
            {
                tasks[i] =Task.Factory.StartNew(obj => {
                    int index = (int)obj;
                    for (int m = 0; m < 50; m++)
                    {
                        DateTime dt = DateTime.Now;
                        Console.WriteLine(">>>>>>>>> NO.{0} begin publish data:{1},Thread ID:{2}", index, dt, System.Threading.Thread.CurrentThread.ManagedThreadId);
                        base.CurrentContext.PublishData(dt);
                        Console.WriteLine("<<<<<<<<< NO.{0} end   publish data:{1},Thread ID:{2}", index, dt, System.Threading.Thread.CurrentThread.ManagedThreadId);
                        Console.WriteLine();

                        Task.Delay(1000).Wait();
                    }
                  
                },i);
            }
            Task.WaitAll(tasks, -1);
            //throw new Exception("test error...............");
        }

        /// <summary>
        ///  在后续的订阅者加入时，服务进行的一些处理并返回给当前订阅者的数据，该数据类型需要与订阅时的数据类型一致。
        /// </summary>
        /// <returns></returns>
        public override object OnSubsequentSubscribersAdding()
        {
            if (base.CurrentContext.Request.MethodName == "ServerTime")
            {
                tc.Execute();
                Console.WriteLine("SubsequentSubscribe:---{0}--{1}--", tc.Now, tc.Count);
                return tc;
            }
            else
            {
                //PublishParallelData(10);
                //return new DateTime(2018, 1, 1);
                Console.WriteLine("SubsequentSubscribe Request.MethodName {0}", base.CurrentContext.Request.MethodName);
                return DateTime.Now;
            }
        }

    }
}
