using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class TimeCount
    {
        private  int allCount=0;
        /// <summary>
        /// 服务端当前时间
        /// </summary>
        public DateTime Now { get; set; }
        /// <summary>
        /// 推送计数
        /// </summary>
        public int Count { get; set; }


        public void Execute()
        {
            this.Now = DateTime.Now;
            this.Count = ++allCount;
        }

    }
}
