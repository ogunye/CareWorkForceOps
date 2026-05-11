using CareWorkOps.Application.Common;
using CareWorkOps.Application.Common.Behaviours;
using CareWorkOps.Application.Tenants.Commands.CreateTenant;
using FluentAssertions;
using FluentValidation;

namespace CareWorkOps.Application.Tests.Common.Behaviours
{
    public sealed class ValidationBehaviourTests
    {
        [Fact]
        public async Task Handle_Should_Call_Next_When_No_Validators_Exist()
        {
            var behaviour = new ValidationBehaviour<TestRequest, Result<TestResponse>>(
                Array.Empty<IValidator<TestRequest>>());

            var nextCalled = false;

            Task<Result<TestResponse>> Next(CancellationToken cancellationToken)
            {
                nextCalled = true;

                return Task.FromResult(
                    Result<TestResponse>.Success(new TestResponse("OK")));
            }

            var result = await behaviour.Handle(
                new TestRequest("valid"),
                Next,
                CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Message.Should().Be("OK");
            nextCalled.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_Should_Call_Next_When_Validation_Passes()
        {
            var validators = new IValidator<CreateTenantCommand>[]
            {
                new CreateTenantCommandValidator()
            };

            var behaviour = new ValidationBehaviour<CreateTenantCommand, Result<CreateTenantResponse>>(
                validators);

            var nextCalled = false;

            Task<Result<CreateTenantResponse>> Next(CancellationToken cancellationToken)
            {
                nextCalled = true;

                return Task.FromResult(Result<CreateTenantResponse>.Success(
                    new CreateTenantResponse(
                        TenantId: Guid.NewGuid(),
                        TenantName: "Alpha Care Ltd",
                        TenantSlug: "alpha-care",
                        IsolationMode: Domain.Tenants.TenantIsolationMode.SharedDatabase,
                        Status: Domain.Tenants.TenantStatus.PendingSetup,
                        AdminUserId: Guid.NewGuid())));
            }

            var command = CreateValidCommand();

            var result = await behaviour.Handle(
                command,
                Next,
                CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            nextCalled.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_When_Validation_Fails_For_Result_Response()
        {
            var validators = new IValidator<CreateTenantCommand>[]
            {
                new CreateTenantCommandValidator()
            };

            var behaviour = new ValidationBehaviour<CreateTenantCommand, Result<CreateTenantResponse>>(
                validators);

            var nextCalled = false;

            Task<Result<CreateTenantResponse>> Next(CancellationToken cancellationToken)
            {
                nextCalled = true;

                return Task.FromResult(Result<CreateTenantResponse>.Success(
                    new CreateTenantResponse(
                        TenantId: Guid.NewGuid(),
                        TenantName: "Alpha Care Ltd",
                        TenantSlug: "alpha-care",
                        IsolationMode: Domain.Tenants.TenantIsolationMode.SharedDatabase,
                        Status: Domain.Tenants.TenantStatus.PendingSetup,
                        AdminUserId: Guid.NewGuid())));
            }

            var invalidCommand = CreateValidCommand() with
            {
                TenantName = "",
                AdminEmail = "invalid-email",
                AdminPassword = "weak"
            };

            var result = await behaviour.Handle(
                invalidCommand,
                Next,
                CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Validation.Error");
            result.Error.Message.Should().Contain("'Tenant Name' must not be empty");
            result.Error.Message.Should().Contain("'Admin Email' is not a valid email address");
            result.Error.Message.Should().Contain("Password must contain at least one uppercase letter");

            nextCalled.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_Should_Throw_ValidationException_When_Response_Is_Not_Result_Type()
        {
            var validators = new IValidator<TestRequest>[]
            {
                new TestRequestValidator()
            };

            var behaviour = new ValidationBehaviour<TestRequest, string>(validators);

            Task<string> Next(CancellationToken cancellationToken)
            {
                return Task.FromResult("OK");
            }

            var action = async () => await behaviour.Handle(
                new TestRequest(""),
                Next,
                CancellationToken.None);

            await action.Should().ThrowAsync<ValidationException>();
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

        private sealed record TestRequest(string Name);

        private sealed record TestResponse(string Message);

        private sealed class TestRequestValidator : AbstractValidator<TestRequest>
        {
            public TestRequestValidator()
            {
                RuleFor(x => x.Name)
                    .NotEmpty();
            }
        }
    }
}