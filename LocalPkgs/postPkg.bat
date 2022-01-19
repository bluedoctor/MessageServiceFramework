@echo off 
echo 请用管理员权限运行本批处理程序。

if not "%OS%"=="Windows_NT" exit
title WindosActive

echo 当前目录：%cd%
REM 变量扩充: %~dp0
cd /D %~dp0
echo 批处理文件目录：%cd%
echo *********************************************
echo ***  MSF 本地程序包 发布程序              ***
echo ***按任意键开始复制，否则请关闭窗口。***
echo **********************************************
pause

copy .\*.nupkg "C:\Program Files (x86)\Microsoft SDKs\NuGetPackages"
pause

