using AwesomeAssertions;
using Xunit;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiMatrixTests;

// ReSharper disable once InconsistentNaming
public class ChiMatrix_AccessTests
{
    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 4)]
    [InlineData(3, 9)]
    [InlineData(4, 16)]
    [InlineData(5, 25)]
    [InlineData(6, 36)]
    [InlineData(7, 49)]
    public void Matrix_CreateSquareOnes_HasCorrectLength(int dimension, int expected)
    {
        // Arrange & Act
        var matrix = ChiMatrix.Ones<double>(dimension, dimension);

        // Assert
        matrix.Span.Length.Should().Be(expected);
        matrix.Length.Should().Be(expected);
        matrix.Length.Should().Be((int)matrix.Span.ToArray().Sum());
    }

    [Fact]
    public void Indexer_Read3x3Matrix_ReturnsCorrectValues()
    {
        // Arrange
        var matrix = ChiMatrix.Zeros<double>(3, 3);
        var span = matrix.Span;

        // Act
        for (var i = 0; i < span.Length; i++)
            span[i] = i * 10.0;

        // Assert
        matrix[0, 0].Should().Be(0.0);
        matrix[0, 1].Should().Be(10.0);
        matrix[0, 2].Should().Be(20.0);
        matrix[1, 0].Should().Be(30.0);
        matrix[1, 1].Should().Be(40.0);
        matrix[1, 2].Should().Be(50.0);
        matrix[2, 0].Should().Be(60.0);
        matrix[2, 1].Should().Be(70.0);
        matrix[2, 2].Should().Be(80.0);
    }

    [Fact]
    public void Indexer_Write2x2Matrix_ReflectsInSpan()
    {
        // Arrange
        var matrix = ChiMatrix.Zeros<double>(2, 2);

        // Act
        matrix[0, 0] = 1.5;
        matrix[0, 1] = 2.5;
        matrix[1, 0] = 3.5;
        matrix[1, 1] = 4.5;

        // Assert
        var span = matrix.Span;
        span[0].Should().Be(1.5);
        span[1].Should().Be(2.5);
        span[2].Should().Be(3.5);
        span[3].Should().Be(4.5);
    }

    [Fact]
    public void Matrix_SpanWrite_ReflectsInIndexer()
    {
        // Arrange
        var matrix = ChiMatrix.Zeros<double>(2, 3);
        var span = matrix.Span;

        // Act 
        span[0] = 10.0;
        span[1] = 20.0;
        span[2] = 30.0;
        span[3] = 40.0;
        span[4] = 50.0;
        span[5] = 60.0;

        // Assert
        matrix[0, 0].Should().Be(10.0);
        matrix[0, 1].Should().Be(20.0);
        matrix[0, 2].Should().Be(30.0);
        matrix[1, 0].Should().Be(40.0);
        matrix[1, 1].Should().Be(50.0);
        matrix[1, 2].Should().Be(60.0);
    }

    [Fact]
    public void Matrix_MultipleSpanAccess_ReferencesSameMemory()
    {
        // Arrange
        var matrix = ChiMatrix.Zeros<double>(2, 2);

        // Act
        var span1 = matrix.Span;
        var span2 = matrix.Span;

        span1[0] = 99.0;

        // Assert
        span1[0].Should().Be(99.0);
        span2[0].Should().Be(99.0);
    }

    [Fact]
    public void Matrix_CreateSquareFloat_SpanHasCorrectValues()
    {
        // Arrange
        var matrix = ChiMatrix.Zeros<float>(2, 2);

        // Act
        matrix[0, 0] = 1.5f;
        matrix[0, 1] = 2.5f;
        matrix[1, 0] = 3.5f;
        matrix[1, 1] = 4.5f;

        // Assert
        var span = matrix.Span;
        span[0].Should().Be(1.5f);
        span[1].Should().Be(2.5f);
        span[2].Should().Be(3.5f);
        span[3].Should().Be(4.5f);
    }

    [Fact]
    public void Matrix_CreateSquareDecimal_SpanHasCorrectValues()
    {
        // Arrange
        var matrix = ChiMatrix.Zeros<decimal>(2, 2);

        // Act
        matrix[0, 0] = 1.5m;
        matrix[0, 1] = 2.5m;
        matrix[1, 0] = 3.5m;
        matrix[1, 1] = 4.5m;

        // Assert
        var span = matrix.Span;
        span[0].Should().Be(1.5m);
        span[1].Should().Be(2.5m);
        span[2].Should().Be(3.5m);
        span[3].Should().Be(4.5m);
    }

    [Fact]
    public void Matrix_RowMajorOrdering_MapsCorrectly()
    {
        // Arrange
        var matrix = ChiMatrix.Zeros<double>(3, 4);

        // Act
        var value = 1.0;
        for (var row = 0; row < 3; row++)
        for (var col = 0; col < 4; col++)
            matrix[row, col] = value++;

        // Assert
        var span = matrix.Span;
        var expected = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

        for (var i = 0; i < expected.Length; i++)
            span[i].Should().Be(expected[i]);
    }

    [Fact]
    public void Matrix_CreateFull_HasCorrectValues()
    {
        // Arrange & Act
        var matrix = ChiMatrix.Full(3, 3, 42.0);
        var span = matrix.Span;

        // Assert
        for (var i = 0; i < span.Length; i++)
            span[i].Should().Be(42.0);
    }

    [Fact]
    public void Matrix_CreateVector_SpanHasCorrectValues()
    {
        // Arrange
        var row = ChiMatrix.Zeros<double>(1, 5);

        // Act
        for (var i = 0; i < 5; i++)
            row[0, i] = i * 2.0;

        // Assert
        var span = row.Span;
        span[0].Should().Be(0.0);
        span[1].Should().Be(2.0);
        span[2].Should().Be(4.0);
        span[3].Should().Be(6.0);
        span[4].Should().Be(8.0);
    }

    [Fact]
    public void Matrix_CreateColumn_SpanHasCorrectValues()
    {
        // Arrange
        var column = ChiMatrix.Zeros<double>(4, 1);

        // Act
        for (var i = 0; i < 4; i++)
            column[i, 0] = i * 3.0;

        // Assert
        var span = column.Span;
        span[0].Should().Be(0.0);
        span[1].Should().Be(3.0);
        span[2].Should().Be(6.0);
        span[3].Should().Be(9.0);
    }

    [Fact]
    public void Matrix_MaxInlineLength_AllowsFullAccess()
    {
        // Arrange & Act
        var matrix = ChiMatrix.Zeros<double>(5, 5);

        // Assert
        matrix.Span.Length.Should().Be(25);

        // Verify we can access all elements
        matrix[4, 4] = 100.0;
        matrix.Span[24].Should().Be(100.0);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    [InlineData(-1, 1)]
    [InlineData(1, -1)]
    [InlineData(0, 0)]
    public void Matrix_CreateInvalid_ThrowsArgumentOutOfRange(int rows, int cols)
    {
        // Act & Assert
        var action = () => ChiMatrix.Zeros<double>(rows, cols);
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    [InlineData(2, 0)]
    [InlineData(0, 2)]
    [InlineData(3, 3)]
    public void Indexer_InvalidAccess_ThrowsArgumentOutOfRange(int row, int col)
    {
        // Arrange
        var matrix = ChiMatrix.Zeros<double>(2, 2);

        // Act
        var action = () => { _ = matrix[row, col]; };

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*out of bounds*");
    }

    [Fact]
    public void Matrix_HeapBacked_AccessAfterDisposeThrows()
    {
        // Arrange
        var matrix = ChiMatrix.Zeros<double>(10, 10);
        matrix.DebugInfo().IsHeapBacked.Should().BeTrue();

        // Act
        matrix.Dispose();
        matrix.DebugInfo().IsValid.Should().BeFalse();

        // Assert
        var access = () => { _ = matrix.Span; };
        access.Should().Throw<ObjectDisposedException>();
    }

    [Fact]
    public void Matrix_InlineBacked_AccessAfterDisposeDoesNotThrow()
    {
        // Arrange
        var matrix = ChiMatrix.Zeros<double>(2, 2);
        matrix.DebugInfo().IsHeapBacked.Should().BeFalse();

        // Act
        matrix.Dispose();
        matrix.DebugInfo().IsValid.Should().BeFalse();

        // Assert
        var access = () => { _ = matrix.Span; };
        access.Should().NotThrow();
    }

    [Fact]
    public void Matrix_CreateIdentity_HasDiagonalOnes()
    {
        // Arrange
        var identity = ChiMatrix.Eye<double>(3);

        // Assert
        identity[0, 0].Should().Be(1.0);
        identity[1, 1].Should().Be(1.0);
        identity[2, 2].Should().Be(1.0);

        identity[0, 1].Should().Be(0.0);
        identity[0, 2].Should().Be(0.0);
        identity[1, 0].Should().Be(0.0);
        identity[1, 2].Should().Be(0.0);
        identity[2, 0].Should().Be(0.0);
        identity[2, 1].Should().Be(0.0);
    }

    [Fact]
    public void Matrix_CreateDiagonal_HasExpectedValues()
    {
        // Arrange
        var matrix = ChiMatrix.Diagonal([3.0, 2.0, 1.0]);

        // Assert
        matrix[0, 0].Should().Be(3.0);
        matrix[1, 1].Should().Be(2.0);
        matrix[2, 2].Should().Be(1.0);

        matrix[0, 1].Should().Be(0.0);
        matrix[1, 2].Should().Be(0.0);
        matrix[2, 0].Should().Be(0.0);
    }

    [Fact]
    public void Matrix_Hilbert_CreatesCorrectMatrix()
    {
        // Arrange
        const int size = 4;

        // Act
        var hilbert = ChiMatrix.Hilbert<double>(size);

        // Assert
        hilbert.RowCount.Should().Be(size);
        hilbert.ColumnCount.Should().Be(size);

        // H(i,j) = 1 / (i + j + 1)
        hilbert[0, 0].Should().BeApproximately(1.0, 1e-9); // 1 / (0+0+1)
        hilbert[0, 1].Should().BeApproximately(1.0 / 2.0, 1e-9); // 1 / (0+1+1)
        hilbert[1, 0].Should().BeApproximately(1.0 / 2.0, 1e-9); // 1 / (1+0+1)
        hilbert[1, 1].Should().BeApproximately(1.0 / 3.0, 1e-9); // 1 / (1+1+1)
        hilbert[2, 3].Should().BeApproximately(1.0 / 6.0, 1e-9); // 1 / (2+3+1)
        hilbert[3, 2].Should().BeApproximately(1.0 / 6.0, 1e-9); // 1 / (3+2+1)
        hilbert[3, 3].Should().BeApproximately(1.0 / 7.0, 1e-9); // 1 / (3+3+1)
    }

    [Fact]
    public void Matrix_Toeplitz_CreatesCorrectSymmetricMatrix()
    {
        // Arrange
        var firstRow = new double[] { 5, 2, 1 };

        // Act
        var toeplitz = ChiMatrix.Toeplitz<double>(firstRow);

        // Assert
        toeplitz.RowCount.Should().Be(3);
        toeplitz.ColumnCount.Should().Be(3);

        // Expected matrix:
        // [ 5, 2, 1 ]
        // [ 2, 5, 2 ]
        // [ 1, 2, 5 ]

        // Check first row
        toeplitz[0, 0].Should().Be(5); // firstRow[|0-0|] = firstRow[0]
        toeplitz[0, 1].Should().Be(2); // firstRow[|0-1|] = firstRow[1]
        toeplitz[0, 2].Should().Be(1); // firstRow[|0-2|] = firstRow[2]

        // Check second row
        toeplitz[1, 0].Should().Be(2); // firstRow[|1-0|] = firstRow[1]
        toeplitz[1, 1].Should().Be(5); // firstRow[|1-1|] = firstRow[0]
        toeplitz[1, 2].Should().Be(2); // firstRow[|1-2|] = firstRow[1]

        // Check third row
        toeplitz[2, 0].Should().Be(1); // firstRow[|2-0|] = firstRow[2]
        toeplitz[2, 1].Should().Be(2); // firstRow[|2-1|] = firstRow[1]
        toeplitz[2, 2].Should().Be(5); // firstRow[|2-2|] = firstRow[0]
    }

    [Fact]
    public void Matrix_Vandermonde_CreatesCorrectSquareMatrix()
    {
        // Arrange
        var vector = new double[] { 1, 2, 3 };

        // Act
        // Create a square Vandermonde matrix by default
        var vander = ChiMatrix.Vandermonde<double>(vector);

        // Assert
        vander.RowCount.Should().Be(3);
        vander.ColumnCount.Should().Be(3);

        // Expected matrix: V(i,j) = vector[i] ^ j
        // [ 1^0, 1^1, 1^2 ]   [ 1, 1, 1 ]
        // [ 2^0, 2^1, 2^2 ] = [ 1, 2, 4 ]
        // [ 3^0, 3^1, 3^2 ]   [ 1, 3, 9 ]

        // Row 0 (vector[0] = 1)
        vander[0, 0].Should().Be(1); // 1^0
        vander[0, 1].Should().Be(1); // 1^1
        vander[0, 2].Should().Be(1); // 1^2

        // Row 1 (vector[1] = 2)
        vander[1, 0].Should().Be(1); // 2^0
        vander[1, 1].Should().Be(2); // 2^1
        vander[1, 2].Should().Be(4); // 2^2

        // Row 2 (vector[2] = 3)
        vander[2, 0].Should().Be(1); // 3^0
        vander[2, 1].Should().Be(3); // 3^1
        vander[2, 2].Should().Be(9); // 3^2
    }

    [Fact]
    public void Matrix_Vandermonde_CreatesCorrectRectangularMatrix()
    {
        // Arrange
        var vector = new[] { 2.0, 4 };

        // Act
        // Create a rectangular 2x4 Vandermonde matrix
        var vander = ChiMatrix.Vandermonde<double>(vector, 4);

        // Assert
        vander.RowCount.Should().Be(2);
        vander.ColumnCount.Should().Be(4);

        // Expected matrix:
        // [ 2^0, 2^1, 2^2, 2^3 ] = [ 1, 2,  4,  8 ]
        // [ 4^0, 4^1, 4^2, 4^3 ] = [ 1, 4, 16, 64 ]

        // Row 0
        vander[0, 0].Should().Be(1);
        vander[0, 1].Should().Be(2);
        vander[0, 2].Should().Be(4);
        vander[0, 3].Should().Be(8);

        // Row 1
        vander[1, 0].Should().Be(1);
        vander[1, 1].Should().Be(4);
        vander[1, 2].Should().Be(16);
        vander[1, 3].Should().Be(64);
    }
}