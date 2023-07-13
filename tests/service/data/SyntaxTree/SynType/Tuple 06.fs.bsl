ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Tuple 06.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Paren
                (Typed
                   (Const (Unit, (3,1--3,3)),
                    Tuple
                      (false,
                       [Type (LongIdent (SynLongIdent ([a], [], [None])));
                        Star (3,7--3,8); Type (FromParseError (3,10--3,10));
                        Star (3,9--3,10);
                        Type (LongIdent (SynLongIdent ([c], [], [None])))],
                       (3,5--3,12)), (3,1--3,12)), (3,0--3,1), Some (3,12--3,13),
                 (3,0--3,13)), (3,0--3,13))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,13), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,9)-(3,10) parse error Expecting type
