using FluentValidation.Results;
using FinalProject.Application.Common.Exceptions;
using Shouldly;

namespace FinalProject.Application.UnitTests.Common.Exceptions;

public class ValidationExceptionTests
{
    [Fact]
    public void DefaultConstructorCreatesAnEmptyErrorDictionary()
    {
        var actual = new ValidationException().Errors;

        actual.Keys.ShouldBeEmpty();
    }

    [Fact]
    public void SingleValidationFailureCreatesASingleElementErrorDictionary()
    {
        var failures = new List<ValidationFailure>
            {
                new ValidationFailure("Age", "must be over 18"),
            };

        var actual = new ValidationException(failures).Errors;

        actual.Keys.ShouldBe(["Age"]);
        actual["Age"].ShouldBe(["must be over 18"]);
    }

    [Fact]
    public void MulitpleValidationFailureForMultiplePropertiesCreatesAMultipleElementErrorDictionaryEachWithMultipleValues()
    {
        var failures = new List<ValidationFailure>
            {
                new ValidationFailure("Age", "must be 18 or older"),
                new ValidationFailure("Age", "must be 25 or younger"),
                new ValidationFailure("Password", "must contain at least 8 characters"),
                new ValidationFailure("Password", "must contain a digit"),
                new ValidationFailure("Password", "must contain upper case letter"),
                new ValidationFailure("Password", "must contain lower case letter"),
            };

        var actual = new ValidationException(failures).Errors;

        actual.Keys.ShouldBe(["Password", "Age"], ignoreOrder: true);

        actual["Age"].ShouldBe([
            "must be 25 or younger",
                "must be 18 or older"
        ], ignoreOrder: true);

        actual["Password"].ShouldBe([
            "must contain lower case letter",
                "must contain upper case letter",
                "must contain at least 8 characters",
                "must contain a digit"
        ], ignoreOrder: true);
    }

    [Fact]
public void DefaultConstructorShouldSetValidationErrorsToEmptyDictionary()
{
    var actual = new ValidationException().Errors;

    actual.Keys.ShouldBeEmpty();
}

[Fact]
public void SingleValidationFailureShouldSetValidationErrors()
{
    var failures = new List<ValidationFailure>
{
        new("Name", "must not be empty"),
    };

    var actual = new ValidationException(failures).Errors;

    actual.Keys.ShouldContain("Name");
    actual["Name"].ShouldContain("must not be empty");
}

[Fact]
public void MultipleValidationFailuresForMultiplePropertiesShouldSetValidationErrors()
{
 var failures = new List<ValidationFailure>
    {
  new("Name", "must not be empty"),
     new("Age", "must be 18 or older"),
    };

    var actual = new ValidationException(failures).Errors;

actual.Keys.ShouldContain("Name");
    actual["Name"].ShouldContain("must not be empty");
    actual.Keys.ShouldContain("Age");
actual["Age"].ShouldContain("must be 18 or older");
}

[Fact]
public void MultipleValidationFailuresForSamePropertyShouldBeInTheSameCollection()
{
var failures = new List<ValidationFailure>
    {
 new("Name", "must not be empty"),
        new("Name", "must not exceed 10 characters"),
    };

    var actual = new ValidationException(failures).Errors;

actual.Keys.ShouldContain("Name");
    actual["Name"].ShouldContain("must not be empty");
    actual["Name"].ShouldContain("must not exceed 10 characters");
}
}
