module Module

match shape with
| Rectangle(width = w; length = l) -> w * l
| Circle(radius = r) -> System.Math.PI * r ** 2.
| Prism(width = w; length = l; height = h) -> w * l * h