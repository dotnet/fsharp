module One =
    let CallGenericStaticMethod x = failwithf "CallGenericStaticMethod"

    let MakeGenericStaticMethod x = failwithf "MakeGenericStaticMethod"

    let MakersCallers FSS = CallGenericStaticMethod FSS, MakeGenericStaticMethod FSS