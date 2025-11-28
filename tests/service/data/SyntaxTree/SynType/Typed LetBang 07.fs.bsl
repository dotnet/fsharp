ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang 07.fs", false, QualifiedNameOfFile Module, [],
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
                             Paren
                               (Typed
                                  (Record
                                     ([NamePatPairField
                                         (SynLongIdent ([Name], [], [None]),
                                          Some (3,17--3,18), (3,12--3,23),
                                          Named
                                            (SynIdent (name, None), false, None,
                                             (3,19--3,23)), None)], (3,10--3,25)),
                                   LongIdent
                                     (SynLongIdent ([Person], [], [None])),
                                   (3,10--3,33)), (3,9--3,34)), None,
                             App
                               (Atomic, false, Ident asyncPerson,
                                Const (Unit, (3,48--3,50)), (3,37--3,50)),
                             (3,4--3,50), Yes (3,4--3,50),
                             { LeadingKeyword = LetBang (3,4--3,8)
                               InlineKeyword = None
                               EqualsRange = Some (3,35--3,36) })]
                        Body =
                         YieldOrReturn
                           ((false, true), Ident name, (5,4--5,15),
                            { YieldOrReturnKeyword = (5,4--5,10) })
                        Range = (3,4--5,15)
                        Trivia = { InKeyword = None }
                        IsFromSource = true }, (2,6--7,1)), (2,0--7,1)),
              (2,0--7,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
