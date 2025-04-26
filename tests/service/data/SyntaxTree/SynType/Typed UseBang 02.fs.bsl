ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed UseBang 02.fs", false, QualifiedNameOfFile Module, [],
      [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (LetOrUseBang
                (Yes (3,0--3,27), true, true,
                 Paren
                   (Typed
                      (Named (SynIdent (res, None), false, None, (3,6--3,9)),
                       App
                         (LongIdent (SynLongIdent ([Async], [], [None])),
                          Some (3,16--3,17),
                          [LongIdent (SynLongIdent ([int], [], [None]))], [],
                          Some (3,20--3,21), false, (3,11--3,21)), (3,6--3,21)),
                    (3,5--3,22)), Const (Unit, (3,25--3,27)), [],
                 ImplicitZero (3,27--3,27), (3,0--4,0),
                 { LetOrUseBangKeyword = (3,0--3,4)
                   EqualsRange = Some (3,23--3,24) }), (3,0--4,0))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,0), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,0)-(4,0) parse error Incomplete structured construct at or before this point in expression
