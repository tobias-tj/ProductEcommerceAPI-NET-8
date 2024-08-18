using ProductAPIApplication.Interfaces;
using ProductAPIPresentation.Controllers;
using ProductAPIDomain.Entities;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using ProductAPIApplication.DTOs;
using ecommerceSharedLibrary.Response;

namespace UnitTest.ProductAPI.Controllers
{
    public class ProductControllerTest
    {
        private readonly IProduct productInterface;
        private readonly ProductsController productsController;

        public ProductControllerTest()
        {
            // Set up dependencies
            productInterface = A.Fake<IProduct>();

            // Set up System Undet Test - SUT
            productsController = new ProductsController(productInterface);
        }

        // Get All Products
        [Fact]
        public async Task GetProduct_WhenProductExists_ReturnOK()
        {
            // Arrange 
            var products = new List<Product>()
            {
                new(){Id = 1, Name = "Product 1", Quantity = 20, Price = 120.50m},
                new(){Id = 2, Name = "Product 2", Quantity = 40, Price = 110.50m},
            };

            // set up fake response for GetAllAsync Method
            A.CallTo(() => productInterface.GetAllAsync()).Returns(products);

            // Act
            var result = await productsController.GetAllProducts();

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            var returnedProducts = okResult.Value as IEnumerable<ProductDTO>;
            returnedProducts.Should().NotBeNull();
            returnedProducts.Should().HaveCount(2);
            returnedProducts!.First().Id.Should().Be(1);
            returnedProducts!.Last().Id.Should().Be(2);
        }

        [Fact]
        public async Task GetProducts_WhenNoProductsExist_ReturnNotFoundResponse()
        {
            // Arrange
            var products = new List<Product>();

            // set up fake response for GetAllAsync Method
            A.CallTo(() => productInterface.GetAllAsync()).Returns(products);

            // Act
            var result = await productsController.GetAllProducts();

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult!.StatusCode.Should().Be(StatusCodes.Status404NotFound);

            var message = notFoundResult.Value as string;
            message.Should().Be("No product found!");
        }

        #region GetProductById

