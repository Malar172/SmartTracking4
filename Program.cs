using AssetTracking;
using System.Linq;
using System.IO;
namespace AssetTracking
{
    internal class Program
    {
        static AssetDbContext context = new AssetDbContext();

        static void Main(string[] args)
        {
            bool running = true;

            while (running)
            {
                Console.Clear();

                Console.WriteLine("=================================");
                Console.WriteLine(" SMART ASSET TRACKING SYSTEM");
                Console.WriteLine("=================================");
                Console.WriteLine("1. Add Asset");
                Console.WriteLine("2. Show Assets");
                Console.WriteLine("3. Update Asset");
                Console.WriteLine("4. Delete Asset");
                Console.WriteLine("5. Search Asset");
                Console.WriteLine("6. Reports");
                Console.WriteLine("7. Export CSV");
                Console.WriteLine("8. Exit");


                Console.Write("\nChoice: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddAsset();
                        break;

                    case "2":
                        ShowAssets();
                        break;

                    case "3":
                        UpdateAsset();
                        break;

                    case "4":
                        DeleteAsset();
                        break;

                    case "5":
                        SearchAsset();
                        break;
                    case "6":
                        ShowReport();
                        break;

                    case "7":
                        ExportToCsv();
                        break;
                    case "8":
                        running = false;
                        break;



                    default:
                        Console.WriteLine("Invalid Option");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void AddAsset()
        {
            Console.Clear();

            Console.Write("Category: ");
            string category = Console.ReadLine() ?? "";

            Console.Write("Brand: ");
            string brand = Console.ReadLine() ?? "";

            Console.Write("Model: ");
            string model = Console.ReadLine() ?? "";

            Console.Write("Office: ");
            string office = Console.ReadLine() ?? "";

            Console.Write("Purchase Date (yyyy-mm-dd): ");

            if (!DateTime.TryParse(Console.ReadLine(), out DateTime purchaseDate))
            {
                Console.WriteLine("Invalid Date");
                Console.ReadKey();
                return;
            }

            Console.Write("Price USD: ");

            if (!decimal.TryParse(Console.ReadLine(), out decimal price))
            {
                Console.WriteLine("Invalid Price");
                Console.ReadKey();
                return;
            }

            Asset asset = new Asset
            {
                Category = category,
                Brand = brand,
                Model = model,
                Office = office,
                PurchaseDate = purchaseDate,
                PriceUSD = price
            };

            context.Assets.Add(asset);
            context.SaveChanges();

            Console.WriteLine("\nAsset Added Successfully!");
            Console.ReadKey();
        }

        static string GetStatus(DateTime purchaseDate)
        {
            DateTime endDate = purchaseDate.AddYears(3);

            double monthsLeft =
                (endDate - DateTime.Now).TotalDays / 30;

            if (monthsLeft < 3)
                return "RED";

            if (monthsLeft < 6)
                return "YELLOW";

            return "NORMAL";
        }

        static void ShowAssets()
        {
            Console.Clear();

            var assets = context.Assets
                .OrderBy(a => a.Category)
                .ThenBy(a => a.PurchaseDate)
                .ToList();

            Console.WriteLine(
                $"{"ID",-4}" +
                $"{"CATEGORY",-13}" +
                $"{"BRAND",-12}" +
                $"{"MODEL",-15}" +
                $"STATUS");

            Console.WriteLine(new string('-', 55));

            foreach (var asset in assets)
            {
                string status = GetStatus(asset.PurchaseDate);

                Console.Write(
                    $"{asset.Id,-4}" +
                    $"{asset.Category,-13}" +
                    $"{asset.Brand,-12}" +
                    $"{asset.Model,-15}"
                );

                if (status == "RED")
                    Console.ForegroundColor = ConsoleColor.Red;
                else if (status == "YELLOW")
                    Console.ForegroundColor = ConsoleColor.Yellow;
                else
                    Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine(status);

                Console.ResetColor();
            }

            Console.ReadKey();
        }

        static void UpdateAsset()
        {
            Console.Clear();

            Console.Write("Enter Asset ID: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID");
                Console.ReadKey();
                return;
            }

            var asset = context.Assets
                .FirstOrDefault(a => a.Id == id);

            if (asset == null)
            {
                Console.WriteLine("Asset Not Found");
                Console.ReadKey();
                return;
            }

            Console.Write("New Brand: ");
            asset.Brand = Console.ReadLine() ?? "";

            Console.Write("New Model: ");
            asset.Model = Console.ReadLine() ?? "";

            context.SaveChanges();

            Console.WriteLine("Asset Updated!");
            Console.ReadKey();
        }

        static void DeleteAsset()
        {
            Console.Clear();

            Console.Write("Enter Asset ID: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID");
                Console.ReadKey();
                return;
            }

            var asset = context.Assets
                .FirstOrDefault(a => a.Id == id);

            if (asset == null)
            {
                Console.WriteLine("Asset Not Found");
                Console.ReadKey();
                return;
            }

            context.Assets.Remove(asset);
            context.SaveChanges();

            Console.WriteLine("Asset Deleted!");
            Console.ReadKey();
        }
        static void SearchAsset()
        {
            Console.Clear();

            Console.WriteLine("Search By:");
            Console.WriteLine("1. Brand");
            Console.WriteLine("2. Model");
            Console.WriteLine("3. Office");
            Console.WriteLine("4. Purchase Year");

            Console.Write("Choice: ");
            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1":
                    Console.Write("Enter Brand: ");
                    string brand = Console.ReadLine() ?? "";

                    var brandResults = context.Assets
                        .Where(a => a.Brand.Contains(brand))
                        .ToList();

                    DisplayResults(brandResults);
                    break;

                case "2":
                    Console.Write("Enter Model: ");
                    string model = Console.ReadLine() ?? "";

                    var modelResults = context.Assets
                        .Where(a => a.Model.Contains(model))
                        .ToList();

                    DisplayResults(modelResults);
                    break;

                case "3":
                    Console.Write("Enter Office: ");
                    string office = Console.ReadLine() ?? "";

                    var officeResults = context.Assets
                        .Where(a => a.Office.Contains(office))
                        .ToList();

                    DisplayResults(officeResults);
                    break;

                case "4":
                    Console.Write("Enter Year: ");

                    int year = int.Parse(Console.ReadLine()!);

                    var yearResults = context.Assets
                        .Where(a => a.PurchaseDate.Year == year)
                        .ToList();

                    DisplayResults(yearResults);
                    break;
            }

            Console.ReadKey();
        }
        static void DisplayResults(List<Asset> assets)
        {
            Console.WriteLine();
            Console.WriteLine("========== SEARCH RESULTS ==========");
            Console.WriteLine();

            Console.WriteLine(
                $"{"ID",-4}" +
                $"{"TYPE",-12}" +
                $"{"MODEL",-15}" +
                $"{"OFFICE",-15}");

            foreach (var asset in assets)
            {
                Console.WriteLine(
                    $"{asset.Id,-4}" +
                    $"{asset.Category,-12}" +
                    $"{asset.Model,-15}" +
                    $"{asset.Office,-15}");
            }
        }


