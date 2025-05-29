ImplFile
  (ParsedImplFileInput
     ("/root/Expression/AnonRecd - Quotation 04.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (AnonRecd
                (false, None,
                 [(SynLongIdent ([Outer], [], [None]), Some (3,9--3,10),
                   AnonRecd
                     (false, None,
                      [(SynLongIdent ([Inner], [], [None]), Some (3,20--3,21),
                        Quote
                          (Ident op_Quotation, false,
                           Const (Int32 1, (3,25--3,26)), false, (3,22--3,29)))],
                      (3,11--3,31), { OpeningBraceRange = (3,11--3,13) }));
                  (SynLongIdent ([Other], [], [None]), Some (3,39--3,40),
                   Quote
                     (Ident op_QuotationUntyped, true,
                      Const
                        (String ("test", Regular, (3,45--3,51)), (3,45--3,51)),
                      false, (3,41--3,55)))], (3,0--3,57),
                 { OpeningBraceRange = (3,0--3,2) }), (3,0--3,57));
           Expr
             (AnonRecd
                (false, None,
                 [(SynLongIdent ([Outer], [], [None]), Some (5,8--5,9),
                   AnonRecd
                     (false, None,
                      [(SynLongIdent ([Inner], [], [None]), Some (5,19--5,20),
                        Quote
                          (Ident op_Quotation, false,
                           Const (Int32 1, (5,24--5,25)), false, (5,21--5,28)))],
                      (5,10--5,30), { OpeningBraceRange = (5,10--5,12) }));
                  (SynLongIdent ([Other], [], [None]), Some (5,38--5,39),
                   Quote
                     (Ident op_QuotationUntyped, true,
                      Const
                        (String ("test", Regular, (5,44--5,50)), (5,44--5,50)),
                      false, (5,40--5,54)))], (5,0--5,57),
                 { OpeningBraceRange = (5,0--5,2) }), (5,0--5,57));
           Expr
             (AnonRecd
                (false, None,
                 [(SynLongIdent ([Outer], [], [None]), Some (7,9--7,10),
                   AnonRecd
                     (false, None,
                      [(SynLongIdent ([Inner], [], [None]), Some (7,20--7,21),
                        Quote
                          (Ident op_Quotation, false,
                           Const (Int32 1, (7,25--7,26)), false, (7,22--7,29)))],
                      (7,11--7,31), { OpeningBraceRange = (7,11--7,13) }));
                  (SynLongIdent ([Other], [], [None]), Some (7,39--7,40),
                   Quote
                     (Ident op_QuotationUntyped, true,
                      Const
                        (String ("test", Regular, (7,45--7,51)), (7,45--7,51)),
                      false, (7,41--7,55)))], (7,0--7,58),
                 { OpeningBraceRange = (7,0--7,2) }), (7,0--7,58))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,58), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
