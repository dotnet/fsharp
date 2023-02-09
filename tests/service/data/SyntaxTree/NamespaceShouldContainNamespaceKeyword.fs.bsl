ImplFile
  (ParsedImplFileInput
     ("/root/NamespaceShouldContainNamespaceKeyword.fs", false,
      QualifiedNameOfFile NamespaceShouldContainNamespaceKeyword, [], [],
      [SynModuleOrNamespace
         ([Foo], false, DeclaredNamespace,
          [NestedModule
             (SynComponentInfo
                ([], None, [], [Bar],
                 PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None,
                 /root/NamespaceShouldContainNamespaceKeyword.fs (3,0--3,10)),
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
                         /root/NamespaceShouldContainNamespaceKeyword.fs (4,4--4,5)),
                      None,
                      Const
                        (Int32 42,
                         /root/NamespaceShouldContainNamespaceKeyword.fs (4,8--4,10)),
                      /root/NamespaceShouldContainNamespaceKeyword.fs (4,4--4,5),
                      Yes
                        /root/NamespaceShouldContainNamespaceKeyword.fs (4,0--4,10),
                      { LeadingKeyword =
                         Let
                           /root/NamespaceShouldContainNamespaceKeyword.fs (4,0--4,3)
                        InlineKeyword = None
                        EqualsRange =
                         Some
                           /root/NamespaceShouldContainNamespaceKeyword.fs (4,6--4,7) })],
                  /root/NamespaceShouldContainNamespaceKeyword.fs (4,0--4,10))],
              false, /root/NamespaceShouldContainNamespaceKeyword.fs (3,0--4,10),
              { ModuleKeyword =
                 Some /root/NamespaceShouldContainNamespaceKeyword.fs (3,0--3,6)
                EqualsRange =
                 Some
                   /root/NamespaceShouldContainNamespaceKeyword.fs (3,11--3,12) })],
          PreXmlDocEmpty, [], None,
          /root/NamespaceShouldContainNamespaceKeyword.fs (2,0--4,10),
          { LeadingKeyword =
             Namespace
               /root/NamespaceShouldContainNamespaceKeyword.fs (2,0--2,9) })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))