ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang 10.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (App
                (NonAtomic, false, Ident async,
                 ComputationExpr
                   (false,
                    LetOrUse
                      { IsRecursive = false
                        Bindings =
                         [SynBinding
                            (None, Normal, false, false, [], PreXmlDocEmpty,
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None),
                             LongIdent
                               (SynLongIdent ([Union], [], [None]), None, None,
                                Pats
                                  [Named
                                     (SynIdent (value, None), false, None,
                                      (3,15--3,20))], None, (3,9--3,20)), None,
                             App
                               (Atomic, false, Ident asyncOption,
                                Const (Unit, (3,34--3,36)), (3,23--3,36)),
                             (3,4--3,36), Yes (3,4--3,36),
                             { LeadingKeyword = LetBang (3,4--3,8)
                               InlineKeyword = None
                               EqualsRange = Some (3,21--3,22) })]
                        Body =
                         YieldOrReturn
                           ((false, true), Ident value, (4,4--4,16),
                            { YieldOrReturnKeyword = (4,4--4,10) })
                        Range = (3,4--4,16)
                        Trivia = { InKeyword = None }
                        IsFromSource = true }, (2,6--5,1)), (2,0--5,1)),
              (2,0--5,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
