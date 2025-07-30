using Sharpcaster.Models.Media;
using System;
using Xunit;

namespace Sharpcaster.Test
{
    public class CastColorTester
    {
        [Fact]
        public void TestCastColorFromRgb()
        {
            var color = CastColor.FromRgb(255, 0, 0);
            Assert.Equal("#FF0000FF", color.ToString());
        }

        [Fact]
        public void TestCastColorFromRgba()
        {
            var color = CastColor.FromRgba(255, 0, 0, 128);
            Assert.Equal("#FF000080", color.ToString());
        }

        [Fact]
        public void TestCastColorFromHex()
        {
            var color = CastColor.FromHex("#00FF00");
            Assert.Equal("#00FF00FF", color.ToString());
        }

        [Fact]
        public void TestCastColorFromHexWithAlpha()
        {
            var color = CastColor.FromHex("#00FF0080");
            Assert.Equal("#00FF0080", color.ToString());
        }

        [Fact]
        public void TestCastColorFromHsl()
        {
            var color = CastColor.FromHsl(120, 100, 50); // Pure green
            Assert.Equal("#00FF00FF", color.ToString());
        }

        [Fact]
        public void TestCastColorPredefinedColors()
        {
            Assert.Equal("#FF0000FF", CastColors.Red.ToString());
            Assert.Equal("#00FF00FF", CastColors.Green.ToString());
            Assert.Equal("#0000FFFF", CastColors.Blue.ToString());
            Assert.Equal("#FFFFFFFF", CastColors.White.ToString());
            Assert.Equal("#000000FF", CastColors.Black.ToString());
            Assert.Equal("#00000000", CastColors.Transparent.ToString());
        }

        [Fact]
        public void TestCastColorImplicitConversion()
        {
            CastColor color = "#FF00FF";
            Assert.Equal("#FF00FFFF", color.ToString());
        }

        [Fact]
        public void TestCastColorEquality()
        {
            var color1 = CastColor.FromRgb(255, 0, 0);
            var color2 = CastColor.FromHex("#FF0000");
            Assert.Equal(color1, color2);
        }

        [Fact]
        public void TestTextTrackStyleWithCastColor()
        {
            var style = new TextTrackStyle
            {
                EdgeColor = CastColors.Green,
                ForegroundColor = CastColor.FromRgb(255, 255, 255),
                BackgroundColor = CastColor.FromRgba(0, 0, 0, 128),
                WindowColor = CastColor.FromHsl(240, 50, 75)
            };

            Assert.Equal(CastColors.Green, style.EdgeColor);
            Assert.Equal("#FFFFFFFF", style.ForegroundColor?.ToString());
            Assert.Equal("#00000080", style.BackgroundColor?.ToString());
            Assert.Equal("#9F9FDFFF", style.WindowColor?.ToString());
        }

        [Fact]
        public void TestInvalidHexColor()
        {
            Assert.Throws<ArgumentException>(() => CastColor.FromHex("invalid"));
            Assert.Throws<ArgumentException>(() => CastColor.FromHex("#GG0000"));
            Assert.Throws<ArgumentException>(() => CastColor.FromHex("#FF00"));
        }
    }
}