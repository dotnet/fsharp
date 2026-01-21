# Reorganize Perf Tools into Skills Folder

## High-Level Goal

✅ **COMPLETED**: Performance profiling tools have been moved and consolidated from `tools/perf-repro/` into `.copilot/skills/perf-tools/`. Scripts have been simplified, duplication eliminated, and the skill's `.md` file is colocated with its scripts.

## Final Structure

```
.copilot/skills/
└── perf-tools/
    ├── PERF_SKILL.md            # Main skill doc
    ├── PerfTestGenerator.fsx    # Consolidated: generates typed/untyped test projects
    ├── PerfProfiler.fsx         # Consolidated: profiles compilation with timing
    ├── RunPerfAnalysis.ps1      # Windows orchestration
    ├── RunPerfAnalysis.sh       # Unix orchestration
    └── .gitignore               # Ignore generated outputs
```

## Key Design Decisions

1. **Consolidate Scripts**: The 3 existing .fsx files have significant overlap - merge AnalyzeTrace.fsx functionality into a simpler model (timing only, no trace analysis - trace analysis was failing anyway).

2. **Simplify**: Remove the dependency on dotnet-trace for the main workflow. The timing-based approach is sufficient and more portable.

3. **Colocate**: Put the skill .md in the same folder as its scripts (`.copilot/skills/perf-tools/`).

4. **Remove Reports**: The PERF_REPORT*.md files are outputs, not source - they belong in generated results, not source control.

5. **Delete tools/perf-repro**: After moving, this folder should be gone entirely.

## What Was Deleted

- `tools/perf-repro/` (entire folder - contents moved and consolidated)
- `PERFORMANCE_ASSISTANT.md` from repo root (content merged into skill)
- `.copilot/skills/PERFORMANCE_ASSISTANT.md` (replaced by perf-tools/PERF_SKILL.md)

## Script Audit Notes

### GenerateXUnitPerfTest.fsx
- **Quality**: Good structure, proper CLI parsing
- **Issues**: Some redundancy in type generation
- **Improvements**: Simplify type variant handling, remove unused helpers

### ProfileCompilation.fsx
- **Quality**: Over-engineered with trace collection that usually fails
- **Issues**: Fallback logic is convoluted
- **Improvements**: Remove trace collection, focus on timing-only (simpler and reliable)

### AnalyzeTrace.fsx
- **Quality**: Mostly generates boilerplate report text
- **Issues**: Trace analysis never works; just produces template text
- **Improvements**: Remove entirely - the timing output from ProfileCompilation is sufficient

## Success Criteria

- `tools/perf-repro/` folder is deleted
- All useful functionality is in `.copilot/skills/perf-tools/`
- Scripts are shorter, simpler, and validated by sample runs
- Skill .md is colocated with scripts

---

# Method Resolution Performance Investigation

## High-Level Goal

Investigate and improve F# compiler performance when resolving heavily overloaded methods like xUnit's `Assert.Equal`. The issue (#18807) shows that each untyped `Assert.Equal` call adds ~100ms to compilation due to expensive overload resolution.

## Problem Summary

- **Symptom**: `Assert.Equal(1, 2)` (untyped) is ~10-20x slower to compile than `Assert.Equal<int>(1, 2)` (typed)
- **Root Cause**: F# compiler tries each overload candidate with full type checking via `FilterEachThenUndo`, even when many can be quickly ruled out
- **Location**: `src/Compiler/Checking/ConstraintSolver.fs` - `ResolveOverloading` function (line ~3438)

## Sprint 1 Findings (2026-01-20)

**Important**: Initial baseline profiling shows **minimal performance difference** between typed and untyped Assert.Equal calls in the current environment:

| Metric | Untyped | Typed | Ratio |
|--------|---------|-------|-------|
| Compilation time (1500 calls) | 1.30s | 1.28s | 1.02x |
| Time per Assert.Equal | 0.87ms | 0.85ms | - |

This differs significantly from the ~10-20x slowdown reported in issue #18807. Possible explanations:
1. Compiler optimizations may have been added since the issue was filed
2. Different xUnit version or overload set (using xUnit 2.4.2)
3. Different SDK/compiler version (.NET SDK 10.0.100-rc.2)
4. Machine-specific characteristics

