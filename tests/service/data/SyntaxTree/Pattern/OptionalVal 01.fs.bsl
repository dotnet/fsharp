ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/OptionalVal 01.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], true, Some x)]],
                        SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([f], [], [None]), None, None,
                     Pats
                       [Paren
                          (Typed
                             (OptionalVal (x, (3,7--3,9)),
                              LongIdent (SynLongIdent ([int], [], [None])),
                              (3,7--3,13)), (3,6--3,14))], None, (3,4--3,14)),
                  None, Const (Unit, (3,17--3,19)), (3,4--3,14), NoneAtLet,
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,15--3,16) })], (3,0--3,19))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,19), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
