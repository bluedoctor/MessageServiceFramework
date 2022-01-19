/***
 * 当前源码文件来自于作者 walterlv 的博客文章参考：https://www.cnblogs.com/walterlv/p/10326495.html
 * MSF开源库不包括此文源码的版权。
 * 2022.1.19
 * 
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MSF_NuProj_Pkg
{
    /// <summary>
    /// 包含在运行时判断编译器编译配置中调试信息相关的属性。
    /// </summary>
    public static class DebuggingProperties
    {
        /// <summary>
        /// 检查当前正在运行的主程序是否是在 Debug 配置下编译生成的。
        /// </summary>
        public static bool IsDebug
        {
            get
            {
                if (_isDebugMode == null)
                {
                    var assembly = Assembly.GetEntryAssembly();
                    if (assembly == null)
                    {
                        // 由于调用 GetFrames 的 StackTrace 实例没有跳过任何帧，所以 GetFrames() 一定不为 null。
                        assembly = new StackTrace().GetFrames().Last().GetMethod().Module.Assembly;
                    }
                    //DebuggableAttribute
                    var atts= assembly.GetCustomAttributes(true);
                    DebuggableAttribute debuggableAttribute = null;
                    foreach (var obj in atts)
                    {
                        if (obj is DebuggableAttribute)
                        { 
                            debuggableAttribute = obj as DebuggableAttribute;
                            break;
                        }
                    }
                    if (debuggableAttribute == null)
                        return false;

                    _isDebugMode = debuggableAttribute.DebuggingFlags
                        .HasFlag(DebuggableAttribute.DebuggingModes.EnableEditAndContinue);
                }

                return _isDebugMode.Value;
            }
        }

        private static bool? _isDebugMode;
    }
}
