using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Printing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SuperGameSystemBasic.UI.Input;

namespace SuperGameSystemBasic.UI.Window
{
    public class AsciiArt : Window
    {
        private Button _closeButton;
        private Button _openButton;
        private int _scrollOffsetX;
        private int _scrollOffsetY;
        private readonly StorageFolder _storage;
        private readonly bool _traverseDirectories;
        private bool PreviewMode;

        public int Rows;
        public int Columns;
        public int currentF;
        public int currentB;
        public int currentS;
        public AsciiArt(int x, int y, IWindow parentWindow, Terminal terminal) : base(x, y, 60, 20, parentWindow)
        {
            Terminal = terminal;
            _storage = BasicOne.LocalStorageFolder;
            _traverseDirectories = true;
            CreateBuffer(Terminal.Columns, Terminal.Rows);
            Create();
        }

        public void CreateBuffer(int cols, int rows)
        {
            Columns = cols;
            Rows = rows;
            Buffer = new Character[Rows][];
            for (var i = 0; i < Rows; i++)
            {
                Buffer[i] = new Character[Columns];
                for (var j = 0; j < Columns; j++)
                    Buffer[i][j] = new Character();
            }
        }
        public Terminal Terminal { get; }

        public Character[][] Buffer { get; set; }

        public Action<StorageFile> SelectFile
        {
            get => Files.SelectFile;
            set => Files.SelectFile = value;
        }

        public string Title { get; set; } = "Glyph Art";

        public int BrowserWidth => Width / 3;

        public int MaxScrollY => Rows - (Height - 4);

        public int ScrollOffsetY
        {
            get => _scrollOffsetY;
            set => _scrollOffsetY = MathHelper.Clamp(value, 0, MaxScrollY);
        }

        public int MaxScrollX => Columns - (Width - (BrowserWidth + 3));

        public int ScrollOffsetX
        {
            get => _scrollOffsetX;
            set => _scrollOffsetX = MathHelper.Clamp(value, 0, MaxScrollX);
        }


        public FileBrowser Files { get; set; }

        private void Create()
        {
            IsDialog = false;
            BackColor = Terminal.CYAN;
            PostionY = ParentWindow.Height / 2 - Height / 2;
            Files = new FileBrowser(1, 1, BrowserWidth, Height - 2, _storage, this, true, ".png,.bmp,.jpg", null,
                _traverseDirectories)
            {
                SelectFile = f => Load(ImageToASCII(f, Columns, Rows))
            };

            Inputs.Add(Files);
            _closeButton = new Button(Width - 8, Height - 1, "CLOSE", this) {Action = delegate()
            {
                Load(Buffer);
                ExitWindow();
            }};
            _openButton = new Button(Width - 15, Height - 1, "OPEN", this) {Action = Files.Enter};            
            Inputs.Add(_closeButton);
            Inputs.Add(_openButton);
            Inputs.Add(new Button(Width - 25, Height - 1, "PREVIEW", this) { Action = ()=>PreviewMode = true});
            CurrentlySelected = Files;            
        }

        public override void Unselect() => Load(Buffer);
        private void Load(Character[][] ascii)
        {
            try
            {
                Buffer = ascii;
                var s = new StringBuilder("DIM splash = {");
                for (var y = 0; y < Rows; y++)
                for (var x = 0; x < Columns; x++)
                {
                    var a = ascii[y][x];
                    s.Append($"{(int) a.Glyph},{a.ForeColor},{a.BackColor},");
                }
                s.Append("}");
                var data = new DataPackage();
                data.SetText(s.ToString());
                Clipboard.SetContent(data);
            }
            catch
            {
            }
        }

        public override bool Update(Keys key, Func<char?> getKey, bool click, int mouseX, int mouseY)
        {
            if (!PreviewMode) return base.Update(key, getKey, click, mouseX, mouseY);
            PreviewMode = false;
            return true;
        }
        public override void Click(int x, int y, bool handled)
        {
            if (x == Width - 1 && y == 0)
            {
                ExitWindow();
            }
            else if (x == 58 && y == 18)
            {
                ScrollOffsetY++;
            }
            else if (x == 58 && y == 2)
            {
                ScrollOffsetY--;
            }
            else if (x == 57 && y == 18)
            {
                ScrollOffsetX++;
            }
            else if (x == 21 && y == 18)
            {
                ScrollOffsetX--;
            }
            else if (x == 58 && y > 2 && y < 18)
            {
                var s = (y - 3f) / 14f * MaxScrollY;
                ScrollOffsetY = (int) s;
            }
            else if (y == 18 && x >= 21 && x <= 56)
            {
                var s = (x - 21f) / 35f * MaxScrollX;
                ScrollOffsetX = (int) s;
            }
            else if (y == 19 && x >= 2 && x <= 17)
            {
                currentF = x - 2;
            }
            else if (y == 20 && x >= 2 && x <= 17)
            {
                currentB = x - 2;
            }
            else if (y == 19 && x >= 19 && x <= 23)
            {
                currentS = x - 19;
            }
            else if (x >= 21 && y >= 2 && x<=75 && y <=17)
            {
                var cx = MathHelper.Clamp(x - 21 + ScrollOffsetX, 0, Columns);
                var cy = MathHelper.Clamp(y - 2 + ScrollOffsetY, 0, Rows);
                if(cx<Columns&&cy<Rows)
                Buffer[cy][cx] = new Character(currentS == 0 ? Glyph.SolidFill : currentS == 1 ? Glyph.DarkFill : currentS == 2 ? Glyph.GrayFill : currentS == 3 ? Glyph.LightFill : Glyph.Space, currentF, currentB, SpriteEffects.None);
            }
            else if (x == 26 && y == 19)
            {
                CreateBuffer(MathHelper.Clamp(Columns-1,1,Terminal.Columns),Rows);
            }
            else if (x == 31 && y == 19)
            {
                CreateBuffer(MathHelper.Clamp(Columns + 1, 1, Terminal.Columns), Rows);
            }
            else if (x == 26 && y == 20)
            {
                CreateBuffer(Columns,MathHelper.Clamp(Rows - 1, 1, Terminal.Rows));
            }
            else if (x == 31 && y == 20)
            {
                CreateBuffer(Columns,MathHelper.Clamp(Rows + 1, 1, Terminal.Rows));
            }
            else
            {
                base.Click(x, y, handled);
            }
        }

