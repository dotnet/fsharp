ImplFile
  (ParsedImplFileInput
     ("/root/Member/Interface 10.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [T],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  ObjectModel
                    (Unspecified,
                     [Interface
                        (FromParseError (4,13--4,13), None, None, (4,4--4,13))],
                     (4,4--4,13)), [], None, (3,5--4,13),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--4,13));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [T2],
                     PreXmlDoc ((6,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (6,5--6,7)),
                  Simple
                    (TypeAbbrev
                       (Ok, LongIdent (SynLongIdent ([int], [], [None])),
                        (6,10--6,13)), (6,10--6,13)), [], None, (6,5--6,13),
                  { LeadingKeyword = Type (6,0--6,4)
                    EqualsRange = Some (6,8--6,9)
                    WithKeyword = None })], (6,0--6,13));
           Expr (Const (Unit, (8,0--8,2)), (8,0--8,2))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--8,2), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(6,0)-(6,4) parse error Incomplete structured construct at or before this point in member definition
