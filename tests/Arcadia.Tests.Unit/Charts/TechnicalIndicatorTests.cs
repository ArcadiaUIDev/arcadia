using FluentAssertions;
using Arcadia.Charts.Core.Indicators;
using Xunit;

namespace Arcadia.Tests.Unit.Charts;

/// <summary>
/// Tests for financial technical indicators: SMA, EMA, Bollinger Bands, RSI.
/// </summary>
public class TechnicalIndicatorTests
{
    // ── SMA ──

    [Fact]
    public void Sma_Period3_CalculatesCorrectMovingAverage()
    {
        var sma = new SimpleMovingAverage { Period = 3 };
        var prices = new double[] { 1, 2, 3, 4, 5 };

        var result = sma.Calculate(prices);

        // SMA(3) at index 2 = (1+2+3)/3 = 2.0
        result[2].Should().BeApproximately(2.0, 1e-10);
        // SMA(3) at index 3 = (2+3+4)/3 = 3.0
        result[3].Should().BeApproximately(3.0, 1e-10);
        // SMA(3) at index 4 = (3+4+5)/3 = 4.0
        result[4].Should().BeApproximately(4.0, 1e-10);
    }

    [Fact]
    public void Sma_FirstPeriodMinus1Values_AreNaN()
    {
        var sma = new SimpleMovingAverage { Period = 3 };
        var prices = new double[] { 10, 20, 30, 40, 50 };

        var result = sma.Calculate(prices);

        result[0].Should().Be(double.NaN);
        result[1].Should().Be(double.NaN);
        result[2].Should().NotBe(double.NaN, "index Period-1 should have a valid value");
    }

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(20)]
    public void Sma_FirstNValues_AreNaN_ForVariousPeriods(int period)
    {
        var sma = new SimpleMovingAverage { Period = period };
        var prices = Enumerable.Range(1, 30).Select(i => (double)i).ToArray();

        var result = sma.Calculate(prices);

        for (var i = 0; i < period - 1; i++)
            result[i].Should().Be(double.NaN, $"index {i} should be NaN for period {period}");

        result[period - 1].Should().NotBe(double.NaN, "first valid index should have a value");
    }

    // ── EMA ──

    [Fact]
    public void Ema_Period3_StartsWithSmaSeedThenExponentiallyWeights()
    {
        var ema = new ExponentialMovingAverage { Period = 3 };
        var prices = new double[] { 1, 2, 3, 4, 5 };

        var result = ema.Calculate(prices);

        // EMA seed at index 2 = SMA(3) = (1+2+3)/3 = 2.0
        result[2].Should().BeApproximately(2.0, 1e-10);

        // EMA at index 3: multiplier = 2/(3+1) = 0.5
        // EMA = (4 - 2.0) * 0.5 + 2.0 = 3.0
        result[3].Should().BeApproximately(3.0, 1e-10);

        // EMA at index 4: (5 - 3.0) * 0.5 + 3.0 = 4.0
        result[4].Should().BeApproximately(4.0, 1e-10);
    }

    [Fact]
    public void Ema_FirstPeriodMinus1Values_AreNaN()
    {
        var ema = new ExponentialMovingAverage { Period = 3 };
        var prices = new double[] { 10, 20, 30, 40, 50 };

        var result = ema.Calculate(prices);

        result[0].Should().Be(double.NaN);
        result[1].Should().Be(double.NaN);
        result[2].Should().NotBe(double.NaN);
    }

    // ── Bollinger Bands ──

    [Fact]
    public void BollingerBands_UpperAboveMiddle_LowerBelowMiddle()
    {
        var bb = new BollingerBands { Period = 3, StdDevMultiplier = 2.0 };
        var prices = new double[] { 10, 12, 11, 13, 14, 12, 15 };

        var middle = bb.Calculate(prices);

        for (var i = 0; i < prices.Length; i++)
        {
            if (double.IsNaN(middle[i])) continue;

            bb.Upper[i].Should().BeGreaterThan(middle[i],
                $"upper band at index {i} should be above middle");
            bb.Lower[i].Should().BeLessThan(middle[i],
                $"lower band at index {i} should be below middle");
        }
    }

    [Fact]
    public void BollingerBands_StdDevMultiplier2_ProducesCorrectWidth()
    {
        var bb = new BollingerBands { Period = 3, StdDevMultiplier = 2.0 };
        var prices = new double[] { 10, 12, 11, 13, 14 };

        var middle = bb.Calculate(prices);

        // At index 2: SMA = (10+12+11)/3 = 11.0
        // StdDev = sqrt(((10-11)^2 + (12-11)^2 + (11-11)^2) / 3) = sqrt(2/3)
        var expectedStdDev = Math.Sqrt(2.0 / 3.0);
        var expectedUpper = 11.0 + 2.0 * expectedStdDev;
        var expectedLower = 11.0 - 2.0 * expectedStdDev;

        middle[2].Should().BeApproximately(11.0, 1e-10);
        bb.Upper[2].Should().BeApproximately(expectedUpper, 1e-10);
        bb.Lower[2].Should().BeApproximately(expectedLower, 1e-10);

        // Bandwidth = Upper - Lower = 2 * 2 * stddev = 4 * stddev
        var bandwidth = bb.Upper[2] - bb.Lower[2];
        bandwidth.Should().BeApproximately(4.0 * expectedStdDev, 1e-10);
    }

    [Fact]
    public void BollingerBands_NaNRegion_MatchesSma()
    {
        var bb = new BollingerBands { Period = 5 };
        var prices = new double[] { 1, 2, 3, 4, 5, 6, 7 };

        var middle = bb.Calculate(prices);

        for (var i = 0; i < 4; i++) // period-1 = 4 NaN values
        {
            middle[i].Should().Be(double.NaN);
            bb.Upper[i].Should().Be(double.NaN);
            bb.Lower[i].Should().Be(double.NaN);
        }
    }

    // ── RSI ──

    [Fact]
    public void Rsi_ValuesBetween0And100()
    {
        var rsi = new RelativeStrengthIndex { Period = 14 };
        // Create varied price data
        var prices = new double[]
        {
            44, 44.34, 44.09, 44.15, 43.61, 44.33, 44.83, 45.10,
            45.42, 45.84, 46.08, 45.89, 46.03, 45.61, 46.28,
            46.28, 46.00, 46.03, 46.41, 46.22, 45.64
        };

        var result = rsi.Calculate(prices);

        for (var i = 0; i < result.Length; i++)
        {
            if (double.IsNaN(result[i])) continue;
            result[i].Should().BeGreaterOrEqualTo(0, $"RSI at index {i} should be >= 0");
            result[i].Should().BeLessOrEqualTo(100, $"RSI at index {i} should be <= 100");
        }
    }

    [Fact]
    public void Rsi_AllGains_Returns100()
    {
        var rsi = new RelativeStrengthIndex { Period = 5 };
        // Strictly increasing prices: every change is a gain
        var prices = new double[] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };

        var result = rsi.Calculate(prices);

        // After the initial period, RSI should be 100 (no losses)
        for (var i = rsi.Period; i < result.Length; i++)
        {
            result[i].Should().BeApproximately(100.0, 1e-10,
                $"RSI at index {i} should be 100 when all changes are gains");
        }
    }

    [Fact]
    public void Rsi_AllLosses_ReturnsNear0()
    {
        var rsi = new RelativeStrengthIndex { Period = 5 };
        // Strictly decreasing prices: every change is a loss
        var prices = new double[] { 100, 99, 98, 97, 96, 95, 94, 93, 92, 91 };

        var result = rsi.Calculate(prices);

        // After the initial period, RSI should be 0 (no gains)
        for (var i = rsi.Period; i < result.Length; i++)
        {
            result[i].Should().BeApproximately(0.0, 1e-10,
                $"RSI at index {i} should be ~0 when all changes are losses");
        }
    }

    [Fact]
    public void Rsi_Period14_First13Values_AreNaN()
    {
        var rsi = new RelativeStrengthIndex { Period = 14 };
        var prices = Enumerable.Range(1, 30).Select(i => (double)(40 + i % 5)).ToArray();

        var result = rsi.Calculate(prices);

        for (var i = 0; i < 13; i++)
        {
            result[i].Should().Be(double.NaN,
                $"RSI at index {i} should be NaN (period=14, need 14 data points)");
        }
        // Index 14 (Period) should have a valid value
        result[14].Should().NotBe(double.NaN, "RSI at index Period should be calculated");
    }
}