**Next steps**: Investigate whether the original issue conditions can be reproduced, or verify if the issue has already been addressed in the current compiler version.

## Sprint 2 Findings (2026-01-20) - ResolveOverloading Deep-Dive

**Deep analysis of the ResolveOverloading hot path revealed:**

### Code Flow Analysis
```
ResolveOverloading
├── IsCandidate filter        ← Already filters by arity (implemented!)
├── exactMatchCandidates      ← Phase 1: TypesEquiv + ArgsEquivOrConvert
│   └── FilterEachThenUndo (creates N traces)
├── applicable                ← Phase 2: TypesEquiv + ArgsMustSubsume  
│   └── FilterEachThenUndo (creates N traces AGAIN)
└── GetMostApplicableOverload ← Pick winner
```

### Key Findings
1. **Early arity pruning is ALREADY implemented** via IsCandidate filter
2. **CalledMeth objects built BEFORE IsCandidate filter** - major inefficiency
3. **Two FilterEachThenUndo passes** double trace allocations
4. **Same candidates tried twice** (exact then subsumption)

### Prioritized Optimization Recommendations
| Priority | Optimization | Impact | Status |
|----------|-------------|--------|--------|
| P0 | Cache CalledMeth per (MethInfo, TyArgs) | 🔥🔥🔥 | New idea |
| P1 | Lazy CalledMeth (after IsCandidate) | 🔥🔥🔥 | Feasible |
| P2 | Merge exact + applicable passes | 🔥🔥 | Medium effort |
| P3 | Full overload resolution cache | 🔥🔥🔥 | Complex |

### Candidate Statistics for Assert.Equal (~19 overloads)
- After IsCandidate filter: ~10-15 remain
- After exactMatch: Usually 0-1
- After applicable: 1-3
- **Waste ratio**: 14:1 (15 tried, 1 succeeds)

## Key Code Paths

1. **Entry Point**: `ResolveOverloading` in `ConstraintSolver.fs:3438`
2. **Candidate Filtering**: `FilterEachThenUndo` in `ConstraintSolver.fs:497` - each candidate is tried with a trace
3. **Type Checking**: `CanMemberSigsMatchUpToCheck` performs full signature checking per candidate
4. **CalledMeth Creation**: `MethodCalls.fs:534` - expensive object creation for each overload

## Optimization Hypotheses

### High Impact
1. **Early Candidate Pruning**: Filter incompatible overloads based on argument count/arity before full type checking
2. **Overload Resolution Caching**: Cache resolution results for identical call patterns (same method group + argument types)
3. **Lazy CalledMeth Construction**: Defer expensive CalledMeth creation until after initial filtering

### Medium Impact
4. **Parameter Type Quick-Check**: Compare argument types without full unification first
5. **Parallel Overload Checking**: Check multiple candidates in parallel (with trace isolation)
6. **Better Indexing**: Index overloads by parameter count/type for faster lookup

### Lower Impact
7. **Trace Pooling**: Reuse Trace objects to reduce allocations
8. **Reduced String Operations**: Minimize string allocations in hot paths

## Profiling Approach

1. Use `.copilot/skills/perf-tools/` scripts to generate test cases
2. Run timing-based profiling with PerfProfiler.fsx
3. Analyze timing differences between typed/untyped patterns
4. Measure baseline, implement hypothesis, measure again

## Constraints & Gotchas

- Changes must not affect correctness of overload resolution
- The trace/undo mechanism is critical for speculative type checking
- Some candidates need full type checking even if arguments look incompatible (due to type-directed conversions)
- Must handle edge cases: param arrays, optional args, named args, generic constraints

## Current Tooling (in `.copilot/skills/perf-tools/`)

- `PerfTestGenerator.fsx` - generates typed/untyped test projects
- `PerfProfiler.fsx` - profiles compilation with timing comparison
- `RunPerfAnalysis.ps1` / `RunPerfAnalysis.sh` - orchestration scripts
- `PERF_SKILL.md` - skill documentation

## Success Criteria

- Reduce per-call overhead for untyped heavily-overloaded method calls by at least 50%
- No regressions in compiler test suite
- No changes to overload resolution semantics
