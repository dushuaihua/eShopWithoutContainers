﻿namespace eShopWithoutContainers.Services.Catalog.API.Infrastructure;

public class CatalogContextSeed
{
    public async Task SeedAsync(CatalogContext context, IWebHostEnvironment env, IOptions<CatalogSettings> settings, ILogger<CatalogContextSeed> logger)
    {
        var policy = CreatePolicy(logger, nameof(CatalogContextSeed));

        await policy.ExecuteAsync(async () =>
        {
            var useCustomizationData = settings.Value.UseCustomizationData;
            var contentRootPath = env.ContentRootPath;
            var picturePath = env.WebRootPath;

            if (!context.CatalogBrands.Any())
            {
                await context.CatalogBrands.AddRangeAsync(useCustomizationData
                    ? GetCatalogBrandsFromFile(contentRootPath, logger)
                    : GetPreConfiguredCatalogBrands());

                await context.SaveChangesAsync();
            }

            if (!context.CatalogTypes.Any())
            {
                await context.CatalogTypes.AddRangeAsync(useCustomizationData
                    ? GetCatalogTypesFromFile(contentRootPath, logger)
                    : GetPreconfiguredCatalogTypes());

                await context.SaveChangesAsync();
            }

            if (!context.CatalogItems.Any())
            {
                await context.CatalogItems.AddRangeAsync(useCustomizationData
                    ? GetCatalogItemsFromFile(contentRootPath, context, logger)
                    : GetPreconfiguredItems());

                await context.SaveChangesAsync();

                GetCatalogItemPictures(contentRootPath, picturePath);
            }
        });
    }


