@echo off
call "C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\VC\Auxiliary\Build\vcvars32.bat" >nul
cl /nologo /LD /O2 /MT /EHsc /W4 Tyler.BullyFix.cpp user32.lib /link /OUT:Tyler.BullyFix.asi
