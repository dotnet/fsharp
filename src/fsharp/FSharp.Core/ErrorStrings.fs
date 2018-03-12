// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace Microsoft.FSharp.Core

    open System
    open System.Collections
    open System.Collections.Generic
    open System.Diagnostics
    open System.Globalization
    open System.Linq
    open System.Text
    open Microsoft.FSharp.Core

    //-------------------------------------------------------------------------
    // The main aim here is to bootstrap the definition of structural hashing 
    // and comparison.  Calls to these form part of the auto-generated 
    // code for each new datatype.

    [<CompiledName("XYZ")>]
    module ErrorStringsLanguagePrimitives =
        [<CompiledName("ErrorStrings")>]
        module (* internal *) ErrorStrings =
            // inline functions cannot call GetString, so we must make these bits public
            let AddressOpNotFirstClassString = SR.GetString(SR.addressOpNotFirstClass)
            let NoNegateMinValueString = SR.GetString(SR.noNegateMinValue)
            // needs to be public to be visible from inline function 'average' and others
            let InputSequenceEmptyString = SR.GetString(SR.inputSequenceEmpty) 
            // needs to be public to be visible from inline function 'average' and others
            let InputArrayEmptyString = SR.GetString(SR.arrayWasEmpty) 
            // needs to be public to be visible from inline function 'average' and others
            let InputMustBeNonNegativeString = SR.GetString(SR.inputMustBeNonNegative)
            
