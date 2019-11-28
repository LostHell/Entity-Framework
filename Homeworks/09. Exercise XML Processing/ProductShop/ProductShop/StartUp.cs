using AutoMapper;
using AutoMapper.QueryableExtensions;
using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            //using (var db = new ProductShopContext())
            //{
            //    db.Database.EnsureDeleted();
            //    db.Database.EnsureCreated();
            //}

            Mapper.Initialize(cfg => cfg.AddProfile<ProductShopProfile>());
            var db = new ProductShopContext();

            ////Problem 1
            //var xmlUser = File.ReadAllText("./../../../Datasets/users.xml");
            //Console.WriteLine(ImportUsers(db, xmlUser));

            ////Problem 2
            //var xmlProduct = File.ReadAllText("./../../../Datasets/products.xml");
            //Console.WriteLine(ImportProducts(db, xmlProduct));

            ////Problem 3
            //var xmlCategory = File.ReadAllText("./../../../Datasets/categories.xml");
            //Console.WriteLine(ImportCategories(db, xmlCategory));

            ////Problem 4
            //var xmlCategoryProduct = File.ReadAllText("./../../../Datasets/categories-products.xml");
            //Console.WriteLine(ImportCategoryProducts(db, xmlCategoryProduct));

            ////Problem 5
            //Console.WriteLine(GetProductsInRange(db));

            ////Problem 6
            //Console.WriteLine(GetSoldProducts(db));

            ////Problem 7
            //Console.WriteLine(GetCategoriesByProductsCount(db));

            //Problem 8
            Console.WriteLine(GetUsersWithProducts(db));
        }

        //Problem 1
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(List<ImportUserDto>), new XmlRootAttribute("Users"));
            var listUsers = (List<ImportUserDto>)serializer.Deserialize(new StringReader(inputXml));
            var users = Mapper.Map<List<User>>(listUsers);

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        //Problem 2
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(List<ImportProductDto>), new XmlRootAttribute("Products"));
            var listProduct = (List<ImportProductDto>)serializer.Deserialize(new StringReader(inputXml));
            var products = Mapper.Map<List<Product>>(listProduct);

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        //Problem 3
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(List<ImportCategoryDto>), new XmlRootAttribute("Categories"));
            var listCategory = (List<ImportCategoryDto>)serializer.Deserialize(new StringReader(inputXml));
            var categories = Mapper.Map<List<Category>>(listCategory);

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }

        //Problem 4
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(List<ImportCategoriesProductsDto>), new XmlRootAttribute("CategoryProducts"));
            var categoryProduct = (List<ImportCategoriesProductsDto>)serializer.Deserialize(new StringReader(inputXml));
            var listCategoryProduct = Mapper.Map<List<CategoryProduct>>(categoryProduct);

            context.CategoryProducts.AddRange(listCategoryProduct);
            context.SaveChanges();

            return $"Successfully imported {listCategoryProduct.Count}";
        }

        //Problem 5
        public static string GetProductsInRange(ProductShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var products = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .Select(x => new ProductDto
                {
                    Name = x.Name,
                    Price = x.Price,
                    Buyer = x.Buyer.FirstName + " " + x.Buyer.LastName
                })
                .OrderBy(x => x.Price)
                .Take(10)
                .ToList();

            var xmlProducts = new XmlSerializer(typeof(List<ProductDto>), new XmlRootAttribute("Products"));

            var namespaces = new XmlSerializerNamespaces();

            namespaces.Add(string.Empty, string.Empty);

            xmlProducts.Serialize(new StringWriter(sb), products, namespaces);

            return sb.ToString().TrimEnd();
        }

        //Problem 6
        public static string GetSoldProducts(ProductShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var users = context.Users
                 .Where(x => x.ProductsSold.Any(a => a.Buyer != null))
                 .Select(x => new SoldProductDto
                 {
                     FirstName = x.FirstName,
                     LastName = x.LastName,
                     Products = x.ProductsSold
                     .Select(a => new ProductDto
                     {
                         Name = a.Name,
                         Price = a.Price
                     })
                     .ToList()
                 })
                 .OrderBy(x => x.LastName)
                 .ThenBy(x => x.FirstName)
                 .Take(5)
                 .ToList();

            XmlSerializer serializer = new XmlSerializer(typeof(List<SoldProductDto>), new XmlRootAttribute("Users"));

            var namespaces = new XmlSerializerNamespaces();

            namespaces.Add(string.Empty, string.Empty);

            serializer.Serialize(new StringWriter(sb), users, namespaces);

            return sb.ToString().TrimEnd();
        }

        //Problem 7
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var countProducts = context.Categories
                .Select(x => new CategoryProductDto
                {
                    Name = x.Name,
                    Count = x.CategoryProducts.Count,
                    AveragePrice = x.CategoryProducts.Average(a => a.Product.Price),
                    TotalRevenue = x.CategoryProducts.Sum(a => a.Product.Price)
                })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.TotalRevenue)
                .ToList();

            XmlSerializer serializer = new XmlSerializer(typeof(List<CategoryProductDto>), new XmlRootAttribute("Categories"));

            var namespaces = new XmlSerializerNamespaces();

            namespaces.Add(string.Empty, string.Empty);

            serializer.Serialize(new StringWriter(sb), countProducts, namespaces);

            return sb.ToString().TrimEnd();
        }

        //Problem 8
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var usersProducts = context.Users
                .Where(a => a.ProductsSold.Any())
                .OrderByDescending(x => x.ProductsSold.Count())
                .Select(x => new UserProductDto
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Age = x.Age,
                    SoldProduct = new ArrayWithCountDto
                    {
                        Count = x.ProductsSold.Count,
                        SoldProduct = x.ProductsSold.Select(a => new UserProductArrayDto
                        {
                            Name = a.Name,
                            Price = a.Price
                        })
                            .OrderByDescending(p => p.Price)
                            .ToArray()
                    }
                })
                .Take(10)
                .ToArray();

            var userAndProductCountDto = new ExportUserAndProductDto()
            {
                Count = context.Users.Count(x => x.ProductsSold.Any()),
                Users = usersProducts
            };

            var xmlSerializer = new XmlSerializer(typeof(ExportUserAndProductDto), new XmlRootAttribute("Users"));

            var namespaces = new XmlSerializerNamespaces();

            namespaces.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(new StringWriter(sb), userAndProductCountDto, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}