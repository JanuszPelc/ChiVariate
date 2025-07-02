using FluentAssertions;
using Xunit;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiMatrixTests;

// ReSharper disable once InconsistentNaming
public class ChiMatrix_MultiplyTests
{
    // ====================================================================
    // Case 1 & 2: Scalar Multiplication
    // ====================================================================

    [Fact]
    public void Multiply_MatrixByScalar_ReturnsCorrectResult()
    {
        // Arrange
        var matrix = ChiMatrix.Full<double>(2, 2, (row, column) => 1 + row * 2 + column);
        var scalar = ChiMatrix.Scalar(2.0);

        // Act
        using var result = matrix.Peek() * scalar.Peek();

        // Assert
        result.RowCount.Should().Be(2);
        result.ColumnCount.Should().Be(2);
        result[0, 0].Should().Be(2.0);
        result[0, 1].Should().Be(4.0);
        result[1, 0].Should().Be(6.0);
        result[1, 1].Should().Be(8.0);
    }

    [Fact]
    public void Multiply_ScalarByMatrix_ReturnsCorrectResult()
    {
        // Arrange
        var matrix = ChiMatrix.Zeros<double>(2, 2);
        matrix[0, 0] = 1;
        matrix[0, 1] = 2;
        matrix[1, 0] = 3;
        matrix[1, 1] = 4;

        var scalar = ChiMatrix.Scalar(3.0);

        // Act
        using var result = scalar.Peek() * matrix.Peek();

        // Assert
        result.RowCount.Should().Be(2);
        result.ColumnCount.Should().Be(2);
        result[0, 0].Should().Be(3.0);
        result[0, 1].Should().Be(6.0);
        result[1, 0].Should().Be(9.0);
        result[1, 1].Should().Be(12.0);
    }

    // ====================================================================
    // Case 3: HadamardProduct Tests
    // ====================================================================

    [Fact]
    public void HadamardProduct_ElementWise_ReturnsCorrectResult()
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
        var result = a.Peek().Hadamard(b.Peek());

        // Assert
        result.RowCount.Should().Be(2);
        result.ColumnCount.Should().Be(2);
        result[0, 0].Should().Be(5.0); // 1*5
        result[0, 1].Should().Be(12.0); // 2*6
        result[1, 0].Should().Be(21.0); // 3*7
        result[1, 1].Should().Be(32.0); // 4*8
    }

    [Fact]
    public void HadamardProduct_IncompatibleDimensions_ThrowsInvalidOperationException()
    {
        var a = ChiMatrix.Zeros<double>(2, 3);
        var b = ChiMatrix.Zeros<double>(2, 2);

        var action = () => { _ = a.Peek().Hadamard(b.Peek()); };

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*dimensions must be identical for the Hadamard product*");
    }

    // ====================================================================
    // Case 4: Dot Product
    // ====================================================================

    [Fact]
    public void Multiply_DotProduct_ReturnsCorrectScalar()
    {
        // Arrange
        var rowVector = ChiMatrix.Zeros<double>(1, 3);
        rowVector[0, 0] = 1;
        rowVector[0, 1] = 2;
        rowVector[0, 2] = 3;

        var colVector = ChiMatrix.Zeros<double>(3, 1);
        colVector[0, 0] = 4;
        colVector[1, 0] = 5;
        colVector[2, 0] = 6;

        // Act
        using var result = rowVector.Peek() * colVector.Peek();

        // Assert
        result.IsScalar.Should().BeTrue();
        result[0, 0].Should().Be(32.0); // (1*4) + (2*5) + (3*6)
    }

    // ====================================================================
    // Case 5 & 6: General and Vector-Matrix Multiplication
    // ====================================================================

    [Fact]
    public void Multiply_MatrixMatrix_ReturnsCorrectResult()
    {
        // Arrange
        // A (2x3)
        var a = ChiMatrix.Zeros<double>(2, 3);
        a[0, 0] = 1;
        a[0, 1] = 2;
        a[0, 2] = 3;
        a[1, 0] = 4;
        a[1, 1] = 5;
        a[1, 2] = 6;

        // B (3x2)
        var b = ChiMatrix.Zeros<double>(3, 2);
        b[0, 0] = 7;
        b[0, 1] = 8;
        b[1, 0] = 9;
        b[1, 1] = 10;
        b[2, 0] = 11;
        b[2, 1] = 12;

        // Act
        // Result should be 2x2
        using var result = a.Peek() * b.Peek();

        // Assert
        result.RowCount.Should().Be(2);
        result.ColumnCount.Should().Be(2);
        result[0, 0].Should().Be(58.0); // (1*7 + 2*9 + 3*11)
        result[0, 1].Should().Be(64.0); // (1*8 + 2*10 + 3*12)
        result[1, 0].Should().Be(139.0); // (4*7 + 5*9 + 6*11)
        result[1, 1].Should().Be(154.0); // (4*8 + 5*10 + 6*12)
    }

    [Fact]
    public void Multiply_MatrixColVector_ReturnsCorrectResult()
    {
        // Arrange
        // A (2x3)
        var a = ChiMatrix.Full(2, 3, static i => (Half)(i + 1));
        // B (3x1 column vector)
        var b = ChiMatrix.Full(3, 1, static i => (Half)(i + 7));

        // Act
        // Result should be 2x1
        using var result = a.Peek() * b.Peek();

        // Assert
        result.IsColumn.Should().BeTrue();
        result.IsRow.Should().BeFalse();
        result.IsVector.Should().BeTrue();
        result.RowCount.Should().Be(2);
        result[0, 0].Should().Be((Half)50.0); // (1*7 + 2*8 + 3*9)
        result[1, 0].Should().Be((Half)122.0); // (4*7 + 5*8 + 6*9)
    }

    [Fact]
    public void Multiply_RowVectorMatrix_ReturnsCorrectResult()
    {
        // Arrange
        var a = ChiMatrix.WithTransposed([3.0m, 4]); // A (1x2 row vector)
        var b = ChiMatrix.With([1.0m, 2, 3], [5, 6, 7]); // B (2x3 matrix)

        // Act
        // Result should be 1x3
        using var result = a.Peek() * b.Peek();

        // Assert
        result.IsRow.Should().BeTrue();
        result.ColumnCount.Should().Be(3);
        result[0, 0].Should().Be(23.0m); // (3*1 + 4*5)
        result[0, 1].Should().Be(30.0m); // (3*2 + 4*6)
        result[0, 2].Should().Be(37.0m); // (3*3 + 4*7)
    }

    // ====================================================================
    // Error Handling
    // ====================================================================

    [Fact]
    public void Multiply_IncompatibleDimensions_ThrowsInvalidOperationException()
    {
        // Arrange
        var a = ChiMatrix.Zeros<double>(2, 3);
        var b = ChiMatrix.Zeros<double>(4, 2); // Incompatible: A.Cols (3) != B.Rows (4)

        // Act
        // ReSharper disable AccessToDisposedClosure
        var action = () => { _ = a.Peek() * b.Peek(); };
        // ReSharper enable AccessToDisposedClosure

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*not compatible for multiplication*");
    }
}