        static void ShowReport()
        {
            Console.Clear();

            Console.WriteLine("============= REPORT =============");
            Console.WriteLine();

            Console.WriteLine("Office Asset Counts");
            Console.WriteLine("----------------------------------");

            var officeCounts = context.Assets
                .GroupBy(a => a.Office)
                .Select(g => new
                {
                    Office = g.Key,
                    Count = g.Count()
                })
                .ToList();

            foreach (var office in officeCounts)
            {
                Console.WriteLine($"{office.Office} Office : {office.Count}");
            }

            Console.WriteLine();
            Console.WriteLine("Assets Near Expiration");
            Console.WriteLine("----------------------------------");
            var expiringAssets = context.Assets
            .ToList()
             .Where(a => a.PurchaseDate.AddYears(3) < DateTime.Now.AddDays(180))
             .ToList();

            foreach (var asset in expiringAssets)
            {
                Console.WriteLine(
                    $"{asset.Brand} {asset.Model}");
            }

            Console.WriteLine();
            Console.WriteLine("Most Expensive Asset");
            Console.WriteLine("----------------------------------");

            var expensiveAsset = context.Assets
                .OrderByDescending(a => a.PriceUSD)
                .FirstOrDefault();

            if (expensiveAsset != null)
            {
                Console.WriteLine(
                    $"{expensiveAsset.Brand} {expensiveAsset.Model} - {expensiveAsset.PriceUSD:C}");
            }

            Console.WriteLine();
            Console.WriteLine("==================================");
            Console.ReadKey();
        }
        static void FilterAssets()
        {
            Console.Clear();
            Console.WriteLine("1. Show Only Laptops");
            Console.WriteLine("2. Show Only Mobiles");

            string choice = Console.ReadLine() ?? "";

            List<Asset> assets = new();

            if (choice == "1")
            {
                assets = context.Assets
                    .Where(a => a.Category == "Laptop")
                    .ToList();
            }
            else if (choice == "2")
            {
                assets = context.Assets
                    .Where(a => a.Category == "Mobile")
                    .ToList();
            }

            DisplayResults(assets);

            Console.ReadKey();
        }
        static void ExportToCsv()
        {
            var assets = context.Assets.ToList();

            string filePath = "AssetsReport.csv";

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Id,Type,Brand,Model,Office,Price");

                foreach (var asset in assets)
                {
                    writer.WriteLine(
                        $"{asset.Id}," +
                        $"{asset.Category}," +
                        $"{asset.Brand}," +
                        $"{asset.Model}," +
                        $"{asset.Office}," +
                        $"{asset.PriceUSD}");
                }
            }

            Console.WriteLine();
            Console.WriteLine("CSV exported successfully!");
            Console.WriteLine("File: " + filePath);
            Console.WriteLine(Path.GetFullPath(filePath));

            Console.ReadKey();
        }
    }
}