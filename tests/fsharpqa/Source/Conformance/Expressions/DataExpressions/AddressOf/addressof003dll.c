// Simple C-style file
// Compile with /LD to make a .dll
// The __declspec(dllexport) directive is used to export the function
// No need to "extern "C" { ... } " to prevent name mangling, since this is a .c file
#include <stdio.h>

__declspec(dllexport) int M(int *ptr)
{
    (*ptr)++;
    return *ptr;
}