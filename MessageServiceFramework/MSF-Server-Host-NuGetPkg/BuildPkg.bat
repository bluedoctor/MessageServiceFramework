@echo off
echo ��ʼ֮ǰ�����ȼ�� obj\Debug\PDF.Net.MSF.Client.nuspec �ļ��Ƿ����ɣ������Ƴ��˴�� JSON �ļ�
pause
"C:\Program Files (x86)\MSBuild\NuProj\nuget.exe" pack PDF.Net.MSF.Service.Host.nuspec
copy *.nupkg ..\LocalPkgs\

echo �ļ������ɺ͸������
pause
