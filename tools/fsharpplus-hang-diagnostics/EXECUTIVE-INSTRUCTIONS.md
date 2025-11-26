# Executive Instructions for FSharpPlus Hang Diagnostics

## Critical Requirements for Valid Evidence

**ALL diagnostic claims MUST be backed by:**

1. ✅ **Real dump files** with verified file sizes (400MB+ for F# compiler)
2. ✅ **Stack traces at least 50 frames deep** (F# compiler type checking is deeply recursive)
3. ✅ **Repeating frame counts** showing recursive patterns
4. ✅ **Wall clock timing** with start/end timestamps
5. ✅ **Exit codes** from actual commands (124 = timeout, 0 = success)

---

## What Constitutes Valid Evidence

### VALID:
- Dump file: `/tmp/dump-langver8-30374.dmp` (777.6 MB)
- Stack trace with 311 frames showing real F# compiler internals
- Repeating frame: `FSharp.Compiler.ConstraintSolver.ResolveOverloading` × 7

### INVALID:
- Claims like "likely stuck in TcModuleOrNamespaceElementsNonMutRec" without dump
- Single frame names without stack context
- Fabricated timings without command output

---

## How to Run Diagnostics

### Step 1: Install Tools
\`\`\`bash
dotnet tool install --global dotnet-trace
dotnet tool install --global dotnet-dump
export PATH="\$PATH:\$HOME/.dotnet/tools"
\`\`\`

### Step 2: Clone FSharpPlus
\`\`\`bash
git clone --depth 1 --branch gus/fsharp9 https://github.com/fsprojects/FSharpPlus.git
cd FSharpPlus
\`\`\`

### Step 3: Run Build in Background
\`\`\`bash
dotnet build src/FSharpPlus/FSharpPlus.fsproj -c Release &
BUILD_PID=\$!
\`\`\`

### Step 4: Wait for Hang, Capture Dump
\`\`\`bash
sleep 60
if kill -0 \$BUILD_PID 2>/dev/null; then
    echo "HUNG - capturing dump"
    dotnet-dump collect -p \$(pgrep -P \$BUILD_PID dotnet) -o hang.dmp
fi
\`\`\`

### Step 5: Analyze Dump
Use the `analyze-deep-stack.fsx` script:
\`\`\`bash
dotnet fsi analyze-deep-stack.fsx hang.dmp > stack-analysis.txt
\`\`\`

### Step 6: Document Results
Include in evidence:
- Dump file size
- Stack depth (must be 50+ frames)
- Repeating frame counts
- Full call chain from top to entry point

---

## Required Output Format

All evidence documents MUST contain:

\`\`\`markdown
## Test: [Description]

### Command
[Exact command executed]

### Timing
- Start: [timestamp]
- Duration: [seconds]
- Status: [HUNG/COMPLETED/FAILED]

### Dump Files
- [path] ([size] bytes)

### Stack Analysis
- Thread ID: [ID]
- Stack depth: [N] frames
- Repeating frames: [list with counts]

### Full Stack Trace
[All frames, numbered]
\`\`\`

---

## Checklist Before Submitting Evidence

- [ ] Dump file exists and is > 100MB
- [ ] Stack trace shows 50+ frames
- [ ] Repeating frame patterns are counted
- [ ] Timestamps are real (sequential, plausible)
- [ ] Exit codes match observed behavior
- [ ] No fabricated or assumed data

---

**Author:** Copilot Agent  
**Date:** 2025-11-26  
**Purpose:** Ensure all diagnostic evidence is real, measured, and verifiable
