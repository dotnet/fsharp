#!/bin/bash

# Master orchestration script for F# compiler performance analysis
# This script runs the complete profiling workflow for issue #18807

set -e  # Exit on error

# Default configuration
TOTAL_ASSERTS=1500
METHODS=10
GENERATED_DIR="./generated"
RESULTS_DIR="./results"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Helper functions
print_header() {
    echo -e "${BLUE}========================================${NC}"
    echo -e "${BLUE}$1${NC}"
    echo -e "${BLUE}========================================${NC}"
}

print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠ $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

print_info() {
    echo -e "${NC}  $1${NC}"
}

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --total)
            TOTAL_ASSERTS="$2"
            shift 2
            ;;
        --methods)
            METHODS="$2"
            shift 2
            ;;
        --generated)
            GENERATED_DIR="$2"
            shift 2
            ;;
        --results)
            RESULTS_DIR="$2"
            shift 2
            ;;
        --help)
            echo "Usage: $0 [options]"
            echo ""
            echo "Options:"
            echo "  --total <n>        Total number of Assert.Equal calls (default: 1500)"
            echo "  --methods <n>      Number of test methods (default: 10)"
            echo "  --generated <path> Directory for generated projects (default: ./generated)"
            echo "  --results <path>   Output directory for results (default: ./results)"
            echo "  --help             Show this help message"
            echo ""
            echo "Example:"
            echo "  $0 --total 1500 --methods 10"
            exit 0
            ;;
        *)
            print_error "Unknown option: $1"
            echo "Use --help for usage information"
            exit 1
            ;;
    esac
done

# Display configuration
print_header "F# Compiler Performance Analysis"
echo ""
print_info "Configuration:"
print_info "  Total Assert.Equal calls: $TOTAL_ASSERTS"
print_info "  Test methods: $METHODS"
print_info "  Generated projects: $GENERATED_DIR"
print_info "  Results directory: $RESULTS_DIR"
echo ""

# Create directories
print_info "Creating directories..."
mkdir -p "$GENERATED_DIR"
mkdir -p "$RESULTS_DIR"
print_success "Directories created"
echo ""

# Step 1: Check prerequisites
print_header "Step 1: Checking Prerequisites"
echo ""

# Check for dotnet
if ! command -v dotnet &> /dev/null; then
    print_error "dotnet CLI not found. Please install .NET SDK."
    exit 1
fi
print_success "dotnet CLI found: $(dotnet --version)"

# Check for dotnet-trace (optional)
if command -v dotnet-trace &> /dev/null; then
    print_success "dotnet-trace found: $(dotnet-trace --version | head -n 1)"
else
    print_warning "dotnet-trace not found. Will use timing-only mode."
    print_info "To install: dotnet tool install -g dotnet-trace"
fi
echo ""

# Step 2: Run profiling workflow
print_header "Step 2: Running Profiling Workflow"
echo ""

START_TIME=$(date +%s)

print_info "Executing ProfileCompilation.fsx..."
if dotnet fsi "$SCRIPT_DIR/ProfileCompilation.fsx" \
    --total "$TOTAL_ASSERTS" \
    --methods "$METHODS" \
    --generated "$GENERATED_DIR" \
    --output "$RESULTS_DIR"; then
    print_success "Profiling completed successfully"
else
    print_error "Profiling failed"
    exit 1
fi

END_TIME=$(date +%s)
ELAPSED=$((END_TIME - START_TIME))

echo ""
print_success "Profiling workflow completed in ${ELAPSED}s"
echo ""

# Step 3: Analyze results and generate report
print_header "Step 3: Generating Analysis Report"
echo ""

print_info "Executing AnalyzeTrace.fsx..."
if dotnet fsi "$SCRIPT_DIR/AnalyzeTrace.fsx" \
    --results "$RESULTS_DIR" \
    --output "$RESULTS_DIR/PERF_REPORT.md"; then
    print_success "Report generated successfully"
else
    print_error "Report generation failed"
    exit 1
fi

echo ""

# Step 4: Display summary
print_header "Step 4: Summary"
echo ""

# Read and display summary
if [ -f "$RESULTS_DIR/summary.txt" ]; then
    cat "$RESULTS_DIR/summary.txt"
    echo ""
fi

# Final message
print_header "Analysis Complete!"
echo ""
print_success "All steps completed successfully"
print_info "Results location: $RESULTS_DIR"
print_info "Performance report: $RESULTS_DIR/PERF_REPORT.md"
echo ""
print_info "To view the report:"
print_info "  cat $RESULTS_DIR/PERF_REPORT.md"
print_info "  # or open with your favorite markdown viewer"
echo ""

# Optional: Display report preview
if [ -f "$RESULTS_DIR/PERF_REPORT.md" ]; then
    print_info "Report preview (first 50 lines):"
    echo ""
    head -n 50 "$RESULTS_DIR/PERF_REPORT.md"
    echo ""
    print_info "..."
    print_info "(see $RESULTS_DIR/PERF_REPORT.md for full report)"
fi

echo ""
print_success "Done!"
