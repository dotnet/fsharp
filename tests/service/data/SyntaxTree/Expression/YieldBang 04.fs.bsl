ImplFile
  (ParsedImplFileInput
     ("/root/Expression/YieldBang 04.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (App
                (NonAtomic, false, Ident seq,
                 ComputationExpr
                   (false,
                    YieldOrReturnFrom
                      ((true, false),
                       Paren
                         (Typed
                            (ArrayOrListComputed
                               (false,
                                IndexRange
                                  (Some (Const (Int32 1, (4,14--4,15))),
                                   (4,16--4,18),
                                   Some (Const (Int32 10, (4,19--4,21))),
                                   (4,14--4,15), (4,19--4,21), (4,14--4,21)),
                                (4,12--4,23)),
                             App
                               (LongIdent (SynLongIdent ([list], [], [None])),
                                None,
                                [LongIdent (SynLongIdent ([int], [], [None]))],
                                [], None, true, (4,25--4,33)), (4,12--4,33)),
                          (4,11--4,12), Some (4,33--4,34), (4,11--4,34)),
                       (4,4--4,34), { YieldOrReturnFromKeyword = (4,4--4,10) }),
                    (3,4--5,1)), (3,0--5,1)), (3,0--5,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
