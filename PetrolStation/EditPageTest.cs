using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PetrolPrice.Areas.Identity.Data;
using PetrolPrice.Data;
using PetrolPrice.Pages.PetrolStation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PetrolPrice.Tests.Pages.PetrolStation
{
    public class EditModelTests
    {
        [Fact]
        public async Task OnGetAsync_ValidId_ReturnsPageResultWithPetrolStation()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PetrolPriceContext>()
                                .UseInMemoryDatabase(databaseName: "TestDatabase")
                                .Options;

            using (var context = new PetrolPriceContext(options))
            {
                var editModel = new EditModel(context);

                // Add a sample petrol station to the database
                var samplePetrolStation = new Models.PetrolStation
                {
                    Id = 31,
                    Name = "Sample Station",
                    Address = "Sample Address",
                    Price = 2.0m
                };
                context.PetrolStation.Add(samplePetrolStation);
                await context.SaveChangesAsync();

                // Act
                var result = await editModel.OnGetAsync(31);

                // Assert
                result.Should().BeOfType<PageResult>();
                editModel.PetrolStation.Should().BeEquivalentTo(samplePetrolStation); // Ensure the petrol station loaded matches the sample petrol station
            }
        }

        [Fact]
        public async Task OnGetAsync_InvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PetrolPriceContext>()
                                .UseInMemoryDatabase(databaseName: "TestDatabase")
                                .Options;

            using (var context = new PetrolPriceContext(options))
            {
                var editModel = new EditModel(context);

                // Act
                var result = await editModel.OnGetAsync(999); // Invalid ID

                // Assert
                result.Should().BeOfType<NotFoundResult>();
                editModel.PetrolStation.Should().BeNull(); // Ensure petrol station property is null when ID is invalid
            }
        }

        [Fact]
        public async Task OnPostAsync_ValidModel_ReturnsRedirectToPageResult()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PetrolPriceContext>()
                                .UseInMemoryDatabase(databaseName: "TestDatabase")
                                .Options;

            using (var context = new PetrolPriceContext(options))
            {
                var editModel = new EditModel(context);

                // Add a sample petrol station to the database
                var samplePetrolStation = new Models.PetrolStation
                {
                    Id = 34,
                    Name = "Sample Station",
                    Address = "Sample Address",
                    Price = 2.0m
                };
                context.PetrolStation.Add(samplePetrolStation);
                await context.SaveChangesAsync();

                // Update the petrol station with new data
                var updatedPetrolStationData = new Models.PetrolStation
                {
                    Id = 34,
                    Name = "Updated Station Name",
                    Address = "Updated Address",
                    Price = 3.0m
                };

                editModel.PetrolStation = updatedPetrolStationData;

                // Act
                var result = await editModel.OnPostAsync(34);

                // Assert
                result.Should().BeOfType<RedirectToPageResult>();
                ((RedirectToPageResult)result).PageName.Should().Be("./Index"); // Ensure it redirects to the Index page
                var updatedPetrolStationInDb = await context.PetrolStation.FindAsync(34);
                updatedPetrolStationInDb.Should().BeEquivalentTo(updatedPetrolStationData); // Ensure the petrol station in the database is updated
            }
        }

        [Fact]
        public async Task OnPostAsync_InvalidModel_ReturnsPageResult()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PetrolPriceContext>()
                                .UseInMemoryDatabase(databaseName: "TestDatabase")
                                .Options;

            using (var context = new PetrolPriceContext(options))
            {
                var editModel = new EditModel(context);

                // Add a sample petrol station to the database
                var samplePetrolStation = new Models.PetrolStation
                {
                    Id = 33,
                    Name = "Sample Station",
                    Address = "Sample Address",
                    Price = 2.0m
                };
                context.PetrolStation.Add(samplePetrolStation);
                await context.SaveChangesAsync();

                // Update the petrol station with new data
                var updatedPetrolStationData = new Models.PetrolStation
                {
                    // Missing required properties 'Name' and 'Address'
                    Price = 2.0m
                };

                editModel.PetrolStation = updatedPetrolStationData;

                // Manually trigger model validation
                editModel.ModelState.Clear(); // Clear existing state to ensure no interference
                editModel.ModelState.AddModelError("PetrolStation.Name", "The Name field is required.");
                editModel.ModelState.AddModelError("PetrolStation.Address", "The Address field is required.");

                // Act
                var result = await editModel.OnPostAsync(33);

                // Assert
                result.Should().BeOfType<PageResult>();
                context.PetrolStation.FirstOrDefault(m => m.Id == samplePetrolStation.Id).Should().NotBeNull(); // Ensure the petrol station still exists in the database
            }
        }
    }
}
