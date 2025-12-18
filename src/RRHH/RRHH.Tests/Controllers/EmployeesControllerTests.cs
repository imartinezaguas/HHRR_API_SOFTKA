using Microsoft.AspNetCore.Mvc;
using Moq;
using RRHH.API.Controllers;
using RRHH.Application.Interfaces.Services;
using RRHH.Domain.DTOs;
using Xunit;

namespace RRHH.Tests.Controllers
{
    public class EmployeesControllerTests
    {
        private readonly Mock<IEmployeeService> _mockService;
        private readonly EmployeesController _controller;

        public EmployeesControllerTests()
        {
            _mockService = new Mock<IEmployeeService>();
            _controller = new EmployeesController(_mockService.Object);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenArgumentExceptionIsThrown()
        {
            // Arrange
            var dto = new EmployeeDto();
            _mockService.Setup(s => s.CreateAsync(dto)).ThrowsAsync(new ArgumentException("Error de validación"));

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = badRequestResult.Value;
            Assert.NotNull(value);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction_WhenSuccessful()
        {
            // Arrange
            var dto = new EmployeeDto { Id = 1, FullName = "Test" };
            _mockService.Setup(s => s.CreateAsync(dto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetById", createdResult.ActionName);
            Assert.Equal(1, createdResult.RouteValues["id"]);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Act
            var result = await _controller.Update(1, new EmployeeDto { Id = 2 });

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenServiceThrowsArgumentException()
        {
             // Arrange
            var dto = new EmployeeDto { Id = 1 };
            _mockService.Setup(s => s.UpdateAsync(dto)).ThrowsAsync(new ArgumentException("Error"));

            // Act
            var result = await _controller.Update(1, dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenServiceReturnsNull()
        {
            // Arrange
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((EmployeeDto?)null);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenFound()
        {
            // Arrange
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new EmployeeDto { Id = 1 });

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsType<EmployeeDto>(okResult.Value);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            // Arrange
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<EmployeeDto>());

            // Act
            var result = await _controller.GetAll();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPaged_ShouldReturnBadRequest_WhenParametersInvalid()
        {
            // Act
            var result = await _controller.GetPaged(0, 0);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPaged_ShouldReturnOk_WhenParametersValid()
        {
            // Arrange
            _mockService.Setup(s => s.GetPagedAsync(1, 10)).ReturnsAsync(new PagedResultDto<EmployeeDto>());

            // Act
            var result = await _controller.GetPaged(1, 10);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task Search_ShouldReturnOk()
        {
            // Arrange
            _mockService.Setup(s => s.SearchAsync("term", 1, 10)).ReturnsAsync(new PagedResultDto<EmployeeDto>());

            // Act
            var result = await _controller.Search("term", 1, 10);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var dto = new EmployeeDto { Id = 1 };
            _mockService.Setup(s => s.UpdateAsync(dto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(1, dto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
