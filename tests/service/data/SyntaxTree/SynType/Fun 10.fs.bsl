ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Fun 10.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Typed
                    (Wild (3,4--3,5),
                     Fun
                       (LongIdent (SynLongIdent ([a], [], [None])),
                        Fun
                          (FromParseError (3,12--3,12),
                           LongIdent (SynLongIdent ([c], [], [None])),
                           (3,12--3,16), { ArrowRange = (3,12--3,14) }),
                        (3,7--3,16), { ArrowRange = (3,9--3,11) }), (3,4--3,16)),
                  None, Const (Unit, (3,19--3,21)), (3,4--3,16), Yes (3,0--3,21),
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,17--3,18) })], (3,0--3,21))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,21), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,12)-(3,14) parse error Expecting type
