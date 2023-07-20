ImplFile
  (ParsedImplFileInput
     ("/root/Type/Type 05.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [T1],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,7)),
                  Simple (None (3,5--3,9), (3,5--3,9)), [], None, (3,5--3,9),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,8--3,9)
                    WithKeyword = None });
               SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [T2],
                     PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (5,4--5,6)),
                  Simple
                    (TypeAbbrev
                       (Ok, LongIdent (SynLongIdent ([int], [], [None])),
                        (5,9--5,12)), (5,9--5,12)), [], None, (5,4--5,12),
                  { LeadingKeyword = And (5,0--5,3)
                    EqualsRange = Some (5,7--5,8)
                    WithKeyword = None })], (3,0--5,12))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,12), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,5)-(3,9) parse error A type definition requires one or more members or other declarations. If you intend to define an empty class, struct or interface, then use 'type ... = class end', 'interface end' or 'struct end'.
