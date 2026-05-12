ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Tuple 15.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (x, None), false, None, (3,4--3,5)),
                  Some
                    (SynBindingReturnInfo
                       (LongIdent (SynLongIdent ([int], [], [None])),
                        (3,7--3,10), [], { ColonRange = Some (3,5--3,6) })),
                  Typed
                    (ArbitraryAfterError ("localBinding2", (3,10--3,10)),
                     LongIdent (SynLongIdent ([int], [], [None])), (3,10--3,10)),
                  (3,4--3,5), Yes (3,0--3,10), { LeadingKeyword = Let (3,0--3,3)
                                                 InlineKeyword = None
                                                 EqualsRange = None })],
              (3,0--3,10), { InKeyword = None })],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,10), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(3,10)-(3,11) parse error Unexpected symbol ',' in binding. Expected '=' or other token.
