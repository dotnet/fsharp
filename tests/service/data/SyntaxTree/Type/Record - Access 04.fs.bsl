ImplFile
  (ParsedImplFileInput
     ("/root/Type/Record - Access 04.fs", false, QualifiedNameOfFile Module, [],
      [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [R],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  Simple
                    (Record
                       (None,
                        [SynField
                           ([], false, Some F,
                            LongIdent (SynLongIdent ([int], [], [None])), false,
                            PreXmlDoc ((5,8), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (5,8--5,23), { LeadingKeyword = None
                                                 MutableKeyword = None })],
                        (4,4--6,5)), (4,4--6,5)), [], None, (3,5--6,5),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--6,5));
           Expr (Const (Unit, (8,0--8,2)), (8,0--8,2))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--8,2), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(5,8)-(5,23) parse error Accessibility modifiers are not permitted on record fields. Use 'type R = internal ...' or 'type R = private ...' to give an accessibility to the whole representation.
