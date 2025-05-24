ImplFile
  (ParsedImplFileInput
     ("/root/Expression/AnonRecd - Quotation 03.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (AnonRecd
                (false, None,
                 [(SynLongIdent ([A], [], [None]), Some (3,5--3,6),
                   Quote
                     (Ident op_Quotation, false,
                      App
                        (NonAtomic, false,
                         App
                           (NonAtomic, true,
                            LongIdent
                              (false,
                               SynLongIdent
                                 ([op_Addition], [],
                                  [Some (OriginalNotation "+")]), None,
                               (3,12--3,13)), Const (Int32 1, (3,10--3,11)),
                            (3,10--3,13)), Const (Int32 1, (3,14--3,15)),
                         (3,10--3,15)), false, (3,7--3,18)));
                  (SynLongIdent ([B], [], [None]), Some (3,22--3,23),
                   Quote
                     (Ident op_QuotationUntyped, true,
                      Const
                        (String ("test", Regular, (3,28--3,34)), (3,28--3,34)),
                      false, (3,24--3,38)))], (3,0--3,40),
                 { OpeningBraceRange = (3,0--3,2) }), (3,0--3,40));
           Expr
             (AnonRecd
                (false, None,
                 [(SynLongIdent ([A], [], [None]), Some (5,4--5,5),
                   Quote
                     (Ident op_Quotation, false,
                      App
                        (NonAtomic, false,
                         App
                           (NonAtomic, true,
                            LongIdent
                              (false,
                               SynLongIdent
                                 ([op_Addition], [],
                                  [Some (OriginalNotation "+")]), None,
                               (5,11--5,12)), Const (Int32 1, (5,9--5,10)),
                            (5,9--5,12)), Const (Int32 1, (5,13--5,14)),
                         (5,9--5,14)), false, (5,6--5,17)));
                  (SynLongIdent ([B], [], [None]), Some (5,21--5,22),
                   Quote
                     (Ident op_QuotationUntyped, true,
                      Const
                        (String ("test", Regular, (5,27--5,33)), (5,27--5,33)),
                      false, (5,23--5,37)))], (5,0--5,40),
                 { OpeningBraceRange = (5,0--5,2) }), (5,0--5,40));
           Expr
             (AnonRecd
                (false, None,
                 [(SynLongIdent ([A], [], [None]), Some (7,5--7,6),
                   Quote
                     (Ident op_Quotation, false,
                      App
                        (NonAtomic, false,
                         App
                           (NonAtomic, true,
                            LongIdent
                              (false,
                               SynLongIdent
                                 ([op_Addition], [],
                                  [Some (OriginalNotation "+")]), None,
                               (7,12--7,13)), Const (Int32 1, (7,10--7,11)),
                            (7,10--7,13)), Const (Int32 1, (7,14--7,15)),
                         (7,10--7,15)), false, (7,7--7,18)));
                  (SynLongIdent ([B], [], [None]), Some (7,22--7,23),
                   Quote
                     (Ident op_QuotationUntyped, true,
                      Const
                        (String ("test", Regular, (7,28--7,34)), (7,28--7,34)),
                      false, (7,24--7,38)))], (7,0--7,41),
                 { OpeningBraceRange = (7,0--7,2) }), (7,0--7,41))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,41), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
