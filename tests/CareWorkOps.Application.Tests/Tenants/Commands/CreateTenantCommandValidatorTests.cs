using CareWorkOps.Application.Tenants.Commands.CreateTenant;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Application.Tests.Tenants.Commands
{
    public sealed class CreateTenantCommandValidatorTests
    {
        private readonly CreateTenantCommandValidator _validator = new();

        [Fact]
        public void Should_Not_Have_Validation_Error_When_Command_Is_Valid()
        {
            var command = CreateValidCommand();

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Should_Have_Error_When_TenantName_Is_Invalid(string tenantName)
        {
            var command = CreateValidCommand() with
            {
                TenantName = tenantName
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.TenantName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("Alpha Care")]
        [InlineData("alpha_care")]
        [InlineData("alpha--care")]
        [InlineData("-alpha-care")]
        [InlineData("alpha-care-")]
        [InlineData("alpha.care")]
        public void Should_Have_Error_When_TenantSlug_Is_Invalid(string tenantSlug)
        {
            var command = CreateValidCommand() with
            {
                TenantSlug = tenantSlug
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.TenantSlug);
        }

        [Fact]
        public void Should_Have_Error_When_AdminEmail_Is_Invalid()
        {
            var command = CreateValidCommand() with
            {
                AdminEmail = "invalid-email"
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.AdminEmail);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("Password")]
        [InlineData("password123!")]
        [InlineData("PASSWORD123!")]
        [InlineData("Password!")]
        [InlineData("Password123")]
        [InlineData("Pass1!")]
        public void Should_Have_Error_When_AdminPassword_Is_Weak(string password)
        {
            var command = CreateValidCommand() with
            {
                AdminPassword = password
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.AdminPassword);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("InvalidMode")]
        [InlineData("shared-database")]
        public void Should_Have_Error_When_IsolationMode_Is_Invalid(string isolationMode)
        {
            var command = CreateValidCommand() with
            {
                IsolationMode = isolationMode
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.IsolationMode);
        }

        [Fact]
        public void Should_Have_Error_When_DedicatedDatabase_Has_No_ConnectionString()
        {
            var command = CreateValidCommand() with
            {
                IsolationMode = "DedicatedDatabase",
                ConnectionString = null
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.ConnectionString);
        }

        [Fact]
        public void Should_Not_Have_Error_When_DedicatedDatabase_Has_ConnectionString()
        {
            var command = CreateValidCommand() with
            {
                IsolationMode = "DedicatedDatabase",
                ConnectionString = "Server=localhost;Database=TenantDb;User Id=sa;Password=Password123!;TrustServerCertificate=True;"
            };

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.ConnectionString);
        }

        private static CreateTenantCommand CreateValidCommand()
        {
            return new CreateTenantCommand(
                TenantName: "Alpha Care Ltd",
                TenantSlug: "alpha-care",
                AdminFirstName: "John",
                AdminLastName: "Smith",
                AdminEmail: "admin@alphacare.com",
                AdminPassword: "Password123!",
                IsolationMode: "SharedDatabase");
        }
    }
    
}
