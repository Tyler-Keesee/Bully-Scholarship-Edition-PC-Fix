#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <stdint.h>
#include <stdio.h>
#include <string.h>
#include "CombatBindings.h"
static uintptr_t base; static LONG started; static FILE* logFile;
static DWORD pulseStart; static bool sprintHeld; static int sprintMouse=5;
static char (__stdcall *oa)(int),(__stdcall *oah)(int),(__stdcall *ob)(int);
static void (__cdecl *oh)(const char*);
static char (__thiscall *op)(void*);
static uintptr_t A(uintptr_t a){return base+a-0x400000;}
static void Log(const char*s){if(logFile){fprintf(logFile,"%s\n",s);fflush(logFile);}}
static char mouseLabel[]="Mouse 5"; static void Label(){uintptr_t*slot=(uintptr_t*)A(0xBC9158);if(*slot==A(0x989708)){DWORD p;if(VirtualProtect(slot,4,PAGE_READWRITE,&p)){*slot=(uintptr_t)mouseLabel;VirtualProtect(slot,4,p,&p);}}}
static int* Keys(){typedef int*(__fastcall*F)(void*,void*,int);return ((F)A(0x454CD0))((void*)A(0xA14810),NULL,0);}
// Native input layout and signatures verified against Bully 1.200 before patching.
struct MenuKeys{
 int* k; int old[16];
 MenuKeys():k(NULL){if(*(int*)A(0x20CECE8)!=0)return;k=Keys();memcpy(old,k,sizeof(old));k[0]=0xCB;k[1]=0xCD;k[2]=0xC8;k[3]=0xD0;k[7]=k[12]=0x1C;k[8]=k[10]=0x0E;}
 ~MenuKeys(){if(k)memcpy(k,old,sizeof(old));}
};
static char __stdcall Accept(int v){MenuKeys m;return oa(v);}
static char __stdcall AcceptHeld(int v){MenuKeys m;return oah(v);}
static char __stdcall Back(int v){MenuKeys m;return ob(v);}
static void __cdecl Hints(const char*s){MenuKeys m;oh(s);}
static bool Down(BYTE*c,int key){if(key>=0&&key<256)return(c[516+key]&0x80)!=0;if(key>=256&&key<260)return c[512+key-256]!=0;return false;}
static void Set(BYTE*c,int key,bool down){if(key>=0&&key<256)c[516+key]=down?0x80:0;else if(key>=256&&key<260)c[512+key-256]=down?0x80:0;}
static BYTE* windowedFlag; static bool useBorderless=true; static DWORD nextWindowCheck;
static int captureSeconds=0; static double frameSamples[8192];static int frameCount;static double capturedMs;static LARGE_INTEGER qpcLast,qpcFreq;static bool captureDone;static char timingPath[MAX_PATH];
static void Borderless(HWND window){
 if(!useBorderless||!windowedFlag||!*windowedFlag)return;DWORD now=GetTickCount();if(now<nextWindowCheck)return;nextWindowCheck=now+1000;
 MONITORINFO monitor={sizeof(monitor)};if(!GetMonitorInfoA(MonitorFromWindow(window,MONITOR_DEFAULTTOPRIMARY),&monitor))return;
 RECT r;if(!GetWindowRect(window,&r))return;LONG style=GetWindowLongA(window,GWL_STYLE);RECT m=monitor.rcMonitor;
 if((style&WS_OVERLAPPEDWINDOW)||r.left!=m.left||r.top!=m.top||r.right!=m.right||r.bottom!=m.bottom){
 SetWindowLongA(window,GWL_STYLE,(style&~WS_OVERLAPPEDWINDOW)|WS_POPUP);
 SetWindowPos(window,NULL,m.left,m.top,m.right-m.left,m.bottom-m.top,SWP_NOZORDER|SWP_NOACTIVATE|SWP_FRAMECHANGED|SWP_NOSENDCHANGING);}
}
static void Timing(){
 if(captureDone||captureSeconds<=0)return;LARGE_INTEGER now;QueryPerformanceCounter(&now);if(!qpcFreq.QuadPart)QueryPerformanceFrequency(&qpcFreq);
 if(qpcLast.QuadPart){double ms=(now.QuadPart-qpcLast.QuadPart)*1000.0/qpcFreq.QuadPart;if(ms>=0.5&&ms<=1000){frameSamples[frameCount++]=ms;capturedMs+=ms;}}
 qpcLast=now;
 if(capturedMs>=captureSeconds*1000.0||frameCount>=8192){FILE*f=NULL;fopen_s(&f,timingPath,"w");if(f){fprintf(f,"Frame,NativeUpdateMs\n");for(int i=0;i<frameCount;i++)fprintf(f,"%d,%.6f\n",i+1,frameSamples[i]);fclose(f);}captureDone=true;}
}
static char __fastcall Poll(BYTE*c,void*){
 char r=op(c);if(c!=(BYTE*)A(0x20CECF0)||*c)return r;
 HWND w=*(HWND*)A(0xBD77FC);if(!w||GetForegroundWindow()!=w){sprintHeld=false;return r;}
 Borderless(w);int*k=Keys();if(c[516+0x44]&0x80){Set(c,k[5],true);r=1;}
 if(*(BYTE*)A(0xC1A99A)){sprintHeld=false;qpcLast.QuadPart=0;return r;}
 // Melee and weapon fire must remain separate; otherwise melee consumes
 // the shared input even while carrying a throwable. Only repair keyboard mode.
 if(*(int*)A(0x20CECE8)==0) SeparateCombatBindings(k);
 Timing();
 bool held=(GetAsyncKeyState(sprintMouse==4?VK_XBUTTON1:VK_XBUTTON2)&0x8000)!=0 || (k[7]!=0x58 && Down(c,k[7])); if(held){DWORD now=GetTickCount();if(!sprintHeld){pulseStart=now;sprintHeld=true;}bool moving=Down(c,k[16])||Down(c,k[17])||Down(c,k[20])||Down(c,k[21]);bool pulse=moving&&!Down(c,k[10]);Set(c,k[7],!pulse||((now-pulseStart)%140)<70);r=1;}else {sprintHeld=false;if(k[7]==0x58)Set(c,k[7],false);}
 return r;
}
static bool Match(uintptr_t a,const BYTE*b,SIZE_T n){BYTE*p=(BYTE*)A(a);if(memcmp(p,b,n)==0)return true;if(p[0]!=0xE9)return false;HMODULE owner=NULL;BYTE*target=p+5+*(int32_t*)(p+1);if(!GetModuleHandleExA(GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS|GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT,(LPCSTR)target,&owner))return false;return owner==GetModuleHandleA("Bully.ModernControls.asi")||owner==GetModuleHandleA("Bully.CommunityControls.asi");}
static void* Hook(uintptr_t a,void*t,SIZE_T n){
 BYTE*p=(BYTE*)A(a),*q=(BYTE*)VirtualAlloc(NULL,n+5,MEM_COMMIT|MEM_RESERVE,PAGE_EXECUTE_READWRITE);if(!q)return NULL;
 if(p[0]==0xE9){q[0]=0xE9;*(int32_t*)(q+1)=(int32_t)((p+5+*(int32_t*)(p+1))-(q+5));for(SIZE_T i=5;i<n;++i)q[i]=0x90;}else memcpy(q,p,n);q[n]=0xE9;*(int32_t*)(q+n+1)=(int32_t)((p+n)-(q+n+5));DWORD x;
 if(!VirtualProtect(p,n,PAGE_EXECUTE_READWRITE,&x))return NULL;
 p[0]=0xE9;*(int32_t*)(p+1)=(int32_t)((BYTE*)t-p-5);for(SIZE_T i=5;i<n;++i)p[i]=0x90;
 VirtualProtect(p,n,x,&x);FlushInstructionCache(GetCurrentProcess(),p,n);return q;
}
static DWORD WINAPI Install(void*mod){
 char path[MAX_PATH];GetModuleFileNameA((HMODULE)mod,path,MAX_PATH);char*dot=strrchr(path,'.');if(dot)strcpy_s(dot,MAX_PATH-(dot-path),".log");fopen_s(&logFile,path,"w");char ini[MAX_PATH];strcpy_s(ini,path);char*ext=strrchr(ini,'.');if(ext)strcpy_s(ext,MAX_PATH-(ext-ini),".ini");sprintMouse=GetPrivateProfileIntA("Controls","SprintMouseButton",5,ini);useBorderless=GetPrivateProfileIntA("Display","Borderless",1,ini)!=0;captureSeconds=GetPrivateProfileIntA("Diagnostics","FrameCaptureSeconds",0,ini);strcpy_s(timingPath,path);char*te=strrchr(timingPath,'.');if(te)strcpy_s(te,MAX_PATH-(te-timingPath),".frame-times.csv");base=(uintptr_t)GetModuleHandleA(NULL);
 const BYTE m[]={0x56,0x8B,0x74,0x24,0x08},p[]={0x83,0xEC,0x24,0x53,0x55},h[]={0x51,0xD9,0x05},k[]={0x80,0xB9,0x08,0x01,0,0,0};
 int n=0;while(n++<600&&!Match(0x50E180,m,5))Sleep(100);
 if(!Match(0x50E180,m,5)||!Match(0x50E1C0,m,5)||!Match(0x50E200,m,5)||!Match(0x50CD30,p,5)||!Match(0x6A6510,h,3)||!Match(0x454CD0,k,7)){Log("Unsupported game code; no patches applied.");return 0;}
 for(uintptr_t i=A(0x401000);i<A(0x900000)-13;i++){BYTE*p=(BYTE*)i;if(p[0]==0x8B&&p[1]==0x0D&&p[6]==0x89&&p[7]==0x48&&p[8]==0x14&&p[9]==0xC3&&p[10]==0xCC&&p[11]==0xCC&&p[12]==0xCC){windowedFlag=*(BYTE**)(p+2);break;}}Label();oa=(decltype(oa))Hook(0x50E180,(void*)Accept,5);oah=(decltype(oah))Hook(0x50E1C0,(void*)AcceptHeld,5);ob=(decltype(ob))Hook(0x50E200,(void*)Back,5);oh=(decltype(oh))Hook(0x6A6510,(void*)Hints,7);op=(decltype(op))Hook(0x50CD30,(void*)Poll,5);
 if(oa&&oah&&ob&&oh&&op)Log("Active: arrows / Enter / Backspace menus; independent gameplay keys; Mouse 5 hold-to-sprint; accurate labels; separate melee / fire bindings; social actions do not repeat; F10 alternate pause.");else Log("Installation failed; close game and disable plugin.");if(logFile){fclose(logFile);logFile=NULL;}return 0;
}
extern "C" __declspec(dllexport) void InitializeASI(){}
BOOL WINAPI DllMain(HINSTANCE h,DWORD reason,LPVOID){if(reason==DLL_PROCESS_ATTACH&&!InterlockedExchange(&started,1)){DisableThreadLibraryCalls(h);HANDLE t=CreateThread(NULL,0,Install,h,0,NULL);if(t)CloseHandle(t);}return TRUE;}



