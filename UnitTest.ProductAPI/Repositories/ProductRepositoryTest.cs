

using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductAPIDomain.Entities;
using ProductAPIInfraestructure.Data;
using ProductAPIInfraestructure.Repositories;
using System.Linq.Expressions;

namespace UnitTest.ProductAPI.Repositories
{
    public class ProductRepositoryTest
    {
        private readonly ProductDbContext productDbContext;
        private readonly ProductRepository productRepository;

        public ProductRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(databaseName: "EcommerceDb").Options;

            productDbContext = new ProductDbContext(options);
            productRepository = new ProductRepository(productDbContext);
        }

        [Fact]
        public void SetUp()
        {
            // Limpiar la base de datos antes de cada prueba
            productDbContext.Database.EnsureDeleted();
            productDbContext.Database.EnsureCreated();
        }

        #region Create Product

        [Fact]
        public async Task CreateAsync_WhenProductAlreadyExists()
        {
            // arrange
            var existingProduct = new Product { Name = "Product 1" };
            productDbContext.Products.Add(existingProduct);
            await productDbContext.SaveChangesAsync();

            // Act
            var result = await productRepository.CreateAsync(existingProduct);

            // Assert
            result.Should().NotBeNull();    
            result.Flag.Should().BeFalse();
            result.Message.Should().Be("Product 1 already added");
        }


        [Fact]
        public async Task CreateAsync_WhenProductSuccess()
        {
            // Act
            var result = await productRepository.CreateAsync(new Product());

            // Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeTrue();
            result.Message.Should().Be(" added to database successfully");
        }

        [Fact]
        public async Task CreateAsync_WhenProductError()
        {
            // Act
            var result = await productRepository.CreateAsync(null);

            // Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeFalse();
            result.Message.Should().Be("Error ocurred adding new product");
        }

        #endregion


        #region DeleteAsync

        [Fact]
        public async Task DeleteAsync_WhenProductSuccess()
        {
            // Arrange
            var product = new Product()
            {
                Id = 1,
                Name = "Product 1",
                Quantity = 20,
                Price = 120.50m
            };
            productDbContext.Products.Add(product);

            // Act
            var result = await productRepository.DeleteAsync(product);

            // Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeTrue();
            result.Message.Should().Be("Product 1 is deleted succesfully!");
        }


        #endregion


        #region FindByIdProduct

        [Fact]
        public async Task FindByIdAsync_WhenProductSuccess()
        {
            // Arrange
            var product = new Product()
            {
                Id = 3,
                Name = "Product 1",
                Quantity = 20,
                Price = 120.50m
            };
            productDbContext.Products.Add(product);
            await productDbContext.SaveChangesAsync();

            // Act
            var result = await productRepository.FindByIdAsync(product.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(3);
            result.Name.Should().Be("Product 1");
        }

        [Fact]
        public async Task FindByIdAsync_WhenProductNull()
        {
            // Act
            var result = await productRepository.FindByIdAsync(2);

            // Assert
            result.Should().BeNull();
        }


        #endregion


        #region GetAll
        [Fact]
        public async Task GetAllAsync_WhenProductSuccess()
        {
            // Arrange
            var products = new List<Product>()
            {
                new(){Id = 10, Name = "Product 1", Quantity = 20, Price = 120.50m},
                new(){Id = 20, Name = "Product 2", Quantity = 40, Price = 110.50m},
            };
            productDbContext.Products.AddRange(products);
            await productDbContext.SaveChangesAsync();

            // Act
            var result = await productRepository.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(6);
            result.Should().Contain(p => p.Name == "Product 1");
            result.Should().Contain(p => p.Name == "Product 2");
        }

        [Fact]
        public async Task GetAllAsync_WhenProductNotFound()
        {
          
            // Act
            var result = await productRepository.GetAllAsync();

            // Assert
            result.Count().Should().Be(0);
        }
        #endregion

        #region GetByAsync


        [Fact]
        public async Task GetByAsync_WhenProductSuccess()
        {
            // Arrange
            var product = new Product()
            {
                Id = 1,
                Name = "Product 1",
                Quantity = 20,
                Price = 120.50m
            };
            productDbContext.Products.Add(product);
            await productDbContext.SaveChangesAsync();
            Expression<Func<Product, bool>> predicate = p => p.Name == "Product 1";

            // Act
            var result = await productRepository.GetByAsync(predicate);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Product 1");
        }


        [Fact]
        public async Task GetByAsync_WhenProductFailed()
        {
            
            Expression<Func<Product, bool>> predicate = p => p.Name == "Product 60";

            // Act
            var result = await productRepository.GetByAsync(predicate);

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region UpdateAsync

        [Fact]
        public async Task UpdateAsync_WhenProductNotFound()
        {
            // Act
            var result = await productRepository.UpdateAsync(new Product());

            // Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeFalse();
            result.Message.Should().Be(" not found!");
        }

        [Fact]
        public async Task UpdateAsync_WhenProductSuccess()
        {
            // Arrange
            var product = new Product()
            {
                Id = 7,
                Name = "Product 1",
                Quantity = 40,
                Price = 150.50m
            };
            productDbContext.Products.Add(product);
            await productDbContext.SaveChangesAsync();

            // Act
            var result = await productRepository.UpdateAsync(product);

            // Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeTrue();
            result.Message.Should().Be("Product 1 is updated successfully");
        }

        #endregion

    }
}
