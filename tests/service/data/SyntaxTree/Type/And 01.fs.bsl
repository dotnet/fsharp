ImplFile
  (ParsedImplFileInput
     ("/root/Type/And 01.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [A],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  Simple
                    (TypeAbbrev
                       (Ok, LongIdent (SynLongIdent ([int], [], [None])),
                        (3,9--3,12)), (3,9--3,12)), [], None, (3,5--3,12),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None });
               SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [],
                     PreXmlDoc ((7,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (7,0--7,3)),
                  Simple (None (7,0--7,3), (7,0--7,3)), [], None, (7,0--7,3),
                  { LeadingKeyword = And (5,0--5,3)
                    EqualsRange = None
                    WithKeyword = None });
               SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [C],
                     PreXmlDoc ((7,4), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (7,4--7,5)),
                  Simple
                    (TypeAbbrev
                       (Ok, LongIdent (SynLongIdent ([int], [], [None])),
                        (7,8--7,11)), (7,8--7,11)), [], None, (7,4--7,11),
                  { LeadingKeyword = And (7,0--7,3)
                    EqualsRange = Some (7,6--7,7)
                    WithKeyword = None })], (3,0--7,11))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,11), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(7,0)-(7,3) parse error Unexpected keyword 'and' in type name