        public override void Draw()
        {
            if (PreviewMode)
            {
                ParentWindow.Clear(Terminal.BLACK);
                for (var y = 0; y < Rows; y++)
                {
                    if (y == Terminal.Rows) break;
                    for (var x = 0; x < Columns; x++)
                    {
                        if (x == Terminal.Columns) break;
                        var g = Buffer[y][x];
                        ParentWindow.WriteGlyph(g.Glyph, x, y, g.ForeColor, g.BackColor);
                    }
                }
                return;
            }
            var pl = (Width - Title.Length) / 2;
            WriteText((" ".PadLeft(pl) + Title).PadRight(Width, ' '), 0, 0, TitleColour, TitleBarColour);
            WriteGlyph(Glyph.Close, Width - 1, 0, TitleColour, TitleBarColour);
            FillRect(0, 1, Width, Height, BackColor, true); //Main Box     

            DrawVLine(Width - 2, 2, Height - 3, Terminal.GRAY, true);
            WriteGlyph(Glyph.TriangleUp, Width - 2, 2, Terminal.GRAY, Terminal.DARK_GRAY);
            WriteGlyph(Glyph.TriangleDown, Width - 2, Height - 2, Terminal.GRAY, Terminal.DARK_GRAY);
            WriteText("-", Width - 2, 3 + (int) ((float) ScrollOffsetY / MaxScrollY * (Height - 6f)),
                Terminal.DARK_GRAY, Terminal.GRAY);

            DrawHLine(BrowserWidth + 1, Height - 2, Width - (BrowserWidth + 3), Terminal.GRAY, true);
            WriteGlyph(Glyph.TriangleLeft, BrowserWidth + 1, Height - 2, Terminal.GRAY, Terminal.DARK_GRAY);
            WriteGlyph(Glyph.TriangleRight, Width - 3, Height - 2, Terminal.GRAY, Terminal.DARK_GRAY);
            WriteText("-",
                BrowserWidth + 2 + (int) ((float) ScrollOffsetX / MaxScrollX * (Width - (BrowserWidth + 6f))),
                Height - 2, Terminal.DARK_GRAY, Terminal.GRAY);

            WriteGlyph(Glyph.FUpper, 1, Height - 1, ForeColor, BackColor);
            WriteGlyph(Glyph.BUpper, 1, Height, ForeColor, BackColor);
            for (var i = 0; i <= 15; i++)
            {
                if(currentF==i)WriteGlyph(Glyph.XUpper, 2+i, Height - 1, i == 15 ? 0 : 15, i);
                else WriteGlyph(Glyph.SolidFill, 2 + i, Height - 1, i, BackColor);
                if (currentB == i) WriteGlyph(Glyph.XUpper, 2 + i, Height, i == 15 ? 0 : 15, i);
                else WriteGlyph(Glyph.SolidFill, 2 + i, Height, i, BackColor);
            }
            WriteGlyph(Glyph.SUpper, 18, Height-1, ForeColor, BackColor);
            WriteGlyph(Glyph.SolidFill,19,Height-1,currentF,currentB);
            WriteGlyph(Glyph.DarkFill, 20, Height - 1, currentF, currentB);
            WriteGlyph(Glyph.GrayFill, 21, Height - 1, currentF, currentB);
            WriteGlyph(Glyph.LightFill, 22, Height - 1, currentF, currentB);
            WriteGlyph(Glyph.Space, 23, Height - 1, currentF, currentB);
            WriteGlyph(Glyph.TriangleUp, 19 + currentS, Height, ForeColor, BackColor);

            WriteGlyph(Glyph.CUpper, 25, Height - 1, ForeColor, BackColor);
            WriteGlyph(Glyph.TriangleLeft, 26, Height - 1, ForeColor, BackColor);
            WriteText(Columns.ToString(),28,Height-1,ForeColor,BackColor);
            WriteGlyph(Glyph.TriangleRight, 31, Height - 1, ForeColor, BackColor);

            WriteGlyph(Glyph.RUpper, 25, Height, ForeColor, BackColor);
            WriteGlyph(Glyph.TriangleLeft, 26, Height, ForeColor, BackColor);
            WriteText(Rows.ToString(), 28, Height, ForeColor, BackColor);
            WriteGlyph(Glyph.TriangleRight, 31, Height, ForeColor, BackColor);
            FillRect(19, 2,39,16,Terminal.DARK_GRAY);
            try
            {
                for (var y = 0; y < Rows; y++)
                {
                    var yOff = y + 2;
                    if (yOff > Height - 3) break;
                    for (var x = 0; x < Columns; x++)
                    {
                        var xOff = x + BrowserWidth + 1;
                        if (xOff > Width - 3) break;
                        var g = Buffer[y + ScrollOffsetY][x + ScrollOffsetX];
                        WriteGlyph(g.Glyph, xOff, yOff, g.ForeColor, g.BackColor);
                    }
                }
            }
            catch (Exception e)
            {
            }
            base.Draw();
        }

