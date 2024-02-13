using Xunit;
using FluentAssertions;
using FakeItEasy;
using PetrolPrice.Pages.PetrolStation;
using Microsoft.EntityFrameworkCore;
using PetrolPrice.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetrolPrice.Data;

namespace PetrolPrice.Tests.Pages.PetrolStation
{
    public class DeleteModelTests
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
                int id = 3;
                var petrolStation = new Models.PetrolStation
                {
                    Id = id,
                    Name = "Test Petrol Station",
                    Address = "Test Address",
                    Price = 2.0m
                };
                context.PetrolStation.Add(petrolStation);
                context.SaveChanges();

                var deleteModel = new DeleteModel(context);

                // Act
                var result = await deleteModel.OnGetAsync(id);

                // Assert
                result.Should().BeOfType<PageResult>();
                deleteModel.PetrolStation.Should().BeEquivalentTo(petrolStation);
            }
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
                var deleteModel = new DeleteModel(context);

                // Act
                var result = await deleteModel.OnGetAsync(null);

                // Assert
                result.Should().BeOfType<NotFoundResult>();
            }
        }

        [Fact]
        public async Task OnPostAsync_ValidId_RemovesPetrolStationAndRedirectsToIndexPage()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PetrolPriceContext>()
                            .UseInMemoryDatabase(databaseName: "TestDatabase")
                            .Options;
            using (var context = new PetrolPriceContext(options))
            {
                int id = 2;
                var petrolStation = new Models.PetrolStation
                {
                    Id = id,
                    Name = "Test Petrol Station",
                    Address = "Test Address",
                    Price = 2.0m
                };
                context.PetrolStation.Add(petrolStation);
                context.SaveChanges();

                var deleteModel = new DeleteModel(context);

                // Act
                var result = await deleteModel.OnPostAsync(id);

                // Assert
                result.Should().BeOfType<RedirectToPageResult>();
                context.PetrolStation.FirstOrDefault(m => m.Id == id).Should().BeNull();
            }
        }
    }
}
