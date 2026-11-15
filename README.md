# Bully: Scholarship Edition — PC Community Preview

**RescuePC · Preview 0.1.1 · Windows · Steam PC 1.200**

A community tool for configuring keyboard/mouse controls and display settings in Bully: Scholarship Edition, with a Windows setup app, backups, and undo.

**This is a work in progress.** Known gameplay and visual problems remain. It is not a universal crash fix, a remaster, or an official Rockstar release.

[**Download Preview 0.1.1**](https://github.com/Tyler-Keesee/Bully-Scholarship-Edition-PC-Fix/releases/tag/v0.1.1-preview) · [Installation](docs/INSTALLATION.md) · [Known issues](KNOWN_ISSUES.md) · [Report a problem](https://github.com/Tyler-Keesee/Bully-Scholarship-Edition-PC-Fix/issues/new/choose)

## What changed in 0.1.1

- Default combat controls are **left click for melee** and **Q for fire/throw**. The previous preview assigned both actions to left click.
- Setup rejects a full layout with overlapping melee/fire keys. With the full-layout option off, it repairs only that overlap, using an unused key starting with Q, then F, V, or B.
- The controls module also separates overlapping combat keys during keyboard/mouse gameplay.
- Leaving the experimental renderer unchecked now removes the matching previously installed renderer, with backup and undo support. Previously, unchecking it did not disable an older installation.

These corrections address confirmed configuration defects. Brick-throwing gameplay and intermittent white flashes still need confirmation on the corrected installation; they are not advertised as resolved.

## What it does

| Area | Preview behavior |
| --- | --- |
| Menus | Arrow keys to navigate, Enter to select, Backspace to go back; F10 is an alternate pause key. |
| Gameplay | Mouse 5 hold-to-sprint, a matching custom prompt, and an optional editable control layout. |
| Combat | Separate melee and fire/throw bindings; default left click melee and Q fire/throw. |
| Display | Select resolution; request borderless windowed mode, VSync, and antialiasing. |
| Existing fixes | Configure FPS in an existing SilentPatch installation and FOV in an existing Widescreen Fix installation. |
| Recovery | Back up changed files and graphics settings; undo the latest install while preserving detected later edits. |

## Before downloading

- You need a legally installed copy of the **supported Steam PC 1.200 executable** and an **already working dinput8 ASI loader**.
- SilentPatch and Bully Widescreen Fix are separate projects. They are **not bundled**. The FPS/FOV controls need their respective existing plugins.
- Testing has focused on keyboard/mouse and one existing mod setup. Other executable versions, Windows/display combinations, and controller behavior have not been certified.
- The setup EXE is **unsigned**. Windows may show an unknown-publisher warning. Review the files and published checksums before deciding whether to run it.
- The optional renderer remains experimental. **Leave it off while investigating flashing.**

See [compatibility and dependencies](docs/INSTALLATION.md#requirements) and [third-party credits](THIRD_PARTY_NOTICES.md).

## Install

1. Download `Bully-Community-Preview-0.1.1.zip` from the [release page](https://github.com/Tyler-Keesee/Bully-Scholarship-Edition-PC-Fix/releases/tag/v0.1.1-preview). Use the named asset, not GitHub's automatic source-code ZIP.
2. Extract the complete folder to a writable location you intend to keep. Backups are stored beside Setup; choose a drive with suitable free space.
3. **Save and fully close Bully.** An installation refused while the game is running has not applied your selected controls.
4. Open **RescuePC Bully Setup.exe**, choose the game folder, and click **Verify package**.
5. Choose your display settings. Leave the experimental renderer unchecked. Enable **Apply full control layout** only if you want the complete displayed layout; duplicate melee/fire bindings are repaired even with this option off.
6. Click **Install / update** and check for a successful installation message. Then launch normally through Steam.

Keep the extracted folder and its `Backups` directory. [Detailed instructions, combat keys, and undo](docs/INSTALLATION.md) cover upgrades and common errors.

## Current limits

Throwables still need gameplay retesting after the binding correction. Keyboard movement feel, NPC animation/chase problems, and intermittent skin/screen flashing remain under investigation. The optional renderer's archived V2 source does not reproduce the bundled V1 binary.

Only 30/60 FPS are offered. A short timing sample is available, but it does not establish long-session stability, correct simulation timing, or universal smoothness. See [known issues](KNOWN_ISSUES.md) and [verification evidence](docs/VERIFICATION.md).

## Source and contributions

The setup and controls source are browsable in [Source](Source). Original artwork and a limited presentation sample are included. See [building](docs/BUILDING.md) for toolchain requirements and the render-source limitation.

Bug reports with the exact package version, controls, installed mods, and reproduction steps are useful. Please omit private saves, full game files, and personal information. See [CONTRIBUTING.md](CONTRIBUTING.md).

## Credits and license

Original custom code and artwork: RescuePC contributors. [License](LICENSE.txt).

[SilentPatch](https://github.com/CookiePLMonster/SilentPatch) and [Widescreen Fixes Pack](https://github.com/ThirteenAG/WidescreenFixesPack) belong to their respective authors. Their work and licenses remain separate. Bully and its game assets belong to their respective rights holders. No affiliation with Rockstar Games is implied.

RescuePC: [rescuepcrepairs.com](https://rescuepcrepairs.com).
