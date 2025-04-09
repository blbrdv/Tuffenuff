@echo off
setlocal EnableDelayedExpansion

call :setESC

:: set variables
set "exitcode=0"
set "fake=dotnet fake"
set "run=run -p .\build.fsx"

set "args=%*"
set "arg1="
set "arg2="

set "helptest=;-h;--help;--list;"
set "verbosetest=;-v;-vv;-s;-n;"

set "verbose=-s"
set "target="

for /F "tokens=1,3 delims=. " %%a in ("%args%") do (
   set "arg1=%%a"
   set "arg2=%2"
)

if "%arg1%" == "" ( set "arg1=-h" )

echo %ESC%[90m%%arg1%%=%arg1%%ESC%[0m
echo %ESC%[90m%%arg2%%=%arg2%%ESC%[0m
echo.

:: do the things
call :exec "dotnet tool restore"
echo.

if "!helptest:;%arg1%;=!" neq "!helptest!" (
    call :exec "%fake% --silent %run% --list"
    goto :exit
)

if "%arg1%" == "--version" (
    call :exec "%fake% --version"
    goto :exit
)

if "%arg2%" == "" (    
    set "target=%arg1%"
) else (
    if "!verbosetest:;%arg1%;=!" neq "!verbosetest!" (
        set "verbose=%arg1%"
    ) else (
        echo %ESC%[31mUnknown verbose level "%arg1%". Existing levels:%ESC%[0m
        for /F "delims=; tokens=1,2,3,4" %%A in ("%verbosetest%") do ( 
            echo   %ESC%[31m%%A%ESC%[0m
            echo   %ESC%[31m%%B%ESC%[0m
            echo   %ESC%[31m%%C%ESC%[0m
            echo   %ESC%[31m%%D%ESC%[0m
        ) 
        set "exitcode=160"
        goto :exit
    )
    
    set "target=%arg2%"
)

if "%verbose%" == "-n" (
    call :exec "%fake% %run% -t %target%"
) else (
    call :exec "%fake% %verbose% %run% -t %target%"
)

goto :exit

:: define functions
:exec
set "command=%~1"
echo %ESC%[1m^> %command%%ESC%[0m
%command%
if %errorlevel% neq 0 ( 
    set "exitcode=%errorlevel%"
    goto :exit
)
goto :eof

:: https://gist.github.com/mlocati/fdabcaeb8071d5c75a2d51712db24011#file-win10colors-cmd-L60-L65
:setESC
for /F "tokens=1,2 delims=#" %%a in ('"prompt #$H#$E# & echo on & for %%b in (1) do rem"') do (
    set ESC=%%b
)
goto :eof

:: exit with code
:exit
exit %exitcode%
