namespace Xunit

open Xunit

module Assert =

    [<assembly: CollectionBehavior(DisableTestParallelization = true)>]
    do()
