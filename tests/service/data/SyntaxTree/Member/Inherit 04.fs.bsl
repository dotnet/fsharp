ImplFile
  (ParsedImplFileInput
     ("/root/Member/Inherit 04.fs", false, QualifiedNameOfFile Module, [], [],
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
                     [Inherit
                        (None, None, (4,4--4,11),
                         { InheritKeyword = (4,4--4,11) })], (4,4--4,11)), [],
                  None, (3,5--4,11), { LeadingKeyword = Type (3,0--3,4)
                                       EqualsRange = Some (3,7--3,8)
                                       WithKeyword = None })], (3,0--4,11));
           Expr (Ident I, (6,0--6,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,4)-(4,11) parse error Type name cannot be empty.
