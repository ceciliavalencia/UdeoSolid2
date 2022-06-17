using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdeoSolid2
{
    class Program
    {
        public enum Color
        {
            Red, Green, Blue
        }

        public enum Size
        {
            Small, Medium, Large, Yuge
        }

        public class Product
        {
            public string Name;
            public Color Color;
            public Size Size;
            public Product(string name, Color color, Size size)
            {
                Name = name ?? throw new ArgumentNullException(paramName: nameof(name));
                Color = color;
                Size = size;
            }
        }

        public class ProductFilter
        {
            public ProductFilter()
            {

            }

            public IEnumerable<Product> FilterByColor(IEnumerable<Product> products, Color color)
            {
                foreach (var p in products)
                {
                    if (p.Color == color)
                    {
                        yield return p;
                    }
                }
            }

            public IEnumerable<Product> FilterBySize(IEnumerable<Product> products, Size size)
            {
                foreach (var p in products)
                {
                    if (p.Size == size)
                    {
                        yield return p;
                    }
                }
            }

            public IEnumerable<Product> FilterBySizeColor(IEnumerable<Product> products, Size size, Color color)
            {
                foreach (var p in products)
                {
                    if (p.Size == size && p.Color == color)
                    {
                        yield return p;
                    }
                }
            }
        }

        public interface ISpecification<T>
        {
            bool IsSatisfied(Product p);
        }

        public interface IFilter<T>
        {
            IEnumerable<T> Filter(IEnumerable<T> items, ISpecification<T> spec);
        }

        public class ColorSpecification : ISpecification<Product>
        {
            private Color color;
            public ColorSpecification(Color color)
            {
                this.color = color;
            }
            public bool IsSatisfied(Product p)
            {
                return p.Color == color;
            }
        }

        public class SizeSpecification : ISpecification<Product>
        {
            private Size size;
            public SizeSpecification(Size size)
            {
                this.size = size;
            }

            public bool IsSatisfied(Product p)
            {
                return p.Size == size;
            }
        }

        public class AndSpecification<T> : ISpecification<T>
        {
            private ISpecification<T> first, second;
            public AndSpecification(ISpecification<T> first, ISpecification<T> second)
            {
                this.first = first ?? throw new ArgumentNullException(paramName: nameof(first));
                this.second = second ?? throw new ArgumentNullException(paramName: nameof(second));
            }

            public bool IsSatisfied(Product p)
            {
                return this.first.IsSatisfied(p) && this.second.IsSatisfied(p);
            }
        }

        public class BetterFilter : IFilter<Product>
        {
            public IEnumerable<Product> Filter(IEnumerable<Product> items, ISpecification<Product> spec)
            {
                foreach (var p in items)
                {
                    if (spec.IsSatisfied(p))
                        yield return p;
                }
            }
        }



        static void Main(string[] args)
        {
            var apple = new Product("Apple", Color.Green, Size.Small);
            var tree = new Product("Tree", Color.Green, Size.Large);
            var house = new Product("House", Color.Blue, Size.Large);

            Product[] products = { apple, tree, house };

            var pf = new ProductFilter();
            Console.WriteLine("************************************");
            Console.WriteLine("Green Productos (old)");
            Console.WriteLine("************************************");
            Console.WriteLine("\n");
            foreach (var p in pf.FilterByColor(products, Color.Green))
                Console.WriteLine($" -{p.Name} is green");
            Console.WriteLine("\n");
            Console.WriteLine("************************************");

            Console.WriteLine("\n");
            Console.WriteLine("************************************");
            Console.WriteLine("Large Productos (old)");
            Console.WriteLine("************************************");
            Console.WriteLine("\n");
            foreach (var p in pf.FilterBySize(products, Size.Large))
                Console.WriteLine($" -{p.Name} is large");
            Console.WriteLine("\n");
            Console.WriteLine("************************************");

            // hasta este punto arriba, solo hemos implementado el principio de responsabilidad unica, falta open close
            // osea que tu función pueda crecer en base a funcionalidades, pero no se modifique su enfoque

            // implementación de open close

            //Global filter print
            var bfilter = new BetterFilter();

            var pfe = new ColorSpecification(Color.Green);
            Console.WriteLine("\n");
            Console.WriteLine("*****************************************");
            Console.WriteLine("Products ColorSpecification Green (New)");
            Console.WriteLine("*****************************************");
            Console.WriteLine("\n");
            foreach (var p in bfilter.Filter(products, pfe))
                Console.WriteLine($" -{p.Name} is Green");
            Console.WriteLine("\n");
            Console.WriteLine("************************************");

            var pfs = new SizeSpecification(Size.Small);
            Console.WriteLine("\n");
            Console.WriteLine("*****************************************");
            Console.WriteLine("Products SizeSpecification Small (New)");
            Console.WriteLine("*****************************************");
            Console.WriteLine("\n");
            foreach (var p in bfilter.Filter(products, pfs))
                Console.WriteLine($" -{p.Name} is Small");
            Console.WriteLine("\n");
            Console.WriteLine("************************************");

            var pfand = new AndSpecification<Product>(pfe, pfs);
            Console.WriteLine("\n");
            Console.WriteLine("***********************************************************************");
            Console.WriteLine("Products AndSpecification using existen class instance pfe, pfs  (New)");
            Console.WriteLine("***********************************************************************");
            Console.WriteLine("\n");
            foreach (var p in bfilter.Filter(products, pfand))
                Console.WriteLine($" -{p.Name} is Green and Small");
            Console.WriteLine("\n");
            Console.WriteLine("***********************************************************************");


            var pfColor = new ColorSpecification(Color.Blue);
            var pfSize = new SizeSpecification(Size.Large);
            var pfandCustom = new AndSpecification<Product>(pfColor, pfSize);
            Console.WriteLine("\n");
            Console.WriteLine("*************************************************************************");
            Console.WriteLine("Products AndSpecification using new instance class pfColor, pfSize  (New)");
            Console.WriteLine("*************************************************************************");
            Console.WriteLine("\n");
            foreach (var p in bfilter.Filter(products, pfandCustom))
                Console.WriteLine($" -{p.Name} is Blu and Large");
            Console.WriteLine("\n");
            Console.WriteLine("***********************************************************************");


            Console.ReadLine();
        }
    }
}
