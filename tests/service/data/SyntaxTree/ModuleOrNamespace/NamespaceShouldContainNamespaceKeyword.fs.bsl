ImplFile
  (ParsedImplFileInput
     ("/root/ModuleOrNamespace/NamespaceShouldContainNamespaceKeyword.fs", false,
      QualifiedNameOfFile NamespaceShouldContainNamespaceKeyword, [], [],
      [SynModuleOrNamespace
         ([Foo], false, DeclaredNamespace,
          [NestedModule
             (SynComponentInfo
                ([], None, [], [Bar],
                 PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (3,0--3,10)), false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([], SynArgInfo ([], false, None)),
                         None),
                      Named (SynIdent (a, None), false, None, (4,4--4,5)), None,
                      Const (Int32 42, (4,8--4,10)), (4,4--4,5), Yes (4,0--4,10),
                      { LeadingKeyword = Let (4,0--4,3)
                        InlineKeyword = None
                        EqualsRange = Some (4,6--4,7) })], (4,0--4,10))], false,
              (3,0--4,10), { ModuleKeyword = Some (3,0--3,6)
                             EqualsRange = Some (3,11--3,12) })], PreXmlDocEmpty,
          [], None, (2,0--4,10), { LeadingKeyword = Namespace (2,0--2,9) })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))
