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
                 PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                 Single None, Some (Const (Int32 10, (12,4--12,6))), (4,0--12,6),
                 { LeadingKeyword = Val (4,0--4,3)
                   InlineKeyword = None
                   WithKeyword = None
                   EqualsRange = Some (4,12--4,13) }), (4,0--12,6))],
          PreXmlDocEmpty, [], None, (2,0--12,6),
          { LeadingKeyword = Namespace (2,0--2,9) })],
      { ConditionalDirectives =
         [If (Not (Ident "DEBUG"), (5,4--5,14));
          If (And (Ident "FOO", Ident "BAR"), (6,8--6,22));
          If (Or (Ident "MEH", Ident "HMM"), (7,12--7,26)); EndIf (9,12--9,18);
          EndIf (10,8--10,14); EndIf (11,4--11,10)]
        CodeComments = [] }, set []))
