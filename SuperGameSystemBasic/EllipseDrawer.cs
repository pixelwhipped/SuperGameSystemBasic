using System;
using Microsoft.Xna.Framework;

namespace SuperGameSystemBasic
{
    public class EllipseDrawer
    {
        private static Vector2 GetEllipsePointFromX(float x, float a, float b)
        {
            //(x/a)^2 + (y/b)^2 = 1
            //(y/b)^2 = 1 - (x/a)^2
            //y/b = -sqrt(1 - (x/a)^2)  --Neg root for upper portion of the plane
            //y = b*-sqrt(1 - (x/a)^2)
            return new Vector2(x, b * -(float) Math.Sqrt(1 - x * x / a / a));
        }

        public static void DrawEllipse(bool[,] pixels, Rectangle area, bool fill)
        {
            // Get the size of the matrix
            var matrixWidth = pixels.GetLength(0);
            var matrixHeight = pixels.GetLength(1);

            var offsetY = area.Top;
            var offsetX = area.Left;

            // Figure out how big the ellipse is
            var ellipseWidth = (float) area.Width;
            var ellipseHeight = (float) area.Height;

            // Figure out the radiuses of the ellipses
            var radiusX = ellipseWidth / 2f;
            var radiusY = ellipseHeight / 2f;

            //Keep track of the previous y position
            var prevY = 0;
            var firstRun = true;

            // Loop through the points in the matrix
            for (var x = 0; x <= radiusX; ++x)
            {
                var xPos = x + offsetX;
                var rxPos = (int) ellipseWidth - x - 1 + offsetX;

                if (xPos < 0 || rxPos < xPos || xPos >= matrixWidth)
                    continue;

                var pointOnEllipseBoundCorrespondingToXMatrixPosition =
                    GetEllipsePointFromX(x - radiusX, radiusX, radiusY);
                var y = (int) Math.Floor(pointOnEllipseBoundCorrespondingToXMatrixPosition.Y + radiusY);
                var yPos = y + offsetY;


                var ryPos = (int) ellipseHeight - y - 1 + offsetY;

                if (yPos >= 0)
                {
                    if (xPos > -1 && xPos < matrixWidth && yPos > -1 && yPos < matrixHeight)
                        pixels[xPos, yPos] = true;

                    if (xPos > -1 && xPos < matrixWidth && ryPos > -1 && ryPos < matrixHeight)
                        pixels[xPos, ryPos] = true;

                    if (rxPos > -1 && rxPos < matrixWidth)
                    {
                        if (yPos > -1 && yPos < matrixHeight)
                            pixels[rxPos, yPos] = true;

                        if (ryPos > -1 && ryPos < matrixHeight)
                            pixels[rxPos, ryPos] = true;
                    }
                }

                //While there's a >1 jump in y, fill in the gap (assumes that this is not the first time we've tracked y, x != 0)
                for (var j = prevY - 1; !firstRun && j > y - 1 && y > 0; --j)
                {
                    var jPos = j + offsetY;
                    var rjPos = (int) ellipseHeight - j - 1 + offsetY;

                    if (jPos == rjPos - 1)
                        continue;

                    if (jPos > -1 && jPos < matrixHeight)
                        pixels[xPos, jPos] = true;

                    if (rjPos > -1 && rjPos < matrixHeight)
                        pixels[xPos, rjPos] = true;

                    if (rxPos > -1 && rxPos < matrixWidth)
                    {
                        if (jPos > -1 && jPos < matrixHeight)
                            pixels[rxPos, jPos] = true;

                        if (rjPos > -1 && rjPos < matrixHeight)
                            pixels[rxPos, rjPos] = true;
                    }
                }

                firstRun = false;
                prevY = y;
                var countTarget = radiusY - y;

                for (var count = 0; fill && count < countTarget; ++count)
                {
                    ++yPos;
                    --ryPos;

                    // Set all four points in the matrix we just learned about
                    //  also, make the indication that for the rest of this row, we need to fill the body of the ellipse
                    if (yPos > -1 && yPos < matrixHeight)
                        pixels[xPos, yPos] = true;

                    if (ryPos > -1 && ryPos < matrixHeight)
                        pixels[xPos, ryPos] = true;

                    if (rxPos > -1 && rxPos < matrixWidth)
                    {
                        if (yPos > -1 && yPos < matrixHeight)
                            pixels[rxPos, yPos] = true;

                        if (ryPos > -1 && ryPos < matrixHeight)
                            pixels[rxPos, ryPos] = true;
                    }
                }
            }
        }
    }
}