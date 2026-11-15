#pragma once
// Native action indices: melee=6, fire/throw=12. Separate overlapping
// bindings without changing the melee key or another assigned action.
inline int SeparateCombatBindings(int* keys) {
 if (keys[6] != keys[12]) return 0;
 const int candidates[] = {16, 33, 47, 48}; // Q, F, V, B (DirectInput)
 for (int candidate : candidates) {
  bool used = false;
  for (int i = 0; i < 25; ++i) if (keys[i] == candidate) used = true;
  if (!used) { keys[12] = candidate; return candidate; }
 }
 return -1;
}
