# Installation and recovery

## Requirements

- The supported Steam PC 1.200 `Bully.exe`. Setup checks its SHA-256 before changing game files:
  `36FF31682081A61BA564CB5E09CD6240F79791879CF1FBFB8E6252F75FADDC6C`.
- An existing working `dinput8.dll` ASI loader in the game directory. Setup checks for the file; its presence alone does not prove that the loader is compatible or working.
- For the FPS selector: an existing `SilentPatchBully.asi` in the game directory.
- For the FOV selector: an existing `plugins/Bully.WidescreenFix.asi`.
- A writable extracted preview folder. Optional full control-layout changes require the supported 236-byte `ControllerSettings` profile in your Documents/Bully Scholarship Edition folder.

Setup does not install or download those dependencies. [Credits and official project links](../THIRD_PARTY_NOTICES.md) identify their authors. Other game builds and mod combinations are not certified.

The Windows application uses .NET Framework 4 and Windows APIs. Broad Windows-version testing remains incomplete.

## First install or upgrade

1. Download the named ZIP asset from the [0.1.1 prerelease](https://github.com/Tyler-Keesee/Bully-Scholarship-Edition-PC-Fix/releases/tag/v0.1.1-preview).
2. Extract everything to a writable folder. Do not run Setup inside the ZIP or copy the EXE by itself.
3. Save and **fully exit Bully**. Setup intentionally does not close the game.
4. Run `RescuePC Bully Setup.exe` and choose the game directory containing `Bully.exe`.
5. Use **Verify package**. Keep the original release files intact; modifying a covered file, including README, causes the package-integrity check to fail.
6. Choose resolution, FOV percentage, and 30/60 FPS. **Use display** selects the monitor containing Setup. FOV is a multiplier percentage, not degrees; 100 preserves the existing widescreen fix's standard correction.
7. Leave the experimental renderer unchecked while troubleshooting flashing. In 0.1.1 this also disables a matching previously installed V1 renderer by removing its ASI after backing it up. A different modified renderer is refused so it can be reviewed separately.
8. Optionally enable **Apply full control layout** to replace the gameplay bindings with the displayed layout. Otherwise, existing controls are retained except an overlapping melee/fire binding, which is repaired.
9. Click **Install / update** and confirm that Setup reports installation succeeded. If it says to close Bully, the operation was refused and the selected layout was not installed.
10. Launch through Steam. The ASI loader loads the installed plugins on subsequent launches.

The setup program is needed for installation, configuration, verification, and undo; it is not a replacement game launcher.

## Combat and menu controls

| Action | Default |
| --- | --- |
| Melee / punch | Left mouse |
| Fire / throw | Q |
| Lock on / social negative | Right mouse |
| Sprint / social positive | Mouse 5 (hold) |
| Jump | Space |
| Grapple / interact | E |
| Crouch | Shift |
| Menu navigation | Arrow keys |
| Menu select / back | Enter / Backspace |
| Pause | Escape; F10 alternate |

For a held throwable, test Q by itself and right mouse + Q. Brick/object behavior still needs gameplay confirmation; separating the bindings does not establish that every throwable interaction works.

Do not bind melee and fire/throw to the same input. When repairing an existing overlap, Setup and the keyboard/mouse runtime preserve melee and select an unused fire key in the order **Q, F, V, B**. If all are occupied, Setup asks for a manually separated layout. Already distinct combat keys are left alone.

Physical F12 remains reserved internally for Mouse 5 sprint/prompt behavior. Do not assign it to another action in this preview. Controller input is left to the original game.

## Display notes

Borderless windowed mode, VSync, and 4x antialiasing are requests. Actual presentation can vary. The existing render V1 can retain vendor-dependent nonmaskable MSAA; explicit 4-sample normalization is unvalidated V2 work.

Setup does not change desktop refresh rate. Multi-monitor and exclusive-fullscreen combinations have not been certified. FPS selection requires SilentPatch; FOV selection requires Widescreen Fix.

White flashes need separate investigation; unchecking the renderer is not proof that all flashing will disappear.

## What changes

Setup can update its listed controls/render plugins and INIs, existing third-party fix INIs, the optional control layout or narrow combat repair, and per-user game graphics registry values. It applies a 4096-byte original custom prompt delta to a hash-verified HUD asset.

Save slots are not replaced. CPU/RAM tuning, voltage, power plans, drivers, desktop refresh rate, and Windows security settings are not changed.

## Backup and undo

Before changing or removing a file, Setup stores its previous contents beside itself under `Backups`, along with a record of graphics values. Keep the preview folder, `LastInstall.txt`, and backups together.

To undo the latest successful installation:

1. Save and fully close Bully.
2. Open the **same extracted Setup folder** used for that installation and select the same game directory.
3. Click **Undo last install**.

Undo refuses if tracked files or graphics values have changed since installation. For a renderer removed by Setup, it also refuses if another file has since been created at that path. This preserves later edits. Keep the backup for manual recovery instead of bypassing the checks. Undo restores the previous state, which may include the previously enabled experimental renderer.

An install failure attempts rollback. If recovery itself fails, Setup reports the retained backup path for inspection; do not treat the installation as successful.

## Common errors

| Message / symptom | What to check |
| --- | --- |
| Save and close Bully first | Fully exit the game before installing. A restart before a successful install still uses the old bindings. |
| Unsupported Steam executable | The executable hash does not match this supported build. Stop and report the version; do not bypass the hash check. |
| ASI loader required | This package does not supply the loader. Check the existing loader and its documentation. |
| HUD asset differs | Another HUD mod or game revision changed the verified target. Use compatible original assets from your own installation. |
| Unsupported control profile | Start the game and save controls first. Preserve your current profile and report its size if it still fails. |
| Package integrity failed | Re-extract the release ZIP; keep all covered files unchanged. |
| Documents write blocked | Inspect Windows Security protection history. If you trust the app, allow that specific application through Controlled Folder Access; do not disable protection globally. |
| Q still does not throw | Confirm installation succeeded and the game restarted; include the actual saved bindings, held object, and installed mods in a bug report. |
