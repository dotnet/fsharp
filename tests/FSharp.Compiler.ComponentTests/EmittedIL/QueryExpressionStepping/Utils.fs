[<AutoOpen>]
module Utils

open System
open System.Text.RegularExpressions
open System.Xml
open System.Xml.Linq

type ProductR = { ProductID : int; ProductName : string; Category : string; UnitPrice : decimal; UnitsInStock : int }

type Product() =
    let mutable id = 0
    let mutable name = ""
    let mutable category = ""
    let mutable price = 0M
    let mutable unitsInStock = 0

    member x.ProductID with get() = id and set(v) = id <- v
    member x.ProductName with get() = name and set(v) = name <- v
    member x.Category with get() = category and set(v) = category <- v
    member x.UnitPrice with get() = price and set(v) = price <- v
    member x.UnitsInStock with get() = unitsInStock and set(v) = unitsInStock <- v

let getProductList() =
    [
    Product(ProductID = 1, ProductName = "Chai", Category = "Beverages", UnitPrice = 18.0000M, UnitsInStock = 39 );
    Product(ProductID = 2, ProductName = "Chang", Category = "Beverages", UnitPrice = 19.0000M, UnitsInStock = 17 ); 
    Product(ProductID = 3, ProductName = "Aniseed Syrup", Category = "Condiments", UnitPrice = 10.0000M, UnitsInStock = 13 );
    Product(ProductID = 4, ProductName = "Chef Anton's Cajun Seasoning", Category = "Condiments", UnitPrice = 22.0000M, UnitsInStock = 53 );
    Product(ProductID = 5, ProductName = "Chef Anton's Gumbo Mix", Category = "Condiments", UnitPrice = 21.3500M, UnitsInStock = 0 );
    Product(ProductID = 6, ProductName = "Grandma's Boysenberry Spread", Category = "Condiments", UnitPrice = 25.0000M, UnitsInStock = 120 );
    Product(ProductID = 7, ProductName = "Uncle Bob's Organic Dried Pears", Category = "Produce", UnitPrice = 30.0000M, UnitsInStock = 15 );
    Product(ProductID = 8, ProductName = "Northwoods Cranberry Sauce", Category = "Condiments", UnitPrice = 40.0000M, UnitsInStock = 6 );
    Product(ProductID = 9, ProductName = "Mishi Kobe Niku", Category = "Meat/Poultry", UnitPrice = 97.0000M, UnitsInStock = 29 );
    Product(ProductID = 10, ProductName = "Ikura", Category = "Seafood", UnitPrice = 31.0000M, UnitsInStock = 31 );
    Product(ProductID = 11, ProductName = "Queso Cabrales", Category = "Dairy Products", UnitPrice = 21.0000M, UnitsInStock = 22 );
    Product(ProductID = 12, ProductName = "Queso Manchego La Pastora", Category = "Dairy Products", UnitPrice = 38.0000M, UnitsInStock = 86 ); 
    Product(ProductID = 13, ProductName = "Konbu", Category = "Seafood", UnitPrice = 6.0000M, UnitsInStock = 24 ); 
    Product(ProductID = 14, ProductName = "Tofu", Category = "Produce", UnitPrice = 23.2500M, UnitsInStock = 35 ); 
    Product(ProductID = 15, ProductName = "Genen Shouyu", Category = "Condiments", UnitPrice = 15.5000M, UnitsInStock = 39 ); 
    Product(ProductID = 16, ProductName = "Pavlova", Category = "Confections", UnitPrice = 17.4500M, UnitsInStock = 29 ); 
    Product(ProductID = 17, ProductName = "Alice Mutton", Category = "Meat/Poultry", UnitPrice = 39.0000M, UnitsInStock = 0 ); 
    Product(ProductID = 18, ProductName = "Carnarvon Tigers", Category = "Seafood", UnitPrice = 62.5000M, UnitsInStock = 42 ); 
    Product(ProductID = 19, ProductName = "Teatime Chocolate Biscuits", Category = "Confections", UnitPrice = 9.2000M, UnitsInStock = 25 ); 
    Product(ProductID = 20, ProductName = "Sir Rodney's Marmalade", Category = "Confections", UnitPrice = 81.0000M, UnitsInStock = 40 ); 
    Product(ProductID = 21, ProductName = "Sir Rodney's Scones", Category = "Confections", UnitPrice = 10.0000M, UnitsInStock = 3 ); 
    Product(ProductID = 22, ProductName = "Gustaf's Knäckebröd", Category = "Grains/Cereals", UnitPrice = 21.0000M, UnitsInStock = 104 ); 
    Product(ProductID = 23, ProductName = "Tunnbröd", Category = "Grains/Cereals", UnitPrice = 9.0000M, UnitsInStock = 61 ); 
    Product(ProductID = 24, ProductName = "Guaraná Fantástica", Category = "Beverages", UnitPrice = 4.5000M, UnitsInStock = 20 ); 
    Product(ProductID = 25, ProductName = "NuNuCa Nuß-Nougat-Creme", Category = "Confections", UnitPrice = 14.0000M, UnitsInStock = 76 ); 
    Product(ProductID = 26, ProductName = "Gumbär Gummibärchen", Category = "Confections", UnitPrice = 31.2300M, UnitsInStock = 15 ); 
    Product(ProductID = 27, ProductName = "Schoggi Schokolade", Category = "Confections", UnitPrice = 43.9000M, UnitsInStock = 49 ); 
    Product(ProductID = 28, ProductName = "Rössle Sauerkraut", Category = "Produce", UnitPrice = 45.6000M, UnitsInStock = 26 ); 
    Product(ProductID = 29, ProductName = "Thüringer Rostbratwurst", Category = "Meat/Poultry", UnitPrice = 123.7900M, UnitsInStock = 0 ); 
    Product(ProductID = 30, ProductName = "Nord-Ost Matjeshering", Category = "Seafood", UnitPrice = 25.8900M, UnitsInStock = 10 ); 
    Product(ProductID = 31, ProductName = "Gorgonzola Telino", Category = "Dairy Products", UnitPrice = 12.5000M, UnitsInStock = 0 ); 
    Product(ProductID = 32, ProductName = "Mascarpone Fabioli", Category = "Dairy Products", UnitPrice = 32.0000M, UnitsInStock = 9 ); 
    Product(ProductID = 33, ProductName = "Geitost", Category = "Dairy Products", UnitPrice = 2.5000M, UnitsInStock = 112 ); 
    Product(ProductID = 34, ProductName = "Sasquatch Ale", Category = "Beverages", UnitPrice = 14.0000M, UnitsInStock = 111 ); 
    Product(ProductID = 35, ProductName = "Steeleye Stout", Category = "Beverages", UnitPrice = 18.0000M, UnitsInStock = 20 ); 
    Product(ProductID = 36, ProductName = "Inlagd Sill", Category = "Seafood", UnitPrice = 19.0000M, UnitsInStock = 112 ); 
    Product(ProductID = 37, ProductName = "Gravad lax", Category = "Seafood", UnitPrice = 26.0000M, UnitsInStock = 11 ); 
    Product(ProductID = 38, ProductName = "Côte de Blaye", Category = "Beverages", UnitPrice = 263.5000M, UnitsInStock = 17 ); 
    Product(ProductID = 39, ProductName = "Chartreuse verte", Category = "Beverages", UnitPrice = 18.0000M, UnitsInStock = 69 ); 
    Product(ProductID = 40, ProductName = "Boston Crab Meat", Category = "Seafood", UnitPrice = 18.4000M, UnitsInStock = 123 ); 
    Product(ProductID = 41, ProductName = "Jack's New England Clam Chowder", Category = "Seafood", UnitPrice = 9.6500M, UnitsInStock = 85 ); 
    Product(ProductID = 42, ProductName = "Singaporean Hokkien Fried Mee", Category = "Grains/Cereals", UnitPrice = 14.0000M, UnitsInStock = 26 ); 
    Product(ProductID = 43, ProductName = "Ipoh Coffee", Category = "Beverages", UnitPrice = 46.0000M, UnitsInStock = 17 ); 
    Product(ProductID = 44, ProductName = "Gula Malacca", Category = "Condiments", UnitPrice = 19.4500M, UnitsInStock = 27 ); 
    Product(ProductID = 45, ProductName = "Rogede sild", Category = "Seafood", UnitPrice = 9.5000M, UnitsInStock = 5 ); 
    Product(ProductID = 46, ProductName = "Spegesild", Category = "Seafood", UnitPrice = 12.0000M, UnitsInStock = 95 ); 
    Product(ProductID = 47, ProductName = "Zaanse koeken", Category = "Confections", UnitPrice = 9.5000M, UnitsInStock = 36 ); 
    Product(ProductID = 48, ProductName = "Chocolade", Category = "Confections", UnitPrice = 12.7500M, UnitsInStock = 15 ); 
    Product(ProductID = 49, ProductName = "Maxilaku", Category = "Confections", UnitPrice = 20.0000M, UnitsInStock = 10 ); 
    Product(ProductID = 50, ProductName = "Valkoinen suklaa", Category = "Confections", UnitPrice = 16.2500M, UnitsInStock = 65 ); 
    Product(ProductID = 51, ProductName = "Manjimup Dried Apples", Category = "Produce", UnitPrice = 53.0000M, UnitsInStock = 20 ); 
    Product(ProductID = 52, ProductName = "Filo Mix", Category = "Grains/Cereals", UnitPrice = 7.0000M, UnitsInStock = 38 ); 
    Product(ProductID = 53, ProductName = "Perth Pasties", Category = "Meat/Poultry", UnitPrice = 32.8000M, UnitsInStock = 0 ); 
    Product(ProductID = 54, ProductName = "Tourtière", Category = "Meat/Poultry", UnitPrice = 7.4500M, UnitsInStock = 21 ); 
    Product(ProductID = 55, ProductName = "Pâté chinois", Category = "Meat/Poultry", UnitPrice = 24.0000M, UnitsInStock = 115 ); 
    Product(ProductID = 56, ProductName = "Gnocchi di nonna Alice", Category = "Grains/Cereals", UnitPrice = 38.0000M, UnitsInStock = 21 ); 
    Product(ProductID = 57, ProductName = "Ravioli Angelo", Category = "Grains/Cereals", UnitPrice = 19.5000M, UnitsInStock = 36 ); 
    Product(ProductID = 58, ProductName = "Escargots de Bourgogne", Category = "Seafood", UnitPrice = 13.2500M, UnitsInStock = 62 ); 
    Product(ProductID = 59, ProductName = "Raclette Courdavault", Category = "Dairy Products", UnitPrice = 55.0000M, UnitsInStock = 79 ); 
    Product(ProductID = 60, ProductName = "Camembert Pierrot", Category = "Dairy Products", UnitPrice = 34.0000M, UnitsInStock = 19 ); 
    Product(ProductID = 61, ProductName = "Sirop d'érable", Category = "Condiments", UnitPrice = 28.5000M, UnitsInStock = 113 ); 
    Product(ProductID = 62, ProductName = "Tarte au sucre", Category = "Confections", UnitPrice = 49.3000M, UnitsInStock = 17 ); 
    Product(ProductID = 63, ProductName = "Vegie-spread", Category = "Condiments", UnitPrice = 43.9000M, UnitsInStock = 24 ); 
    Product(ProductID = 64, ProductName = "Wimmers gute Semmelknödel", Category = "Grains/Cereals", UnitPrice = 33.2500M, UnitsInStock = 22 ); 
    Product(ProductID = 65, ProductName = "Louisiana Fiery Hot Pepper Sauce", Category = "Condiments", UnitPrice = 21.0500M, UnitsInStock = 76 ); 
    Product(ProductID = 66, ProductName = "Louisiana Hot Spiced Okra", Category = "Condiments", UnitPrice = 17.0000M, UnitsInStock = 4 ); 
    Product(ProductID = 67, ProductName = "Laughing Lumberjack Lager", Category = "Beverages", UnitPrice = 14.0000M, UnitsInStock = 52 ); 
    Product(ProductID = 68, ProductName = "Scottish Longbreads", Category = "Confections", UnitPrice = 12.5000M, UnitsInStock = 6 ); 
    Product(ProductID = 69, ProductName = "Gudbrandsdalsost", Category = "Dairy Products", UnitPrice = 36.0000M, UnitsInStock = 26 ); 
    Product(ProductID = 70, ProductName = "Outback Lager", Category = "Beverages", UnitPrice = 15.0000M, UnitsInStock = 15 ); 
    Product(ProductID = 71, ProductName = "Flotemysost", Category = "Dairy Products", UnitPrice = 21.5000M, UnitsInStock = 26 ); 
    Product(ProductID = 72, ProductName = "Mozzarella di Giovanni", Category = "Dairy Products", UnitPrice = 34.8000M, UnitsInStock = 14 ); 
    Product(ProductID = 73, ProductName = "Röd Kaviar", Category = "Seafood", UnitPrice = 15.0000M, UnitsInStock = 101 ); 
    Product(ProductID = 74, ProductName = "Longlife Tofu", Category = "Produce", UnitPrice = 10.0000M, UnitsInStock = 4 ); 
    Product(ProductID = 75, ProductName = "Rhönbräu Klosterbier", Category = "Beverages", UnitPrice = 7.7500M, UnitsInStock = 125 ); 
    Product(ProductID = 76, ProductName = "Lakkalikööri", Category = "Beverages", UnitPrice = 18.0000M, UnitsInStock = 57 ); 
    Product(ProductID = 77, ProductName = "Original Frankfurter grüne Soße", Category = "Condiments", UnitPrice = 13.0000M, UnitsInStock = 32 )
    ]

