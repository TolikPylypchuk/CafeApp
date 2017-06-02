.\.paket\paket.bootstrapper.exe

IF %ERRORLEVEL% NEQ 0 EXIT /B %ERRORLEVEL%

.\.paket\paket.exe restore

IF %ERRORLEVEL% NEQ 0 EXIT /B %ERRORLEVEL%

.\packages\FAKE\tools\FAKE.exe % --fsiargs build.fsx --log build.log
