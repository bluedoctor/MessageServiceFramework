'const vbOKOnly =0
'const vbOKCancel=1
'const vbOK =1
'const vbCancel=2

if vbOK = MsgBox ("���� PWMIS ����������",vbOKCancel,"��Ϣ������")  then
  Set objShell = CreateObject("Wscript.Shell")
  objShell.Run("PdfNetEF.MessageServiceHost.exe")
end if