let getProductListAsRecords() =
    [{ ProductID = 1; ProductName = "Chai"; Category = "Beverages"; UnitPrice = 18.0000M; UnitsInStock = 39 };
    { ProductID = 2; ProductName = "Chang"; Category = "Beverages"; UnitPrice = 19.0000M; UnitsInStock = 17 }; 
    { ProductID = 3; ProductName = "Aniseed Syrup"; Category = "Condiments"; UnitPrice = 10.0000M; UnitsInStock = 13 };
    { ProductID = 4; ProductName = "Chef Anton's Cajun Seasoning"; Category = "Condiments"; UnitPrice = 22.0000M; UnitsInStock = 53 };
    { ProductID = 5; ProductName = "Chef Anton's Gumbo Mix"; Category = "Condiments"; UnitPrice = 21.3500M; UnitsInStock = 0 };
    { ProductID = 6; ProductName = "Grandma's Boysenberry Spread"; Category = "Condiments"; UnitPrice = 25.0000M; UnitsInStock = 120 };
    { ProductID = 7; ProductName = "Uncle Bob's Organic Dried Pears"; Category = "Produce"; UnitPrice = 30.0000M; UnitsInStock = 15 };
    { ProductID = 8; ProductName = "Northwoods Cranberry Sauce"; Category = "Condiments"; UnitPrice = 40.0000M; UnitsInStock = 6 };
    { ProductID = 9; ProductName = "Mishi Kobe Niku"; Category = "Meat/Poultry"; UnitPrice = 97.0000M; UnitsInStock = 29 };
    { ProductID = 10; ProductName = "Ikura"; Category = "Seafood"; UnitPrice = 31.0000M; UnitsInStock = 31 };
    { ProductID = 11; ProductName = "Queso Cabrales"; Category = "Dairy Products"; UnitPrice = 21.0000M; UnitsInStock = 22 };
    { ProductID = 12; ProductName = "Queso Manchego La Pastora"; Category = "Dairy Products"; UnitPrice = 38.0000M; UnitsInStock = 86 }; 
    { ProductID = 13; ProductName = "Konbu"; Category = "Seafood"; UnitPrice = 6.0000M; UnitsInStock = 24 }; 
    { ProductID = 14; ProductName = "Tofu"; Category = "Produce"; UnitPrice = 23.2500M; UnitsInStock = 35 }; 
    { ProductID = 15; ProductName = "Genen Shouyu"; Category = "Condiments"; UnitPrice = 15.5000M; UnitsInStock = 39 }; 
    { ProductID = 16; ProductName = "Pavlova"; Category = "Confections"; UnitPrice = 17.4500M; UnitsInStock = 29 }; 
    { ProductID = 17; ProductName = "Alice Mutton"; Category = "Meat/Poultry"; UnitPrice = 39.0000M; UnitsInStock = 0 }; 
    { ProductID = 18; ProductName = "Carnarvon Tigers"; Category = "Seafood"; UnitPrice = 62.5000M; UnitsInStock = 42 }; 
    { ProductID = 19; ProductName = "Teatime Chocolate Biscuits"; Category = "Confections"; UnitPrice = 9.2000M; UnitsInStock = 25 }; 
    { ProductID = 20; ProductName = "Sir Rodney's Marmalade"; Category = "Confections"; UnitPrice = 81.0000M; UnitsInStock = 40 }; 
    { ProductID = 21; ProductName = "Sir Rodney's Scones"; Category = "Confections"; UnitPrice = 10.0000M; UnitsInStock = 3 }; 
    { ProductID = 22; ProductName = "Gustaf's Knäckebröd"; Category = "Grains/Cereals"; UnitPrice = 21.0000M; UnitsInStock = 104 }; 
    { ProductID = 23; ProductName = "Tunnbröd"; Category = "Grains/Cereals"; UnitPrice = 9.0000M; UnitsInStock = 61 }; 
    { ProductID = 24; ProductName = "Guaraná Fantástica"; Category = "Beverages"; UnitPrice = 4.5000M; UnitsInStock = 20 }; 
    { ProductID = 25; ProductName = "NuNuCa Nuß-Nougat-Creme"; Category = "Confections"; UnitPrice = 14.0000M; UnitsInStock = 76 }; 
    { ProductID = 26; ProductName = "Gumbär Gummibärchen"; Category = "Confections"; UnitPrice = 31.2300M; UnitsInStock = 15 }; 
    { ProductID = 27; ProductName = "Schoggi Schokolade"; Category = "Confections"; UnitPrice = 43.9000M; UnitsInStock = 49 }; 
    { ProductID = 28; ProductName = "Rössle Sauerkraut"; Category = "Produce"; UnitPrice = 45.6000M; UnitsInStock = 26 }; 
    { ProductID = 29; ProductName = "Thüringer Rostbratwurst"; Category = "Meat/Poultry"; UnitPrice = 123.7900M; UnitsInStock = 0 }; 
    { ProductID = 30; ProductName = "Nord-Ost Matjeshering"; Category = "Seafood"; UnitPrice = 25.8900M; UnitsInStock = 10 }; 
    { ProductID = 31; ProductName = "Gorgonzola Telino"; Category = "Dairy Products"; UnitPrice = 12.5000M; UnitsInStock = 0 }; 
    { ProductID = 32; ProductName = "Mascarpone Fabioli"; Category = "Dairy Products"; UnitPrice = 32.0000M; UnitsInStock = 9 }; 
    { ProductID = 33; ProductName = "Geitost"; Category = "Dairy Products"; UnitPrice = 2.5000M; UnitsInStock = 112 }; 
    { ProductID = 34; ProductName = "Sasquatch Ale"; Category = "Beverages"; UnitPrice = 14.0000M; UnitsInStock = 111 }; 
    { ProductID = 35; ProductName = "Steeleye Stout"; Category = "Beverages"; UnitPrice = 18.0000M; UnitsInStock = 20 }; 
    { ProductID = 36; ProductName = "Inlagd Sill"; Category = "Seafood"; UnitPrice = 19.0000M; UnitsInStock = 112 }; 
    { ProductID = 37; ProductName = "Gravad lax"; Category = "Seafood"; UnitPrice = 26.0000M; UnitsInStock = 11 }; 
    { ProductID = 38; ProductName = "Côte de Blaye"; Category = "Beverages"; UnitPrice = 263.5000M; UnitsInStock = 17 }; 
    { ProductID = 39; ProductName = "Chartreuse verte"; Category = "Beverages"; UnitPrice = 18.0000M; UnitsInStock = 69 }; 
    { ProductID = 40; ProductName = "Boston Crab Meat"; Category = "Seafood"; UnitPrice = 18.4000M; UnitsInStock = 123 }; 
    { ProductID = 41; ProductName = "Jack's New England Clam Chowder"; Category = "Seafood"; UnitPrice = 9.6500M; UnitsInStock = 85 }; 
    { ProductID = 42; ProductName = "Singaporean Hokkien Fried Mee"; Category = "Grains/Cereals"; UnitPrice = 14.0000M; UnitsInStock = 26 }; 
    { ProductID = 43; ProductName = "Ipoh Coffee"; Category = "Beverages"; UnitPrice = 46.0000M; UnitsInStock = 17 }; 
    { ProductID = 44; ProductName = "Gula Malacca"; Category = "Condiments"; UnitPrice = 19.4500M; UnitsInStock = 27 }; 
    { ProductID = 45; ProductName = "Rogede sild"; Category = "Seafood"; UnitPrice = 9.5000M; UnitsInStock = 5 }; 
    { ProductID = 46; ProductName = "Spegesild"; Category = "Seafood"; UnitPrice = 12.0000M; UnitsInStock = 95 }; 
    { ProductID = 47; ProductName = "Zaanse koeken"; Category = "Confections"; UnitPrice = 9.5000M; UnitsInStock = 36 }; 
    { ProductID = 48; ProductName = "Chocolade"; Category = "Confections"; UnitPrice = 12.7500M; UnitsInStock = 15 }; 
    { ProductID = 49; ProductName = "Maxilaku"; Category = "Confections"; UnitPrice = 20.0000M; UnitsInStock = 10 }; 
    { ProductID = 50; ProductName = "Valkoinen suklaa"; Category = "Confections"; UnitPrice = 16.2500M; UnitsInStock = 65 }; 
    { ProductID = 51; ProductName = "Manjimup Dried Apples"; Category = "Produce"; UnitPrice = 53.0000M; UnitsInStock = 20 }; 
    { ProductID = 52; ProductName = "Filo Mix"; Category = "Grains/Cereals"; UnitPrice = 7.0000M; UnitsInStock = 38 }; 
    { ProductID = 53; ProductName = "Perth Pasties"; Category = "Meat/Poultry"; UnitPrice = 32.8000M; UnitsInStock = 0 }; 
    { ProductID = 54; ProductName = "Tourtière"; Category = "Meat/Poultry"; UnitPrice = 7.4500M; UnitsInStock = 21 }; 
    { ProductID = 55; ProductName = "Pâté chinois"; Category = "Meat/Poultry"; UnitPrice = 24.0000M; UnitsInStock = 115 }; 
    { ProductID = 56; ProductName = "Gnocchi di nonna Alice"; Category = "Grains/Cereals"; UnitPrice = 38.0000M; UnitsInStock = 21 }; 
    { ProductID = 57; ProductName = "Ravioli Angelo"; Category = "Grains/Cereals"; UnitPrice = 19.5000M; UnitsInStock = 36 }; 
    { ProductID = 58; ProductName = "Escargots de Bourgogne"; Category = "Seafood"; UnitPrice = 13.2500M; UnitsInStock = 62 }; 
    { ProductID = 59; ProductName = "Raclette Courdavault"; Category = "Dairy Products"; UnitPrice = 55.0000M; UnitsInStock = 79 }; 
    { ProductID = 60; ProductName = "Camembert Pierrot"; Category = "Dairy Products"; UnitPrice = 34.0000M; UnitsInStock = 19 }; 
    { ProductID = 61; ProductName = "Sirop d'érable"; Category = "Condiments"; UnitPrice = 28.5000M; UnitsInStock = 113 }; 
    { ProductID = 62; ProductName = "Tarte au sucre"; Category = "Confections"; UnitPrice = 49.3000M; UnitsInStock = 17 }; 
    { ProductID = 63; ProductName = "Vegie-spread"; Category = "Condiments"; UnitPrice = 43.9000M; UnitsInStock = 24 }; 
    { ProductID = 64; ProductName = "Wimmers gute Semmelknödel"; Category = "Grains/Cereals"; UnitPrice = 33.2500M; UnitsInStock = 22 }; 
    { ProductID = 65; ProductName = "Louisiana Fiery Hot Pepper Sauce"; Category = "Condiments"; UnitPrice = 21.0500M; UnitsInStock = 76 }; 
    { ProductID = 66; ProductName = "Louisiana Hot Spiced Okra"; Category = "Condiments"; UnitPrice = 17.0000M; UnitsInStock = 4 }; 
    { ProductID = 67; ProductName = "Laughing Lumberjack Lager"; Category = "Beverages"; UnitPrice = 14.0000M; UnitsInStock = 52 }; 
    { ProductID = 68; ProductName = "Scottish Longbreads"; Category = "Confections"; UnitPrice = 12.5000M; UnitsInStock = 6 }; 
    { ProductID = 69; ProductName = "Gudbrandsdalsost"; Category = "Dairy Products"; UnitPrice = 36.0000M; UnitsInStock = 26 }; 
    { ProductID = 70; ProductName = "Outback Lager"; Category = "Beverages"; UnitPrice = 15.0000M; UnitsInStock = 15 }; 
    { ProductID = 71; ProductName = "Flotemysost"; Category = "Dairy Products"; UnitPrice = 21.5000M; UnitsInStock = 26 }; 
    { ProductID = 72; ProductName = "Mozzarella di Giovanni"; Category = "Dairy Products"; UnitPrice = 34.8000M; UnitsInStock = 14 }; 
    { ProductID = 73; ProductName = "Röd Kaviar"; Category = "Seafood"; UnitPrice = 15.0000M; UnitsInStock = 101 }; 
    { ProductID = 74; ProductName = "Longlife Tofu"; Category = "Produce"; UnitPrice = 10.0000M; UnitsInStock = 4 }; 
    { ProductID = 75; ProductName = "Rhönbräu Klosterbier"; Category = "Beverages"; UnitPrice = 7.7500M; UnitsInStock = 125 }; 
    { ProductID = 76; ProductName = "Lakkalikööri"; Category = "Beverages"; UnitPrice = 18.0000M; UnitsInStock = 57 }; 
    { ProductID = 77; ProductName = "Original Frankfurter grüne Soße"; Category = "Condiments"; UnitPrice = 13.0000M; UnitsInStock = 32 }]


