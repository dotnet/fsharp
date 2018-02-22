#!/bin/bash

# note: expects to run from top directory
# suitable for Ubuntu 16.04 - get latest Mono stable
../mono/latest-mono-stable.sh && \
make Configuration=$@ && \
sudo make install Configuration=$@ && \
../mono/test-mono.sh