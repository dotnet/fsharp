ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/As 09.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  As (Wild (3,4--3,5), Wild (3,8--3,8), (3,4--3,8)), None,
                  ArbitraryAfterError ("localBinding2", (3,8--3,8)), (3,4--3,8),
                  Yes (3,0--3,8), { LeadingKeyword = Let (3,0--3,3)
                                    InlineKeyword = None
                                    EqualsRange = None })], (3,0--3,8));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((5,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Wild (5,4--5,5), None, Const (Unit, (5,8--5,10)), (5,4--5,5),
                  Yes (5,0--5,10), { LeadingKeyword = Let (5,0--5,3)
                                     InlineKeyword = None
                                     EqualsRange = Some (5,6--5,7) })],
              (5,0--5,10))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,10), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(3,6)-(3,8) parse error Expecting pattern
(5,0)-(5,3) parse error Incomplete structured construct at or before this point in binding. Expected '=' or other token.
