# browser_extensions
编译调试过程：
1、VS2013导入解决方案
2、修改工程的属性，Build Events，修改Post-build event command line中RegAsm.exe的路径为开发机器的对应路径
3、rebuild工程
4、把bin\Debug下的DiagnoseAssistant.icon文件放到c盘根目录，把mysql.data.dll,itextsharp.dll放到IE浏览器根目录
5、右击工程，debug->start new instance

部署到其他环境：
1、卸载DLL
"C:\Windows\Microsoft.NET\Framework\v4.0.30319\RegAsm.exe" /unregister "c:\DiagnosseAssistant.dll"
2、注册dll
"C:\Windows\Microsoft.NET\Framework\v4.0.30319\RegAsm.exe"  /codebase "c:\DiagnosseAssistant.dll"
3、把DiagnoseAssistant.icon文件放到c盘根目录，把mysql.data.dll,itextsharp.dll放到IE浏览器根目录
4、重启IE浏览器