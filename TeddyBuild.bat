@echo off

:BEGIN
   if "%TRICKYMSBUILD%"=="" goto DEFINEMSBUILD
   goto BUILDLAUNCHER
   
:DEFINEMSBUILD
   set TRICKYMSBUILD="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe"
   
:BUILDLAUNCHER
   echo 
   cd TeddyLaunch
   %TRICKYMSBUILD% /p:Configuration=Release TeddyBear.sln > ..\ErrorLog\Launch.txt
   if errorlevel 1 goto ERRORLAUNCHER
   cd ..
   goto BUILDEDITOR
   
   
:ERRORLAUNCHER
   cd ..
   cls
   echo:Building the launcher failed!
   echo:
   type ErrorLog\Launch.txt
   goto :FINALE
   
:BUILDEDITOR
   cd TeddyEdit
   %TRICKYMSBUILD% /p:Configuration=Release TeddyEdit.sln > ..\ErrorLog\Edit.txt
   if errorlevel 1 goto ERROREDITOR
   cd ..
   goto FINALE
   
   
:ERROREDITOR
   cd ..
   cls
   echo:Building the editor failed!
   echo:
   type ErrorLog\Edit.txt
   goto :FINALE
   
:FINALE
   echo:
   echo:Have a nice day!
   
