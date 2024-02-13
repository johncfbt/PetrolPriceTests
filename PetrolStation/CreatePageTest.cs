using FluentAssertions;
using FakeItEasy;
using PetrolPrice.Pages.PetrolStation;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetrolPrice.Data;
using Microsoft.AspNetCore.Identity;
using PetrolPrice.Areas.Identity.Data;

namespace PetrolPrice.Tests.Pages.PetrolStation
{
    public class CreateModelTests
    {
        [Fact]
        public async Task OnPostAsync_ValidModel_AddsPetrolStationAndRedirectsToIndexPage()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PetrolPriceContext>()
                            .UseInMemoryDatabase(databaseName: "TestDatabase")
                            .Options;

            var userManager = A.Fake<UserManager<PetrolPriceUser>>();

            using (var context = new PetrolPriceContext(options))
            {
                var createModel = new CreateModel(context, userManager);

                var currentUser = new PetrolPriceUser
                {
                    Id = "1",
                    Email = "test@example.com"
                };

                A.CallTo(() => userManager.GetUserAsync(A<System.Security.Claims.ClaimsPrincipal>.Ignored))
                    .Returns(Task.FromResult(currentUser));

                var petrolStation = new Models.PetrolStation
                {
                    Id = 21,
                    Name = "Test Petrol Station",
                    Address = "Test Address",
                    Price = 2.0m,
                    PetrolPriceUser = currentUser // Assigning the current user to the petrol station
                };

                createModel.PetrolStation = petrolStation;

                // Act
                var result = await createModel.OnPostAsync();

                // Assert
                result.Should().BeOfType<RedirectToPageResult>();
                context.PetrolStation.FirstOrDefault(m => m.Id == petrolStation.Id).Should().NotBeNull();
                context.PetrolStation.FirstOrDefault(m => m.Id == petrolStation.Id).PetrolPriceUser.Should().Be(currentUser);
            }
        }

        [Fact]
        public async Task OnPostAsync_InvalidModel_ReturnsPageResult()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PetrolPriceContext>()
                            .UseInMemoryDatabase(databaseName: "TestDatabase")
                            .Options;

            var userManager = A.Fake<UserManager<PetrolPriceUser>>();

            using (var context = new PetrolPriceContext(options))
            {
                var createModel = new CreateModel(context, userManager);
                var currentUser = new PetrolPriceUser
                {
                    Id = "22",
                    Email = "test@example.com"
                };
                A.CallTo(() => userManager.GetUserAsync(A<System.Security.Claims.ClaimsPrincipal>.Ignored))
                    .Returns(Task.FromResult(currentUser));

                var petrolStation = new Models.PetrolStation
                {
                    // Missing required properties 'Name' and 'Address'
                    Price = 2.0m
                };

                createModel.PetrolStation = petrolStation;

                // Manually trigger model validation, as model binding and validation process not simulated like during real HTTP request processing.
                createModel.ModelState.Clear(); // Clear existing state to ensure no interference
                createModel.ModelState.AddModelError("PetrolStation.Name", "The Name field is required.");
                createModel.ModelState.AddModelError("PetrolStation.Address", "The Address field is required.");

                // Act
                var result = await createModel.OnPostAsync();

                // Assert
                result.Should().BeOfType<PageResult>();
                context.PetrolStation.FirstOrDefault(m => m.Id == petrolStation.Id).Should().BeNull();
            }
        }
    }
}
