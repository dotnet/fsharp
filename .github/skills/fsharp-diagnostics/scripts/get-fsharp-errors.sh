#!/usr/bin/env bash
set -euo pipefail

# get-fsharp-errors.sh — minimal passthrough client for fsharp-diag-server
# Usage:
#   get-fsharp-errors.sh [--parse-only] <file.fs>
#   get-fsharp-errors.sh --check-project <project.fsproj>
#   get-fsharp-errors.sh --ping
#   get-fsharp-errors.sh --shutdown

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
SERVER_PROJECT="$(cd "$SCRIPT_DIR/../server" && pwd)"
SOCK_DIR="$HOME/.fsharp-diag"

get_repo_root() {
    git rev-parse --show-toplevel 2>/dev/null || pwd
}

get_socket_path() {
    local root="$1"
    local hash
    hash=$(printf '%s' "$root" | shasum -a 256 | cut -c1-16)
    echo "$SOCK_DIR/${hash}.sock"
}

ensure_server() {
    local root="$1"
    local sock="$2"
    
    # Check if socket exists and server responds to ping
    if [ -S "$sock" ]; then
        local pong
        pong=$(printf '{"command":"ping"}\n' | nc -U "$sock" 2>/dev/null || true)
        if echo "$pong" | grep -q '"ok"'; then
            return 0
        fi
        # Stale socket
        rm -f "$sock"
    fi

    # Start server
    mkdir -p "$SOCK_DIR"
    local log_hash
    log_hash=$(printf '%s' "$root" | shasum -a 256 | cut -c1-16)
    local log_file="$SOCK_DIR/${log_hash}.log"
    
    nohup dotnet run -c Release --project "$SERVER_PROJECT" -- --repo-root "$root" > "$log_file" 2>&1 &
    
    # Wait for socket to appear (max 60s)
    local waited=0
    while [ ! -S "$sock" ] && [ $waited -lt 60 ]; do
        sleep 1
        waited=$((waited + 1))
    done

    if [ ! -S "$sock" ]; then
        echo '{"error":"Server failed to start within 60s. Check log: '"$log_file"'"}' >&2
        exit 1
    fi
}

send_request() {
    local sock="$1"
    local request="$2"
    printf '%s\n' "$request" | nc -U "$sock"
}

# --- Main ---

REPO_ROOT=$(get_repo_root)
SOCK_PATH=$(get_socket_path "$REPO_ROOT")

case "${1:-}" in
    --ping)
        ensure_server "$REPO_ROOT" "$SOCK_PATH"
        send_request "$SOCK_PATH" '{"command":"ping"}'
        ;;
    --shutdown)
        send_request "$SOCK_PATH" '{"command":"shutdown"}'
        ;;
    --parse-only)
        shift
        FILE=$(cd "$(dirname "$1")" && pwd)/$(basename "$1")
        ensure_server "$REPO_ROOT" "$SOCK_PATH"
        send_request "$SOCK_PATH" "{\"command\":\"parseOnly\",\"file\":\"$FILE\"}"
        ;;
    --check-project)
        ensure_server "$REPO_ROOT" "$SOCK_PATH"
        send_request "$SOCK_PATH" '{"command":"checkProject"}'
        ;;
    --find-refs)
        shift
        FILE=$(cd "$(dirname "$1")" && pwd)/$(basename "$1")
        LINE="$2"
        COL="$3"
        ensure_server "$REPO_ROOT" "$SOCK_PATH"
        send_request "$SOCK_PATH" "{\"command\":\"findRefs\",\"file\":\"$FILE\",\"line\":$LINE,\"col\":$COL}"
        ;;
    --type-hints)
        shift
        FILE=$(cd "$(dirname "$1")" && pwd)/$(basename "$1")
        START_LINE="$2"
        END_LINE="$3"
        ensure_server "$REPO_ROOT" "$SOCK_PATH"
        send_request "$SOCK_PATH" "{\"command\":\"typeHints\",\"file\":\"$FILE\",\"startLine\":$START_LINE,\"endLine\":$END_LINE}"
        ;;
    -*)
        echo "Usage: get-fsharp-errors [--parse-only] <file.fs>" >&2
        echo "       get-fsharp-errors --check-project <project.fsproj>" >&2
        echo "       get-fsharp-errors --ping | --shutdown" >&2
        exit 1
        ;;
    *)
        FILE=$(cd "$(dirname "$1")" && pwd)/$(basename "$1")
        ensure_server "$REPO_ROOT" "$SOCK_PATH"
        send_request "$SOCK_PATH" "{\"command\":\"check\",\"file\":\"$FILE\"}"
        ;;
esac
