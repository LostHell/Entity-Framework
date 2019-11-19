using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            //using (var dbo = new CarDealerContext())
            //{
            //    dbo.Database.EnsureDeleted();
            //    dbo.Database.EnsureCreated();
            //}
            var db = new CarDealerContext();

            //Problem 1
            //var path1 = File.ReadAllText("./../../../Datasets/suppliers.json");
            //var result1 = ImportSuppliers(db, path1);
            //Console.WriteLine(result1);

            //Problem 2
            //var path2 = File.ReadAllText("./../../../Datasets/parts.json");
            //var result2 = ImportParts(db, path2);
            //Console.WriteLine(result2);

            //Problem 3
            //var path3 = File.ReadAllText("./../../../Datasets/cars.json");
            //var result3 = ImportCars(db, path3);
            //Console.WriteLine(result3);

            //Problem 4
            //var path4 = File.ReadAllText("./../../../Datasets/customers.json");
            //var result4 = ImportCustomers(db, path4);
            //Console.WriteLine(result4);

            //Problem 5
            //var path5 = File.ReadAllText("./../../../Datasets/sales.json");
            //var result5 = ImportSales(db, path5);
            //Console.WriteLine(result5);

            //Problem 6
             Console.WriteLine(GetOrderedCustomers(db));

            //Problem 7
            //Console.WriteLine(GetCarsFromMakeToyota(db));

            //Problem 8
            //Console.WriteLine(GetLocalSuppliers(db));

            //Problem 9
            //Console.WriteLine(GetCarsWithTheirListOfParts(db));

            //Problem 10
            //Console.WriteLine(GetTotalSalesByCustomer(db));

            //Problem 11
            //Console.WriteLine(GetSalesWithAppliedDiscount(db));
        }

        //Problem 1
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var Suppliers = JsonConvert.DeserializeObject<List<Supplier>>(inputJson);

            context.Suppliers.AddRange(Suppliers);
            context.SaveChanges();

            return $"Successfully imported {Suppliers.Count}.";
        }

        //Problem 2
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var Parts = JsonConvert.DeserializeObject<List<Part>>(inputJson);
            List<Part> partsToImport = new List<Part>();

            foreach (var part in Parts)
            {
                if (part.SupplierId <= context.Suppliers
                    .OrderByDescending(x => x.Id)
                    .Select(x => x.Id)
                    .FirstOrDefault())
                {
                    partsToImport.Add(part);
                }
            }

            context.Parts.AddRange(partsToImport);
            context.SaveChanges();

            return $"Successfully imported {partsToImport.Count()}.";
        }

        //Problem 3
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var cars = JsonConvert.DeserializeObject<CarPartsImporter[]>(inputJson);

            foreach (var carDto in cars)
            {
                Car car = new Car
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TravelledDistance = carDto.TravelledDistance
                };

                context.Cars.Add(car);

                foreach (var partId in carDto.PartsId)
                {
                    PartCar partCar = new PartCar
                    {
                        CarId = car.Id,
                        PartId = partId
                    };

                    if (car.PartCars.FirstOrDefault(p => p.PartId == partId) == null)
                    {
                        context.PartCars.Add(partCar);
                    }
                }
            }

            context.SaveChanges();

            return $"Successfully imported {cars.Count()}.";
        }

        //Problem 4
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var Customers = JsonConvert.DeserializeObject<List<Customer>>(inputJson);

            context.Customers.AddRange(Customers);
            context.SaveChanges();

            return $"Successfully imported {Customers.Count}.";
        }

        //Problem 5
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var Sales = JsonConvert.DeserializeObject<List<Sale>>(inputJson);

            context.Sales.AddRange(Sales);
            context.SaveChanges();

            return $"Successfully imported {Sales.Count}.";
        }

        //Problem 6
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customer = context.Customers
                .OrderBy(x => x.BirthDate)
                .ThenBy(x => x.IsYoungDriver)
                .Select(x => new
                {
                    Name = x.Name,
                    BirthDate = x.BirthDate.ToString("dd/MM/yyyy"),
                    IsYoungDrive = x.IsYoungDriver
                })
                .ToList();

            var json = JsonConvert.SerializeObject(customer, Formatting.Indented);

            return json;
        }

        //Problem 7
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var carMake = context.Cars
                .Where(x => x.Make == "Toyota")
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .Select(x => new
                {
                    Id = x.Id,
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .ToList();

            var json = JsonConvert.SerializeObject(carMake, Formatting.Indented);

            return json;
        }

        //Problem 8
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var supplier = context.Suppliers
                .Where(x => x.IsImporter == false)
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartsCount = x.Parts.Count()
                })
                .ToList();

            var json = JsonConvert.SerializeObject(supplier, Formatting.Indented);

            return json;
        }

        //Problem 9
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carParts = context.Cars
                .Select(x => new
                {
                    car = new CarDTO
                    {
                        Make = x.Make,
                        Model = x.Model,
                        TravelledDistance = x.TravelledDistance
                    },
                    parts = x.PartCars.Select(p => new PartDTO
                    {
                        Name = p.Part.Name,
                        Price = $"{p.Part.Price:F2}"
                    })
                    .ToList()
                })
                .ToList();

            var json = JsonConvert.SerializeObject(carParts, Formatting.Indented);

            return json;
        }

        //Problem 10
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(x => x.Sales.Count() >= 1)
                .Select(x => new CustomerWithCarDTO
                {
                    FullName = x.Name,
                    BoughtCars = x.Sales.Count(),
                    SpentMoney = x.Sales.Sum(a => a.Car.PartCars.Sum(z => z.Part.Price))
                })
                .OrderByDescending(x => x.SpentMoney)
                .ThenByDescending(x => x.BoughtCars)
                .ToList();

            var json = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return json;
        }

        //Proble 11
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var salesDiscount = context.Sales
                .Select(x => new
                {
                    car = new CarDTO
                    {
                        Make = x.Car.Make,
                        Model = x.Car.Model,
                        TravelledDistance = x.Car.TravelledDistance
                    },
                    customerName = x.Customer.Name,
                    Discount = $"{x.Discount:F2}",
                    price = $"{x.Car.PartCars.Sum(a => a.Part.Price):F2}",
                    priceWithDiscount = $@"{(x.Car.PartCars.Sum(p => p.Part.Price) -
                    x.Car.PartCars.Sum(p => p.Part.Price) * x.Discount / 100):F2}"
                })
                .Take(10)
                .ToList();

            var json = JsonConvert.SerializeObject(salesDiscount, Formatting.Indented);

            return json;
        }
    }
}