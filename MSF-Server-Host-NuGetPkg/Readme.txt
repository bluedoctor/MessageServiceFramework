PDF.NET Message Service Framework Service Host NuGet package.

MSF Server Host Ver 1.3.3
https://github.com/bluedoctor/MessageServiceFramework

PDF.NET SOD.
http://www.pwmis.com/sqlmap
https://github.com/znlgis/sod

=============nuget project==================================
1,--PWMIS.EnterpriseFramework--
PWMIS.EnterpriseFramework.IOC
PWMIS.EnterpriseFramework.ModuleRoute


2,--Service Client--
PWMIS.EnterpriseFramework.Service.Client
PWMIS.EnterpriseFramework.Message.SubscriberLib
PWMIS.EnterpriseFramework.Service.Basic
PWMIS.EnterpriseFramework.Common
PWMIS.EnterpriseFramework.Message.PublishService
Newtonsoft.Json

3,--Service Rumtime (include [Serivce Client] & [PWMIS.EnterpriseFramework])--
PWMIS.EnterpriseFramework.Service.Runtime


4,--Service Group (include [Service Runtime] & [Service Client])--
PWMIS.EnterpriseFramework.Service.Group

5,--Service Host (include [Serivce Group])--
PWMIS.EnterpriseFramework.Message.PublisherLib