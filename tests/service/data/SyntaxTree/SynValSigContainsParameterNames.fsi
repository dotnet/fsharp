module Meh
val InferSynValData:
    memberFlagsOpt: SynMemberFlags option * pat: SynPat option * SynReturnInfo option * origRhsExpr: SynExpr ->
        x: string ->
            SynValData2