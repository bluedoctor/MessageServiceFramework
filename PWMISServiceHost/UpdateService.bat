@echo off
echo ------------------------------
echo PWMIS MSF Service Host Update
echo Power by bluedoctor, 2012.5.22
echo ע�����ļ����뱣��Ϊ ANSI ��ʽ
echo ------------------------------
if "%1"=="" goto err

echo ���������...
:start
ping 127.0.0.1 -n 2 >nul 2>nul
tasklist >tasklist.txt
find /i tasklist.txt "PdfNetEF.MessageServiceHo"
if errorlevel 1 ((del /q tasklist.txt)&(goto end))
if errorlevel 0 ((goto start))
:end 
echo ��������Ѿ��˳���ִ���ļ����Ʋ���...
copy %1%\*.* .\ >CopyedFile.txt
echo �ļ�ȫ���������
call run.bat
goto ok

:err
echo �������������δָ��ԴĿ¼

:ok
exit





