SigFile
  (ParsedSigFileInput
     ("/root/ConditionalDirective/NestedIfEndifWithComplexExpressionsSignatureFile.fsi",
      QualifiedNameOfFile NestedIfEndifWithComplexExpressionsSignatureFile, [],
      [],
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
                      (Int32 10,
                       /root/ConditionalDirective/NestedIfEndifWithComplexExpressionsSignatureFile.fsi (12,4--12,6))),
                 /root/ConditionalDirective/NestedIfEndifWithComplexExpressionsSignatureFile.fsi (4,0--12,6),
                 { LeadingKeyword =
                    Val
                      /root/ConditionalDirective/NestedIfEndifWithComplexExpressionsSignatureFile.fsi (4,0--4,3)
                   InlineKeyword = None
                   WithKeyword = None
                   EqualsRange =
                    Some
                      /root/ConditionalDirective/NestedIfEndifWithComplexExpressionsSignatureFile.fsi (4,12--4,13) }),
              /root/ConditionalDirective/NestedIfEndifWithComplexExpressionsSignatureFile.fsi (4,0--12,6))],
          PreXmlDocEmpty, [], None,
          /root/ConditionalDirective/NestedIfEndifWithComplexExpressionsSignatureFile.fsi (2,0--12,6),
          { LeadingKeyword =
             Namespace
               /root/ConditionalDirective/NestedIfEndifWithComplexExpressionsSignatureFile.fsi (2,0--2,9) })],
      { ConditionalDirectives =
         [If
            (Not (Ident "DEBUG"),
             /root/ConditionalDirective/NestedIfEndifWithComplexExpressionsSignatureFile.fsi (5,4--5,14));
          If
            (And (Ident "FOO", Ident "BAR"),
             /root/ConditionalDirective/NestedIfEndifWithComplexExpressionsSignatureFile.fsi (6,8--6,22));
          If
            (Or (Ident "MEH", Ident "HMM"),
             /root/ConditionalDirective/NestedIfEndifWithComplexExpressionsSignatureFile.fsi (7,12--7,26));
          EndIf
            /root/ConditionalDirective/NestedIfEndifWithComplexExpressionsSignatureFile.fsi (9,12--9,18);
          EndIf
            /root/ConditionalDirective/NestedIfEndifWithComplexExpressionsSignatureFile.fsi (10,8--10,14);
          EndIf
            /root/ConditionalDirective/NestedIfEndifWithComplexExpressionsSignatureFile.fsi (11,4--11,10)]
        CodeComments = [] }, set []))