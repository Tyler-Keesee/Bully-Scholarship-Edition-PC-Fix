@echo off
setlocal
pushd "%~dp0"
call "C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\VC\Auxiliary\Build\vcvars32.bat"
if errorlevel 1 exit /b 1
cl /nologo /LD /O2 /MT /EHsc /W4 Tyler.BullyFix.cpp user32.lib /link /OUT:..\Payload\Tyler.BullyFix.asi
if errorlevel 1 exit /b 1
cl /nologo /O2 /MT /EHsc /W4 test-combat-bindings.cpp /Fe:test-combat-bindings.exe
if errorlevel 1 exit /b 1
test-combat-bindings.exe
if errorlevel 1 exit /b 1
echo Combat binding tests passed.
popd
