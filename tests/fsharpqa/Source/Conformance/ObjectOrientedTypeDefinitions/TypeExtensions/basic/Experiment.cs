// Regression test for FSHARP1.0:3473
// Signature checking issue with extension of a type defined in a C# dll
// <Expects status=success></Expects>

namespace Experiment {

    /// <summary>The C# version of Test.Test</summary>    
    public class Test2<A, B> {
        public Test2(A a, B b) {
            valA = a;
            valB = b;
        }

        private A valA;
        private B valB;

        public A ValA { get { return valA; } }
        public B ValB { get { return valB; } }
    }
}
