# Source and build notes

## Scope

The setup and controls sources correspond to Preview 0.1.1. The original custom artwork is included. Prebuilt Windows downloads are available from [Releases](https://github.com/Tyler-Keesee/Bully-Scholarship-Edition-PC-Fix/releases).

**Render limitation:** `Source/Tyler.BullyRender.cpp` is archived V2 development source. The optional release render binary remains V1. Building this source does not reproduce that binary, and the V2 result is not validated for release.

## Tools

- Visual Studio 2019 x86 C++ build tools and Windows SDK.
- .NET Framework 4 C# compiler.
- Windows for the installer/self-tests.

The archived command files reference the original Visual Studio 2019 installation path. Adjust the toolchain path for your machine. This project does not claim a fully reproducible release pipeline.

## Controls and regression test

From an x86 Visual Studio developer command prompt in `Source`:

```bat
cl /nologo /LD /O2 /MT /EHsc /W4 Tyler.BullyFix.cpp user32.lib /link /OUT:Tyler.BullyFix.asi
cl /nologo /O2 /MT /EHsc /W4 test-combat-bindings.cpp /Fe:test-combat-bindings.exe
test-combat-bindings.exe
```

Keep `CombatBindings.h` beside the source. The test covers separating duplicate keys, preserving the melee key and every unrelated action, avoiding occupied fallbacks, and leaving the layout unchanged if no fallback is available.

The package's `Source/build-and-test.cmd` builds the controls ASI into the sibling `Payload` directory and runs this test.

## Setup

From the project/package root, with the .NET Framework compiler on PATH:

```bat
csc /nologo /target:winexe /optimize+ /reference:System.Drawing.dll /reference:System.Windows.Forms.dll /reference:System.Web.Extensions.dll /win32icon:Source\favicon.ico /out:"RescuePC Bully Setup.exe" Source\RescueBullySetup.cs
```

The original transparent icon is retained. A self-built EXE is unsigned unless separately signed by its distributor.

## Package assembly

A working package requires the complete payload, original artwork delta, license, README, source, evidence, and a matching `payload-sha256.json` file. A GitHub source-code ZIP is not an installable release.

Regenerate hashes for every covered file after a change. Then run:

```bat
"RescuePC Bully Setup.exe" --verify
```

This writes `Verification.txt` and returns a nonzero status on failure. It checks package integrity, prompt data, control profile preservation/repair, and file-operation rollback, including renderer removal. It does not install into the game, edit game graphics settings, launch gameplay, or establish visual correctness.

Do not distribute original game assets, local backups, private saves, generated compiler intermediates, or unrelated third-party runtime dependencies. See [verification evidence](VERIFICATION.md) for the actual release checks and remaining gaps.
