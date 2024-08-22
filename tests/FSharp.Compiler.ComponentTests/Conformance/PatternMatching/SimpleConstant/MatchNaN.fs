// #Conformance #PatternMatching #Constants 
#light

// Verify the ability to match against NaN. Note that 
// this should 

let rec TestIsNaN x =
    match box x with
    | :? float as fx     -> isNaNFloat fx
    | :? float32 as f32x -> isNaNFloat32 f32x

and isNaNFloat x =
    match x with
    | System.Double.NaN -> failwith "Should never match"
    | 0.0               -> failwith "Should never match"
    | _                 -> ()

and isNaNFloat32 x =
    match x with
    | System.Single.NaN -> failwith "Should never match"
    | 0.0f              -> failwith "Should never match"
    | _                 -> ()

// This seems strange, but according to the IEEE spec
// NaN <> NaN
TestIsNaN System.Single.NaN
TestIsNaN System.Double.NaN

exit 0
