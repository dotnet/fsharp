ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang 15.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (App
                (NonAtomic, false, Ident async,
                 ComputationExpr
                   (false,
                    LetOrUse
                      (false, false, true, true,
                       [SynBinding
                          (None, Normal, false, false, [], PreXmlDocEmpty,
                           SynValData
                             (None,
                              SynValInfo ([], SynArgInfo ([], false, None)),
                              None),
                           Typed
                             (Paren
                                (As
                                   (Named
                                      (SynIdent (x, None), false, None,
                                       (4,10--4,11)),
                                    Named
                                      (SynIdent (y, None), false, None,
                                       (4,15--4,16)), (4,10--4,16)), (4,9--4,17)),
                              FromParseError (4,18--4,18), (4,9--4,18)), None,
                           App
                             (Atomic, false, Ident asyncString,
                              Const (Unit, (5,39--5,41)), (5,28--5,41)),
                           (4,4--6,16), Yes (4,4--5,41),
                           { LeadingKeyword = Let (4,4--4,8)
                             InlineKeyword = None
                             EqualsRange = Some (5,26--5,27) })],
                       YieldOrReturn
                         ((false, true),
                          App
                            (NonAtomic, false,
                             App
                               (NonAtomic, true,
                                LongIdent
                                  (false,
                                   SynLongIdent
                                     ([op_Addition], [],
                                      [Some (OriginalNotation "+")]), None,
                                   (6,13--6,14)), Ident x, (6,11--6,14)),
                             Ident b, (6,11--6,16)), (6,4--6,16),
                          { YieldOrReturnKeyword = (6,4--6,10) }), (4,4--6,16),
                       { LetOrUseKeyword = (4,4--4,8)
                         InKeyword = None
                         EqualsRange = Some (5,26--5,27) }), (3,6--7,1)),
                 (3,0--7,1)), (3,0--7,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(5,4)-(5,8) parse error Incomplete structured construct at or before this point in expression
(4,18)-(4,18) parse error Expecting type
