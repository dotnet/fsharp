ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang 17.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Paren
                    (Typed
                       (As
                          (LongIdent
                             (SynLongIdent ([Even], [], [None]), None, None,
                              Pats [], None, (3,5--3,9)),
                           Named (SynIdent (x, None), false, None, (3,13--3,14)),
                           (3,5--3,14)),
                        LongIdent (SynLongIdent ([int], [], [None])),
                        (3,5--3,19)), (3,4--3,20)), None,
                  Const (Int32 1, (3,23--3,24)), (3,4--3,20), Yes (3,0--3,24),
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,21--3,22) })], (3,0--3,24));
           Expr
             (LetOrUse
                (false, false, true, true,
                 [SynBinding
                    (None, Normal, false, false, [], PreXmlDocEmpty,
                     SynValData
                       (None, SynValInfo ([], SynArgInfo ([], false, None)),
                        None),
                     Paren
                       (Typed
                          (As
                             (LongIdent
                                (SynLongIdent ([Even], [], [None]), None, None,
                                 Pats [], None, (5,6--5,10)),
                              Named
                                (SynIdent (x, None), false, None, (5,14--5,15)),
                              (5,6--5,15)),
                           LongIdent (SynLongIdent ([int], [], [None])),
                           (5,6--5,20)), (5,5--5,21)), None,
                     App
                       (NonAtomic, false, Ident async,
                        ComputationExpr
                          (false,
                           YieldOrReturn
                             ((false, true), Const (Int32 2, (5,39--5,40)),
                              (5,32--5,40),
                              { YieldOrReturnKeyword = (5,32--5,38) }),
                           (5,30--5,42)), (5,24--5,42)), (5,0--5,42),
                     Yes (5,0--5,42), { LeadingKeyword = Let (5,0--5,4)
                                        InlineKeyword = None
                                        EqualsRange = Some (5,22--5,23) })],
                 ImplicitZero (5,42--5,42), (5,0--5,42),
                 { LetOrUseKeyword = (5,0--5,4)
                   InKeyword = None
                   EqualsRange = Some (5,22--5,23) }), (5,0--5,42))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,42), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(5,0)-(5,42) parse error Incomplete structured construct at or before this point in expression
