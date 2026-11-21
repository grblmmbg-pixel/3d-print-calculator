@echo off
set CSC=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe
set WPF=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\WPF
set NET=C:\Windows\Microsoft.NET\Framework64\v4.0.30319

echo ==============================================
echo  Compiling 3D Print Calculator (WPF Standalone)
echo ==============================================

"%CSC%" /nologo /target:winexe /codepage:65001 /optimize+ ^
  /win32icon:"%~dp0app.ico" ^
  /r:System.dll ^
  /r:System.Core.dll ^
  /r:"%WPF%\PresentationFramework.dll" ^
  /r:"%WPF%\PresentationCore.dll" ^
  /r:"%WPF%\WindowsBase.dll" ^
  /r:"%NET%\System.Xaml.dll" ^
  /r:"%NET%\System.Web.Extensions.dll" ^
  /out:"%~dp03DPrintCalculator.exe" ^
  "%~dp0Models.cs" ^
  "%~dp0Localization.cs" ^
  "%~dp0DataService.cs" ^
  "%~dp0MainWindow.cs" ^
  "%~dp0App.cs"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo [SUCCESS] 3DPrintCalculator.exe built successfully!
) else (
    echo.
    echo [ERROR] Build failed!
)
pause