        public Character[][] ImageToASCII(StorageFile file, int columns, int rows)
        {
            var c = new Character[rows][];

            // Create a bitmap from the image
            var bmp = ScaledWImage(file, 80, 24);
            if (bmp == null)
                return null;
            var array = bmp.PixelBuffer.ToArray();
            int y;
            for (y = 0; y < rows; y++)
            {
                c[y] = new Character[columns];
                int x;
                for (x = 0; x < columns; x++)
                {                   
                    c[y][x] = new Character(Glyph.Space,Terminal.BLACK,Terminal.BLACK,SpriteEffects.None);
                }
            }
            // Loop through each pixel in the bitmap
            try
            {                
                for (y = 0; y < bmp.PixelHeight; y++)
                {
                    //c[y] = new Character[columns];
                    int x;
                    for (x = 0; x < bmp.PixelWidth; x++)
                    {
                        // Get the color of the current pixel
                        var col = GetPixel(array, y, x, 80, 24);
                        c[y][x] = Terminal.ClosesetShade(new Color(col.R, col.G, col.B));
                    }
                }
            }
            catch (Exception e)
            {
            }
            return c;
        }

        public Color GetPixel(byte[] pixels, int x, int y, uint width, uint height)
        {
            var i = x;
            var j = y;
            var k = (i * (int) width + j) * 4;
            var b = pixels[k + 0];
            var g = pixels[k + 1];
            var r = pixels[k + 2];
            return new Color(r, g, b, (byte) 255);
        }

        public static WriteableBitmap ScaledWImage(StorageFile file, int maxWidth, int maxHeight)
        {
            WriteableBitmap newImage = null;
            try
            {
                var bitmap = new BitmapImage();
                var fileStream = Task.Run(async () => await file.OpenReadAsync()).Result;
                bitmap.SetSource(fileStream);
                var origHeight = bitmap.PixelHeight;
                var origWidth = bitmap.PixelWidth;
                var ratioX = maxWidth / (float) origWidth;
                var ratioY = maxHeight / (float) origHeight;
                //var ratio = Math.Min(ratioX, ratioY);
                var newHeight = (int) (origHeight * ratioY);
                var newWidth = (int) (origWidth * ratioX);

                if (ratioX > 1 && ratioY > 1)
                {
                    newHeight = origHeight;
                    newWidth = origWidth;
                }


                newImage = new WriteableBitmap(newWidth, newHeight);
                fileStream.Seek(0);
                //var fileStream = Task.Run(async () => await BitmapDecoder.CreateAsync(fileStream)).Result;
                var decoder =
                    Task.Run(async () => await BitmapDecoder.CreateAsync(fileStream))
                        .Result; //BitmapDecoder.CreateAsync(fileStream).GetResults();

                // Scale image to appropriate size 
                var transform = new BitmapTransform
                {
                    ScaledWidth = Convert.ToUInt32(newImage.PixelWidth),
                    ScaledHeight = Convert.ToUInt32(newImage.PixelHeight)
                };
                var pixelData = Task.Run(async () => await decoder.GetPixelDataAsync(
                    BitmapPixelFormat.Bgra8, // WriteableBitmap uses BGRA format 
                    BitmapAlphaMode.Straight,
                    transform,
                    ExifOrientationMode.IgnoreExifOrientation, // This sample ignores Exif orientation 
                    ColorManagementMode.DoNotColorManage
                )).Result;

                // An array containing the decoded image data, which could be modified before being displayed 
                var sourcePixels = pixelData.DetachPixelData();

                // Open a stream to copy the image contents to the WriteableBitmap's pixel buffer 
                using (var stream = newImage.PixelBuffer.AsStream())
                {
                    stream.WriteAsync(sourcePixels, 0, sourcePixels.Length).GetAwaiter().GetResult();
                }
            }
            catch (Exception e)
            {
            }

            return newImage;
        }
    }
}