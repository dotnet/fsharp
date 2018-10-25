#!/bin/sh

_SCRIPT_DIR="$( cd -P -- "$(dirname -- "$(command -v -- "$0")")" && pwd -P )"

mono $_SCRIPT_DIR/fsc.exe "$@"
