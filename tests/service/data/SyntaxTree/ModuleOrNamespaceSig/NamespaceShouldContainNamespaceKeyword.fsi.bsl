SigFile
  (ParsedSigFileInput
     ("/root/ModuleOrNamespaceSig/NamespaceShouldContainNamespaceKeyword.fsi",
      QualifiedNameOfFile NamespaceShouldContainNamespaceKeyword, [], [],
      [SynModuleOrNamespaceSig
         ([Foo], false, DeclaredNamespace,
          [NestedModule
             (SynComponentInfo
                ([], None, [], [Bar],
                 PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None,
                 /root/ModuleOrNamespaceSig/NamespaceShouldContainNamespaceKeyword.fsi (3,0--3,10)),
              false,
              [Val
                 (SynValSig
                    ([], SynIdent (a, None), SynValTyparDecls (None, true),
                     LongIdent (SynLongIdent ([int], [], [None])),
                     SynValInfo ([], SynArgInfo ([], false, None)), false, false,
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     None, None,
                     /root/ModuleOrNamespaceSig/NamespaceShouldContainNamespaceKeyword.fsi (4,0--4,10),
                     { LeadingKeyword =
                        Val
                          /root/ModuleOrNamespaceSig/NamespaceShouldContainNamespaceKeyword.fsi (4,0--4,3)
                       InlineKeyword = None
                       WithKeyword = None
                       EqualsRange = None }),
                  /root/ModuleOrNamespaceSig/NamespaceShouldContainNamespaceKeyword.fsi (4,0--4,10))],
              /root/ModuleOrNamespaceSig/NamespaceShouldContainNamespaceKeyword.fsi (3,0--4,10),
              { ModuleKeyword =
                 Some
                   /root/ModuleOrNamespaceSig/NamespaceShouldContainNamespaceKeyword.fsi (3,0--3,6)
                EqualsRange =
                 Some
                   /root/ModuleOrNamespaceSig/NamespaceShouldContainNamespaceKeyword.fsi (3,11--3,12) })],
          PreXmlDocEmpty, [], None,
          /root/ModuleOrNamespaceSig/NamespaceShouldContainNamespaceKeyword.fsi (2,0--4,10),
          { LeadingKeyword =
             Namespace
               /root/ModuleOrNamespaceSig/NamespaceShouldContainNamespaceKeyword.fsi (2,0--2,9) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
