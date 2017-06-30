%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\installutil.exe HomeCostWindowsService.exe
Net Start HomeCostService
sc config HomeCostService start= auto
pause