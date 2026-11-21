$ErrorActionPreference = "Stop"

$dir = "C:\Users\workshop\.gemini\antigravity\scratch\3DPrintCalculator"

$csc = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
$wpf = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\WPF"
$net = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319"

Write-Host "Compiling 3DPrintCalculator.exe..." -ForegroundColor Cyan

$sources = @(
    "$dir\Models.cs",
    "$dir\Localization.cs",
    "$dir\DataService.cs",
    "$dir\MainWindow.cs",
    "$dir\App.cs"
)

$out = "$dir\3DPrintCalculator.exe"

$compilerArgs = @(
    "/nologo",
    "/target:winexe",
    "/codepage:65001",
    "/optimize+",
    "/win32icon:$dir\app.ico",
    "/r:System.dll",
    "/r:System.Core.dll",
    "/r:$wpf\PresentationFramework.dll",
    "/r:$wpf\PresentationCore.dll",
    "/r:$wpf\WindowsBase.dll",
    "/r:$net\System.Xaml.dll",
    "/r:$net\System.Web.Extensions.dll",
    "/out:$out"
) + $sources

& $csc $compilerArgs

if ($LASTEXITCODE -eq 0) {
    Write-Host "[SUCCESS] 3DPrintCalculator.exe created successfully!" -ForegroundColor Green
    Get-Item $out | Select-Object Name, Length, LastWriteTime
} else {
    Write-Host "[ERROR] Compilation failed." -ForegroundColor Red
}
