MSF / iMSF
===

Message Service Framework 消息服务框架
-------------------------------------

框架简介：
---------
 “一切都是消息”  --这是iMSF（即时消息服务框架）的设计哲学。

MSF/iMSF 的名字是 Message Service Framework 的简称，由于目前框架主要功能在于处理即时（immediately）消息，
所以iMSF就是 immediately Message Service Framework，中文名称：即时消息服务框架，它是PDF.NET框架的一部分。
MSF诞生至今已有10年历史！

框架的主要功能基于消息订阅，实现了下面三种消息通讯模式：
#### 1，【请求-响应】模式
（同步/异步）消息模式，一对一单次通信。
#### 2，【订阅-发布】模式
（异步）消息模式，一对多端通信，广播推送，支持定时推送或者事件推送。
#### 3，【订阅-回调】模式
（异步）消息模式，一对多次通信，服务端可多次回调客户端的函数。


## 框架主要分为下面三部分：

* PDF.Net.MSF.Client  --MSF客户端
* PDF.Net.MSF.Service  --MSF服务组件
* PDF.Net.MSF.Service.Host --MSF服务宿主

入门详细内容，请看博文：
https://www.cnblogs.com/bluedoctor/p/7605737.html

功能演示示例程序，请下载测试项目运行：
https://github.com/bluedoctor/MSFTest

### 此外，框架还支持下列功能：
* 内置缓存服务
* 内置会话服务
* 内置权限认证服务
* 支持集群负载均衡

MSF新版本说明：
--------------
MSF程序集版本2.0.22.120以上为新版本，该新版本的主要变化为所支持的.NET版本发了变化，MSF客户端和 MSF服务的支持.NET 4.0升级到：

* 1，MSF 服务端升级到 .NET 4.7.1

* 2，MSF 客户端升级到 .NET 4.5

如果你的产品需要继续使用 .NET 4.0，请使用MSF之前的版本，对应的Nuget包发布日期在2022.1.1日之前的版本。

当前版本和之前版本的MSF基于WCF技术构建，本次升级主要是为了利用较高版本的.NET中的WCF增强的功能。

如何使用解决方案
-------------------------------
当前版本支持VS2017\2019\2022

如果是VS2017、VS2019，可以在VS中安装 Nuget Package Project 扩展，然后就可以打开Nuget解决方案目录下的项目。
如果全部编译成功，可以运行 Test\WinClient 启动服务，然后开始测试。

附：WCF、.NET 和Windows系统版本之间的关系
--------------------------------

WCF在 .NET 4.5和.NET 4.7有很多新增功能和功能升级，详细内容请参看：
.NET Framework 中的新增功能
https://docs.microsoft.com/zh-cn/dotnet/framework/whats-new/?redirectedfrom=MSDN#whats-new-in-net-framework-45

有关Windows操作系统版本和.NET版本之间的关系，请参考下面内容：
.NET Framework 版本和依赖关系
https://docs.microsoft.com/zh-cn/dotnet/framework/migration-guide/versions-and-dependencies


---------------
编辑：深蓝医生 （QQ：45383850）

时间：2022.1.19



