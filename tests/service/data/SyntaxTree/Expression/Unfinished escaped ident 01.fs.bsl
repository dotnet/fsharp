ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Unfinished escaped ident 01.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Do (FromParseError (Ident , (4,4--4,5)), (3,0--4,5)), (3,0--4,5));
           Expr (Do (Ident  , (6,0--7,7)), (6,0--7,7));
           Expr (Do (Ident ` , (9,0--10,8)), (9,0--10,8));
           Expr
             (Do (FromParseError (Ident , (13,4--13,8)), (12,0--13,8)),
              (12,0--13,8)); Expr (Do (Ident  , (15,0--16,9)), (15,0--16,9));
           Expr
             (Do
                (App
                   (NonAtomic, false, FromParseError (Ident , (19,4--19,5)),
                    Ident  ` , (19,4--19,11)), (18,0--19,11)), (18,0--19,11));
           Expr
             (Do
                (App
                   (NonAtomic, false, FromParseError (Ident , (22,4--22,5)),
                    Ident  , (22,4--22,11)), (21,0--22,11)), (21,0--22,11));
           Expr
             (Do
                (App
                   (NonAtomic, false, FromParseError (Ident , (25,4--25,8)),
                    FromParseError (Ident , (25,8--25,12)), (25,4--25,12)),
                 (24,0--25,12)), (24,0--25,12));
           Expr
             (Do
                (App
                   (NonAtomic, false, FromParseError (Ident , (28,4--28,8)),
                    Ident ` , (28,4--28,12)), (27,0--28,12)), (27,0--28,12));
           Expr
             (Do
                (App
                   (NonAtomic, false, FromParseError (Ident , (31,4--31,8)),
                    Ident  , (31,4--31,11)), (30,0--31,11)), (30,0--31,11));
           Expr
             (Do
                (App
                   (NonAtomic, false, FromParseError (Ident , (34,4--34,8)),
                    FromParseError (Ident , (34,8--34,9)), (34,4--34,9)),
                 (33,0--34,9)), (33,0--34,9));
           Expr (Do (Ident ` , (36,0--37,10)), (36,0--37,10));
           Expr
             (Do
                (App
                   (NonAtomic, false, Ident ` ,
                    FromParseError (Ident , (40,10--40,11)), (40,4--40,11)),
                 (39,0--40,11)), (39,0--40,11));
           Expr
             (Do
                (DotGet
                   (FromParseError (Ident , (43,4--43,8)), (43,8--43,9),
                    SynLongIdent ([P], [], [None]), (43,4--43,10)),
                 (42,0--43,10)), (42,0--43,10));
           Expr
             (Do
                (App
                   (NonAtomic, false,
                    App
                      (NonAtomic, true,
                       LongIdent
                         (false,
                          SynLongIdent
                            ([op_Addition], [], [Some (OriginalNotation "+")]),
                          None, (46,6--46,7)), Const (Int32 1, (46,4--46,5)),
                       (46,4--46,7)), FromParseError (Ident , (46,8--46,9)),
                    (46,4--46,9)), (45,0--46,9)), (45,0--46,9))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--46,9), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,4)-(4,5) parse error This is not a valid identifier
(7,4)-(7,7) parse error This is not a valid identifier
(10,4)-(10,8) parse error This is not a valid identifier
(13,4)-(13,8) parse error This is not a valid identifier
(19,4)-(19,5) parse error This is not a valid identifier
(19,6)-(19,11) parse error This is not a valid identifier
(22,4)-(22,5) parse error This is not a valid identifier
(25,4)-(25,8) parse error This is not a valid identifier
(25,8)-(25,12) parse error This is not a valid identifier
(28,4)-(28,8) parse error This is not a valid identifier
(28,8)-(28,12) parse error This is not a valid identifier
(31,4)-(31,8) parse error This is not a valid identifier
(31,8)-(31,11) parse error This is not a valid identifier
(34,4)-(34,8) parse error This is not a valid identifier
(34,8)-(34,9) parse error This is not a valid identifier
(40,10)-(40,11) parse error This is not a valid identifier
(43,4)-(43,8) parse error This is not a valid identifier
(46,8)-(46,9) parse error This is not a valid identifier
