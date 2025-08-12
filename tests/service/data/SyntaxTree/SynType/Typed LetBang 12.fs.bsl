ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang 12.fs", false, QualifiedNameOfFile Module, [],
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
                             (LongIdent
                                (SynLongIdent ([Union], [], [None]), None, None,
                                 Pats
                                   [Named
                                      (SynIdent (value, None), false, None,
                                       (3,15--3,20))], None, (3,9--3,20)),
                              App
                                (LongIdent (SynLongIdent ([option], [], [None])),
                                 None,
                                 [LongIdent (SynLongIdent ([int], [], [None]))],
                                 [], None, true, (3,22--3,32)), (3,9--3,32)),
                           None,
                           App
                             (Atomic, false, Ident asyncOption,
                              Const (Unit, (3,46--3,48)), (3,35--3,48)),
                           (3,4--4,16), Yes (3,4--3,48),
                           { LeadingKeyword = Let (3,4--3,8)
                             InlineKeyword = None
                             EqualsRange = Some (3,33--3,34) })],
                       YieldOrReturn
                         ((false, true), Ident value, (4,4--4,16),
                          { YieldOrReturnKeyword = (4,4--4,10) }), (3,4--4,16),
                       { LetOrUseKeyword = (3,4--3,8)
                         InKeyword = None
                         EqualsRange = Some (3,33--3,34) }), (2,6--5,1)),
                 (2,0--5,1)), (2,0--5,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
