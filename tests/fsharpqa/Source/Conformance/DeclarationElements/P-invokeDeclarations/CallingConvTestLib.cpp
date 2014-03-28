// This is the source for the checked in copies of CallingConvTestLib in case any future modification is necessary
// It's currently compiled with the CRT statically linked which is necessary to run across Dev10/SP1/Dev11

typedef struct _ComplexStd {
       double re;
       double im;
} ComplexStd;

extern "C" __declspec(dllexport) ComplexStd* __stdcall CreateComplexStd(double x, double y);

ComplexStd* __stdcall CreateComplexStd(double x, double y)
{
       ComplexStd* complex = new ComplexStd;
       complex->re = x;
       complex->im = y;
       return complex;
}

typedef struct _ComplexCDecl {
       double re;
       double im;
       //_Complex* peer;
} ComplexCDecl;

extern "C" __declspec(dllexport) ComplexCDecl* CreateComplexCDecl(double x, double y);

ComplexCDecl* CreateComplexCDecl(double x, double y)
{
       ComplexCDecl* complex = new ComplexCDecl;
       complex->re = x;
       complex->im = y;
       return complex;
}