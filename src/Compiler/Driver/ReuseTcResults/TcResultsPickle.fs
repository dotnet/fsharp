module internal FSharp.Compiler.ReuseTcResults.TcResultsPickle

open FSharp.Compiler.CheckDeclarations
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreePickle

type PickledTcInfo =
    {
        TopAttribs: TopAttribs
        DeclaredImpls: CheckedImplFile list
    }

let pickleTcInfo (tcInfo: PickledTcInfo) (st: WriterState) =
    p_tup4
        p_attribs
        p_attribs
        p_attribs
        (p_list p_checked_impl_file)
        (tcInfo.TopAttribs.mainMethodAttrs, tcInfo.TopAttribs.netModuleAttrs, tcInfo.TopAttribs.assemblyAttrs, tcInfo.DeclaredImpls)
        st

let unpickleTcInfo st : PickledTcInfo =
    let mainMethodAttrs, netModuleAttrs, assemblyAttrs, declaredImpls =
        u_tup4 u_attribs u_attribs u_attribs (u_list u_checked_impl_file) st

    let attribs: TopAttribs =
        {
            mainMethodAttrs = mainMethodAttrs
            netModuleAttrs = netModuleAttrs
            assemblyAttrs = assemblyAttrs
        }

    {
        TopAttribs = attribs
        DeclaredImpls = declaredImpls
    }