        [Fact]
        public async Task GetProduct_WhenProductByIdNotExist_FailedProductNull()
        {

            // Arrange
            int nonExistingProductId = 99999;

            // set up fake response for GetAllAsync Method
            A.CallTo(() => productInterface.FindByIdAsync(nonExistingProductId)).Returns<Product>(null!);

            //Act
            var result = await productsController.GetProductById(nonExistingProductId);


            // Assert
            var resultOperation = result.Result as NotFoundObjectResult;
            resultOperation.Should().NotBeNull();
            resultOperation!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        public async Task GetProduct_WhenProductByIdExists_ReturnOK()
        {
            // Arrange 
            var product = new Product()
            {
             Id = 1, 
             Name = "Product 1", 
             Quantity = 20,
             Price = 120.50m
            };

            // set up fake response for GetAllAsync Method
            A.CallTo(() => productInterface.FindByIdAsync(A<int>.Ignored)).Returns(product);

            // Act
            var result = await productsController.GetProductById(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var returnedProducts = okResult.Value as ProductDTO;
            returnedProducts.Should().NotBeNull();
        }

        #endregion



        #region Create Product

        [Fact]
        public async Task CreateProduct_WhenModelStateIsInvalid()
        {
            // Arrange
            var productDTO = new ProductDTO(1, "Product 1", 10, 20.32m);
            productsController.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await productsController.CreateProduct(productDTO);

            // Assert
            var badResult = result.Result as BadRequestObjectResult;
            badResult.Should().NotBeNull();
            badResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task CreateProduct_WhenCreateIsSuccess()
        {
            // Arrange
            var productDto = new ProductDTO(1, "Product 1", 25, 39.40m);
            var response = new ResponseModel(true, "Product Created Success!");

            // set up fake response for GetAllAsync Method
            A.CallTo(() => productInterface.CreateAsync(A<Product>.Ignored)).Returns(response);

            // Act
            var result = await productsController.CreateProduct(productDto);

            // Assert
            var successResult = result.Result as OkObjectResult;
            successResult.Should().NotBeNull();
            successResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var responseResult = successResult.Value as ResponseModel;
            responseResult!.Message.Should().Be("Product Created Success!");
        }
        [Fact]
        public async Task CreateProduct_WhenCreateFails()
        {
            // Arrange
            var productDto = new ProductDTO(1, "Product 1", 25, 39.40m);
            var response = new ResponseModel(false, "Product Created Failed!");

            // set up fake response for GetAllAsync Method
            A.CallTo(() => productInterface.CreateAsync(A<Product>.Ignored)).Returns(response);

            // Act
            var result = await productsController.CreateProduct(productDto);

            // Assert
            var failedResult = result.Result as BadRequestObjectResult;
            failedResult.Should().NotBeNull();
            failedResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var responseResult = failedResult.Value as ResponseModel;
            responseResult!.Message.Should().Be("Product Created Failed!");
        }
        #endregion



        #region UpdateProduct

        [Fact]
        public async Task UpdateProduct_WhenModelStateIsInvalid()
        {
            // Arrange
            var productDTO = new ProductDTO(1, "Product 1", 10, 20.32m);
            productsController.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await productsController.UpdateProduct(productDTO);

            // Assert
            var badResult = result.Result as BadRequestObjectResult;
            badResult.Should().NotBeNull();
            badResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task UpdateProduct_WhenSuccess()
        {
            // Arrange
            var productDto = new ProductDTO(1, "Product 1", 25, 39.40m);
            var response = new ResponseModel(true, "Product Updated Success!");

            // set up fake response for GetAllAsync Method
            A.CallTo(() => productInterface.UpdateAsync(A<Product>.Ignored)).Returns(response);

            // Act
            var result = await productsController.UpdateProduct(productDto);

            // Assert
            var successResult = result.Result as OkObjectResult;
            successResult.Should().NotBeNull();
            successResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var responseResult = successResult.Value as ResponseModel;
            responseResult!.Message.Should().Be("Product Updated Success!");
        }

        [Fact]
        public async Task UpdateProduct_WhenFailded()
        {
            // Arrange
            var productDto = new ProductDTO(1, "Product 1", 25, 39.40m);
            var response = new ResponseModel(false, "Product Error!");

            // set up fake response for GetAllAsync Method
            A.CallTo(() => productInterface.UpdateAsync(A<Product>.Ignored)).Returns(response);

            // Act
            var result = await productsController.UpdateProduct(productDto);

            // Assert
            var successResult = result.Result as BadRequestObjectResult;
            successResult.Should().NotBeNull();
            successResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var responseResult = successResult.Value as ResponseModel;
            responseResult!.Message.Should().Be("Product Error!");
        }

        #endregion


        #region DeleteProduct

        [Fact]
        public async Task DeleteProduct_WhenSuccess()
        {
            // Arrange
            var productDto = new ProductDTO(1, "Product 1", 25, 39.40m);
            var response = new ResponseModel(true, "Product Deleted!");

            // set up fake response for GetAllAsync Method
            A.CallTo(() => productInterface.DeleteAsync(A<Product>.Ignored)).Returns(response);

            // Act
            var result = await productsController.DeleteProduct(productDto);

            // Assert
            var successResult = result.Result as OkObjectResult;
            successResult.Should().NotBeNull();
            successResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var responseResult = successResult.Value as ResponseModel;
            responseResult!.Message.Should().Be("Product Deleted!");
        }

        [Fact]
        public async Task DeleteProduct_WhenFailed()
        {
            // Arrange
            var productDto = new ProductDTO(1, "Product 1", 25, 39.40m);
            var response = new ResponseModel(false, "Product Error!");

            // set up fake response for GetAllAsync Method
            A.CallTo(() => productInterface.DeleteAsync(A<Product>.Ignored)).Returns(response);

            // Act
            var result = await productsController.DeleteProduct(productDto);

            // Assert
            var successResult = result.Result as BadRequestObjectResult;
            successResult.Should().NotBeNull();
            successResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var responseResult = successResult.Value as ResponseModel;
            responseResult!.Message.Should().Be("Product Error!");
        }

        #endregion

    }
}
