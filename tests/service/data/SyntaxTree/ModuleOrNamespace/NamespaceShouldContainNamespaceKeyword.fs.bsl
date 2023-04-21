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
                      PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([], SynArgInfo ([], false, None)),
                         None),
                      Named (SynIdent (a, None), false, None, (4,8--4,9)), None,
                      Const (Int32 42, (4,12--4,14)), (4,8--4,9),
                      Yes (4,4--4,14), { LeadingKeyword = Let (4,4--4,7)
                                         InlineKeyword = None
                                         EqualsRange = Some (4,10--4,11) })],
                  (4,4--4,14))], false, (3,0--4,14),
              { ModuleKeyword = Some (3,0--3,6)
                EqualsRange = Some (3,11--3,12) })], PreXmlDocEmpty, [], None,
          (2,0--4,14), { LeadingKeyword = Namespace (2,0--2,9) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
