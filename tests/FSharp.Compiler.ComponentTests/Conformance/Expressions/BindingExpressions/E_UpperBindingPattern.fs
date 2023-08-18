module One =
    let CallGenericStaticMethod = failwithf "CallGenericStaticMethod"

    let MakeGenericStaticMethod = failwithf "MakeGenericStaticMethod"

    let MakersCallers FSS = CallGenericStaticMethod FSS, MakeGenericStaticMethod FSS