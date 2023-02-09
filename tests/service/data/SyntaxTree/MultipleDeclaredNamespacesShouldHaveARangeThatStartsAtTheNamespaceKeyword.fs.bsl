ImplFile
  (ParsedImplFileInput
     ("/root/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs",
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
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (4,5--4,8)),
                  ObjectModel
                    (Class, [],
                     /root/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (4,11--4,20)),
                  [], None,
                  /root/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (3,0--4,20),
                  { LeadingKeyword =
                     Type
                       /root/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (4,0--4,4)
                    EqualsRange =
                     Some
                       /root/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (4,9--4,10)
                    WithKeyword = None })],
              /root/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (3,0--4,20))],
          PreXmlDocEmpty, [], None,
          /root/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (1,0--4,20),
          { LeadingKeyword =
             Namespace
               /root/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (1,0--1,9) });
       SynModuleOrNamespace
         ([Foobar], false, DeclaredNamespace,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((8,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named
                    (SynIdent (x, None), false, None,
                     /root/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (8,4--8,5)),
                  None,
                  Const
                    (Int32 42,
                     /root/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (8,8--8,10)),
                  /root/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (8,4--8,5),
                  Yes
                    /root/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (8,0--8,10),
                  { LeadingKeyword =
                     Let
                       /root/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (8,0--8,3)
                    InlineKeyword = None
                    EqualsRange =
                     Some
                       /root/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (8,6--8,7) })],
              /root/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (8,0--8,10))],
          PreXmlDocEmpty, [], None,
          /root/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (6,0--8,10),
          { LeadingKeyword =
             Namespace
               /root/MultipleDeclaredNamespacesShouldHaveARangeThatStartsAtTheNamespaceKeyword.fs (6,0--6,9) })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))