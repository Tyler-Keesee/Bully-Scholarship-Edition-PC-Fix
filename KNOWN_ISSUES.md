# Known issues — Preview 0.1.1

This preview has incomplete gameplay and compatibility coverage. The items below are deliberately kept visible.

| Area | Status |
| --- | --- |
| Bricks and other throwable objects | The confirmed shared melee/fire binding is corrected in 0.1.1. Actual throwing after a successful install/restart still needs confirmation. Do not describe all throwables as fixed. |
| Intermittent white flashes / skin flashing | Still reported. A matching previously installed experimental renderer can now be disabled by leaving its checkbox off and successfully installing. Flashing may have other causes. |
| Keyboard movement | Feel and behavior still need work. |
| NPC animation / chase behavior | Unresolved. |
| Simulation and animation at higher FPS | Not certified. Setup offers 30 or 60 FPS only. |
| Experimental rendering | Optional, off by default, and not recommended while diagnosing flashing. No broad rendering correctness claim is made. |
| Render source / binary parity | The included render binary is observed V1. Archived render source is later V2 development and does not reproduce that binary. |
| Compatibility | Only one Steam 1.200 executable hash is supported. Existing loaders, other mods, controllers, displays, and Windows versions need broader testing. |
| Crash freedom / long sessions | Not established by this preview or the short presentation sample. |
| In-game branding | No replacement for SilentPatch's in-game build label is implemented. Third-party credits must remain intact. |

## Corrected configuration defects in 0.1.1

- Preview 0.1 and the observed saved profile bound both melee and fire/throw to left click.
- Preview 0.1 left an already installed experimental renderer active when its checkbox was unchecked.
- An install refused because Bully was running did not apply the selected layout. The new documentation makes the success/error distinction explicit.

## Testing still needed

Test a successful install followed by a complete game restart, then verify melee, Q throwing, aimed throwing, projectile weapons, social interactions, sprint, menu navigation, and normal mission progress. Compare flashing with the experimental renderer actually absent. Also test clean supported installations, other displays, and install/undo with later user edits.

Please use the [bug-report form](https://github.com/Tyler-Keesee/Bully-Scholarship-Edition-PC-Fix/issues/new/choose) and describe what was observed, rather than inferring success from a checkbox or FPS counter.
