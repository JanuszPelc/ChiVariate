using System.Numerics;
using AwesomeAssertions;
using Xunit;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiMatrixTests;

// ReSharper disable once InconsistentNaming
public class ChiMatrix_AlgebraTests
{
    // A helper method to create a matrix and fill it with predictable, non-trivial values.
    private static ChiMatrix<T> CreateAndFill<T>(int rows, int cols) where T : unmanaged, IFloatingPoint<T>
    {
        var matrix = ChiMatrix.Zeros<T>(rows, cols);
        for (var i = 0; i < matrix.Length; i++)
            // Use a simple formula to get varied positive and negative values
            matrix.Span[i] = T.CreateChecked(i % 10 + 1) * T.CreateChecked(i % 3 == 0 ? -0.5 : 1.0);
        return matrix;
    }

    // A helper to compare two DOUBLE matrices for approximate equality.
    private static void AssertMatricesApproximatelyEqual(ChiMatrix<double> a, ChiMatrix<double> b, double tolerance)
    {
        a.RowCount.Should().Be(b.RowCount);
        a.ColumnCount.Should().Be(b.ColumnCount);

        for (var i = 0; i < a.Length; i++) a.Span[i].Should().BeApproximately(b.Span[i], tolerance);
    }

    // An overload to compare two DECIMAL matrices for approximate equality.
    private static void AssertMatricesApproximatelyEqual(ChiMatrix<decimal> a, ChiMatrix<decimal> b, decimal tolerance)
    {
        a.RowCount.Should().Be(b.RowCount);
        a.ColumnCount.Should().Be(b.ColumnCount);

        for (var i = 0; i < a.Length; i++)
            // Fluent Assertions requires a cast to a supported type for BeApproximately
            ((double)a.Span[i]).Should().BeApproximately((double)b.Span[i], (double)tolerance);
    }

    [Theory]
    [InlineData(10, 20, 15)] // A(10x20) * (B(20x15) + C(20x15))
    [InlineData(50, 50, 50)] // Large square matrices
    public void Multiply_Double_DistributesOverAddition(int m, int n, int p)
    {
        var a = CreateAndFill<double>(m, n);
        var b = CreateAndFill<double>(n, p);
        var c = CreateAndFill<double>(n, p);

        // --- Left Side: A * (B + C) ---
        var bPlusC = b.Peek() + c.Peek();
        var leftSideResult = a.Peek() * bPlusC.Peek();

        // --- Right Side: A*B + A*C ---
        var aTimesB = a.Peek() * b.Peek();
        var aTimesC = a.Peek() * c.Peek();
        var rightSideResult = aTimesB.Peek() + aTimesC.Peek();

        AssertMatricesApproximatelyEqual(leftSideResult, rightSideResult, 1e-9);
    }

    [Theory]
    [InlineData(8, 12, 10)]
    public void Multiply_Decimal_DistributesOverAddition(int m, int n, int p)
    {
        var a = CreateAndFill<decimal>(m, n);
        var b = CreateAndFill<decimal>(n, p);
        var c = CreateAndFill<decimal>(n, p);

        // --- Left Side: A * (B + C) ---
        var bPlusC = b.Peek() + c.Peek();
        var leftSideResult = a.Peek() * bPlusC.Peek();

        // --- Right Side: A*B + A*C ---
        var aTimesB = a.Peek() * b.Peek();
        var aTimesC = a.Peek() * c.Peek();
        var rightSideResult = aTimesB.Peek() + aTimesC.Peek();

        AssertMatricesApproximatelyEqual(leftSideResult, rightSideResult, 1e-12m);
    }

    [Fact]
    public void ToArray_OnLargeHeapMatrix_ReturnsIndependentCopy()
    {
        const int rows = 100;
        const int cols = 100;

        // Using statement ensures disposal even if an assertion fails early
        var matrix = ChiMatrix.Zeros<double>(rows, cols);
        matrix.DebugInfo().IsHeapBacked.Should().BeTrue();

        for (var i = 0; i < matrix.Length; i++)
            matrix.Span[i] = i;

        double n = matrix.Length;
        var expectedSum = n * (0 + (n - 1)) / 2.0;
        var expectedAverage = expectedSum / n;

        var array = matrix.ToArray();

        // Check array properties before disposing the source matrix
        array.Length.Should().Be(rows * cols);
        array.Cast<double>().Sum().Should().Be(expectedSum);
        array.Cast<double>().Average().Should().BeApproximately(expectedAverage, 1e-9);

        // Now, dispose the original matrix
        matrix.Dispose();

        // Final check: Prove the copy is independent by accessing it after the source is disposed.
        var action = () => { _ = array[rows - 1, cols - 1]; };
        action.Should().NotThrow();
    }
}