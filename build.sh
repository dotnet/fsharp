
# At the moment all we build is the Mono version of the F# compiler
export BUILD_NET40=1

./before_install.sh

# This is a very, very limited build script for Mono which bootstraps the compiler
xbuild src/fsharp-proto-build.proj 
xbuild build-everything.proj /p:Configuration=release
