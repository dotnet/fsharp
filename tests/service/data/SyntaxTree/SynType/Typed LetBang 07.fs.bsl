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
                      (false, false, true, true,
                       [SynBinding
                          (None, Normal, false, false, [], PreXmlDocEmpty,
                           SynValData
                             (None,
                              SynValInfo ([], SynArgInfo ([], false, None)),
                              None),
                           Paren
                             (Typed
                                (Record
                                   ([(([], Name), Some (3,17--3,18),
                                      Named
                                        (SynIdent (name, None), false, None,
                                         (3,19--3,23)))], (3,10--3,25)),
                                 LongIdent (SynLongIdent ([Person], [], [None])),
                                 (3,10--3,33)), (3,9--3,34)), None,
                           App
                             (Atomic, false, Ident asyncPerson,
                              Const (Unit, (3,48--3,50)), (3,37--3,50)),
                           (3,4--5,15), Yes (3,4--3,50),
                           { LeadingKeyword = Let (3,4--3,8)
                             InlineKeyword = None
                             EqualsRange = Some (3,35--3,36) })],
                       YieldOrReturn
                         ((false, true), Ident name, (5,4--5,15),
                          { YieldOrReturnKeyword = (5,4--5,10) }), (3,4--5,15),
                       { LetOrUseKeyword = (3,4--3,8)
                         InKeyword = None
                         EqualsRange = Some (3,35--3,36) }), (2,6--7,1)),
                 (2,0--7,1)), (2,0--7,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
