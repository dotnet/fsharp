SigFile
  (ParsedSigFileInput
     ("/root/ConditionalDirective/IfElifElseEndifSignatureFile.fsi",
      QualifiedNameOfFile IfElifElseEndifSignatureFile, [],
      [SynModuleOrNamespaceSig
         ([Foobar], false, DeclaredNamespace,
          [Val
             (SynValSig
                ([], SynIdent (x, None), SynValTyparDecls (None, true),
                 LongIdent (SynLongIdent ([int], [], [None])),
                 SynValInfo ([], SynArgInfo ([], false, None)), false, false,
                 PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                 Single None, Some (Const (Int32 3, (10,4--10,5))), (4,0--10,5),
                 { LeadingKeyword = Val (4,0--4,3)
                   InlineKeyword = None
                   WithKeyword = None
                   EqualsRange = Some (4,12--4,13) }), (4,0--10,5))],
          PreXmlDocEmpty, [], None, (2,0--10,5),
          { LeadingKeyword = Namespace (2,0--2,9) })],
      { ConditionalDirectives =
         [If (Ident "DEBUG", (5,4--5,13)); Elif (Ident "RELEASE", (7,4--7,17));
          Else (9,4--9,9); EndIf (11,4--11,10)]
        WarnDirectives = []
        CodeComments = [] }, set []))
