# Product Increments

This file is updated after each sprint completes. Use it to understand what was delivered.

---

## Sprint 1: Setup perf test folder

**Summary:** Completed in 5 iterations

**Files touched:** Check git log for details.

---

## Sprint 2: Build baseline compiler

**Summary:** Built baseline F# compiler from origin/main

**Details:**
- Baseline commit hash: `def2b8239e52583fd714992e3c8e4c50813717df`
- Artifacts folder cleaned completely (3.97 GB removed)
- Build.cmd -c Release completed successfully (0 errors, 0 warnings, Time Elapsed 00:03:50.35)
- Baseline fsc.dll path: `Q:\source\fsharp\fsharp\artifacts\bin\fsc\Release\net10.0\fsc.dll`
- Baseline fsc.dll size: 47,616 bytes
- Build timestamp: 2026-01-22 10:20:26 AM

**Verification:**
- [x] fsharp repo is at origin/main commit
- [x] Build.cmd -c Release completes successfully  
- [x] artifacts\bin\fsc\Release\net10.0\fsc.dll exists
- [x] Baseline commit hash documented

---

## Sprint 3: Measure baseline perf

**Summary:** Configured test project to use baseline fsc.dll, ran 5 compilation measurements

**Details:**
- Test project configured with `DotnetFscCompilerPath` pointing to baseline fsc.dll
- Created `run-baseline-perf.ps1` script for automated performance measurement
- Build log verification confirms baseline compiler is used (not SDK compiler)
- 5 compilation runs completed with --times output captured

**Key Results:**
| Phase | Mean (s) | Std Dev (s) |
|-------|----------|-------------|
| ImportMscorlib+FSharp.Core | 0.7939 | 0.0122 |
| ParseInputs | 0.2441 | 0.0034 |
| Import non-system refs | 0.0739 | 0.0032 |
| **Typecheck** | **2.5785** | **0.1723** |
| Optimizations | 0.142 | 0.0123 |
| TAST -> IL | 0.3099 | 0.0212 |
| Write .NET Binary | 0.4283 | 0.0262 |
| **Total Elapsed** | **7.036** | **0.1768** |

**Files created/modified:**
- `Q:\source\fsharp\overload-perf-test\OverloadPerfTest.fsproj` - Added DotnetFscCompilerPath config
- `Q:\source\fsharp\overload-perf-test\run-baseline-perf.ps1` - Performance measurement script
- `Q:\source\fsharp\overload-perf-test\baseline-perf-results.md` - Raw results document

**Verification:**
- [x] Test project uses baseline fsc.dll (verified via build log)
- [x] 5+ compilation runs completed
- [x] --times output captured for each run
- [x] Mean and standard deviation calculated
- [x] Results documented in raw form

---

## Sprint 3: Measure baseline perf

**Summary:** Completed in 2 iterations

**Files touched:** Check git log for details.

---

## Sprint 4: Build optimized compiler

**Summary:** Built optimized F# compiler from PR branch with all four overload resolution improvements

**Details:**
- Optimized commit hash: `f3f07201028c8a70335ec689f1017ddaae1d9bb1`
- PR Branch: `copilot/create-performance-profiling-automation`
- Artifacts folder cleaned completely (3.97 GB removed)
- Build.cmd -c Release completed successfully (0 errors, 0 warnings, Time Elapsed 00:03:55.08)
- Optimized fsc.dll path: `Q:\source\fsharp\fsharp\artifacts\bin\fsc\Release\net10.0\fsc.dll`
- Optimized fsc.dll size: 47,616 bytes
- Build timestamp: 2026-01-22 10:44:39 AM

**Verification:**
- [x] fsharp repo is at PR branch commit
- [x] Build.cmd -c Release completes successfully
- [x] artifacts\bin\fsc\Release\net10.0\fsc.dll exists
- [x] Optimized commit hash documented

---

## Sprint 4: Build optimized compiler

**Summary:** Completed in 2 iterations

**Files touched:** Check git log for details.

---

## Sprint 5: Measure optimized perf

**Summary:** Configured test project to use optimized fsc.dll, ran 5 compilation measurements

**Details:**
- Test project configured with `DotnetFscCompilerPath` pointing to optimized fsc.dll (from PR branch)
- Created `run-optimized-perf.ps1` script for automated performance measurement
- Build log verification confirms optimized compiler is used (overrides SDK default)
- 5 compilation runs completed with --times output captured

**Key Results:**
| Phase | Mean (s) | Std Dev (s) |
|-------|----------|-------------|
| ImportMscorlib+FSharp.Core | 0.7786 | 0.0141 |
| ParseInputs | 0.2475 | 0.0042 |
| Import non-system refs | 0.0743 | 0.0034 |
| **Typecheck** | **1.8859** | **0.0192** |
| Optimizations | 0.1337 | 0.0026 |
| TAST -> IL | 0.3001 | 0.0201 |
| Write .NET Binary | 0.3931 | 0.0369 |
| **Total Elapsed** | **6.258** | **0.123** |

**Comparison to Baseline:**
| Phase | Baseline (s) | Optimized (s) | Delta (s) | Improvement |
|-------|--------------|---------------|-----------|-------------|
| **Typecheck** | 2.5785 | 1.8859 | -0.6926 | **26.9%** |
| **Total Elapsed** | 7.036 | 6.258 | -0.778 | **11.1%** |

**Files created/modified:**
- `Q:\source\fsharp\overload-perf-test\run-optimized-perf.ps1` - Optimized performance measurement script
- `Q:\source\fsharp\overload-perf-test\optimized-perf-results.md` - Raw results document

**Verification:**
- [x] Test project uses optimized fsc.dll (verified via build log)
- [x] 5+ compilation runs completed
- [x] --times output captured for each run
- [x] Mean and standard deviation calculated
- [x] Results documented in raw form

---
