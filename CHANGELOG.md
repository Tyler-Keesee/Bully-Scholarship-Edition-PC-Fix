# Changelog

## 0.1.1 preview — 2026-11-15

### Controls

- Changed default fire/throw to Q while keeping melee on left click.
- Reject a full layout with overlapping melee/fire inputs.
- Repair an existing overlap without replacing other bindings when the full layout is off. Keep melee and choose an unused fire key from Q, F, V, B.
- Add the same narrow separation to the keyboard/mouse runtime.
- Add regression checks for overlap repair, unchanged unrelated bindings/profile bytes, already distinct keys, occupied fallbacks, and no available fallback.

### Setup and rendering

- Leaving the experimental renderer unchecked now backs up and removes a matching previously installed V1 ASI.
- Refuse automatic removal of a different renderer binary.
- Extend undo checks to removed files and preserve later replacements.
- Add renderer-removal/undo regression checks and report renderer state after installation.

### Project

- Add installation, recovery, known-issue, verification, build, attribution, and contribution documentation.
- Expose the packaged source and original artwork for review.
- Provide a clearly marked prerelease and package checksums.

Gameplay confirmation of throwing and intermittent flashing remains pending.

## 0.1 preview — 2026-11-15

Initial public package: Windows setup, custom menu/sprint controls and prompt, optional layout, display configuration, existing SilentPatch/Widescreen Fix configuration, optional experimental renderer, backups, and undo. Known gameplay and visual problems remained.
