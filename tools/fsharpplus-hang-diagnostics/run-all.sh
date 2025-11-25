#!/bin/bash
# run-all.sh
# Master script that runs the complete diagnostic pipeline
# for investigating FSharpPlus build hang with F# 10

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
OUTPUT_DIR="${SCRIPT_DIR}/output"

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

echo_step() {
    echo -e "${BLUE}[STEP]${NC} $1"
}

echo_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

echo_warn() {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

echo_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Banner
echo ""
echo "=================================================="
echo "  FSharpPlus Build Hang Diagnostic Pipeline"
echo "  Issue: https://github.com/dotnet/fsharp/issues/19116"
echo "=================================================="
echo ""

# Check prerequisites
echo_step "Checking prerequisites..."

# Check .NET SDK
if ! command -v dotnet &> /dev/null; then
    echo_error "dotnet SDK not found. Please install .NET SDK 10.0.100 or later."
    exit 1
fi

SDK_VERSION=$(dotnet --version)
echo_info ".NET SDK version: ${SDK_VERSION}"

# Check git
if ! command -v git &> /dev/null; then
    echo_error "git not found. Please install git."
    exit 1
fi

echo_info "Git version: $(git --version)"

# Create output directory
mkdir -p "${OUTPUT_DIR}"

# Step 1: Collect diagnostics
echo ""
echo_step "Step 1/5: Collecting diagnostics..."
echo ""

chmod +x "${SCRIPT_DIR}/collect-diagnostics.sh"
"${SCRIPT_DIR}/collect-diagnostics.sh" || true

echo ""
echo_step "Step 2/5: Analyzing trace file..."
echo ""

# Check if trace file exists
TRACE_FILE="${OUTPUT_DIR}/hang-trace.nettrace"
if [ -f "${TRACE_FILE}" ]; then
    echo_info "Trace file found: ${TRACE_FILE}"
    echo_info "Size: $(du -h "${TRACE_FILE}" | cut -f1)"
    
    # Run trace analysis
    cd "${SCRIPT_DIR}"
    dotnet fsi analyze-trace.fsx || echo_warn "Trace analysis completed with warnings"
else
    echo_warn "Trace file not found. Creating placeholder analysis..."
    cat > "${OUTPUT_DIR}/trace-analysis.md" << 'EOF'
# Trace Analysis Report

## ⚠️ No Trace File Found

The trace file (`hang-trace.nettrace`) was not generated.

This may be because:
1. The `dotnet-trace` tool failed to start
2. The process was killed before trace data could be flushed
3. There was insufficient disk space

### Next Steps

1. Run the collection manually with verbose logging
2. Check system resources (disk space, memory)
3. Try with a shorter timeout and manual dump collection

---
*Report generated automatically*
EOF
fi

echo ""
echo_step "Step 3/5: Analyzing dump files..."
echo ""

# Run dump analysis
cd "${SCRIPT_DIR}"
dotnet fsi analyze-dump.fsx || echo_warn "Dump analysis completed with warnings"

echo ""
echo_step "Step 4/5: Generating diagnostic run report..."
echo ""

cd "${SCRIPT_DIR}"
dotnet fsi generate-diagnostic-run.fsx || echo_warn "Diagnostic run report generation completed with warnings"

echo ""
echo_step "Step 5/5: Generating combined final report..."
echo ""

cd "${SCRIPT_DIR}"
dotnet fsi combined-analysis.fsx || echo_warn "Combined analysis completed with warnings"

# Summary
echo ""
echo "=================================================="
echo "  Diagnostic Pipeline Complete"
echo "=================================================="
echo ""

echo_info "Generated reports:"
ls -la "${OUTPUT_DIR}"/*.md 2>/dev/null || echo_warn "No markdown reports found"

echo ""
echo_info "Generated artifacts:"
ls -la "${OUTPUT_DIR}"/*.nettrace "${OUTPUT_DIR}"/*.dmp 2>/dev/null || echo_warn "No trace/dump files found"

echo ""
echo_info "All output is in: ${OUTPUT_DIR}"
echo ""

# Print key findings
if [ -f "${OUTPUT_DIR}/FINAL-REPORT.md" ]; then
    echo_info "Key findings from FINAL-REPORT.md:"
    echo ""
    head -50 "${OUTPUT_DIR}/FINAL-REPORT.md"
    echo ""
    echo_info "... (see full report for more details)"
fi

echo ""
echo_info "Done! Review the following reports:"
echo "  - ${OUTPUT_DIR}/DIAGNOSTIC-RUN.md"
echo "  - ${OUTPUT_DIR}/trace-analysis.md"
echo "  - ${OUTPUT_DIR}/dump-analysis.md"
echo "  - ${OUTPUT_DIR}/FINAL-REPORT.md"
echo ""
