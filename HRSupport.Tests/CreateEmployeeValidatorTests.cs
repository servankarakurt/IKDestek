using FluentValidation.TestHelper;
using HRSupport.Application.Features.Employees.Commands;
using HRSupport.Domain.Enum;
using System;
using Xunit;

namespace HRSupport.Tests
{
    /// <summary>
    /// Çalışan eklerken zorunlu alanlar ve validasyon (gereksinim: "Stajyer/Çalışan eklerken zorunlu alanlar dolu değilse validasyon hatası" benzeri).
    /// </summary>
    public class CreateEmployeeValidatorTests
    {
        private readonly CreateEmployeeValidator _validator = new CreateEmployeeValidator();

        [Fact]
        public void When_FirstName_Empty_Should_HaveValidationError()
        {
            var cmd = new CreateEmployeeCommand
            {
                FirstName = "",
                LastName = "Soyad",
                Email = "test@test.com",
                Phone = "1234567890",
                CardID = 1,
                BirthDate = DateTime.Now.AddYears(-25),
                StartDate = DateTime.Now.AddDays(-1),
                Department = Department.Yazilim,
                Roles = Roles.Çalışan
            };
            var result = _validator.TestValidate(cmd);
            result.ShouldHaveValidationErrorFor(c => c.FirstName);
        }

        [Fact]
        public void When_Email_Invalid_Should_HaveValidationError()
        {
            var cmd = new CreateEmployeeCommand
            {
                FirstName = "Ad",
                LastName = "Soyad",
                Email = "gecersiz-email",
                Phone = "1234567890",
                CardID = 1,
                BirthDate = DateTime.Now.AddYears(-25),
                StartDate = DateTime.Now.AddDays(-1),
                Department = Department.Yazilim,
                Roles = Roles.Çalışan
            };
            var result = _validator.TestValidate(cmd);
            result.ShouldHaveValidationErrorFor(c => c.Email);
        }

        [Fact]
        public void When_AllRequiredFieldsValid_Should_NotHaveValidationError()
        {
            var cmd = new CreateEmployeeCommand
            {
                FirstName = "Ad",
                LastName = "Soyad",
                Email = "test@company.com",
                Phone = "1234567890",
                CardID = 1,
                BirthDate = DateTime.Now.AddYears(-25),
                StartDate = DateTime.Now.AddDays(-1),
                Department = Department.Yazilim,
                Roles = Roles.Çalışan
            };
            var result = _validator.TestValidate(cmd);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void When_Phone_NotTenDigits_Should_HaveValidationError()
        {
            var cmd = new CreateEmployeeCommand
            {
                FirstName = "Ad",
                LastName = "Soyad",
                Email = "test@test.com",
                Phone = "123",
                CardID = 1,
                BirthDate = DateTime.Now.AddYears(-25),
                StartDate = DateTime.Now.AddDays(-1),
                Department = Department.Yazilim,
                Roles = Roles.Çalışan
            };
            var result = _validator.TestValidate(cmd);
            result.ShouldHaveValidationErrorFor(c => c.Phone);
        }
    }
}
