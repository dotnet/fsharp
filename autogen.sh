#!/usr/bin/env sh

which autoreconf > /dev/null || (echo "Please install autoconf" && exit 1)
autoreconf && ./configure $@
