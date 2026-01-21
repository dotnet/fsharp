# RFC Quality Assessment: FS-XXXX "Most Concrete" Tiebreaker

**Assessment Date:** 2026-01-21  
**Assessed Against:** 30+ RFCs from [fsharp/fslang-design](https://github.com/fsharp/fslang-design)  
**Reference RFCs:** FS-1093 (Additional Conversions), FS-1146 (Scoped Nowarn), FS-1043 (SRTP Extensions), FS-1087 (Resumable Code), FS-1097 (Task Builder)

---

## Executive Summary

**Overall Grade: B-** (Strong draft, but lacks critical RFC-mandated sections)

The RFC draft demonstrates excellent technical depth in certain areas but has significant structural and content gaps compared to approved F# RFCs. The modular section approach (13 separate files) is unusual and may fragment the narrative. The core algorithm is well-defined, but several mandatory RFC sections are missing or incomplete.

---

## Structural Analysis vs. RFC Template

### ✅ Present and Adequate

| Section | Status | Comments |
|---------|--------|----------|
| Summary | ✅ Adequate | Clear one-paragraph summary |
| Motivation | ✅ Strong | 7 real-world examples with links |
| Detailed Design | ✅ Strong | Algorithm well-specified |
| Prior Art | ✅ Excellent | C#, Scala, Haskell, Rust, Swift, OCaml covered |
| Compatibility | ✅ Adequate | Breaking change analysis present |
| Examples | ✅ Excessive | 15 examples - may be overkill |

### ❌ Missing or Critically Deficient

| Section | Status | Severity | Comments |
|---------|--------|----------|----------|
| **Changes to F# Spec** | ⚠️ Present but vague | HIGH | No actual diff or section numbers from [fsharp.github.io/fslang-spec](https://fsharp.github.io/fslang-spec/) |
| **Drawbacks** | ❌ MISSING | CRITICAL | Every RFC MUST have this. FS-1093 dedicates significant space to drawbacks |
| **Alternatives** | ❌ MISSING | CRITICAL | What other approaches were considered and rejected? |
| **Interop** | ❌ MISSING | HIGH | What happens when C# consumes F# code with this feature? |
| **Pragmatics: Performance** | ⚠️ Incomplete | MEDIUM | No performance impact analysis |
| **Pragmatics: Scaling** | ❌ MISSING | MEDIUM | Expected bounds on overload candidates? |
| **Pragmatics: Tooling** | ⚠️ Minimal | MEDIUM | Debugging, error recovery not addressed |
| **Unresolved Questions** | ❌ MISSING | MEDIUM | Every RFC has open questions |
| **Discussion Link** | ❌ MISSING | LOW | No fslang-design discussion thread |
| **Implementation Link** | ❌ MISSING | LOW | No PR linked |

---

## Detailed Critique

### 1. DRAWBACKS SECTION: COMPLETELY MISSING

**This is the most critical gap.** Every single approved RFC in fslang-design has a Drawbacks section. Compare:

**FS-1093 (Additional Conversions):**
```markdown
# Drawbacks

### Expressions may change type when extracted

Despite appearances, the existing F# approach to type checking 
prior to this change has advantages:

1. When a sub-expression is extracted to a `let` binding for a value 
   or function, its inferred type rarely changes...
2. Information loss is made explicit in many important places...
```

**Your RFC:** *No drawbacks section exists.*

**Required content for this RFC:**
- What if developers relied on ambiguity errors as a "guardrail" to catch incorrect code?
- The "more concrete" selection may be surprising when developers expected the generic overload
- Potential for subtle behavioral changes when library authors add new overloads
- Longer compile times due to additional comparison logic in overload resolution
- Risk of different behavior when code is moved between F# versions

---

### 2. ALTERNATIVES SECTION: COMPLETELY MISSING

**Every RFC must justify why THIS design was chosen over others.** Compare:

**FS-1146 (Scoped Nowarn) - Alternatives:**
```markdown
# Alternatives

## Functionality

The following alternatives have been considered:

Alternative 1: NOWARN and WARNON disable/enable the warning until 
a corresponding WARNON / NOWARN...

Alternative 2: NOWARN and WARNON disable/enable the warning...

Alternative 3: NOWARN disables the warning (independent of the defaults)...
```

**Your RFC:** *No alternatives discussed.*

**Required alternatives for this RFC:**
1. **Do nothing** - Keep FS0041 errors, require type annotations
2. **Explicit attribute** - Require `[<PreferredOverload>]` on more concrete overloads
3. **Warning instead of silent resolution** - Always warn when tiebreaker is used (currently off by default)
4. **Constraint-based approach** - Use type constraints rather than instantiation depth
5. **.NET 9 OverloadResolutionPriorityAttribute** - Integrate with .NET's explicit priority system instead

---

### 3. SPEC CHANGES: VAGUE AND INCOMPLETE

**FS-1146 shows proper spec changes format:**
```markdown
# Detailed specification

1. The compiler shall recognize a new "warnon" *compiler directive* 
   (to be added to §12.4 of the F# spec).

2. A warn directive is a single line of source code that consists of...
```

**FS-1043 shows another proper format:**
```markdown
# Detailed design

The proposed change is as follows, in the internal logic of the 
constraint solving process:

1. During constraint solving, the record of each SRTP constraint 
   incorporates the relevant extension methods...
```

**Your RFC:** The "section-spec-changes.md" says:
> "The 'most concrete' tiebreaker for generic overloads requires modifications to **Section 14: Inference Procedures**"

But then provides a **proposed text** rather than **actual diff against the current spec**. The spec text at https://fsharp.github.io/fslang-spec/ should be referenced with specific existing wording being replaced.

**Required:**
- Quote the EXACT current text from Section 14.4 step 7
- Show the EXACT replacement text
- Provide line-by-line diff if possible

---

### 4. INTEROP SECTION: COMPLETELY MISSING

**The RFC template explicitly asks:**
```markdown
# Interop

* What happens when this feature is consumed by another .NET language?
* Are there any planned or proposed features for another .NET language 
  that we would want this feature to interoperate with?
```

**Critical questions not addressed:**
1. When C# code consumes an F# library with ambiguous overloads, which overload does C# select vs F#?
2. How does this interact with .NET 9's `OverloadResolutionPriorityAttribute`?
3. If F# doesn't recognize the priority attribute, will F# and C# select different overloads for the same call?

---

### 5. UNRESOLVED QUESTIONS: COMPLETELY MISSING

**Every RFC has open questions.** Compare FS-1093:
```markdown
# Unresolved questions

* [x] Proof using XML APIs that make existing use of op_Implicit
* [ ] Proof using Newtonsoft Json APIs...
* [ ] "another popular library to validate with is StackExchange.Redis..."
```

**Open questions this RFC should address:**
1. Should constraint count affect concreteness? (Currently proposed: yes, but controversial)
2. Should this feature be gated behind a language version flag?
3. What's the performance impact on large codebases?
4. Should there be an opt-out attribute for specific overloads?
5. How does this interact with future union types or discriminated union improvements?

---

### 6. PERFORMANCE ANALYSIS: SUPERFICIAL

**The RFC template requires:**
```markdown
## Performance

Please list any notable concerns for impact on the performance of 
compilation and/or generated code:

* For existing code
* For the new features
```

**Your section-diagnostics.md mentions:** "the 'more concrete' tiebreaker produces correct, intuitive results"

But there's **no analysis of:**
- Additional type comparisons per overload resolution
- Memory allocation for tracking concreteness levels
- Worst-case complexity with many overloads
- Benchmark data from prototype implementation

---

### 7. EXCESSIVE FRAGMENTATION

**The RFC is split into 13 separate files:**
```
FS-XXXX-most-concrete-overload-tiebreaker.md
section-adhoc-rules.md
section-algorithm.md
section-byref-span.md
section-compatibility.md
section-diagnostics.md
section-examples.md
section-extension-methods.md
section-motivation.md
section-optional-params.md
section-prior-art.md
section-spec-changes.md
section-tdc-interaction.md
```

**No approved RFC uses this structure.** All approved RFCs are single documents. This fragmentation:
- Makes it harder to review holistically
- Creates confusion about the authoritative content
- Duplicates content between files (e.g., examples appear in both main file and section-examples.md)
- Cannot be submitted as a PR to fslang-design in this format

**Recommendation:** Consolidate into a single RFC document following the template.

---

### 8. OVERLY VERBOSE EXAMPLES

**15 examples in section-examples.md is excessive.** Compare approved RFCs:

| RFC | Example Count | Approach |
|-----|---------------|----------|
| FS-1093 | ~8 inline | Focused, progressive complexity |
| FS-1146 | ~5 inline | Minimal, directly supports spec |
| FS-1043 | ~6 | Includes advanced edge cases |

**Your RFC:** 15 numbered examples + 7 in motivation section = 22 total

**Recommendation:** Reduce to 6-8 carefully chosen examples:
1. Basic generic vs concrete (the poster child case)
2. Nested generics
3. Multiple type parameters (incomparable case)
4. Interaction with existing rules (TDC, extension methods)
5. Real-world library case (pick ONE: ValueTask, FsToolkit, or TaskBuilder)
6. Edge case that remains ambiguous

---

### 9. MISSING COMMUNITY DISCUSSION REFERENCE

**Approved RFCs reference their discussion threads:**

FS-1093:
```markdown
- [x] [Community Review Meeting](https://github.com/fsharp/fslang-design/issues/589)
- [x] [Discussion](https://github.com/fsharp/fslang-design/discussions/525)
```

**Your RFC:** Has placeholder `XXX` values for discussion and implementation links.

---

### 10. DESIGN PRINCIPLES NOT STATED

**FS-1093 explicitly states design principles:**
```markdown
# Design Principles

The intent of this RFC is to give a user experience where:

1. Interop is easier (including interop with some F# libraries)
2. You don't notice the feature and are barely aware of its existence
3. Fewer upcasts are needed when programming with types that support subtyping
...

NOTE: The aim is to make a feature which is trustworthy and barely noticed.
```

**Your RFC:** Has no equivalent "Design Principles" section explaining the philosophy behind the design choices.

---

## Scoring Breakdown

| Criterion | Weight | Score | Notes |
|-----------|--------|-------|-------|
| Template Compliance | 20% | 50% | Missing 4 critical sections |
| Technical Accuracy | 25% | 85% | Algorithm well-specified |
| Completeness | 20% | 60% | Fragmented, missing interop/perf |
| Clarity | 15% | 70% | Good examples, but too many |
| Real-world Grounding | 10% | 90% | Excellent library examples |
| Prior Art Research | 10% | 95% | Comprehensive cross-language survey |

**Weighted Score: 71% (B-)**

---

## Action Items for RFC Completion

### Critical (Must Fix Before Submission)

1. **Add Drawbacks section** - At least 5 drawbacks with mitigations
2. **Add Alternatives section** - At least 3 alternatives with rejection rationale
3. **Add Interop section** - Especially .NET 9 OverloadResolutionPriorityAttribute
4. **Consolidate into single document** - Following RFC_template.md structure
5. **Add Unresolved Questions** - Open issues requiring F# team input

### High Priority

6. **Add Performance analysis** - Compilation time impact, benchmark data
7. **Fix Spec Changes** - Quote actual current spec text, show diff
8. **Add Design Principles** - Philosophy statement like FS-1093
9. **Reduce examples to 6-8** - Quality over quantity
10. **Create fslang-design discussion thread** - Before PR

### Medium Priority

11. Add Scaling section (expected bounds on overload counts)
12. Add Tooling section (debugging, IDE support)
13. Add Culture-aware section (N/A statement is fine)
14. Get PR number for implementation link

---

## Comparative Quality Chart

```
┌────────────────────────────────────────────────────────────────┐
│ RFC Quality Comparison (0-100%)                                │
├────────────────────────────────────────────────────────────────┤
│ FS-1087 Resumable Code    ████████████████████████████████  95%│
│ FS-1093 Addl Conversions  ██████████████████████████████    90%│
│ FS-1097 Task Builder      █████████████████████████████     88%│
│ FS-1043 SRTP Extensions   ████████████████████████████      85%│
│ FS-1146 Scoped Nowarn     ███████████████████████████       82%│
│ ► YOUR RFC (current)      ██████████████████████            71%│
│ ► YOUR RFC (potential)    ████████████████████████████████  92%│
└────────────────────────────────────────────────────────────────┘
```

---

## Conclusion

This RFC draft shows **strong technical research** and **excellent motivation examples**. The algorithm design is sound and the prior art survey is comprehensive. However, it **fails to meet the minimum structural requirements** of the F# RFC process.

The main document was clearly written by someone who understands the F# compiler deeply, but the RFC appears to have been written in isolation without consulting the [RFC_template.md](https://github.com/fsharp/fslang-design/blob/main/RFC_template.md) or studying how approved RFCs are structured.

**Primary recommendation:** Before any further technical work, restructure the entire RFC as a single document using RFC_template.md as a strict guide. Add all missing sections (Drawbacks, Alternatives, Interop, Unresolved Questions). Only then refine the technical content.

The potential is there for an excellent RFC - but significant restructuring is required.

---

*Assessment by: RFC Quality Review Agent*  
*Reference corpus: 30+ RFCs from fsharp/fslang-design (FSharp-4.0 through FSharp-9.0, RFCs/, preview/)*
