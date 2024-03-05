open System

module Extractions2 =

    let f (x: ActivitySource) =
       x.CreateActivity("aaa", ActivityKind.Client)

    let source = new ActivitySource("aaab")
    let activity = f source
    let baggage = activity.Baggage // TODO NULLNESS: should trigger a nullness warning

    // TODO NULLNESS test matrix
    // - Import null annotated .NET non-generic type (above)
    // - Import null annotated .NET array type
    // - Import null annotated .NET generic type instantiated at non-null .NET type 
    // - Import .NET generic type instantiated at null annotated .NET type 


