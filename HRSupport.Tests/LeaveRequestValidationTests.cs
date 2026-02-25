using FluentValidation.TestHelper;
using HRSupport.Application.Features.LeaveRequests.Commands;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entities;
using HRSupport.Domain.Enum;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HRSupport.Tests
{
    /// <summary>
    /// İzin talebi use case testleri (gereksinim: "İzin başlangıç tarihi > bitiş tarihi olduğunda hata dönmesi").
    /// </summary>
    public class LeaveRequestValidationTests
    {
        [Fact]
        public async Task CreateLeaveRequest_WhenStartDateGreaterThanEndDate_ReturnsFailure()
        {
            var leaveRepo = new Mock<ILeaveRequestRepository>();
            var employeeRepo = new Mock<IEmployeeRepository>();
            var currentUser = new Mock<ICurrentUserService>();
            currentUser.Setup(c => c.Role).Returns("Admin");
            currentUser.Setup(c => c.UserId).Returns((int?)1);
            employeeRepo.Setup(e => e.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Employee { Id = 1, Department = Department.Yazilim, Email = "a@b.com", FirstName = "A", LastName = "B" });

            leaveRepo.Setup(r => r.AddAsync(It.IsAny<LeaveRequest>())).ReturnsAsync(1);

            var handler = new CreateLeaveRequestHandler(
                leaveRepo.Object,
                employeeRepo.Object,
                currentUser.Object);

            var command = new CreateLeaveRequestCommand
            {
                EmployeeId = 1,
                StartDate = new DateTime(2025, 6, 15),
                EndDate = new DateTime(2025, 6, 10),
                Type = LeaveType.Yıllık
            };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Contains("Başlangıç tarihi bitişten büyük olamaz", result.Error);
            leaveRepo.Verify(r => r.AddAsync(It.IsAny<LeaveRequest>()), Times.Never);
        }

        [Fact]
        public async Task CreateLeaveRequest_WhenStartDateEqualsEndDate_Succeeds()
        {
            var leaveRepo = new Mock<ILeaveRequestRepository>();
            var employeeRepo = new Mock<IEmployeeRepository>();
            var currentUser = new Mock<ICurrentUserService>();
            currentUser.Setup(c => c.Role).Returns("Admin");
            employeeRepo.Setup(e => e.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Employee { Id = 1, Department = Department.Yazilim, Email = "a@b.com", FirstName = "A", LastName = "B" });
            leaveRepo.Setup(r => r.AddAsync(It.IsAny<LeaveRequest>())).ReturnsAsync(1);

            var handler = new CreateLeaveRequestHandler(leaveRepo.Object, employeeRepo.Object, currentUser.Object);
            var command = new CreateLeaveRequestCommand
            {
                EmployeeId = 1,
                StartDate = new DateTime(2025, 6, 10),
                EndDate = new DateTime(2025, 6, 10),
                Type = LeaveType.Yıllık
            };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            leaveRepo.Verify(r => r.AddAsync(It.IsAny<LeaveRequest>()), Times.Once);
        }
    }
}
