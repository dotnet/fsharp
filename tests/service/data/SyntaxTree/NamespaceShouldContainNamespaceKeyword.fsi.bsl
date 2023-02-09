SigFile
  (ParsedSigFileInput
     ("/root/NamespaceShouldContainNamespaceKeyword.fsi",
      QualifiedNameOfFile NamespaceShouldContainNamespaceKeyword, [], [],
      [SynModuleOrNamespaceSig
         ([Foo], false, DeclaredNamespace,
          [NestedModule
             (SynComponentInfo
                ([], None, [], [Bar],
                 PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None,
                 /root/NamespaceShouldContainNamespaceKeyword.fsi (2,0--2,10)),
              false,
              [Val
                 (SynValSig
                    ([], SynIdent (a, None), SynValTyparDecls (None, true),
                     LongIdent (SynLongIdent ([int], [], [None])),
                     SynValInfo ([], SynArgInfo ([], false, None)), false, false,
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     None, None,
                     /root/NamespaceShouldContainNamespaceKeyword.fsi (3,0--3,10),
                     { LeadingKeyword =
                        Val
                          /root/NamespaceShouldContainNamespaceKeyword.fsi (3,0--3,3)
                       InlineKeyword = None
                       WithKeyword = None
                       EqualsRange = None }),
                  /root/NamespaceShouldContainNamespaceKeyword.fsi (3,0--3,10))],
              /root/NamespaceShouldContainNamespaceKeyword.fsi (2,0--3,10),
              { ModuleKeyword =
                 Some
                   /root/NamespaceShouldContainNamespaceKeyword.fsi (2,0--2,6)
                EqualsRange =
                 Some
                   /root/NamespaceShouldContainNamespaceKeyword.fsi (2,11--2,12) })],
          PreXmlDocEmpty, [], None,
          /root/NamespaceShouldContainNamespaceKeyword.fsi (1,0--3,10),
          { LeadingKeyword =
             Namespace
               /root/NamespaceShouldContainNamespaceKeyword.fsi (1,0--1,9) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))