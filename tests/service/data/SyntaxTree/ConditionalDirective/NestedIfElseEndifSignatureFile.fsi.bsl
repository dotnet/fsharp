SigFile
  (ParsedSigFileInput
     ("/root/ConditionalDirective/NestedIfElseEndifSignatureFile.fsi",
      QualifiedNameOfFile NestedIfElseEndifSignatureFile, [], [],
      [SynModuleOrNamespaceSig
         ([Foobar], false, DeclaredNamespace,
          [Val
             (SynValSig
                ([], SynIdent (v, None), SynValTyparDecls (None, true),
                 LongIdent (SynLongIdent ([int], [], [None])),
                 SynValInfo ([], SynArgInfo ([], false, None)), false, false,
                 PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector), None,
                 Some
                   (Const
                      (Int32 3,
                       /root/ConditionalDirective/NestedIfElseEndifSignatureFile.fsi (12,8--12,9))),
                 /root/ConditionalDirective/NestedIfElseEndifSignatureFile.fsi (4,0--12,9),
                 { LeadingKeyword =
                    Val
                      /root/ConditionalDirective/NestedIfElseEndifSignatureFile.fsi (4,0--4,3)
                   InlineKeyword = None
                   WithKeyword = None
                   EqualsRange =
                    Some
                      /root/ConditionalDirective/NestedIfElseEndifSignatureFile.fsi (4,12--4,13) }),
              /root/ConditionalDirective/NestedIfElseEndifSignatureFile.fsi (4,0--12,9))],
          PreXmlDocEmpty, [], None,
          /root/ConditionalDirective/NestedIfElseEndifSignatureFile.fsi (2,0--12,9),
          { LeadingKeyword =
             Namespace
               /root/ConditionalDirective/NestedIfElseEndifSignatureFile.fsi (2,0--2,9) })],
      { ConditionalDirectives =
         [If
            (Ident "FOO",
             /root/ConditionalDirective/NestedIfElseEndifSignatureFile.fsi (5,4--5,11));
          If
            (Ident "MEH",
             /root/ConditionalDirective/NestedIfElseEndifSignatureFile.fsi (6,8--6,15));
          Else
            /root/ConditionalDirective/NestedIfElseEndifSignatureFile.fsi (8,8--8,13);
          EndIf
            /root/ConditionalDirective/NestedIfElseEndifSignatureFile.fsi (10,8--10,14);
          Else
            /root/ConditionalDirective/NestedIfElseEndifSignatureFile.fsi (11,4--11,9);
          EndIf
            /root/ConditionalDirective/NestedIfElseEndifSignatureFile.fsi (13,4--13,10)]
        CodeComments = [] }, set []))