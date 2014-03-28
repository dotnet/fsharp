namespace Microsoft.FSharp.Compatibility

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Char = 
    let compare (x:char) y = Operators.compare x y

    let code (c:char) = int c 
    let chr (n:int) =  char n 

#if FX_NO_TO_LOWER_INVARIANT
    let lowercase (c:char) = System.Char.ToLower(c, System.Globalization.CultureInfo.InvariantCulture)
    let uppercase (c:char) = System.Char.ToUpper(c, System.Globalization.CultureInfo.InvariantCulture)
#else
    let lowercase (c:char) = System.Char.ToLowerInvariant(c)
    let uppercase (c:char) = System.Char.ToUpperInvariant(c)
#endif
