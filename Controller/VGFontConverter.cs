using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NRasterizer;
using OpenVG;
using SixLabors.ImageSharp;

namespace EMinor
{
    public class VGFontConverter : NRasterizer.IGlyphRasterizer
    {
        private readonly IOpenVG vg;

        private List<PathSegment> segments;
        private List<float> coords;
        private float[] escapement;

        public VGFontConverter(IOpenVG vg)
        {
            this.vg = vg;
        }

        /// <summary>
        /// Loads a bitmap font from a JSON+PNG pair of files. Compatible with font textures generated
        /// from https://evanw.github.io/font-texture-generator/ tool. https://github.com/evanw/font-texture-generator
        /// </summary>
        public VGFont FromODX(String jsonPath)
        {
            var textureDescriptor = JsonConvert.DeserializeObject<FontTextureDescriptor>(File.ReadAllText(jsonPath));

            var image = Image.Load(textureDescriptor.Name + ".png");

            var escapements = new Dictionary<uint, float[]>(textureDescriptor.Characters.Count);

            // Create OpenVG font object:
            var destFont = vg.CreateFont(textureDescriptor.Characters.Count);

            int height = textureDescriptor.Height;
            int width = textureDescriptor.Width;

            // Create main parent image:
            var parent = vg.CreateImage(ImageFormat.VG_A_8, width, height, ImageQuality.VG_IMAGE_QUALITY_BETTER | ImageQuality.VG_IMAGE_QUALITY_FASTER | ImageQuality.VG_IMAGE_QUALITY_NONANTIALIASED);
            vg.ThrowIfError();

            unsafe
            {
                // We need to y-flip the image and cut the format down from RGB888 to just A8 (only alpha; no colors):
                fixed (byte* data = new byte[width * height])
                {
                    byte* p = data;
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            *p++ = image[x, (height - 1) - y].R;
                        }
                    }

                    // Upload parent image:
                    vg.ImageSubData(parent, data, width, ImageFormat.VG_A_8, 0, 0, width, height);
                    vg.ThrowIfError();
                }
            }

            // Set the glyphs for the font to child images of the parent image:
            foreach (var entry in textureDescriptor.Characters)
            {
                var ch = entry.Key[0];
                var desc = entry.Value;

                //Console.WriteLine($"img for '{ch}': {parent}, x={desc.X}, y={(height - 1) - (desc.Y + desc.Height - 1)}, {desc.Width}, {desc.Height}");
                var child = vg.ChildImage(parent, desc.X, (height - 1) - (desc.Y + desc.Height - 1), desc.Width, desc.Height);
                vg.ThrowIfError();

                var origin = new float[2] { desc.OriginX, (desc.Height - 1) - desc.OriginY };
                var escapement = new float[2] { desc.Advance, 0f };
                escapements.Add(ch, escapement);

                //Console.WriteLine($"set glyph: origin = {origin[0]},{origin[1]}; escapement = {escapement[0]},{escapement[1]}");
                vg.SetGlyphToImage(destFont, ch, child, origin, escapement);
                vg.ThrowIfError();

                vg.DestroyImage(child);
                vg.ThrowIfError();
            }

            vg.DestroyImage(parent);

            return new VGFont(vg, destFont, escapements, textureDescriptor.Size);
        }

        public VGFont FromTTF(String path)
        {
            Typeface typeFace;
            using (var fi = System.IO.File.OpenRead(path))
            {
                Debug.WriteLine("OpenTypeReader");
                typeFace = new OpenTypeReader().Read(fi);
            }

            // Create OpenVG font object:
            var destFont = vg.CreateFont(typeFace.Glyphs.Count);

            // Create a renderer instance that renders glyphs to OpenVG paths:
            var renderer = new NRasterizer.Renderer(typeFace, this);

            var allChars = typeFace.AllCharacters().ToList();
            var escapements = new Dictionary<uint, float[]>(allChars.Count);

            // Run through all glyphs defined and create OpenVG paths for them:
            foreach (var c in allChars)
            {
                renderer.RenderChar(0, 0, c, 1, false);
                this.SetGlyphToPath(destFont, c);
                escapements[c] = escapement;
            }

            return new VGFont(vg, destFont, escapements, 18.0f);
        }

        public void SetGlyphToPath(FontHandle font, uint glyphIndex)
        {
            PathHandle path = PathHandle.Invalid;
            if (segments.Count != 0)
            {
                byte[] segmentBytes = segments.Cast<byte>().ToArray();
                float[] coordsFloats = coords.ToArray();

                path = vg.CreatePath(
                    Constants.VG_PATH_FORMAT_STANDARD,
                    PathDatatype.VG_PATH_DATATYPE_F,
                    1.0f / 2048,
                    0.0f,
                    segmentBytes.Length,
                    coordsFloats.Length,
                    PathCapabilities.VG_PATH_CAPABILITY_ALL
                );

                vg.AppendPathData(path, segmentBytes, coordsFloats);
            }

            var origin = new float[] { 0.0f, 0.0f };
            vg.SetGlyphToPath(font, glyphIndex, path, false, origin, escapement);

            if (path != PathHandle.Invalid)
            {
                vg.DestroyPath(path);
            }
        }

        #region IGlyphRasterizer

        public int Resolution => 1;

        public void BeginRead(int countourCount)
        {
            this.segments = new List<PathSegment>(countourCount);
            this.coords = new List<float>(countourCount * 6);
        }

        public void EndRead(double escapementX, double escapementY)
        {
            this.escapement = new float[] { (float)escapementX / 2048, (float)escapementY / 2048 };
        }

        public void CloseFigure()
        {
        }

        public void MoveTo(double x, double y)
        {
            segments.Add(PathSegment.VG_MOVE_TO);
            coords.Add((float)x);
            coords.Add((float)y);
        }

        public void LineTo(double x, double y)
        {
            segments.Add(PathSegment.VG_LINE_TO);
            coords.Add((float)x);
            coords.Add((float)y);
        }

        public void Curve3(double p2x, double p2y, double x, double y)
        {
            segments.Add(PathSegment.VG_QUAD_TO);
            coords.Add((float)p2x);
            coords.Add((float)p2y);
            coords.Add((float)x);
            coords.Add((float)y);
        }

        public void Curve4(double p2x, double p2y, double p3x, double p3y, double x, double y)
        {
            segments.Add(PathSegment.VG_CUBIC_TO);
            coords.Add((float)p2x);
            coords.Add((float)p2y);
            coords.Add((float)p3x);
            coords.Add((float)p3y);
            coords.Add((float)x);
            coords.Add((float)y);
        }

        public void Flush()
        {
        }

        #endregion
    }

    public class FontTextureDescriptor
    {
        public string Name { get; set; }
        public int Size { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Dictionary<string, FontTextureChar> Characters { get; set; }
    }

    public class FontTextureChar
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int OriginX { get; set; }
        public int OriginY { get; set; }
        public int Advance { get; set; }
    }
}
