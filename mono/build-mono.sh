#!/bin/sh

# Bootstrap the compiler
# Install the compiler
./mono/prepare-mono.sh && \
make && \
sudo make install

