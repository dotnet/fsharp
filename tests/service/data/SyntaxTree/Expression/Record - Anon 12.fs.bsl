ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Record - Anon 12.fs", false, QualifiedNameOfFile Module,
      [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (AnonRecd
                (false, Some (Ident F1, ((3,5--3,5), None)), [], (3,0--3,5),
                 { OpeningBraceRange = (3,0--3,2) }), (3,0--3,5));
           Expr
             (App
                (NonAtomic, false,
                 App
                   (NonAtomic, true,
                    LongIdent
                      (false,
                       SynLongIdent
                         ([op_Equality], [], [Some (OriginalNotation "=")]),
                       None, (4,6--4,7)), Ident F2, (4,3--4,7)),
                 Const (Int32 2, (4,8--4,9)), (4,3--4,9)), (4,3--4,9))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,9), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,3)-(3,5) parse error Field bindings must have the form 'id = expr;'
(3,6)-(4,3) parse error Incomplete structured construct at or before this point in definition. Expected '|}' or other token.
(3,0)-(3,2) parse error Unmatched '{|'
(4,10)-(4,12) parse error Unexpected symbol '|}' in definition. Expected incomplete structured construct at or before this point or other token.
