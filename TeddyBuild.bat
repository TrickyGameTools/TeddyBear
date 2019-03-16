@echo off

:BEGIN
   if "%TRICKYMSBUILD%"=="" goto DEFINEMSBUILD
   goto BUILDLAUNCHER
   
:DEFINEMSBUILD
   echo:Configuring
   set TRICKYMSBUILD=C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe
   
:BUILDLAUNCHER
   echo Compiling Launcher
   cd TeddyLaunch
   "%TRICKYMSBUILD%" /p:Configuration=Release TeddyBear.sln > ..\ErrorLog\Launch.txt
   if errorlevel 1 goto ERRORLAUNCHER
   cd ..
   goto BUILDWIZARD
   
   
:ERRORLAUNCHER
   cd ..
   cls
   echo:Building the launcher failed!
   echo:
   type ErrorLog\Launch.txt
   goto :FINALE

:BUILDWIZARD
   echo Compiling Wizard
   cd TeddyWizard 
   "%TRICKYMSBUILD%" /p:Configuration=Release TeddyWizard.sln > ..\ErrorLog\Wizard.txt
   if errorlevel 1 goto ERRORWizard
   cd ..                                                                        
   goto BUILDEDITOR

:ERRORWIZARD                                                                    
   cd ..                                                                        
   cls                                                                          
   echo:Building the wizard failed!                                             
   echo:                                                                        
   type ErrorLog\Wizard.txt
   goto :FINALE
   
:BUILDEDITOR
   echo Compiling Editor
   cd TeddyEdit
   "%TRICKYMSBUILD%" /p:Configuration=Release TeddyEdit.sln > ..\ErrorLog\Edit.txt
   if errorlevel 1 goto ERROREDITOR
   cd ..
   goto DORELEASE

:ERROREDITOR
   cd ..
   cls
   echo:Building the editor failed!
   echo:
   type ErrorLog\Edit.txt
   goto FINALE


:DORELEASE
   echo:Creating Release
   echo:Copying Launcher
   copy TeddyLaunch\bin\release\*.exe Release
   echo:Copying Wizard
   copy TeddyWizard\bin\release\*.exe Release
   echo:Copying Editor
   copy TeddyEdit\bin\Windows\x86\Release\*.exe Release
   copy TeddyEdit\bin\Windows\x86\Release\*.dll Release
   copy TeddyEdit\bin\Windows\x86\Release\*.xml Release
   goto FINALE
   
   
   
:FINALE
   echo:
   echo:Have a nice day!
   
   
   
