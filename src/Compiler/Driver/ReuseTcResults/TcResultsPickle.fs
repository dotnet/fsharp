module internal FSharp.Compiler.ReuseTcResults.TcResultsPickle

open FSharp.Compiler.CheckDeclarations
open FSharp.Compiler.TypedTreePickle

type TcSharedData = { TopAttribs: TopAttribs }

// pickling

let pickleSharedData sharedData st =
    p_tup3
        p_attribs
        p_attribs
        p_attribs
        (sharedData.TopAttribs.mainMethodAttrs, sharedData.TopAttribs.netModuleAttrs, sharedData.TopAttribs.assemblyAttrs)
        st

let pickleCheckedImplFile checkedImplFile st = p_checked_impl_file checkedImplFile st

// unpickling

let unpickleSharedData st =
    let mainMethodAttrs, netModuleAttrs, assemblyAttrs =
        u_tup3 u_attribs u_attribs u_attribs st

    let attribs =
        {
            mainMethodAttrs = mainMethodAttrs
            netModuleAttrs = netModuleAttrs
            assemblyAttrs = assemblyAttrs
        }

    { TopAttribs = attribs }

let unpickleCheckedImplFile st = u_checked_impl_file st
