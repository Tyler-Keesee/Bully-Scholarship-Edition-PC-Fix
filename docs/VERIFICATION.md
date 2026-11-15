# Verification and evidence

## Preview 0.1.1

The controls ASI and Windows setup app were rebuilt from the published 0.1.1 sources. Native combat-binding regression tests passed.

Package verification checks:

- All covered files match their recorded SHA-256 values.
- The original prompt delta matches its expected hash.
- Full control-profile replacement preserves unrelated header/tail bytes.
- Duplicate combat repair changes only the four-byte fire/throw slot.
- Already distinct combat keys are retained.
- Occupied fallback keys are not reused; exhaustion is rejected.
- File replacement/creation can be rolled back.
- Experimental renderer removal can be undone, with later replacements protected.

The packaged `Verification.txt` records the self-test result. The release provides a whole-ZIP checksum alongside its internal `payload-sha256.json`.

**Gameplay limits:** These checks do not prove that bricks or all other objects throw correctly, that white flashing is resolved, or that extended gameplay is stable. Those observations are still needed after a successful install and restart.

## Original presentation sample

[JSON summary](../Evidence/presentation-timing-before-explicit-msaa.json) and [CSV intervals](../Evidence/presentation-timing-before-explicit-msaa.csv) preserve a short sample from the development system.

| Measurement | Observed |
| --- | ---: |
| Samples | 1,195 |
| Average FPS | 59.72 |
| Median interval | 16.6677 ms |
| 95th percentile | 17.1695 ms |
| 99th percentile | 19.2408 ms |
| Intervals over 25 ms | 7 |
| Intervals over 50 ms | 0 |

This roughly 20-second presentation sample predates the combat correction. It is not a benchmark of 0.1.1, a simulation/animation test, or a crash-free guarantee. It also does not prove that the unvalidated V2 renderer is correct.

## Additional coverage needed

Clean supported installations; alternate loaders/mod combinations; controller input; different Windows/display setups; gameplay with several throwables and weapons; repeated install/undo after user edits; mission/minigame regressions; and longer sessions.