type Order(orderID, orderDate : DateTime, total) =
    member x.OrderID = orderID
    member x.OrderDate = orderDate
    member x.Total = total

type Customer(customerID, companyName, address, city, region, postalCode, country, phone, fax, orders) =
    member x.CustomerID = customerID
    member x.CompanyName = companyName
    member x.Address = address
    member x.City = city
    member x.Region = region
    member x.PostalCode = postalCode
    member x.Country = country
    member x.Phone = phone
    member x.Fax = fax
    member x.Orders = orders

let getCustomerList() =
    let doc = XmlReader.Create(Environment.CurrentDirectory + "\\customers.xml")
    
    let xname s = XName.Get(s)
    let ge (x : XElement) (s : string) = 
        try 
            let r = x.Element(XName.Get(s)).ToString() 
            Regex.Match(r, ">(.*)<").Groups.[1].Value            
        with 
            | :? NullReferenceException -> ""
    query {
        for e in XDocument.Load(doc).Root.Elements(xname "customer") do
        let get = ge e
        let orders = 
            query {
                for o in e.Elements(xname "orders").Elements(xname "order") do
                let date = ge o "orderdate"
                let dateParts = date.Substring(0, date.Length-9).Split([|'-'|])
                let orderDate = DateTime(Int32.Parse(dateParts.[0]), Int32.Parse(dateParts.[1]), Int32.Parse(dateParts.[2]))
                let order = o.Element(xname "id") |> int
                let total = o.Element(xname "total") |> decimal
                select (Order(order, orderDate, total))
            } |> Seq.toArray
        let c = Customer(get "id", get "name", get "address", get "city", get "region", get "postcalcode", get "country", get "phone", get "fax", orders)
        select c
    } |> Seq.toList