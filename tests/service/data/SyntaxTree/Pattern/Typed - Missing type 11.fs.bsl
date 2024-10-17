ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Typed - Missing type 11.fs", false,
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
                          (None,
                           SynValInfo
                             ([[SynArgInfo ([], false, Some i)]],
                              SynArgInfo ([], false, None)), None),
                        LongIdent
                          (SynLongIdent ([f], [], [None]), None, None,
                           Pats
                             [Paren
                                (Typed
                                   (Named
                                      (SynIdent (i, None), false, None,
                                       (4,11--4,12)),
                                    FromParseError (4,13--4,13), (4,11--4,13)),
                                 (4,10--4,14))], None, (4,8--4,14)), None,
                        Const (Int32 1, (4,17--4,18)), (4,8--4,14), NoneAtLet,
                        { LeadingKeyword = Let (4,4--4,7)
                          InlineKeyword = None
                          EqualsRange = Some (4,15--4,16) })],
                    Const (Unit, (6,4--6,6)), (4,4--6,6),
                    { LetOrUseKeyword = (4,4--4,7)
                      InKeyword = None }), (3,0--6,6)), (3,0--6,6))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,6), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,13)-(4,14) parse error Unexpected symbol ')' in pattern
