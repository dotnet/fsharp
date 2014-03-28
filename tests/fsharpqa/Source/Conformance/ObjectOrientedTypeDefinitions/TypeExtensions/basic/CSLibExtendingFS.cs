// #Regression #Conformance #ObjectOrientedTypes #TypeExtensions
// Library for Regression for Dev11:51674, extending an F# type

using System;
using FSLib;

namespace CSLib
{
    public static class ExtMeth
    {
        public static void Delay(this Foo foo)
        {
        }
    }
}

