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
                 None,
                 /root/ModuleOrNamespace/NamespaceShouldContainNamespaceKeyword.fs (3,0--3,10)),
              false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([], SynArgInfo ([], false, None)),
                         None),
                      Named
                        (SynIdent (a, None), false, None,
                         /root/ModuleOrNamespace/NamespaceShouldContainNamespaceKeyword.fs (4,4--4,5)),
                      None,
                      Const
                        (Int32 42,
                         /root/ModuleOrNamespace/NamespaceShouldContainNamespaceKeyword.fs (4,8--4,10)),
                      /root/ModuleOrNamespace/NamespaceShouldContainNamespaceKeyword.fs (4,4--4,5),
                      Yes
                        /root/ModuleOrNamespace/NamespaceShouldContainNamespaceKeyword.fs (4,0--4,10),
                      { LeadingKeyword =
                         Let
                           /root/ModuleOrNamespace/NamespaceShouldContainNamespaceKeyword.fs (4,0--4,3)
                        InlineKeyword = None
                        EqualsRange =
                         Some
                           /root/ModuleOrNamespace/NamespaceShouldContainNamespaceKeyword.fs (4,6--4,7) })],
                  /root/ModuleOrNamespace/NamespaceShouldContainNamespaceKeyword.fs (4,0--4,10))],
              false,
              /root/ModuleOrNamespace/NamespaceShouldContainNamespaceKeyword.fs (3,0--4,10),
              { ModuleKeyword =
                 Some
                   /root/ModuleOrNamespace/NamespaceShouldContainNamespaceKeyword.fs (3,0--3,6)
                EqualsRange =
                 Some
                   /root/ModuleOrNamespace/NamespaceShouldContainNamespaceKeyword.fs (3,11--3,12) })],
          PreXmlDocEmpty, [], None,
          /root/ModuleOrNamespace/NamespaceShouldContainNamespaceKeyword.fs (2,0--4,10),
          { LeadingKeyword =
             Namespace
               /root/ModuleOrNamespace/NamespaceShouldContainNamespaceKeyword.fs (2,0--2,9) })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))