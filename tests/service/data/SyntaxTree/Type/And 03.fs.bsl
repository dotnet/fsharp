ImplFile
  (ParsedImplFileInput
     ("/root/Type/And 03.fs", false, QualifiedNameOfFile Module, [], [],
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
                    ([], None, [], [B],
                     PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (5,4--5,5)),
                  Simple (None (5,4--5,7), (5,4--5,7)), [], None, (5,4--5,7),
                  { LeadingKeyword = And (5,0--5,3)
                    EqualsRange = Some (5,6--5,7)
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
        CodeComments = [] }, set []))

(5,4)-(5,7) parse error A type definition requires one or more members or other declarations. If you intend to define an empty class, struct or interface, then use 'type ... = class end', 'interface end' or 'struct end'.