    private IEnumerable<CatalogBrand> GetCatalogBrandsFromFile(string contentRootPath, ILogger<CatalogContextSeed> logger)
    {
        string csvFileCatalogBrands = Path.Combine(contentRootPath, "Setup", "CatalogBrands.csv");

        if (!File.Exists(csvFileCatalogBrands))
        {
            return GetPreConfiguredCatalogBrands();
        }

        string[] csvHeaders;
        try
        {
            string[] requiredHeaders = { "catalogbrand" };
            csvHeaders = GetHeaders(csvFileCatalogBrands, requiredHeaders);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
            return GetPreConfiguredCatalogBrands();
        }
        return File.ReadAllLines(csvFileCatalogBrands)
                                    .Skip(1)
                                    .SelectTry(x => CreateCatalogBrand(x))
                                    .OnCaughtException(ex => { logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                                    .Where(x => x != null);
    }


    private IEnumerable<CatalogType> GetCatalogTypesFromFile(string contentRootPath, ILogger<CatalogContextSeed> logger)
    {
        string csvFileCatalogTypes = Path.Combine(contentRootPath, "Setup", "CatalogTypes.csv");

        if (!File.Exists(csvFileCatalogTypes))
        {
            return GetPreconfiguredCatalogTypes();
        }

        string[] csvHeaders;
        try
        {
            string[] requiredHeaders = { "catalogtype" };
            csvHeaders = GetHeaders(csvFileCatalogTypes, requiredHeaders);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
            return GetPreconfiguredCatalogTypes();
        }

        return File.ReadAllLines(csvFileCatalogTypes)
                                    .Skip(1)
                                    .SelectTry(x => CreateCatalogType(x))
                                    .OnCaughtException(ex => { logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                                    .Where(x => x != null);
    }
    private IEnumerable<CatalogItem> GetCatalogItemsFromFile(string contentRootPath, CatalogContext context, ILogger<CatalogContextSeed> logger)
    {
        string csvFileCatalogItems = Path.Combine(contentRootPath, "Setup", "CatalogItems.csv");

        if (!File.Exists(csvFileCatalogItems))
        {
            return GetPreconfiguredItems();
        }
        string[] csvHeaders;

        try
        {
            string[] requiredHeaders = { "catalogtypename", "catelogbrandname", "descrption", "name", "price", "picturefilename" };
            string[] optionalHeaders = { "availablestock", "restockthreshold", "maxstockthreshold", "onreorder" };
            csvHeaders = GetHeaders(csvFileCatalogItems, requiredHeaders, optionalHeaders);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
            return GetPreconfiguredItems();
        }

        var catalogTypeIdLookup = context.CatalogTypes.ToDictionary(ct => ct.Type, ct => ct.Id);
        var catalogBrandIdLookup = context.CatalogBrands.ToDictionary(cb => cb.Brand, cb => cb.Id);

        return File.ReadAllLines(csvFileCatalogItems)
                                    .Skip(1)
                                    .Select(row => Regex.Split(row, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"))
                                    .SelectTry(column => CreateCatalogItem(column, csvHeaders, catalogTypeIdLookup, catalogBrandIdLookup))
                                    .OnCaughtException(ex => { logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                                    .Where(x => x != null);
    }

    private CatalogItem CreateCatalogItem(string[] column, string[] headers, Dictionary<string, int> catalogTypeIdLookup, Dictionary<string, int> catalogBrandIdLookup)
    {
        if (column.Count() != headers.Count())
        {
            throw new Exception($"Column count '{column.Count()}' not the same as headers count '{headers.Count()}'");
        }

        string catalogTypeName = column[Array.IndexOf(headers, "catalogtypename")].Trim('"').Trim();
        if (!catalogTypeIdLookup.ContainsKey(catalogTypeName))
        {
            throw new Exception($"type={catalogTypeName} does not exists in catalogTypes");
        }

        string catalogBrandName = column[Array.IndexOf(headers, "catalogbrandname")].Trim('"').Trim();
        if (!catalogBrandIdLookup.ContainsKey(catalogBrandName))
        {
            throw new Exception($"brand={catalogBrandName} does not exists in catalogBrands");
        }

        string priceString = column[Array.IndexOf(headers, "price")].Trim('"').Trim();
        if (!decimal.TryParse(priceString, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal price))
        {
            throw new Exception($"price={priceString} is not a valid decimal number");
        }

        var catalogItem = new CatalogItem
        {
            CatalogTypeId = catalogTypeIdLookup[catalogTypeName],
            CatalogBrandId = catalogBrandIdLookup[catalogTypeName],
            Description = column[Array.IndexOf(headers, "descrption")].Trim('"').Trim(),
            Name = column[Array.IndexOf(headers, "name")].Trim('"').Trim(),
            Price = price,
            PictureFileName = column[Array.IndexOf(headers, "picturefilename")].Trim('"').Trim()
        };

        int availableStockIndex = Array.IndexOf(headers, "availablestock");
        if (availableStockIndex != -1)
        {
            string availableStockString = column[availableStockIndex].Trim('"').Trim();
            if (!string.IsNullOrEmpty(availableStockString))
            {
                if (int.TryParse(availableStockString, out int availableStock))
                {
                    catalogItem.AvailableStock = availableStock;
                }
                else
                {
                    throw new Exception($"availableStock={availableStockString} is not a valid integer");
                }
            }
        }

        int restockThresholdIndex = Array.IndexOf(headers, "restockthreshold");
        if (restockThresholdIndex != -1)
        {
            string restockThresholdString = column[restockThresholdIndex].Trim('"').Trim();
            if (!string.IsNullOrEmpty(restockThresholdString))
            {
                if (int.TryParse(restockThresholdString, out int restockThreshold))
                {
                    catalogItem.RestockThreshold = restockThreshold;
                }
                else
                {
                    throw new Exception($"restockThreshold={restockThreshold} is not a valid integer");
                }
            }
        }

        int maxStockThresholdIndex = Array.IndexOf(headers, "maxstockthreshold");
        if (maxStockThresholdIndex != -1)
        {
            string maxStockThresholdString = column[maxStockThresholdIndex].Trim('"').Trim();
            if (!string.IsNullOrEmpty(maxStockThresholdString))
            {
                if (int.TryParse(maxStockThresholdString, out int maxStockThreshold))
                {
                    catalogItem.MaxStockThreshold = maxStockThreshold;
                }
                else
                {
                    throw new Exception($"maxStockThreshold={maxStockThreshold} is not a valid integer");
                }
            }
        }

        int onReorderIndex = Array.IndexOf(headers, "onreorder");
        if (onReorderIndex != -1)
        {
            string onReorderString = column[onReorderIndex].Trim('"').Trim();
            if (!string.IsNullOrEmpty(onReorderString))
            {
                if (bool.TryParse(onReorderString, out bool onReorder))
                {
                    catalogItem.OnReorder = onReorder;
                }
                else
                {
                    throw new Exception($"onReorder={onReorderString} is not a valid boolean");
                }
            }
        }

        return catalogItem;
    }

    private CatalogBrand CreateCatalogBrand(string brand)
    {
        brand = brand.Trim('"').Trim();

        if (string.IsNullOrEmpty(brand))
        {
            throw new Exception("Catalog Brand name is empty");
        }
        return new CatalogBrand
        {
            Brand = brand
        };
    }

    private CatalogType CreateCatalogType(string type)
    {
        type = type.Trim('"').Trim();

        if (string.IsNullOrEmpty(type))
        {
            throw new Exception("Catalog Type name is empty");
        }

        return new CatalogType
        {
            Type = type,
        };
    }

    private IEnumerable<CatalogItem> GetPreconfiguredItems()
    {
        return new List<CatalogItem>
        {
            new CatalogItem { CatalogTypeId = 2, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Bot Black Hoodie", Name = ".NET Bot Black Hoodie", Price = 19.5M, PictureFileName = "1.png" },
            new CatalogItem { CatalogTypeId = 1, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Black & White Mug", Name = ".NET Black & White Mug", Price= 8.50M, PictureFileName = "2.png" },
            new CatalogItem { CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock = 100, Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, PictureFileName = "3.png" },
            new CatalogItem { CatalogTypeId = 2, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Foundation T-shirt", Name = ".NET Foundation T-shirt", Price = 12, PictureFileName = "4.png" },
            new CatalogItem { CatalogTypeId = 3, CatalogBrandId = 5, AvailableStock = 100, Description = "Roslyn Red Sheet", Name = "Roslyn Red Sheet", Price = 8.5M, PictureFileName = "5.png" },
            new CatalogItem { CatalogTypeId = 2, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Blue Hoodie", Name = ".NET Blue Hoodie", Price = 12, PictureFileName = "6.png" },
            new CatalogItem { CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock = 100, Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, PictureFileName = "7.png" },
            new CatalogItem { CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock = 100, Description = "Kudu Purple Hoodie", Name = "Kudu Purple Hoodie", Price = 8.5M, PictureFileName = "8.png" },
            new CatalogItem { CatalogTypeId = 1, CatalogBrandId = 5, AvailableStock = 100, Description = "Cup<T> White Mug", Name = "Cup<T> White Mug", Price = 12, PictureFileName = "9.png" },
            new CatalogItem { CatalogTypeId = 3, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Foundation Sheet", Name = ".NET Foundation Sheet", Price = 12, PictureFileName = "10.png" },
            new CatalogItem { CatalogTypeId = 3, CatalogBrandId = 2, AvailableStock = 100, Description = "Cup<T> Sheet", Name = "Cup<T> Sheet", Price = 8.5M, PictureFileName = "11.png" },
            new CatalogItem { CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock = 100, Description = "Prism White TShirt", Name = "Prism White TShirt", Price = 12, PictureFileName = "12.png" },
        };
    }
    private IEnumerable<CatalogBrand> GetPreConfiguredCatalogBrands()
    {
        return new List<CatalogBrand>()
        {
            new CatalogBrand() { Brand = "Azure"},
            new CatalogBrand() { Brand = ".NET" },
            new CatalogBrand() { Brand = "Visual Studio" },
            new CatalogBrand() { Brand = "SQL Server" },
            new CatalogBrand() { Brand = "Other" }
        };
    }

    private IEnumerable<CatalogType> GetPreconfiguredCatalogTypes()
    {
        return new List<CatalogType>()
        {
            new CatalogType() { Type = "Mug"},
            new CatalogType() { Type = "T-Shirt" },
            new CatalogType() { Type = "Sheet" },
            new CatalogType() { Type = "USB Memory Stick" }
        };
    }
    private void GetCatalogItemPictures(string contentRootPath, string picturePath)
    {
        if (picturePath != null)
        {
            DirectoryInfo directory = new DirectoryInfo(picturePath);
            foreach (FileInfo file in directory.GetFiles())
            {
                file.Delete();
            }

            string zipFileCatalogItemPictures = Path.Combine(contentRootPath, "Setup", "CatalogItems.zip");
            ZipFile.ExtractToDirectory(zipFileCatalogItemPictures, picturePath);
        }
    }


    private string[] GetHeaders(string csvFile, string[] requiredHeaders, string[] optionalHeaders = null)
    {
        string[] csvHeaders = File.ReadLines(csvFile).First().ToLower().Split(',');

        if (csvHeaders.Count() < requiredHeaders.Count())
        {
            throw new Exception($"requiredHeader count '{requiredHeaders.Count()}' is bigger than csv header count '{csvHeaders.Count()}' ");
        }

        if (optionalHeaders != null)
        {
            if (csvHeaders.Count() > (requiredHeaders.Count() + optionalHeaders.Count()))
            {
                throw new Exception($"csv header count '{csvHeaders.Count()}' is larger than required '{requiredHeaders.Count()}' and optional '{optionalHeaders.Count()}' headers count");
            }
        }

        foreach (var requiredHeader in requiredHeaders)
        {
            if (!csvHeaders.Contains(requiredHeader))
            {
                throw new Exception($"does not contain required header '{requiredHeader}'");
            }
        }

        return csvHeaders;
    }

    private AsyncRetryPolicy CreatePolicy(ILogger<CatalogContextSeed> logger, string prefix, int retries = 3)
    {
        return Policy.Handle<SqlException>()
            .WaitAndRetryAsync(
                retryCount: retries,
                sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                onRetry: (exception, timeSpan, retry, ctx) =>
                {
                    logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                }
            );
    }
}