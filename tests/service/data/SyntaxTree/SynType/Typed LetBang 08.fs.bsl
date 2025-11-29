ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang 08.fs", false, QualifiedNameOfFile Module, [],
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
                             Record
                               ([NamePatPairField
                                   (SynLongIdent ([Name], [], [None]),
                                    Some (3,16--3,17), (3,11--3,22),
                                    Named
                                      (SynIdent (name, None), false, None,
                                       (3,18--3,22)), None)], (3,9--3,24)),
                             Some
                               (SynBindingReturnInfo
                                  (LongIdent
                                     (SynLongIdent ([Person], [], [None])),
                                   (3,26--3,32), [],
                                   { ColonRange = Some (3,24--3,25) })),
                             App
                               (Atomic, false, Ident asyncPerson,
                                Const (Unit, (3,46--3,48)), (3,35--3,48)),
                             (3,4--3,48), Yes (3,4--3,48),
                             { LeadingKeyword = LetBang (3,4--3,8)
                               InlineKeyword = None
                               EqualsRange = Some (3,33--3,34) })]
                        Body =
                         YieldOrReturn
                           ((false, true), Ident name, (4,4--4,15),
                            { YieldOrReturnKeyword = (4,4--4,10) })
                        Range = (3,4--4,15)
                        Trivia = { InKeyword = None }
                        IsFromSource = true }, (2,6--6,1)), (2,0--6,1)),
              (2,0--6,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
