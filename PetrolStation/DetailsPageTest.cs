using FluentAssertions;
using PetrolPrice.Pages.PetrolStation;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetrolPrice.Data;

namespace PetrolPrice.Tests.Pages.PetrolStation
{

    public class DetailsModelTests
    {
        [Fact]
        public async Task OnGetAsync_ValidId_ReturnsPageResultWithPetrolStation()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PetrolPriceContext>()
                            .UseInMemoryDatabase(databaseName: "TestDatabase")
                            .Options;
            var context = new PetrolPriceContext(options);
            var detailsModel = new DetailsModel(context);
            int id = 1; // Example id
            var petrolStation = new Models.PetrolStation
            {
                Id = id,
                Name = "test",
                Address = "test address",
                Price = 2.0m
            }; // Example petrol station
            context.PetrolStation.Add(petrolStation);
            context.SaveChanges();

            // Act
            var result = await detailsModel.OnGetAsync(id);

            // Assert
            result.Should().BeOfType<PageResult>();
            detailsModel.PetrolStation.Should().BeEquivalentTo(petrolStation);
        }

        [Fact]
        public async Task OnGetAsync_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PetrolPriceContext>()
                            .UseInMemoryDatabase(databaseName: "TestDatabase")
                            .Options;
            using (var context = new PetrolPriceContext(options))
            {
                var detailsModel = new DetailsModel(context);

                // Act
                var result = await detailsModel.OnGetAsync(null);

                // Assert
                result.Should().BeOfType<NotFoundResult>();
            }
        }
    }


    /* Explain line by line:
    public class DetailsModelTests
    {
        [Fact] // fact means a single unit test.

        // "Task" means a asyn method. Method name is descriptive.
        public async Task OnGetAsync_ValidId_ReturnsPageResultWithPetrolStation() 
        {

            // Arrange
            // Below 2 lines: create in-memory db context using 'DbContextOptionsBuilder'. Then create an instance of 'PetrolPriceContext' using these options.
            var options = new DbContextOptionsBuilder<PetrolPriceContext>()
                            .UseInMemoryDatabase(databaseName: "TestDatabase")
                            .Options;
            var context = new PetrolPriceContext(options);

            // create instance of 'DetailsModel' class, passing the 'context' created earlier. Then define an example 'petrolStation' object with some dummy data.
            var detailsModel = new DetailsModel(context);
            int id = 1; // Example id
            var petrolStation = new Models.PetrolStation { 
                Id = id,
                Name = "test",
                Address = "test address",
                Price = 2.0m
                }; // Example petrol station

            //add the 'petrolStation' object to the context and save changes to db, ensuring 'PetrolStation' is available when Act method is executed.
            context.PetrolStation.Add(petrolStation);
            context.SaveChanges();

            // Act
            var result = await detailsModel.OnGetAsync(id);

            // Assert
            //we expect the 'result' to be of type 'PageResult'
            result.Should().BeOfType<PageResult>();
            detailsModel.PetrolStation.Should().BeEquivalentTo(petrolStation);
        }
    }
    */
}
