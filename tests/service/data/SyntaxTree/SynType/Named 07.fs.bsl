ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Named 07.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Tuple
                    (false,
                     [Named
                        (SynIdent (a, None), false, Some (Private (3,4--3,11)),
                         (3,4--3,13));
                      Named (SynIdent (b, None), false, None, (3,15--3,16))],
                     [(3,13--3,14)], (3,4--3,16)), None,
                  Tuple
                    (false,
                     [Const (Int32 1, (3,19--3,20));
                      Const (Int32 2, (3,22--3,23))], [(3,20--3,21)],
                     (3,19--3,23)), (3,4--3,16), Yes (3,0--3,23),
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,17--3,18) })], (3,0--3,23))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,23), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
