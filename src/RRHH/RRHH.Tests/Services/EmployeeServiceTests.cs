using Moq;
using RRHH.Application.Interfaces.Repositories;
using RRHH.Domain.DTOs;
using RRHH.Domain.Entities;
using RRHH.Services.Services;
using Xunit;

namespace RRHH.Tests.Services
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IGenericRepository<Employee>> _mockRepository;
        private readonly EmployeeService _employeeService;

        public EmployeeServiceTests()
        {
            _mockRepository = new Mock<IGenericRepository<Employee>>();
            _employeeService = new EmployeeService(_mockRepository.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowArgumentException_WhenSalaryIsNegative()
        {
            // Arrange
            var employeeDto = new EmployeeDto
            {
                FullName = "Juan Perez",
                HireDate = DateTime.Now,
                Position = "Dev",
                Department = "IT",
                Salary = -100 // Invalid
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _employeeService.CreateAsync(employeeDto));
            Assert.Equal("El salario debe ser mayor a cero.", exception.Message);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowArgumentException_WhenHireDateIsMissing()
        {
            // Arrange
            var employeeDto = new EmployeeDto
            {
                FullName = "Juan Perez",
                // HireDate intentionally default (0001-01-01)
                Position = "Dev",
                Department = "IT",
                Salary = 2000
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _employeeService.CreateAsync(employeeDto));
            Assert.Equal("La fecha de contratación es obligatoria.", exception.Message);
        }

        [Theory]
        [InlineData("", "Position", "Department", "El nombre completo es obligatorio.")]
        [InlineData("Juan", "", "Department", "El cargo es obligatorio.")]
        [InlineData("Juan", "Position", "", "El departamento es obligatorio.")]
        public async Task CreateAsync_ShouldThrowArgumentException_WhenFieldsAreEmpty(string name, string position, string department, string expectedMessage)
        {
            // Arrange
            var employeeDto = new EmployeeDto
            {
                FullName = name,
                HireDate = DateTime.Now,
                Position = position,
                Department = department,
                Salary = 2000
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _employeeService.CreateAsync(employeeDto));
            Assert.Equal(expectedMessage, exception.Message);
        }

        [Fact]
        public async Task CreateAsync_ShouldCallAddAsync_WhenDataIsValid()
        {
            // Arrange
            var employeeDto = new EmployeeDto
            {
                FullName = "Valid Employee",
                HireDate = DateTime.Now,
                Position = "Manager",
                Department = "Sales",
                Salary = 5000
            };

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Employee>())).Returns(Task.CompletedTask);

            // Act
            await _employeeService.CreateAsync(employeeDto);

            // Assert
            _mockRepository.Verify(r => r.AddAsync(It.Is<Employee>(e => 
                e.FullName == employeeDto.FullName && 
                e.Salary == employeeDto.Salary
            )), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowArgumentException_WhenValidationFails()
        {
             // Arrange
            var employeeDto = new EmployeeDto
            {
                Id = 1,
                FullName = "Valid",
                Salary = 0 // Invalid
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _employeeService.UpdateAsync(employeeDto));
        }

        [Fact]
        public async Task UpdateAsync_ShouldCallUpdateAsync_WhenDataIsValid()
        {
            // Arrange
            var employeeDto = new EmployeeDto
            {
                Id = 1,
                FullName = "Valid Employee",
                HireDate = DateTime.Now,
                Position = "Manager",
                Department = "Sales",
                Salary = 5000
            };

            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Employee>())).Returns(Task.CompletedTask);

            // Act
            await _employeeService.UpdateAsync(employeeDto);

            // Assert
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Employee>()), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnMappedDtos()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { Id = 1, FullName = "A", Salary = 100 },
                new Employee { Id = 2, FullName = "B", Salary = 200 }
            };

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(employees);

            // Act
            var result = await _employeeService.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal("A", result.First().FullName);
        }
        
        [Fact]
        public async Task SearchAsync_ShouldCallGetPagedAsync_WhenTermIsWhiteSpace()
        {
             // Arrange
            _mockRepository.Setup(r => r.GetPagedAsync(1, 10)).ReturnsAsync((new List<Employee>(), 0));

            // Act
            await _employeeService.SearchAsync("   ", 1, 10);

            // Assert
            _mockRepository.Verify(r => r.GetPagedAsync(1, 10), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallDeleteAsync()
        {
            // Arrange
            int id = 1;
            _mockRepository.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);

            // Act
            await _employeeService.DeleteAsync(id);

            // Assert
            _mockRepository.Verify(r => r.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnDto_WhenFound()
        {
            // Arrange
            var employee = new Employee { Id = 1, FullName = "Found" };
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);

            // Act
            var result = await _employeeService.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Found", result.FullName);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Employee?)null);

            // Act
            var result = await _employeeService.GetByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetPagedAsync_ShouldReturnPagedResult()
        {
             // Arrange
             var employees = new List<Employee> { new Employee { Id = 1 } };
             _mockRepository.Setup(r => r.GetPagedAsync(1, 10)).ReturnsAsync((employees, 1));

             // Act
             var result = await _employeeService.GetPagedAsync(1, 10);

             // Assert
             Assert.Equal(1, result.TotalRecords);
             Assert.Single(result.Data);
        }

        [Fact]
        public async Task SearchAsync_ShouldCallSearchAsync_WhenTermIsProvided()
        {
            // Arrange
            var employees = new List<Employee> { new Employee { Id = 1, FullName = "Term" } };
            _mockRepository.Setup(r => r.SearchAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Employee, bool>>>(), 1, 10))
                           .ReturnsAsync((employees, 1));

            // Act
            var result = await _employeeService.SearchAsync("Term", 1, 10);

            // Assert
            Assert.Single(result.Data);
            _mockRepository.Verify(r => r.SearchAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Employee, bool>>>(), 1, 10), Times.Once);
        }
    }
}
