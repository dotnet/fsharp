ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Yield 03.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (App
                (NonAtomic, false, Ident seq,
                 ComputationExpr
                   (false,
                    YieldOrReturn
                      ((true, false),
                       Paren
                         (Typed
                            (ArrayOrListComputed
                               (false,
                                IndexRange
                                  (Some (Const (Int32 1, (4,13--4,14))),
                                   (4,15--4,17),
                                   Some (Const (Int32 10, (4,18--4,20))),
                                   (4,13--4,14), (4,18--4,20), (4,13--4,20)),
                                (4,11--4,22)),
                             App
                               (LongIdent (SynLongIdent ([list], [], [None])),
                                None,
                                [LongIdent (SynLongIdent ([int], [], [None]))],
                                [], None, true, (4,24--4,32)), (4,11--4,32)),
                          (4,10--4,11), Some (4,32--4,33), (4,10--4,33)),
                       (4,4--4,33), { YieldOrReturnKeyword = (4,4--4,9) }),
                    (3,4--5,1)), (3,0--5,1)), (3,0--5,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
