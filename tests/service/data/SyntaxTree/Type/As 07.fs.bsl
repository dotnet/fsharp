ImplFile
  (ParsedImplFileInput
     ("/root/Type/As 07.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [T],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  Simple (None (3,5--3,6), (3,5--3,6)),
                  [ImplicitCtor
                     (None, [], SimplePats ([], [], (3,6--3,8)), Some this,
                      PreXmlDoc ((3,6), FSharp.Compiler.Xml.XmlDocCollector),
                      (3,5--3,6), { AsKeyword = Some (3,9--3,11) })], None,
                  (3,5--3,6), { LeadingKeyword = Type (3,0--3,4)
                                EqualsRange = None
                                WithKeyword = None })], (3,0--3,6));
           Expr (Const (Unit, (5,0--5,2)), (5,0--5,2))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,2), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,17)-(5,0) parse error Incomplete structured construct at or before this point in type definition. Expected '=' or other token.
