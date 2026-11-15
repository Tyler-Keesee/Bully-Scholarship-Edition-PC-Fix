#include <assert.h>
#include <string.h>
#include "CombatBindings.h"
int main() {
 int keys[25]={203,205,200,208,50,1,256,88,57,18,257,260,256,261,19,42,30,17,264,262,32,31,265,263,258};
 int before[25]; memcpy(before,keys,sizeof(keys));
 assert(SeparateCombatBindings(keys)==16);
 assert(keys[6]==256 && keys[12]==16);
 for(int i=0;i<25;++i) if(i!=12) assert(keys[i]==before[i]);
 assert(SeparateCombatBindings(keys)==0);
 keys[12]=256; keys[4]=16; assert(SeparateCombatBindings(keys)==33);
 keys[12]=256; keys[0]=16;keys[1]=33;keys[2]=47;keys[3]=48;
 memcpy(before,keys,sizeof(keys)); assert(SeparateCombatBindings(keys)==-1);
 assert(memcmp(keys,before,sizeof(keys))==0);
 return 0;
}
