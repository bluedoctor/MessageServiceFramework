/*
 * 在 .nuproj 文件中，修改下面的内容
<PropertyGroup>
<NuProjPath Condition=" '$(NuProjPath)' == '' ">$(MSBuildExtensionsPath)\NuProj\</NuProjPath>
</PropertyGroup>   
为下面的内容：    
<PropertyGroup>
<NuProjPath>..\packages\NuProj.0.11.30\tools\</NuProjPath>
</PropertyGroup>

参考：https://docs.microsoft.com/zh-cn/nuget/create-packages/symbol-packages
      https://www.add-in-express.com/forum/read.php?FID=5&TID=15072
      https://github.com/nuproj/nuproj/issues/301
      https://github.com/nuproj/nuproj/blob/master/docs/Build.md

tools version 参考：
      https://stackoverflow.com/questions/61207245/toolsversion-in-visual-studio-2019
MSBuild 保留属性和已知属性：
https://docs.microsoft.com/zh-cn/visualstudio/msbuild/msbuild-reserved-and-well-known-properties?view=vs-2022
https://social.msdn.microsoft.com/forums/vstudio/en-US/a9df6d28-a6b2-416a-801a-be33c4e0b456/location-of-msbuildextensionspath32
使用 nuget.exe CLI 创建包：
https://docs.microsoft.com/zh-cn/nuget/create-packages/creating-a-package

 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSF_NuProj_Pkg
{
    class Program
    {
        static void Main(string[] args)
        {
            string projectConfiguration = DebuggingProperties.IsDebug ? "Debug" : "Release";
            string blog = "http://www.cnblogs.com/bluedoctor";
            Console.WriteLine("此项目是为了解决NuPrj问题而创建，详细请看我博客文章：\r\n {0}",blog);
            Console.WriteLine("当前程序运行模式：{0}", projectConfiguration);
            Console.WriteLine("需要复制解决方案 NugetProject 项目生产的程序包到 解决方案文件夹 LocalPkgs，请按1，其他输入则退出。");
            string input = Console.ReadLine();
            if (input != "1")
                return;

            string[] nugetProjects = { 
                "MSF-Client-NuGetPkg", 
                "MSF-Server-Group-NuGetPkg",
                "MSF-Server-Host-NuGetPkg",
                "MSF-Server-NuGetPkg",
                "PWMIS.EnterpriseFramework-NugetPkg"
            };

            string nugetPrjBasePath = System.Configuration.ConfigurationManager.AppSettings["NugetProjectBasePath"];

            foreach (string proj in nugetProjects)
            {
                string pkgFilePath = nugetPrjBasePath+ proj +"\\bin\\"+projectConfiguration+"\\";
                foreach (string pkgFile in System.IO.Directory.GetFiles(pkgFilePath, "*.nupkg"))
                {
                    string objFileName = nugetPrjBasePath+ "\\LocalPkgs\\" + System.IO.Path.GetFileName(pkgFile);
                    System.IO.File.Copy(pkgFile, objFileName,true);
                    Console.WriteLine("copy file: {0}",pkgFile);
                }
            }
            Console.WriteLine();
            Console.WriteLine("All nuget package file in [{0}\\LocalPkgs ].\r\nAfter you can execute postPkg.bat to make you Local NuGetPackages .", nugetPrjBasePath);
            Console.WriteLine("Press [Enter] exit.");
            Console.ReadLine();

        }
    }
}
