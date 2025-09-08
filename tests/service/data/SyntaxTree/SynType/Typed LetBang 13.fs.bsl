ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang 13.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Tuple
                (false,
                 [App
                    (NonAtomic, false, Ident async,
                     Record (None, None, [], (2,6--2,7)), (2,0--2,7)); Ident y],
                 [(3,15--3,16)], (2,0--3,18)), (2,0--3,18))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,18), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(3,15)-(3,16) parse error Unexpected symbol ',' in expression. Expected '=' or other token.
(2,6)-(2,7) parse error Unmatched '{'
(4,4)-(4,10) parse error Incomplete structured construct at or before this point in implementation file
