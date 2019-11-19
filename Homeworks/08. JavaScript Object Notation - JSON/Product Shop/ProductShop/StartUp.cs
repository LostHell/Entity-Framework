using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.Dto.Export;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var db = new ProductShopContext();

            //db.Database.EnsureDeleted();
            //db.Database.EnsureCreated();

            //Problem 1
            //var path = File.ReadAllText("./../../../Datasets/users.json");
            //var result = ImportUsers(db, path);
            //Console.WriteLine(result);

            //Problem 2
            //var path = File.ReadAllText("./../../../Datasets/products.json");
            //var result = ImportProducts(db, path);
            //Console.WriteLine(result);

            //Problem 3
            //var path = File.ReadAllText("./../../../Datasets/categories.json");
            //var result = ImportCategories(db, path);
            //Console.WriteLine(result);

            //Problem 4
            //var path = File.ReadAllText("./../../../Datasets/categories-products.json");
            //var result = ImportCategoryProducts(db, path);
            //Console.WriteLine(result);

            ////Problem 5
            //Console.WriteLine(GetProductsInRange(db));

            //Problem 6
            //Console.WriteLine(GetSoldProducts(db));

            //Problem 7
            //Console.WriteLine(GetCategoriesByProductsCount(db));

            //Problem 8
            Console.WriteLine(GetUsersWithProducts(db));
        }

        //Problem 1
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var users = JsonConvert.DeserializeObject<User[]>(inputJson);

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Length}";
        }

        //Problem 2
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var products = JsonConvert.DeserializeObject<Product[]>(inputJson);

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Length}";
        }

        //Problem 3
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var categories = JsonConvert.DeserializeObject<List<Category>>(inputJson);
            int count = 0;
            foreach (var category in categories)
            {
                if (category.Name != null)
                {
                    context.Categories.Add(category);
                    context.SaveChanges();
                    count++;
                }
            }

            return $"Successfully imported {count}";
        }

        //Problem 4
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoryProducts = JsonConvert.DeserializeObject<CategoryProduct[]>(inputJson);

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Length}";
        }

        //Problem 5
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .OrderBy(a => a.Price)
                .Select(x => new
                {
                    name = x.Name,
                    price = x.Price,
                    seller = $"{x.Seller.FirstName} {x.Seller.LastName}"
                })
                .ToList();


            var json = JsonConvert.SerializeObject(products, Formatting.Indented);

            return json;
        }

        //Problem 6
        public static string GetSoldProducts(ProductShopContext context)
        {
            var filterUsers = context.Users
                .Where(x => x.ProductsSold.Any(a => a.Buyer != null))
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    soldtProduct = x.ProductsSold
                    .Where(a => a.Buyer != null)
                    .Select(a => new
                    {
                        name = a.Name,
                        price = a.Price,
                        buyerFirstName = a.Buyer.FirstName,
                        buyerLastName = a.Buyer.LastName
                    })
                    .ToList()
                })
                .ToList();

            var json = JsonConvert.SerializeObject(filterUsers, Formatting.Indented);

            return json;
        }

        //Problem 7
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .OrderByDescending(x => x.CategoryProducts.Count())
                .Select(x => new
                {
                    category = x.Name,
                    productsCount = x.CategoryProducts.Count(),
                    averagePrice = $"{x.CategoryProducts.Select(a => a.Product.Price).Average():F2}",
                    totalRevenue = $"{x.CategoryProducts.Select(a => a.Product.Price).Sum():F2}"
                })
                .ToList();

            //var categories = context.CategoryProducts
            //    .Where(x => x.Category.Id == x.CategoryId)
            //    .OrderByDescending(x => x.Category.CategoryProducts.Count())
            //    .Select(x => new
            //    {
            //        category = x.Category.Name,
            //        productsCount = x.Category.CategoryProducts.Count(),
            //        averagePrice = $"{x.Category.CategoryProducts.Average(a => a.Product.Price):F2}",
            //        totalRevenue = $"{x.Category.CategoryProducts.Sum(a => a.Product.Price):F2}"
            //    });

            var json = JsonConvert.SerializeObject(categories, Formatting.Indented);

            return json;
        }

        //Problem 8
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .OrderByDescending(p => p.ProductsSold.Count(ps => ps.Buyer != null))
                .Select(u => new UserWithProductsDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new SoldProductsToUserDto
                    {
                        Count = u.ProductsSold.Count(p => p.Buyer != null),
                        Products = u.ProductsSold
                        .Where(p => p.Buyer != null)
                        .Select(p => new SoldProductsDto
                        {
                            Name = p.Name,
                            Price = p.Price
                        })
                        .ToList()
                    }
                })
                .ToList();

            var result = new UsersAndProductsDto
            {
                UsersCount = users.Count(),
                Users = users
            };

            var json = JsonConvert.SerializeObject(result,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

            return json;
        }
    }
}