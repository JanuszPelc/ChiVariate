using FluentAssertions;
using Xunit;

#pragma warning disable CS1591

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiMatrixTests;

// ReSharper disable once InconsistentNaming
public class ChiMatrix_AddSubtractTests
{
    // ====================================================================
    // Addition Tests
    // ====================================================================

    [Fact]
    public void Add_MatrixAndMatrix_ReturnsCorrectResult()
    {
        // Arrange
        var a = ChiMatrix.Zeros<double>(2, 2);
        a[0, 0] = 1;
        a[0, 1] = 2;
        a[1, 0] = 3;
        a[1, 1] = 4;

        var b = ChiMatrix.Zeros<double>(2, 2);
        b[0, 0] = 5;
        b[0, 1] = 6;
        b[1, 0] = 7;
        b[1, 1] = 8;

        // Act
        using var result = a.Peek() + b.Peek();

        // Assert
        result.RowCount.Should().Be(2);
        result.ColumnCount.Should().Be(2);
        result[0, 0].Should().Be(6.0);
        result[0, 1].Should().Be(8.0);
        result[1, 0].Should().Be(10.0);
        result[1, 1].Should().Be(12.0);
    }

    [Fact]
    public void Add_MatrixAndScalar_ReturnsCorrectResult()
    {
        // Arrange
        var matrix = ChiMatrix.Zeros<double>(2, 2);
        matrix[0, 0] = 1;
        matrix[0, 1] = 2;
        matrix[1, 0] = 3;
        matrix[1, 1] = 4;

        var scalar = ChiMatrix.Scalar(10.0);

        // Act
        using var result = matrix.Peek() + scalar.Peek();

        // Assert
        result[0, 0].Should().Be(11.0);
        result[0, 1].Should().Be(12.0);
        result[1, 0].Should().Be(13.0);
        result[1, 1].Should().Be(14.0);
    }

    [Fact]
    public void Add_ScalarAndMatrix_ReturnsCorrectResult()
    {
        // Arrange
        var matrix = ChiMatrix.Zeros<double>(2, 2);
        matrix[0, 0] = 1;
        matrix[0, 1] = 2;
        matrix[1, 0] = 3;
        matrix[1, 1] = 4;

        var scalar = ChiMatrix.Scalar(10.0);

        // Act
        using var result = scalar.Peek() + matrix.Peek();

        // Assert
        result[0, 0].Should().Be(11.0);
        result[0, 1].Should().Be(12.0);
        result[1, 0].Should().Be(13.0);
        result[1, 1].Should().Be(14.0);
    }

    // ====================================================================
    // Subtraction Tests
    // ====================================================================

    [Fact]
    public void Subtract_MatrixAndMatrix_ReturnsCorrectResult()
    {
        // Arrange
        var a = ChiMatrix.Zeros<decimal>(2, 2);
        a[0, 0] = 10;
        a[0, 1] = 20;
        a[1, 0] = 30;
        a[1, 1] = 40;

        var b = ChiMatrix.Zeros<decimal>(2, 2);
        b[0, 0] = 1;
        b[0, 1] = 2;
        b[1, 0] = 3;
        b[1, 1] = 4;

        // Act
        using var result = a.Peek() - b.Peek();

        // Assert
        result.RowCount.Should().Be(2);
        result.ColumnCount.Should().Be(2);
        result[0, 0].Should().Be(9m);
        result[0, 1].Should().Be(18m);
        result[1, 0].Should().Be(27m);
        result[1, 1].Should().Be(36m);
    }

    [Fact]
    public void Subtract_ScalarFromMatrix_ReturnsCorrectResult()
    {
        // Arrange
        var matrix = ChiMatrix.Zeros<decimal>(2, 2);
        matrix[0, 0] = 10;
        matrix[0, 1] = 20;
        matrix[1, 0] = 30;
        matrix[1, 1] = 40;

        var scalar = ChiMatrix.Scalar(5m);

        // Act
        using var result = matrix.Peek() - scalar.Peek();

        // Assert
        result[0, 0].Should().Be(5m);
        result[0, 1].Should().Be(15m);
        result[1, 0].Should().Be(25m);
        result[1, 1].Should().Be(35m);
    }

    [Fact]
    public void Subtract_MatrixFromScalar_ReturnsCorrectResult()
    {
        // Arrange
        var matrix = ChiMatrix.Zeros<decimal>(2, 2);
        matrix[0, 0] = 1;
        matrix[0, 1] = 2;
        matrix[1, 0] = 3;
        matrix[1, 1] = 4;

        var scalar = ChiMatrix.Scalar(10m);

        // Act
        using var result = scalar.Peek() - matrix.Peek();

        // Assert
        result[0, 0].Should().Be(9m);
        result[0, 1].Should().Be(8m);
        result[1, 0].Should().Be(7m);
        result[1, 1].Should().Be(6m);
    }

    // ====================================================================
    // Error Handling
    // ====================================================================

    [Fact]
    public void Add_IncompatibleDimensions_ThrowsInvalidOperationException()
    {
        // Arrange
        var a = ChiMatrix.Zeros<double>(2, 3);
        var b = ChiMatrix.Zeros<double>(2, 2);

        // Act
        // ReSharper disable once AccessToDisposedClosure
        var action = () => { _ = a.Peek() + b.Peek(); };

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*must be identical for addition*");
    }

    [Fact]
    public void Subtract_IncompatibleDimensions_ThrowsInvalidOperationException()
    {
        // Arrange
        var a = ChiMatrix.Zeros<double>(3, 2);
        var b = ChiMatrix.Zeros<double>(2, 2);

        // Act
        // ReSharper disable once AccessToDisposedClosure
        var action = () => { _ = a.Peek() - b.Peek(); };

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*must be identical for subtraction*");
    }
}