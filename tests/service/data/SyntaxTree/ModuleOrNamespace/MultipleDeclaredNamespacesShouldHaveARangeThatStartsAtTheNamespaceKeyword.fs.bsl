ImplFile
  (ParsedImplFileInput
     ("/root/ModuleOrNamespace/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs",
      false,
      QualifiedNameOfFile
        MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword,
      [], [],
      [SynModuleOrNamespace
         ([TypeEquality], false, DeclaredNamespace,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Teq],
                     PreXmlDoc ((5,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/ModuleOrNamespace/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (5,5--5,8)),
                  ObjectModel
                    (Class, [],
                     /root/ModuleOrNamespace/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (5,11--5,20)),
                  [], None,
                  /root/ModuleOrNamespace/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (4,0--5,20),
                  { LeadingKeyword =
                     Type
                       /root/ModuleOrNamespace/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (5,0--5,4)
                    EqualsRange =
                     Some
                       /root/ModuleOrNamespace/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (5,9--5,10)
                    WithKeyword = None })],
              /root/ModuleOrNamespace/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (4,0--5,20))],
          PreXmlDocEmpty, [], None,
          /root/ModuleOrNamespace/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (2,0--5,20),
          { LeadingKeyword =
             Namespace
               /root/ModuleOrNamespace/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (2,0--2,9) });
       SynModuleOrNamespace
         ([Foobar], false, DeclaredNamespace,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((9,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named
                    (SynIdent (x, None), false, None,
                     /root/ModuleOrNamespace/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (9,4--9,5)),
                  None,
                  Const
                    (Int32 42,
                     /root/ModuleOrNamespace/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (9,8--9,10)),
                  /root/ModuleOrNamespace/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (9,4--9,5),
                  Yes
                    /root/ModuleOrNamespace/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (9,0--9,10),
                  { LeadingKeyword =
                     Let
                       /root/ModuleOrNamespace/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (9,0--9,3)
                    InlineKeyword = None
                    EqualsRange =
                     Some
                       /root/ModuleOrNamespace/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (9,6--9,7) })],
              /root/ModuleOrNamespace/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (9,0--9,10))],
          PreXmlDocEmpty, [], None,
          /root/ModuleOrNamespace/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (7,0--9,10),
          { LeadingKeyword =
             Namespace
               /root/ModuleOrNamespace/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (7,0--7,9) })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))
