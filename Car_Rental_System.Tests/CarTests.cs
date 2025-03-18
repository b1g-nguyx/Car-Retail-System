using System;
using System.ComponentModel.DataAnnotations;
using Car_Rental_System.Models;
using Car_Rental_System.Models.Enums;
using FluentAssertions;
using Xunit;

public class CarTests
{
    [Fact]
    public void Car_Model_ShouldHaveRequiredFields()
    {
        // Arrange
        var car = new Car();
        var validationResults = new System.Collections.Generic.List<ValidationResult>();
        var context = new ValidationContext(car);

        // Act
        bool isValid = Validator.TryValidateObject(car, context, validationResults, true);

        // Assert
        isValid.Should().BeFalse();
        validationResults.Should().Contain(v => v.ErrorMessage.Contains("The Model field is required."));
    }

    [Fact]
    public void Car_Year_ShouldBeWithinValidRange()
    {
        // Arrange
        var car = new Car { Year = 1800 }; // Invalid year
        var validationResults = new System.Collections.Generic.List<ValidationResult>();
        var context = new ValidationContext(car);

        // Act
        bool isValid = Validator.TryValidateObject(car, context, validationResults, true);

        // Assert
        isValid.Should().BeFalse();
        validationResults.Should().Contain(v => v.ErrorMessage.Contains("The field Year must be between 1900 and 2100."));
    }

    [Fact]
    public void Car_PricePerDay_ShouldBePositive()
    {
        // Arrange
        var car = new Car { PricePerDay = -1 }; // Invalid price
        var validationResults = new System.Collections.Generic.List<ValidationResult>();
        var context = new ValidationContext(car);

        // Act
        bool isValid = Validator.TryValidateObject(car, context, validationResults, true);

        // Assert
        isValid.Should().BeFalse();
        validationResults.Should().Contain(v => v.ErrorMessage.Contains("The field PricePerDay must be between 0 and 1.79769313486232E+308."));
    }

    [Fact]
    public void Car_Seats_ShouldBeInValidRange()
    {
        // Arrange
        var car = new Car { Seats = 1 }; // Invalid seat number
        var validationResults = new System.Collections.Generic.List<ValidationResult>();
        var context = new ValidationContext(car);

        // Act
        bool isValid = Validator.TryValidateObject(car, context, validationResults, true);

        // Assert
        isValid.Should().BeFalse();
        validationResults.Should().Contain(v => v.ErrorMessage.Contains("The field Seats must be between 2 and 20."));
    }

    [Fact]
    public void Car_ShouldBeValidWithCorrectData()
    {
        // Arrange
        var car = new Car
        {
            BrandId = 1,
            CategoryId = 1,
            Model = "Toyota Camry",
            Year = 2022,
            PricePerDay = 500000,
            LicensePlates = "30A-12345",
            Kilometers = 10000,
            FuelType = "Gasoline",
            Transmission = "Automatic",
            Seats = 5,
            Description = "A reliable and comfortable car",
            Status = RentalStatus.Available
        };
        var validationResults = new System.Collections.Generic.List<ValidationResult>();
        var context = new ValidationContext(car);

        // Act
        bool isValid = Validator.TryValidateObject(car, context, validationResults, true);

        // Assert
        isValid.Should().BeTrue();
        validationResults.Should().BeEmpty();
    }
}
