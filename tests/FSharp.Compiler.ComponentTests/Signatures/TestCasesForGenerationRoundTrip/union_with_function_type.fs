module Example.Context

type Context = { Events: string list }

type internal ColMultilineItem =
    | ColMultilineItem of
        // current expression
        expr: (Context -> Context) *
        // sepNln of current item
        sepNln: (Context -> Context)