ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Tuple 07.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Paren
                (Typed
                   (Const (Unit, (3,1--3,3)),
                    Tuple
                      (false,
                       [Type (LongIdent (SynLongIdent ([a], [], [None])));
                        Star (3,7--3,8);
                        Type (LongIdent (SynLongIdent ([b], [], [None])));
                        Star (3,11--3,12);
                        Type (LongIdent (SynLongIdent ([c], [], [None])));
                        Star (3,15--3,16); Type (FromParseError (3,16--3,16))],
                       (3,5--3,16)), (3,1--3,16)), (3,0--3,1), Some (3,16--3,17),
                 (3,0--3,17)), (3,0--3,17))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,17), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,16)-(3,17) parse error Unexpected symbol ')' in type
