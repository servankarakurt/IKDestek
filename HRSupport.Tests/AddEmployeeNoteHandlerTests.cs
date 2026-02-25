using HRSupport.Application.Features.Employees.Commands;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entities;
using HRSupport.Domain.Enum;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HRSupport.Tests
{
    /// <summary>
    /// Çalışana not ekleme: boş not metni ile hata dönmesi.
    /// </summary>
    public class AddEmployeeNoteHandlerTests
    {
        [Fact]
        public async Task AddEmployeeNote_WhenNoteTextEmpty_ReturnsFailure()
        {
            var employeeRepo = new Mock<IEmployeeRepository>();
            var noteRepo = new Mock<IEmployeeNoteRepository>();
            var currentUser = new Mock<ICurrentUserService>();
            currentUser.Setup(c => c.Role).Returns("IK");
            currentUser.Setup(c => c.UserId).Returns(1);
            employeeRepo.Setup(e => e.GetByIdAsync(1)).ReturnsAsync(new Employee { Id = 1, Department = Department.InsanKaynaklari });

            var handler = new AddEmployeeNoteHandler(employeeRepo.Object, noteRepo.Object, currentUser.Object);
            var command = new AddEmployeeNoteCommand { EmployeeId = 1, NoteText = "   " };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Contains("boş", result.Error);
            noteRepo.Verify(r => r.AddAsync(It.IsAny<HRSupport.Domain.Entities.EmployeeNote>()), Times.Never);
        }

        [Fact]
        public async Task AddEmployeeNote_WhenValid_ReturnsSuccess()
        {
            var employeeRepo = new Mock<IEmployeeRepository>();
            var noteRepo = new Mock<IEmployeeNoteRepository>();
            var currentUser = new Mock<ICurrentUserService>();
            currentUser.Setup(c => c.Role).Returns("IK");
            currentUser.Setup(c => c.UserId).Returns(1);
            currentUser.Setup(c => c.Email).Returns("ik@test.com");
            employeeRepo.Setup(e => e.GetByIdAsync(1)).ReturnsAsync(new Employee { Id = 1, Department = Department.InsanKaynaklari });
            noteRepo.Setup(r => r.AddAsync(It.IsAny<HRSupport.Domain.Entities.EmployeeNote>())).ReturnsAsync(1);

            var handler = new AddEmployeeNoteHandler(employeeRepo.Object, noteRepo.Object, currentUser.Object);
            var command = new AddEmployeeNoteCommand { EmployeeId = 1, NoteText = "Örnek not" };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            noteRepo.Verify(r => r.AddAsync(It.IsAny<HRSupport.Domain.Entities.EmployeeNote>()), Times.Once);
        }
    }
}
