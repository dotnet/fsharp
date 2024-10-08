ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Typed - Missing type 06.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Do
                (LetOrUse
                   (false, false,
                    [SynBinding
                       (None, Normal, false, false, [],
                        PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                        SynValData
                          (None, SynValInfo ([], SynArgInfo ([], false, None)),
                           None),
                        Named (SynIdent (i, None), false, None, (4,8--4,9)),
                        Some
                          (SynBindingReturnInfo
                             (FromParseError (4,10--4,10), (4,10--4,10), [],
                              { ColonRange = Some (4,9--4,10) })),
                        Typed
                          (Const (Int32 1, (4,13--4,14)),
                           FromParseError (4,10--4,10), (4,13--4,14)),
                        (4,8--4,9), Yes (4,4--4,14),
                        { LeadingKeyword = Let (4,4--4,7)
                          InlineKeyword = None
                          EqualsRange = Some (4,11--4,12) })],
                    Const (Unit, (6,4--6,6)), (4,4--6,6),
                    { LetOrUseKeyword = (4,4--4,7)
                      InKeyword = None }), (3,0--6,6)), (3,0--6,6))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,6), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,11)-(4,12) parse error Unexpected symbol '=' in binding
