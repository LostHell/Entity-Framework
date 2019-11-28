using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.Dtos.Export;
using CarDealer.Dtos.Import;
using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            //using (var dbto = new CarDealerContext())
            //{
            //    dbto.Database.EnsureDeleted();
            //    dbto.Database.EnsureCreated();
            //}

            Mapper.Initialize(cfg => cfg.AddProfile<CarDealerProfile>());

            var db = new CarDealerContext();

            ////Problem 1
            //var xmlSuppliers = File.ReadAllText("./../../../Datasets/suppliers.xml");
            //Console.WriteLine(ImportSuppliers(db, xmlSuppliers));

            ////Problem 2
            //var xmlParts = File.ReadAllText("./../../../Datasets/parts.xml");
            //Console.WriteLine(ImportParts(db, xmlParts));

            ////Problem 3
            //var xmlCars = File.ReadAllText("./../../../Datasets/cars.xml");
            //Console.WriteLine(ImportCars(db, xmlCars));

            ////Problem 4
            //var xmlCustomers = File.ReadAllText("./../../../Datasets/customers.xml");
            //Console.WriteLine(ImportCustomers(db, xmlCustomers));

            ////Problem 5
            //var xmlSales = File.ReadAllText("./../../../Datasets/sales.xml");
            //Console.WriteLine(ImportSales(db, xmlSales));

            ////Problem 6
            //Console.WriteLine(GetCarsWithDistance(db));

            ////Problem 7
            //Console.WriteLine(GetCarsFromMakeBmw(db));

            ////Prolbem 8
            //Console.WriteLine(GetLocalSuppliers(db));

            ////Problem 9
            //Console.WriteLine(GetCarsWithTheirListOfParts(db));

            ////Problem 10
            //Console.WriteLine(GetTotalSalesByCustomer(db));

            //Problem 11
            Console.WriteLine(GetSalesWithAppliedDiscount(db));
        }

        //Problem 1
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(List<SupplierDto>), new XmlRootAttribute("Suppliers"));
            var suppliersDto = (List<SupplierDto>)serializer.Deserialize(new StringReader(inputXml));

            foreach (var supplierDto in suppliersDto)
            {
                var supplier = Mapper.Map<Supplier>(supplierDto);

                context.Suppliers.Add(supplier);
            }

            int count = context.SaveChanges();

            return $"Successfully imported {count}";
        }

        //Problem 2
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(List<PartDto>), new XmlRootAttribute("Parts"));
            var partsDto = (List<PartDto>)serializer.Deserialize(new StringReader(inputXml));

            foreach (var partDto in partsDto)
            {
                var part = Mapper.Map<Part>(partDto);
                var supplier = context.Suppliers
                    .Select(x => x.Id)
                    .ToList();

                if (supplier.Contains(part.SupplierId))
                {
                    context.Parts.Add(part);
                }
            }

            int count = context.SaveChanges();

            return $"Successfully imported {count}";
        }

        //Problem 3
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(List<CarDto>), new XmlRootAttribute("Cars"));
            var carsDto = (List<CarDto>)serializer.Deserialize(new StringReader(inputXml));

            foreach (var carDto in carsDto)
            {
                var car = Mapper.Map<Car>(carDto);
                context.Cars.Add(car);

                foreach (var PartIds in carDto.Parts.PartIds)
                {
                    if (car.PartCars.FirstOrDefault(x => x.PartId == PartIds.Id) == null &&
                        context.Parts.Find(PartIds.Id) != null)
                    {
                        var autoPart = new PartCar()
                        {
                            PartId = PartIds.Id,
                            CarId = car.Id
                        };

                        context.PartCars.Add(autoPart);
                    }
                }
            }
            ;

            context.SaveChanges();

            return $"Successfully imported {carsDto.Count}";
        }

        //Problem 4
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(List<CustomerDto>), new XmlRootAttribute("Customers"));
            var customersDto = (List<CustomerDto>)serializer.Deserialize(new StringReader(inputXml));

            foreach (var customerDto in customersDto)
            {
                var customer = Mapper.Map<Customer>(customerDto);
                context.Customers.Add(customer);
            }

            int count = context.SaveChanges();

            return $"Successfully imported {count}";
        }

        //Problem 5
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(List<SaleDto>), new XmlRootAttribute("Sales"));
            var salesDto = (List<SaleDto>)serializer.Deserialize(new StringReader(inputXml));

            foreach (var saleDto in salesDto)
            {
                var sale = Mapper.Map<Sale>(saleDto);
                if (context.Cars.FirstOrDefault(x => x.Id == sale.CarId) != null)
                {
                    context.Sales.Add(sale);
                }
            }

            int count = context.SaveChanges();

            return $"Successfully imported {count}";
        }

        //Problem 6
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            StringBuilder sb = new StringBuilder();

            var cars = context.Cars
                .Where(x => x.TravelledDistance >= 2000000)
                .Select(x => new
                {
                    x.Make,
                    x.Model,
                    x.TravelledDistance
                })
                .OrderBy(x => x.Make)
                .ThenBy(x => x.Model)
                .Take(10)
                .ProjectTo<ExportCarDto>()
                .ToList();

            var xmlSerializer = new XmlSerializer(typeof(List<ExportCarDto>), new XmlRootAttribute("cars"));

            var namespaces = new XmlSerializerNamespaces();

            namespaces.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(new StringWriter(sb), cars, namespaces);

            return sb.ToString().TrimEnd();
        }

        //Problem 7
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            StringBuilder sb = new StringBuilder();

            var cars = context.Cars
                .Where(x => x.Make == "BMW")
                .Select(x => new
                {
                    x.Id,
                    x.Model,
                    x.TravelledDistance
                })
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .ProjectTo<CarBMWDto>()
                .ToList();

            var xmlSerializer = new XmlSerializer(typeof(List<CarBMWDto>), new XmlRootAttribute("cars"));

            var namespaces = new XmlSerializerNamespaces();

            namespaces.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(new StringWriter(sb), cars, namespaces);

            return sb.ToString().TrimEnd();
        }

        //Problem 8
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            StringBuilder sb = new StringBuilder();

            var suppliers = context.Suppliers
                .Where(x => x.IsImporter == false)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Parts.Count
                })
                .ProjectTo<ExportSupplierDto>()
                .ToList();

            var xmlSerializer = new XmlSerializer(typeof(List<ExportSupplierDto>), new XmlRootAttribute("suppliers"));

            var namespaces = new XmlSerializerNamespaces();

            namespaces.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(new StringWriter(sb), suppliers, namespaces);

            return sb.ToString().TrimEnd();
        }

        //Problem 9
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            StringBuilder sb = new StringBuilder();

            var cars = context.Cars
                .Select(x => new ExportCarSpecificDto
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance,
                    Parts = x.PartCars.Select(a => new PartSpecificDto
                    {
                        Name = a.Part.Name,
                        Price = a.Part.Price
                    })
                    .OrderByDescending(p => p.Price)
                    .ToList()
                })
                .OrderByDescending(x => x.TravelledDistance)
                .ThenBy(x => x.Model)
                .Take(5)
                .ToList();

            var xmlSerializer = new XmlSerializer(typeof(List<ExportCarSpecificDto>), new XmlRootAttribute("cars"));

            var namespaces = new XmlSerializerNamespaces();

            namespaces.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(new StringWriter(sb), cars, namespaces);

            return sb.ToString().TrimEnd();
        }

        //Problem 10
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            StringBuilder sb = new StringBuilder();

            var customers = context.Customers
                .Where(x => x.Sales.Count >= 1)
                .Select(x => new ExportCustomerDto
                {
                    FullName = x.Name,
                    Count = x.Sales.Count,
                    Money = x.Sales.Sum(a => a.Car.PartCars.Sum(z => z.Part.Price))
                })
                .OrderByDescending(x => x.Money)
                .ToList();

            var xmlSerializer = new XmlSerializer(typeof(List<ExportCustomerDto>), new XmlRootAttribute("customers"));

            var namespaces = new XmlSerializerNamespaces();

            namespaces.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(new StringWriter(sb), customers, namespaces);

            return sb.ToString().TrimEnd();
        }

        //Problem 11
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            StringBuilder sb = new StringBuilder();

            var sales = context.Sales
                .Select(x => new SalesDiscountDto
                {
                    Car = new SalesDiscountCarDto
                    {
                        Make = x.Car.Make,
                        Model = x.Car.Model,
                        TravelledDistance = x.Car.TravelledDistance
                    },
                    Discount = x.Discount,
                    Name = x.Customer.Name,
                    Price = x.Car.PartCars.Sum(a => a.Part.Price),
                    PriceWithDiscount = x.Car.PartCars.Sum(a => a.Part.Price) -
                    x.Car.PartCars.Sum(a => a.Part.Price) *
                    x.Discount / 100
                })
                .ToList();

            var xmlSerializer = new XmlSerializer(typeof(List<SalesDiscountDto>), new XmlRootAttribute("sales"));

            var namespaces = new XmlSerializerNamespaces();

            namespaces.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(new StringWriter(sb), sales, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}