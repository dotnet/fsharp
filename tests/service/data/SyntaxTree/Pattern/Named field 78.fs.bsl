ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Named field 78.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Paren
                    (LongIdent
                       (SynLongIdent ([CaseA], [], [None]), None, None,
                        NamePatPairs
                          ([NamePatPairField
                              (SynLongIdent ([x], [], [None]), Some (3,13--3,14),
                               (3,11--3,16),
                               Named
                                 (SynIdent (a, None), false, None, (3,15--3,16)),
                               Some (Semicolon ((3,16--3,17), Some (3,17))));
                            NamePatPairField
                              (SynLongIdent ([y], [], [None]), Some (3,20--3,21),
                               (3,18--3,23),
                               Named
                                 (SynIdent (b, None), false, None, (3,22--3,23)),
                               None)], (3,11--3,24),
                           { ParenRange = (3,10--3,24) }), None, (3,5--3,24)),
                     (3,4--3,25)), None, Const (Unit, (3,28--3,30)), (3,4--3,25),
                  Yes (3,0--3,30), { LeadingKeyword = Let (3,0--3,3)
                                     InlineKeyword = None
                                     EqualsRange = Some (3,26--3,27) })],
              (3,0--3,30))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,30), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
