@echo off 
echo ���ù���ԱȨ�����б����������

if not "%OS%"=="Windows_NT" exit
title WindosActive

echo ��ǰĿ¼��%cd%
REM ��������: %~dp0
cd /D %~dp0
echo �������ļ�Ŀ¼��%cd%
echo *********************************************
echo ***  MSF ���س���� ��������              ***
echo ***���������ʼ���ƣ�������رմ��ڡ�***
echo **********************************************
pause

copy .\*.nupkg "C:\Program Files (x86)\Microsoft SDKs\NuGetPackages"
pause

