// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Core.UnitTests.Linq

open System
open System.Collections.Generic
open System.Linq

[<AllowNullLiteral>]
type Product() =
    member val Id = 0 with get, set
    member val Name = "" with get, set
    member val Category = "" with get, set
    override this.ToString() = $"Product#{this.Id}"

[<AllowNullLiteral>]
type OrderLine() =
    member val Id = 0 with get, set
    member val OrderId = 0 with get, set
    member val ProductId = 0 with get, set
    member val Quantity = 0 with get, set
    member val Price = 0.0 with get, set
    member val Discount = Nullable<int>() with get, set
    override this.ToString() = $"OrderLine#{this.Id}"

[<AllowNullLiteral>]
type Order() =
    member val Id = 0 with get, set
    member val CustomerId = 0 with get, set
    member val Total = 0.0 with get, set
    member val Lines: ICollection<OrderLine> = List<OrderLine>() with get, set
    override this.ToString() = $"Order#{this.Id}"

[<AllowNullLiteral>]
type Customer() =
    member val Id = 0 with get, set
    member val Name = "" with get, set
    member val City = "" with get, set
    member val Age = 0 with get, set
    member val Registered = Nullable<DateTime>() with get, set
    member val Orders: ICollection<Order> = List<Order>() with get, set
    override this.ToString() = $"Customer#{this.Id}"

type CustomerView() =
    member val ViewName = "" with get, set
    member val ViewAge = 0 with get, set
    override this.ToString() = $"CustomerView({this.ViewName}, {this.ViewAge})"

type NameAndAge = { PersonName: string; PersonAge: int }

type NameAgeCity =
    { FullName: string
      Years: int
      Town: string }

[<CLIMutable>]
type NameAndAgeDto = { DtoName: string; DtoAge: int }

type CityAndAge = { KeyCity: string; KeyAge: int }

type OrderAndQuantity = { OrderNumber: int; Units: int }

type Data() =
    let line id orderId productId quantity price discount =
        OrderLine(Id = id, OrderId = orderId, ProductId = productId, Quantity = quantity, Price = price, Discount = discount)

    let order id customerId total (lines: OrderLine list) =
        Order(Id = id, CustomerId = customerId, Total = total, Lines = List<OrderLine> lines)

    let customer id name city age registered (orders: Order list) =
        Customer(Id = id, Name = name, City = city, Age = age, Registered = registered, Orders = List<Order> orders)

    let orders =
        [ order 10 1 12.5 [ line 100 10 1 2 2.5 (Nullable 1); line 101 10 2 1 7.5 (Nullable()) ]
          order 11 1 3.0 [ line 102 11 3 1 3.0 (Nullable 2) ]
          order 12 2 5.0 [ line 103 12 1 2 2.5 (Nullable()) ]
          order 13 3 0.0 [] ]

    let ordersOf customerId =
        orders |> List.filter (fun order -> order.CustomerId = customerId)

    member val Products =
        [ Product(Id = 1, Name = "Pen", Category = "Office")
          Product(Id = 2, Name = "Ink", Category = "Office")
          Product(Id = 3, Name = "Tea", Category = "Food") ]

    member val Customers =
        [ customer 1 "Alice" "London" 30 (Nullable(DateTime(2020, 1, 1))) (ordersOf 1)
          customer 2 "Bob" "Paris" 25 (Nullable()) (ordersOf 2)
          customer 3 "Carol" "London" 35 (Nullable(DateTime(2021, 6, 15))) (ordersOf 3)
          customer 4 "Dave" "Berlin" 28 (Nullable()) [] ]

    member _.Orders = orders
    member _.Lines = orders |> List.collect (fun order -> List.ofSeq order.Lines)

[<NoEquality; NoComparison>]
type Db =
    { Customers: IQueryable<Customer>
      Orders: IQueryable<Order>
      Lines: IQueryable<OrderLine>
      Products: IQueryable<Product> }

module Db =

    let inMemory (provider: CapturingQueryProvider) (data: Data) =
        { Customers = provider.Source("Customers", data.Customers)
          Orders = provider.Source("Orders", data.Orders)
          Lines = provider.Source("Lines", data.Lines)
          Products = provider.Source("Products", data.Products) }
