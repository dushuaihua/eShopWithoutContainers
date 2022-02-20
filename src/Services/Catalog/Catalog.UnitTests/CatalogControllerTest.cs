using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using eShopWithoutContainers.Services.Catalog.API.Infrastructure;
using eShopWithoutContainers.Services.Catalog.API.Model;
using eShopWithoutContainers.Services.Catalog.API;
using eShopWithoutContainers.Services.Catalog.API.IntegrationEvents;
using eShopWithoutContainers.Services.Catalog.API.Controllers;
using eShopWithoutContainers.Services.Catalog.API.ViewModel;

namespace Catalog.UnitTests
{
    public class CatalogControllerTest
    {
        private readonly DbContextOptions<CatalogContext> _dbOptions;

        public CatalogControllerTest()
        {
            _dbOptions = new DbContextOptionsBuilder<CatalogContext>()
                .UseInMemoryDatabase(databaseName: "in-memory")
                .Options;

            using (var dbContext = new CatalogContext(_dbOptions))
            {
                dbContext.AddRange(GetFakeCatalog());
                dbContext.SaveChanges();
            }
        }

        [Fact]
        public async Task Get_catalog_items_success()
        {
            var brandFilterApplied = 1;
            var typesFilterApplied = 2;
            var pageSize = 4;
            var pageIndex = 1;

            var exceptedItemsInPage = 2;
            var exceptedTotalItems = 6;

            var catalogContext = new CatalogContext(_dbOptions);
            var catalogSettings = new TestCatalogSettings();

            var integrationServiceMock = new Mock<ICatalogIntegrationEventService>();

            var orderController = new CatalogController(catalogContext, catalogSettings, integrationServiceMock.Object);
            var actionResult = await orderController.ItemsByTypeIdAndBrandIdAsync(typesFilterApplied, brandFilterApplied, pageSize, pageIndex);

            Assert.IsType<ActionResult<PaginatedItemsViewModel<CatalogItem>>>(actionResult);

            var page = Assert.IsAssignableFrom<PaginatedItemsViewModel<CatalogItem>>(actionResult.Value);
            Assert.Equal(exceptedTotalItems, page.Count);
            Assert.Equal(pageIndex, page.PageIndex);
            Assert.Equal(pageSize, page.PageSize);
            Assert.Equal(exceptedItemsInPage, page.Data.Count());
        }

        private List<CatalogItem> GetFakeCatalog()
        {
            return new List<CatalogItem>()
            {
                new CatalogItem()
                {
                    Id = 1,
                    Name = "fakeItemA",
                    CatalogTypeId = 2,
                    CatalogBrandId = 1,
                    PictureFileName = "fakeItemA.png"
                },
                new CatalogItem()
                {
                    Id = 2,
                    Name = "fakeItemB",
                    CatalogTypeId = 2,
                    CatalogBrandId = 1,
                    PictureFileName = "fakeItemB.png"
                },
                new CatalogItem()
                {
                    Id = 3,
                    Name = "fakeItemC",
                    CatalogTypeId = 2,
                    CatalogBrandId = 1,
                    PictureFileName = "fakeItemC.png"
                },
                new CatalogItem()
                {
                    Id = 4,
                    Name = "fakeItemD",
                    CatalogTypeId = 2,
                    CatalogBrandId = 1,
                    PictureFileName = "fakeItemD.png"
                },
                new CatalogItem()
                {
                    Id = 5,
                    Name = "fakeItemE",
                    CatalogTypeId = 2,
                    CatalogBrandId = 1,
                    PictureFileName = "fakeItemE.png"
                },
                new CatalogItem()
                {
                    Id = 6,
                    Name = "fakeItemF",
                    CatalogTypeId = 2,
                    CatalogBrandId = 1,
                    PictureFileName = "fakeItemF.png"
                }
            };
        }
    }

    public class TestCatalogSettings : IOptionsSnapshot<CatalogSettings>
    {
        public CatalogSettings Value => new CatalogSettings
        {
            PicBaseUrl = "http://image-server.com/",
            AzureStorageEnabled = true
        };
        public CatalogSettings Get(string name) => Value;
    }
}