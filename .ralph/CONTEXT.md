# Product Increments

This file is updated after each sprint completes. Use it to understand what was delivered.

---

## Sprint 1: Profiling Infrastructure Setup

**Summary:** Completed in 2 iterations

**Files touched:** Check git log for details.

---

## Sprint 2: Analyze ResolveOverloading Hot Path

**Summary:** Deep-dive analysis of ResolveOverloading function completed

**Deliverables:**
- Documented code flow analysis of ResolveOverloading (4 phases identified)
- Sub-operation time breakdown estimated from code complexity
- Candidate count statistics: 15 tried vs 1 succeeded (14:1 waste ratio)
- 4 allocation hotspots identified:
  1. CalledMeth construction (10-15 per call)
  2. Trace allocations (20-30 per call)
  3. List allocations in CanMemberSigsMatchUpToCheck
  4. CalledArg records (50-100+ per call)
- 6 optimization ideas prioritized (P0-P5) with impact/effort/risk matrix
- 2 existing optimizations confirmed as already implemented:
  - Early arity pruning (IsCandidate filter)
  - Skip subsumption for exact matches

**Key Findings:**
- CalledMeth objects built BEFORE IsCandidate filter (inefficient)
- Two FilterEachThenUndo passes double trace allocations
- No caching of CalledMeth across identical method calls

**Top 3 Optimization Recommendations (Data-Driven):**
1. P0: Cache CalledMeth per (MethInfo, CalledTyArgs) - Very High Impact
2. P1: Lazy CalledMeth construction after IsCandidate - High Impact, Low Risk
3. P2: Merge exact match + applicable phases - Medium-High Impact

**Files updated:**
- `METHOD_RESOLUTION_PERF_IDEAS.md` - Added Experiment 2, new ideas #9 and #10, updated all ideas with Sprint 2 findings
- `.ralph/VISION.md` - Added Sprint 2 findings section

---
