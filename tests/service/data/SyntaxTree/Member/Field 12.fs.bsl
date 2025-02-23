ImplFile
  (ParsedImplFileInput
     ("/root/Member/Field 12.fs", false, QualifiedNameOfFile Module, [],
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
                     [ValField
                        (SynField
                           ([], true, Some F, FromParseError (4,32--4,32), true,
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            Some (Private (4,23--4,30)), (4,4--4,32),
                            { LeadingKeyword =
                               Some (StaticVal ((4,4--4,10), (4,11--4,14)))
                              MutableKeyword = Some (4,15--4,22) }), (4,4--4,32))],
                     (4,4--4,32)), [], None, (3,5--4,32),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--4,32));
           Expr (Const (Unit, (6,0--6,2)), (6,0--6,2))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,2), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(6,0)-(6,1) parse error Incomplete structured construct at or before this point in type definition. Expected ':' or other token.
