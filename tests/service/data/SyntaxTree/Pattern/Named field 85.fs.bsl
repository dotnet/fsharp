ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Named field 85.fs", false, QualifiedNameOfFile Module, [],
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
                               Some (Comma ((3,16--3,17), Some (3,17))));
                            NamePatPairField
                              (SynLongIdent ([y], [], [None]), Some (3,20--3,21),
                               (3,18--3,26),
                               Tuple
                                 (false,
                                  [Named
                                     (SynIdent (b, None), false, None,
                                      (3,22--3,23)); Wild (3,25--3,26)],
                                  [(3,23--3,24)], (3,22--3,26)), None)],
                           (3,11--3,26), { ParenRange = (3,10--3,27) }), None,
                        (3,5--3,27)), (3,4--3,28)), None,
                  Const (Unit, (3,31--3,33)), (3,4--3,28), Yes (3,0--3,33),
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,29--3,30) })], (3,0--3,33))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,33), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
