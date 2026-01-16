# CompilerOptions/fsi Migration Notes

## Migrated Tests
- **langversion** (3 tests) - Migrated to `CompilerOptions/fsi/Langversion.fs`
- **nologo** (2 tests) - Migrated to `CompilerOptions/fsi/Nologo.fs`

## Migration Blockers

The following test folders could NOT be migrated due to infrastructure limitations:

### help (4 tests) - DesktopOnly
Tests `-?`, `--help`, `/?` FSI help options. Original tests compare full help output to baseline files.
**Blocker**: Help options cause `StopProcessingExn` which crashes `FsiEvaluationSession.Create`.

### highentropyva (1 test) - DesktopOnly  
Tests that `--highentropyva+` is rejected by FSI.
**Blocker**: Unrecognized options cause `StopProcessingExn` before session creation completes.

### subsystemversion (1 test) - DesktopOnly
Tests that `--subsystemversion:4.00` is rejected by FSI.
**Blocker**: Same as highentropyva - session fails before output can be captured.

## Technical Details
The `runFsi` function uses `FsiEvaluationSession.Create` which throws `StopProcessingExn` when:
- An unrecognized option is passed
- A help option (`-?`, `--help`, `/?`) is passed

These scenarios require running the actual FSI executable and capturing stdout/stderr, which is not currently supported by the component test infrastructure.
