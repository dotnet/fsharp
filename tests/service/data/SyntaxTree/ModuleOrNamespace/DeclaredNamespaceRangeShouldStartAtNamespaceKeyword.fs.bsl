ImplFile
  (ParsedImplFileInput
     ("/root/ModuleOrNamespace/DeclaredNamespaceRangeShouldStartAtNamespaceKeyword.fs",
      false,
      QualifiedNameOfFile DeclaredNamespaceRangeShouldStartAtNamespaceKeyword,
      [], [],
      [SynModuleOrNamespace
         ([TypeEquality], false, DeclaredNamespace,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([],
                     Some
                       (PostfixList
                          ([SynTyparDecl ([], SynTypar (a, None, false));
                            SynTyparDecl ([], SynTypar (b, None, false))], [],
                           (5,8--5,16))), [], [Teq],
                     PreXmlDoc ((5,0), FSharp.Compiler.Xml.XmlDocCollector),
                     true, None, (5,5--5,8)),
                  Simple (None (5,5--5,8), (5,5--5,8)), [], None, (4,0--5,8),
                  { LeadingKeyword = Type (5,0--5,4)
                    EqualsRange = None
                    WithKeyword = None })], (4,0--5,8))], PreXmlDocEmpty, [],
          None, (2,0--5,8), { LeadingKeyword = Namespace (2,0--2,9) })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))
