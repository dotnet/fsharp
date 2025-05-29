ImplFile
  (ParsedImplFileInput
     ("/root/Type/Record 01.fs", false, QualifiedNameOfFile Foo, [],
      [SynModuleOrNamespace
         ([Foo], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [AU],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,7)),
                  Simple
                    (Record
                       (None,
                        [SynField
                           ([], false, Some Invest,
                            LongIdent (SynLongIdent ([int], [], [None])), false,
                            PreXmlDoc ((5,8), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (5,8--5,19), { LeadingKeyword = None
                                                 MutableKeyword = None });
                         SynField
                           ([], false, Some T, FromParseError (6,9--6,9), false,
                            PreXmlDoc ((6,8), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (6,8--6,9), { LeadingKeyword = None
                                                MutableKeyword = None })],
                        (4,4--7,5)), (4,4--7,5)), [], None, (3,5--7,5),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,8--3,9)
                    WithKeyword = None })], (3,0--7,5));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((9,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (meh, None), false, None, (9,4--9,7)), None,
                  Const (Unit, (9,10--9,12)), (9,4--9,7), Yes (9,0--9,12),
                  { LeadingKeyword = Let (9,0--9,3)
                    InlineKeyword = None
                    EqualsRange = Some (9,8--9,9) })], (9,0--9,12))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--9,12), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(7,4)-(7,5) parse error Unexpected symbol '}' in field declaration. Expected ':' or other token.
