ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed UseBang 01.fs", false, QualifiedNameOfFile Module, [],
      [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (LetOrUseBang
                (Yes (3,0--3,25), true, true,
                 Typed
                   (Named (SynIdent (res, None), false, None, (3,5--3,8)),
                    App
                      (LongIdent (SynLongIdent ([Async], [], [None])),
                       Some (3,15--3,16),
                       [LongIdent (SynLongIdent ([int], [], [None]))], [],
                       Some (3,19--3,20), false, (3,10--3,20)), (3,5--3,20)),
                 Const (Unit, (3,23--3,25)), [], ImplicitZero (3,25--3,25),
                 (3,0--4,0), { LetOrUseBangKeyword = (3,0--3,4)
                               EqualsRange = Some (3,21--3,22) }), (3,0--4,0))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,0), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,0)-(4,0) parse error Incomplete structured construct at or before this point in expression
