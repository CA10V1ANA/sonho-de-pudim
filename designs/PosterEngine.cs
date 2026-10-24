using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;

public class PosterEngine
{
    public static readonly Color Caramel = ColorTranslator.FromHtml("#9f4d17");
    public static readonly Color CaramelDeep = ColorTranslator.FromHtml("#77310d");
    public static readonly Color Chocolate = ColorTranslator.FromHtml("#3d1b0f");
    public static readonly Color Espresso = ColorTranslator.FromHtml("#220d06");
    public static readonly Color Cream = ColorTranslator.FromHtml("#fff7e5");
    public static readonly Color WarmWhite = ColorTranslator.FromHtml("#fffaf0");
    public static readonly Color Milk = ColorTranslator.FromHtml("#f7e7bd");
    public static readonly Color Honey = ColorTranslator.FromHtml("#edae49");
    public static readonly Color GoldLight = ColorTranslator.FromHtml("#ffd58e");
    public static readonly Color Orange = ColorTranslator.FromHtml("#cf691b");
    public static readonly Color Ink = ColorTranslator.FromHtml("#422416");
    public static readonly Color Muted = ColorTranslator.FromHtml("#765643");
    public static readonly Color IFoodRed = ColorTranslator.FromHtml("#ea1d2c");
    public static readonly Color SageBg = ColorTranslator.FromHtml("#e8f5ec");
    public static readonly Color SageBorder = ColorTranslator.FromHtml("#8ec99e");
    public static readonly Color SageText = ColorTranslator.FromHtml("#185a2d");

    private string assetsDir;
    private string outDir;

    public PosterEngine(string assetsDirectory, string outputDirectory)
    {
        assetsDir = assetsDirectory;
        outDir = outputDirectory;
        Directory.CreateDirectory(outDir);
    }

    private Bitmap LoadAsset(string name)
    {
        return new Bitmap(Path.Combine(assetsDir, name));
    }

    private Graphics InitCanvas(Bitmap bmp)
    {
        var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.CompositingQuality = CompositingQuality.HighQuality;
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        return g;
    }

    public static GraphicsPath RoundedRect(RectangleF bounds, float tl, float tr, float br, float bl)
    {
        var path = new GraphicsPath();
        float x = bounds.X, y = bounds.Y, w = bounds.Width, h = bounds.Height;
        if (tl > 0) path.AddArc(x, y, tl * 2, tl * 2, 180, 90); else path.AddLine(x, y, x, y);
        if (tr > 0) path.AddArc(x + w - tr * 2, y, tr * 2, tr * 2, 270, 90); else path.AddLine(x + w, y, x + w, y);
        if (br > 0) path.AddArc(x + w - br * 2, y + h - br * 2, br * 2, br * 2, 0, 90); else path.AddLine(x + w, y + h, x + w, y + h);
        if (bl > 0) path.AddArc(x, y + h - bl * 2, bl * 2, bl * 2, 90, 90); else path.AddLine(x, y + h, x, y + h);
        path.CloseFigure();
        return path;
    }

    public static GraphicsPath UniformRect(RectangleF b, float r)
    {
        return RoundedRect(b, r, r, r, r);
    }

    public static void DrawSoftShadow(Graphics g, RectangleF rect, float tl, float tr, float br, float bl, float offsetY, float spread, int maxAlpha)
    {
        int steps = 10;
        for (int i = steps; i >= 1; i--)
        {
            float grow = (spread * i) / steps;
            int alpha = Math.Max(2, (int)(maxAlpha * Math.Pow(1.0 - (double)(i - 1) / steps, 2.0) / 3.2));
            var r = new RectangleF(rect.X - grow, rect.Y + offsetY - grow * 0.5f, rect.Width + grow * 2, rect.Height + grow * 2);
            using (var p = RoundedRect(r, tl + grow, tr + grow, br + grow, bl + grow))
            using (var b = new SolidBrush(Color.FromArgb(alpha, 52, 20, 6)))
            {
                g.FillPath(b, p);
            }
        }
    }

    public static void DrawRadialGlow(Graphics g, float cx, float cy, float radius, Color centerColor, int alpha)
    {
        using (var path = new GraphicsPath())
        {
            path.AddEllipse(cx - radius, cy - radius, radius * 2, radius * 2);
            using (var pgb = new PathGradientBrush(path))
            {
                pgb.CenterColor = Color.FromArgb(alpha, centerColor);
                pgb.SurroundColors = new Color[] { Color.FromArgb(0, centerColor) };
                g.FillPath(pgb, path);
            }
        }
    }

    public static void DrawCaramelTopDrips(Graphics g, int width, Color dripColor)
    {
        using (var b = new SolidBrush(dripColor))
        {
            g.FillRectangle(b, 0, 0, width, 16);
            int[] xs = { 70, 230, 440, 680, 880 };
            int[] ws = { 120, 170, 210, 150, 130 };
            int[] hs = { 42, 62, 48, 68, 44 };
            for (int i = 0; i < xs.Length; i++)
            {
                g.FillEllipse(b, xs[i], -hs[i] / 2, ws[i], hs[i]);
            }
        }
    }

    public static void DrawSparkle(Graphics g, float cx, float cy, float size, Color color)
    {
        using (var path = new GraphicsPath())
        {
            float s = size, q = size * 0.18f;
            path.AddBezier(cx, cy - s, cx + q, cy - q, cx + q, cy - q, cx + s, cy);
            path.AddBezier(cx + s, cy, cx + q, cy + q, cx + q, cy + q, cx, cy + s);
            path.AddBezier(cx, cy + s, cx - q, cy + q, cx - q, cy + q, cx - s, cy);
            path.AddBezier(cx - s, cy, cx - q, cy - q, cx - q, cy - q, cx, cy - s);
            path.CloseFigure();
            using (var b = new SolidBrush(color))
                g.FillPath(b, path);
        }
    }

    public static void DrawHeart(Graphics g, float cx, float cy, float size, Color color)
    {
        using (var path = new GraphicsPath())
        {
            float w = size, h = size * 0.92f;
            path.AddBezier(cx, cy + h * 0.55f, cx - w * 0.95f, cy - h * 0.05f, cx - w * 0.55f, cy - h * 0.75f, cx, cy - h * 0.22f);
            path.AddBezier(cx, cy - h * 0.22f, cx + w * 0.55f, cy - h * 0.75f, cx + w * 0.95f, cy - h * 0.05f, cx, cy + h * 0.55f);
            path.CloseFigure();
            using (var b = new SolidBrush(color))
                g.FillPath(b, path);
        }
    }

    public static void DrawImageCover(Graphics g, Image img, RectangleF dest, GraphicsPath clipPath)
    {
        var state = g.Save();
        if (clipPath != null) g.SetClip(clipPath);
        float imgRatio = (float)img.Width / img.Height;
        float destRatio = dest.Width / dest.Height;
        float drawW, drawH, drawX, drawY;
        if (imgRatio > destRatio)
        {
            drawH = dest.Height;
            drawW = dest.Height * imgRatio;
            drawX = dest.X - (drawW - dest.Width) / 2f;
            drawY = dest.Y;
        }
        else
        {
            drawW = dest.Width;
            drawH = dest.Width / imgRatio;
            drawX = dest.X;
            drawY = dest.Y - (drawH - dest.Height) / 2f;
        }
        g.DrawImage(img, new RectangleF(drawX, drawY, drawW, drawH));
        g.Restore(state);
    }

    public static void DrawImageContain(Graphics g, Image img, RectangleF dest, bool dropShadow)
    {
        float imgRatio = (float)img.Width / img.Height;
        float destRatio = dest.Width / dest.Height;
        float drawW, drawH;
        if (imgRatio > destRatio)
        {
            drawW = dest.Width;
            drawH = dest.Width / imgRatio;
        }
        else
        {
            drawH = dest.Height;
            drawW = dest.Height * imgRatio;
        }
        float drawX = dest.X + (dest.Width - drawW) / 2f;
        float drawY = dest.Y + (dest.Height - drawH) / 2f;

        if (dropShadow)
        {
            DrawRadialGlow(g, drawX + drawW / 2f, drawY + drawH * 0.88f, Math.Min(drawW, drawH) * 0.42f, Color.FromArgb(255, 42, 16, 4), 75);
        }
        g.DrawImage(img, new RectangleF(drawX, drawY, drawW, drawH));
    }

    public static void DrawCenteredMixedLine(Graphics g, float centerX, float y, string part1, string italicPart2, float ptSize, Color col1, Color col2)
    {
        using (var fReg = new Font("Georgia", ptSize, FontStyle.Bold, GraphicsUnit.Point))
        using (var fIt = new Font("Georgia", ptSize, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point))
        using (var sf = new StringFormat(StringFormat.GenericTypographic) { FormatFlags = StringFormatFlags.MeasureTrailingSpaces })
        {
            float w1 = string.IsNullOrEmpty(part1) ? 0 : g.MeasureString(part1, fReg, 2000, sf).Width;
            float w2 = string.IsNullOrEmpty(italicPart2) ? 0 : g.MeasureString(italicPart2, fIt, 2000, sf).Width;
            float startX = centerX - (w1 + w2) / 2f;
            if (!string.IsNullOrEmpty(part1))
            {
                using (var b1 = new SolidBrush(col1))
                    g.DrawString(part1, fReg, b1, startX, y, sf);
            }
            if (!string.IsNullOrEmpty(italicPart2))
            {
                using (var b2 = new SolidBrush(col2))
                    g.DrawString(italicPart2, fIt, b2, startX + w1, y, sf);
            }
        }
    }

    public static void DrawCenteredParagraph(Graphics g, string text, RectangleF box, string family, float ptSize, FontStyle style, Color color)
    {
        using (var f = new Font(family, ptSize, style, GraphicsUnit.Point))
        using (var b = new SolidBrush(color))
        using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near })
        {
            g.DrawString(text, f, b, box, sf);
        }
    }

    public static void DrawEyebrowUpper(Graphics g, string text, float centerX, float y, float ptSize, Color color)
    {
        // Spaced uppercase eyebrow
        string spaced = string.Join(" ", text.ToUpperInvariant().ToCharArray()).Replace("   ", "    ");
        using (var f = new Font("Segoe UI", ptSize, FontStyle.Bold, GraphicsUnit.Point))
        using (var b = new SolidBrush(color))
        using (var sf = new StringFormat { Alignment = StringAlignment.Center })
        {
            g.DrawString(spaced, f, b, centerX, y, sf);
        }
    }

    public static RectangleF DrawPill(Graphics g, string text, float centerX, float centerY, Color bg, Color border, Color fg, float ptSize, float padX, float padY, bool shadow = false)
    {
        using (var f = new Font("Segoe UI", ptSize, FontStyle.Bold, GraphicsUnit.Point))
        {
            var sz = g.MeasureString(text, f);
            float w = sz.Width + padX * 2;
            float h = sz.Height + padY * 2;
            var rect = new RectangleF(centerX - w / 2f, centerY - h / 2f, w, h);
            if (shadow) DrawSoftShadow(g, rect, h / 2f, h / 2f, h / 2f, h / 2f, 10, 18, 55);
            using (var path = UniformRect(rect, h / 2f))
            {
                using (var b = new SolidBrush(bg)) g.FillPath(b, path);
                if (border.A > 0)
                {
                    using (var p = new Pen(border, 2.5f)) g.DrawPath(p, path);
                }
            }
            using (var bFg = new SolidBrush(fg))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString(text, f, bFg, rect, sf);
            }
            return rect;
        }
    }

    public static void DrawPillRow(Graphics g, float centerX, float centerY, string[] labels, Color[] bgs, Color[] borders, Color[] fgs, float ptSize, float padX, float padY, float gap)
    {
        using (var f = new Font("Segoe UI", ptSize, FontStyle.Bold, GraphicsUnit.Point))
        {
            float[] widths = new float[labels.Length];
            float h = 0;
            float totalW = 0;
            for (int i = 0; i < labels.Length; i++)
            {
                var sz = g.MeasureString(labels[i], f);
                widths[i] = sz.Width + padX * 2;
                h = Math.Max(h, sz.Height + padY * 2);
                totalW += widths[i];
            }
            totalW += gap * (labels.Length - 1);
            float curX = centerX - totalW / 2f;
            for (int i = 0; i < labels.Length; i++)
            {
                DrawPill(g, labels[i], curX + widths[i] / 2f, centerY, bgs[i], borders[i], fgs[i], ptSize, padX, padY, true);
                curX += widths[i] + gap;
            }
        }
    }

    public static void DrawArtisanalSeal(Graphics g, float cx, float cy, float radius, string smallTop, string line1, string line2)
    {
        var rect = new RectangleF(cx - radius, cy - radius, radius * 2, radius * 2);
        DrawSoftShadow(g, rect, radius, radius, radius, radius, 12, 22, 70);
        using (var b = new SolidBrush(WarmWhite)) g.FillEllipse(b, rect);
        using (var p1 = new Pen(Honey, 3.5f)) g.DrawEllipse(p1, rect);
        var inner = new RectangleF(cx - radius + 10, cy - radius + 10, (radius - 10) * 2, (radius - 10) * 2);
        using (var p2 = new Pen( Color.FromArgb(130, Orange), 1.5f) { DashStyle = DashStyle.Dash }) g.DrawEllipse(p2, inner);

        using (var fSmall = new Font("Segoe UI", 10.5f, FontStyle.Bold, GraphicsUnit.Point))
        using (var fBig = new Font("Georgia", 15.5f, FontStyle.Bold, GraphicsUnit.Point))
        using (var bOrange = new SolidBrush(Orange))
        using (var bChoc = new SolidBrush(Chocolate))
        using (var sf = new StringFormat { Alignment = StringAlignment.Center })
        {
            g.DrawString(smallTop.ToUpperInvariant(), fSmall, bOrange, cx, cy - radius * 0.56f, sf);
            g.DrawString(line1, fBig, bChoc, cx, cy - radius * 0.22f, sf);
            g.DrawString(line2, fBig, bChoc, cx, cy + radius * 0.08f, sf);
        }
        DrawHeart(g, cx, cy + radius * 0.54f, 16, Orange);
    }

    public static void DrawFooter(Graphics g, int width, float y, string leftText, string midText, string badgeText, Color lineCol, Color txtCol, Color badgeBg, Color badgeFg)
    {
        float margin = 64f;
        using (var pen = new Pen(lineCol, 2f))
            g.DrawLine(pen, margin, y, width - margin, y);

        float rowCy = y + 48f;
        using (var f = new Font("Segoe UI", 16f, FontStyle.Bold, GraphicsUnit.Point))
        using (var b = new SolidBrush(txtCol))
        using (var sfLeft = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center })
        using (var sfCenter = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
        {
            g.DrawString(leftText, f, b, new RectangleF(margin, rowCy - 24, 360, 48), sfLeft);
            if (!string.IsNullOrEmpty(midText))
                g.DrawString(midText, f, b, new RectangleF(width / 2f - 220, rowCy - 24, 440, 48), sfCenter);
        }

        if (!string.IsNullOrEmpty(badgeText))
        {
            using (var fB = new Font("Segoe UI", 14.5f, FontStyle.Bold, GraphicsUnit.Point))
            {
                var sz = g.MeasureString(badgeText, fB);
                float bw = sz.Width + 44f;
                DrawPill(g, badgeText, width - margin - bw / 2f, rowCy, badgeBg, Color.Transparent, badgeFg, 14.5f, 22f, 9f, false);
            }
        }
    }

    public void GenerateAll()
    {
        Generate01Vitrine();
        Generate02Tamanhos();
        Generate03SaborCafe();
        Generate04ZeroLactose();
        Generate05DeliveryIfood();
        Generate06Eventos();
        Generate07Revenda();
        Generate08Presentear();
        Generate09StoryCardapio();
        Generate10StoryHorarios();
    }

    // 01: VITRINE OFICIAL
    private void Generate01Vitrine()
    {
        int W = 1080, H = 1350;
        using (var bmp = new Bitmap(W, H))
        using (var g = InitCanvas(bmp))
        using (var logo = LoadAsset("logo.png"))
        using (var pudim = LoadAsset("pudim-hd.png"))
        {
            using (var bg = new LinearGradientBrush(new Rectangle(0, 0, W, H), Cream, ColorTranslator.FromHtml("#f3dcb0"), 90f))
                g.FillRectangle(bg, 0, 0, W, H);
            DrawRadialGlow(g, W / 2f, 740, 520, Honey, 95);
            DrawCaramelTopDrips(g, W, Caramel);
            DrawSparkle(g, 130, 510, 26, Honey);
            DrawSparkle(g, 950, 880, 22, Orange);
            DrawHeart(g, 145, 920, 24, Color.FromArgb(140, Orange));

            // Brand header badge
            var logoRect = new RectangleF(315, 58, 86, 86);
            DrawSoftShadow(g, logoRect, 43, 43, 43, 43, 6, 12, 50);
            using (var clip = UniformRect(logoRect, 43))
                DrawImageCover(g, logo, logoRect, clip);
            using (var pRing = new Pen(Honey, 3f)) g.DrawEllipse(pRing, logoRect);

            using (var fEye = new Font("Segoe UI", 12f, FontStyle.Bold, GraphicsUnit.Point))
            using (var fBrand = new Font("Georgia", 23f, FontStyle.Bold, GraphicsUnit.Point))
            using (var bOr = new SolidBrush(Orange))
            using (var bCh = new SolidBrush(Chocolate))
            {
                g.DrawString("CONFEITARIA ARTESANAL", fEye, bOr, 418, 66);
                g.DrawString("Sonho de Pudim", fBrand, bCh, 416, 94);
            }

            // Title
            DrawCenteredMixedLine(g, W / 2f, 168, "Um sonho em", "", 54f, Chocolate, Orange);
            DrawCenteredMixedLine(g, W / 2f, 252, "forma de ", "pudim.", 54f, Chocolate, Orange);
            DrawCenteredParagraph(g, "Textura super lisinha, sem furinhos e com muita calda de caramelo brilhante.", new RectangleF(140, 354, 800, 80), "Segoe UI", 19.5f, FontStyle.Regular, Muted);

            // Arch photo centerpiece
            var outerArch = new RectangleF(210, 445, 660, 575);
            DrawSoftShadow(g, outerArch, 330, 330, 46, 46, 22, 36, 85);
            using (var pOuter = RoundedRect(outerArch, 330, 330, 46, 46))
            using (var brFrame = new LinearGradientBrush(outerArch, Honey, Caramel, 65f))
                g.FillPath(brFrame, pOuter);

            var innerArch = new RectangleF(224, 459, 632, 547);
            using (var pInner = RoundedRect(innerArch, 316, 316, 36, 36))
                DrawImageCover(g, pudim, innerArch, pInner);

            DrawArtisanalSeal(g, 855, 515, 92, "100% Caseiro", "Feito com", "Carinho");

            // 3 Flavor Pills
            DrawPillRow(
                g, W / 2f, 1098,
                new[] { "Tradicional", "Zero Lactose", "Sabor Café" },
                new[] { WarmWhite, Milk, Chocolate },
                new[] { Honey, Orange, Chocolate },
                new[] { Chocolate, CaramelDeep, Cream },
                17.5f, 30f, 12f, 20f
            );

            DrawFooter(g, W, 1215, "@sonhodepudim_", "Centro • Horizonte - CE", "iFood & WhatsApp", Color.FromArgb(65, Caramel), CaramelDeep, IFoodRed, Color.White);
            bmp.Save(Path.Combine(outDir, "01-vitrine-sonho-de-pudim.png"), ImageFormat.Png);
        }
    }

    // 02: NOSSOS TAMANHOS (120ml & 500ml)
    private void Generate02Tamanhos()
    {
        int W = 1080, H = 1350;
        using (var bmp = new Bitmap(W, H))
        using (var g = InitCanvas(bmp))
        using (var pInd = LoadAsset("pudim-individual.png"))
        using (var pFam = LoadAsset("pudim-familia.png"))
        {
            using (var bg = new LinearGradientBrush(new Rectangle(0, 0, W, H), Caramel, ColorTranslator.FromHtml("#592207"), 95f))
                g.FillRectangle(bg, 0, 0, W, H);
            DrawRadialGlow(g, W / 2f, 300, 520, Honey, 70);
            DrawSparkle(g, 110, 140, 22, GoldLight);
            DrawSparkle(g, 970, 220, 24, GoldLight);

            DrawEyebrowUpper(g, "Nossos Tamanhos Artesanais", W / 2f, 62, 13.5f, GoldLight);
            DrawCenteredMixedLine(g, W / 2f, 108, "Dois tamanhos,", "", 50f, Cream, GoldLight);
            DrawCenteredMixedLine(g, W / 2f, 188, "o mesmo ", "encanto.", 50f, Cream, GoldLight);

            // Left Card: 120ml
            var c1 = new RectangleF(64, 305, 458, 765);
            DrawSoftShadow(g, c1, 38, 38, 38, 38, 22, 34, 95);
            using (var p1 = UniformRect(c1, 38))
            using (var b1 = new SolidBrush(WarmWhite))
                g.FillPath(b1, p1);

            var img1Rect = new RectangleF(64, 305, 458, 390);
            using (var pImg1 = RoundedRect(img1Rect, 38, 38, 0, 0))
            {
                DrawImageCover(g, pInd, img1Rect, pImg1);
            }
            DrawPill(g, "120 ml • 1 pessoa", 215, 355, Chocolate, Color.Transparent, Cream, 13.5f, 20f, 8f, true);

            using (var fCardTitle = new Font("Georgia", 26f, FontStyle.Bold, GraphicsUnit.Point))
            using (var bChoc = new SolidBrush(Chocolate))
                g.DrawString("Pudim Individual", fCardTitle, bChoc, 96, 724);

            using (var fBody = new Font("Segoe UI", 16.5f, FontStyle.Regular, GraphicsUnit.Point))
            using (var bMut = new SolidBrush(Muted))
                g.DrawString("Na medida certa para matar a vontade de um doce artesanal e cremoso a qualquer hora.", fBody, bMut, new RectangleF(96, 784, 394, 160));

            DrawPill(g, "Porção Individual • 120 ml", 293, 995, Milk, Honey, CaramelDeep, 15f, 26f, 10f, false);

            // Right Card: 500ml
            var c2 = new RectangleF(558, 305, 458, 765);
            DrawSoftShadow(g, c2, 38, 38, 38, 38, 22, 34, 95);
            using (var p2 = UniformRect(c2, 38))
            using (var b2 = new SolidBrush(WarmWhite))
                g.FillPath(b2, p2);

            var img2Rect = new RectangleF(558, 305, 458, 390);
            using (var pImg2 = RoundedRect(img2Rect, 38, 38, 0, 0))
            using (var bGold = new LinearGradientBrush(img2Rect, ColorTranslator.FromHtml("#fff3cf"), ColorTranslator.FromHtml("#e2a544"), 90f))
            {
                g.FillPath(bGold, pImg2);
            }
            DrawImageContain(g, pFam, new RectangleF(582, 340, 410, 335), true);
            DrawPill(g, "500 ml • Até 8 fatias", 726, 355, Caramel, Color.Transparent, Cream, 13.5f, 20f, 8f, true);

            using (var fCardTitle = new Font("Georgia", 26f, FontStyle.Bold, GraphicsUnit.Point))
            using (var bChoc = new SolidBrush(Chocolate))
                g.DrawString("Tamanho Família", fCardTitle, bChoc, 590, 724);

            using (var fBody = new Font("Segoe UI", 16.5f, FontStyle.Regular, GraphicsUnit.Point))
            using (var bMut = new SolidBrush(Muted))
                g.DrawString("Perfeito para compartilhar no almoço em família, visitas especiais e comemorações.", fBody, bMut, new RectangleF(590, 784, 394, 160));

            DrawPill(g, "Para Compartilhar • 500 ml", 787, 995, Honey, Color.Transparent, Chocolate, 15f, 26f, 10f, false);

            DrawCenteredParagraph(g, "Versões disponíveis:  Tradicional   •   Zero Lactose   •   Sabor Café", new RectangleF(64, 1118, 952, 50), "Segoe UI", 18.5f, FontStyle.Bold, GoldLight);

            DrawFooter(g, W, 1215, "@sonhodepudim_", "(85) 99200-1749", "Pedir no iFood", Color.FromArgb(65, Cream), Cream, IFoodRed, Color.White);
            bmp.Save(Path.Combine(outDir, "02-tamanhos-120ml-e-500ml.png"), ImageFormat.Png);
        }
    }

    // 03: SABOR CAFÉ ESPECIAL
    private void Generate03SaborCafe()
    {
        int W = 1080, H = 1350;
        using (var bmp = new Bitmap(W, H))
        using (var g = InitCanvas(bmp))
        using (var pCafe = LoadAsset("pudim-cafe.png"))
        {
            using (var bg = new LinearGradientBrush(new Rectangle(0, 0, W, H), Chocolate, Espresso, 90f))
                g.FillRectangle(bg, 0, 0, W, H);
            DrawRadialGlow(g, W / 2f, 680, 540, Caramel, 110);
            DrawSparkle(g, 130, 380, 24, Honey);
            DrawSparkle(g, 945, 430, 20, GoldLight);

            DrawPill(g, "SABOR ESPECIAL ARTESANAL", W / 2f, 84, Color.FromArgb(45, Honey), Honey, GoldLight, 13.5f, 28f, 9f, false);

            DrawCenteredMixedLine(g, W / 2f, 142, "O encontro perfeito:", "", 48f, Cream, Honey);
            DrawCenteredMixedLine(g, W / 2f, 222, "", "pudim + café.", 54f, Cream, Honey);
            DrawCenteredParagraph(g, "Textura ultra cremosa com o toque marcante do café e calda intensa de caramelo.", new RectangleF(130, 322, 820, 80), "Segoe UI", 19f, FontStyle.Regular, ColorTranslator.FromHtml("#e6cdb7"));

            var frameOuter = new RectangleF(175, 420, 730, 610);
            DrawSoftShadow(g, frameOuter, 48, 48, 48, 48, 26, 40, 120);
            using (var pOut = UniformRect(frameOuter, 48))
            using (var bGold = new LinearGradientBrush(frameOuter, GoldLight, Caramel, 60f))
                g.FillPath(bGold, pOut);

            var frameInner = new RectangleF(187, 432, 706, 586);
            using (var pIn = UniformRect(frameInner, 38))
                DrawImageCover(g, pCafe, frameInner, pIn);

            DrawPill(g, "Disponível em 120 ml e 500 ml", W / 2f, 1030, WarmWhite, Honey, Chocolate, 17.5f, 36f, 12f, true);

            DrawPillRow(
                g, W / 2f, 1134,
                new[] { "Sem Furinhos", "Aroma Marcante", "Calda Generosa" },
                new[] { Color.FromArgb(40, Cream), Color.FromArgb(40, Cream), Color.FromArgb(40, Cream) },
                new[] { Color.FromArgb(90, Honey), Color.FromArgb(90, Honey), Color.FromArgb(90, Honey) },
                new[] { GoldLight, GoldLight, GoldLight },
                14.5f, 22f, 9f, 16f
            );

            DrawFooter(g, W, 1215, "Sonho de Pudim", "@sonhodepudim_", "iFood & WhatsApp", Color.FromArgb(55, Cream), Milk, Honey, Chocolate);
            bmp.Save(Path.Combine(outDir, "03-sabor-cafe-especial.png"), ImageFormat.Png);
        }
    }

    // 04: OPÇÃO ZERO LACTOSE
    private void Generate04ZeroLactose()
    {
        int W = 1080, H = 1350;
        using (var bmp = new Bitmap(W, H))
        using (var g = InitCanvas(bmp))
        using (var pudim = LoadAsset("pudim-hd.png"))
        using (var mascote = LoadAsset("mascote-padrao.png"))
        {
            using (var bg = new LinearGradientBrush(new Rectangle(0, 0, W, H), WarmWhite, ColorTranslator.FromHtml("#f2dfB3"), 110f))
                g.FillRectangle(bg, 0, 0, W, H);
            DrawRadialGlow(g, 330, 740, 420, Honey, 75);

            DrawPill(g, "LEVEZA SEM ABRIR MÃO DO SABOR", W / 2f, 82, SageBg, SageBorder, SageText, 13.5f, 28f, 9f, false);

            DrawCenteredMixedLine(g, W / 2f, 138, "Zero Lactose.", "", 52f, Chocolate, Caramel);
            DrawCenteredMixedLine(g, W / 2f, 220, "", "100% cremoso.", 52f, Chocolate, Caramel);
            DrawCenteredParagraph(g, "Toda a suavidade e a calda brilhante da nossa receita artesanal para você aproveitar sem restrições.", new RectangleF(120, 318, 840, 82), "Segoe UI", 18.5f, FontStyle.Regular, Muted);

            // Left Photo
            var photoCard = new RectangleF(64, 425, 515, 650);
            DrawSoftShadow(g, photoCard, 40, 40, 40, 40, 20, 32, 75);
            using (var pOut = UniformRect(photoCard, 40))
            using (var bW = new SolidBrush(Color.White))
                g.FillPath(bW, pOut);

            var photoIn = new RectangleF(76, 437, 491, 626);
            using (var pIn = UniformRect(photoIn, 32))
                DrawImageCover(g, pudim, photoIn, pIn);

            DrawPill(g, "ZERO LACTOSE", 215, 488, SageBg, SageBorder, SageText, 14f, 22f, 8f, true);

            // Right Benefit Cards + Mascot
            Action<float, string, string> drawBenefit = (yPos, title, desc) =>
            {
                var r = new RectangleF(608, yPos, 408, 190);
                DrawSoftShadow(g, r, 28, 28, 28, 28, 10, 20, 45);
                using (var p = UniformRect(r, 28))
                {
                    using (var b = new SolidBrush(Color.White)) g.FillPath(b, p);
                    using (var pen = new Pen(ColorTranslator.FromHtml("#e2c99d"), 2f)) g.DrawPath(pen, p);
                }
                using (var fT = new Font("Segoe UI", 15f, FontStyle.Bold, GraphicsUnit.Point))
                using (var fD = new Font("Segoe UI", 15.5f, FontStyle.Regular, GraphicsUnit.Point))
                using (var bOr = new SolidBrush(Orange))
                using (var bIn = new SolidBrush(Ink))
                {
                    g.DrawString(title, fT, bOr, 638, yPos + 28);
                    g.DrawString(desc, fD, bIn, new RectangleF(638, yPos + 68, 350, 105));
                }
            };

            drawBenefit(425, "✓  TEXTURA IMPECÁVEL", "Super lisinho, sem furinhos e com a mesma cremosidade do tradicional.");
            drawBenefit(638, "✓  DOIS TAMANHOS", "Disponível no pote Individual (120 ml) e Tamanho Família (500 ml).");

            DrawImageContain(g, mascote, new RectangleF(685, 845, 260, 245), true);

            DrawFooter(g, W, 1215, "@sonhodepudim_", "Horizonte - CE", "(85) 99200-1749", Color.FromArgb(60, Caramel), CaramelDeep, Caramel, Cream);
            bmp.Save(Path.Combine(outDir, "04-pudim-zero-lactose.png"), ImageFormat.Png);
        }
    }

    // 05: DELIVERY IFOOD
    private void Generate05DeliveryIfood()
    {
        int W = 1080, H = 1350;
        using (var bmp = new Bitmap(W, H))
        using (var g = InitCanvas(bmp))
        using (var mDeliv = LoadAsset("mascote-delivery.png"))
        using (var pInd = LoadAsset("pudim-individual.png"))
        {
            using (var bg = new LinearGradientBrush(new Rectangle(0, 0, W, H), ColorTranslator.FromHtml("#fff6dc"), ColorTranslator.FromHtml("#e59f38"), 95f))
                g.FillRectangle(bg, 0, 0, W, H);
            DrawRadialGlow(g, W / 2f, 690, 440, Color.White, 165);
            DrawSparkle(g, 135, 480, 26, Caramel);
            DrawSparkle(g, 920, 460, 22, IFoodRed);

            DrawPill(g, "DELIVERY RÁPIDO  •  PEÇA PELO IFOOD", W / 2f, 84, IFoodRed, Color.Transparent, Color.White, 15f, 32f, 11f, true);

            DrawCenteredMixedLine(g, W / 2f, 145, "Bateu a vontade", "", 52f, Chocolate, IFoodRed);
            DrawCenteredMixedLine(g, W / 2f, 228, "de ", "doce agora?", 52f, Chocolate, IFoodRed);
            DrawCenteredParagraph(g, "O seu Sonho de Pudim chega fresquinho, bem embalado e pronto para adoçar o seu dia!", new RectangleF(130, 326, 820, 80), "Segoe UI", 19.5f, FontStyle.Regular, ColorTranslator.FromHtml("#5a3219"));

            // Mascot scooter hero
            DrawImageContain(g, mDeliv, new RectangleF(165, 395, 650, 620), true);

            // Real product circular inset badge
            var inset = new RectangleF(725, 735, 250, 250);
            DrawSoftShadow(g, inset, 125, 125, 125, 125, 16, 28, 85);
            using (var bW = new SolidBrush(WarmWhite)) g.FillEllipse(bW, inset);
            var insetIn = new RectangleF(737, 747, 226, 226);
            using (var pIn = UniformRect(insetIn, 113))
                DrawImageCover(g, pInd, insetIn, pIn);
            DrawPill(g, "120 ml & 500 ml", 850, 985, Caramel, Color.Transparent, Cream, 13f, 20f, 7f, true);

            // Big CTA Banner
            DrawPill(g, "Busque por  “Sonho de Pudim”  no iFood!", W / 2f, 1105, Chocolate, Honey, Cream, 19.5f, 44f, 16f, true);

            DrawFooter(g, W, 1215, "@sonhodepudim_", "Centro • Horizonte - CE", "Pedir no iFood", Color.FromArgb(65, Chocolate), Chocolate, IFoodRed, Color.White);
            bmp.Save(Path.Combine(outDir, "05-delivery-ifood.png"), ImageFormat.Png);
        }
    }

    // 06: ENCOMENDAS PARA EVENTOS
    private void Generate06Eventos()
    {
        int W = 1080, H = 1350;
        using (var bmp = new Bitmap(W, H))
        using (var g = InitCanvas(bmp))
        using (var mEventos = LoadAsset("mascote-eventos.png"))
        using (var pudim = LoadAsset("pudim-hd.png"))
        {
            using (var bg = new LinearGradientBrush(new Rectangle(0, 0, W, H), WarmWhite, ColorTranslator.FromHtml("#ebc070"), 100f))
                g.FillRectangle(bg, 0, 0, W, H);
            DrawRadialGlow(g, W / 2f, 690, 460, Color.White, 150);
            DrawCaramelTopDrips(g, W, Honey);

            DrawEyebrowUpper(g, "Aniversários • Casamentos • Celebrações", W / 2f, 68, 13f, Caramel);
            DrawCenteredMixedLine(g, W / 2f, 114, "Vamos adoçar", "", 52f, Chocolate, Orange);
            DrawCenteredMixedLine(g, W / 2f, 198, "o seu ", "evento!", 52f, Chocolate, Orange);
            DrawCenteredParagraph(g, "Encante seus convidados com pudins artesanais impecáveis na apresentação e no sabor.", new RectangleF(130, 298, 820, 80), "Segoe UI", 19f, FontStyle.Regular, Muted);

            // Left: Festive Mascot | Right: Arch Photo of Pudim
            DrawImageContain(g, mEventos, new RectangleF(65, 395, 535, 565), true);

            var archOut = new RectangleF(595, 415, 410, 520);
            DrawSoftShadow(g, archOut, 205, 205, 36, 36, 18, 30, 80);
            using (var pOut = RoundedRect(archOut, 205, 205, 36, 36))
            using (var bFr = new SolidBrush(WarmWhite))
                g.FillPath(bFr, pOut);

            var archIn = new RectangleF(609, 429, 382, 492);
            using (var pIn = RoundedRect(archIn, 191, 191, 28, 28))
                DrawImageCover(g, pudim, archIn, pIn);

            DrawPillRow(
                g, W / 2f, 1025,
                new[] { "Potes Individuais 120 ml", "Tamanho Família 500 ml" },
                new[] { WarmWhite, Caramel },
                new[] { Honey, Caramel },
                new[] { Chocolate, Cream },
                16.5f, 30f, 11f, 22f
            );

            DrawCenteredParagraph(g, "Solicite seu orçamento personalizado com antecedência pelo WhatsApp!", new RectangleF(100, 1108, 880, 50), "Segoe UI", 17.5f, FontStyle.Bold, Chocolate);

            DrawFooter(g, W, 1215, "@sonhodepudim_", "Horizonte - CE", "(85) 99200-1749", Color.FromArgb(65, Caramel), Chocolate, Chocolate, Cream);
            bmp.Save(Path.Combine(outDir, "06-encomendas-eventos.png"), ImageFormat.Png);
        }
    }

    // 07: PARCERIA & REVENDA
    private void Generate07Revenda()
    {
        int W = 1080, H = 1350;
        using (var bmp = new Bitmap(W, H))
        using (var g = InitCanvas(bmp))
        using (var mRev = LoadAsset("mascote-revenda.png"))
        {
            using (var bg = new LinearGradientBrush(new Rectangle(0, 0, W, H), Chocolate, CaramelDeep, 115f))
                g.FillRectangle(bg, 0, 0, W, H);
            DrawRadialGlow(g, 300, 720, 420, Honey, 75);

            DrawPill(g, "SEJA UM ESTABELECIMENTO PARCEIRO", W / 2f, 80, Color.FromArgb(40, Honey), Honey, GoldLight, 13.5f, 28f, 9f, false);

            DrawCenteredMixedLine(g, W / 2f, 138, "Leve o Sonho de Pudim", "", 44f, Cream, Honey);
            DrawCenteredMixedLine(g, W / 2f, 212, "para o seu ", "negócio.", 48f, Cream, Honey);
            DrawCenteredParagraph(g, "A sobremesa artesanal perfeita para restaurantes, cafeterias, padarias, docerias e empórios.", new RectangleF(120, 308, 840, 80), "Segoe UI", 18.5f, FontStyle.Regular, ColorTranslator.FromHtml("#e7d0bc"));

            // Mascot on left
            DrawImageContain(g, mRev, new RectangleF(52, 415, 485, 595), true);

            // 3 B2B Value Cards on right
            Action<float, string, string> drawB2B = (yPos, title, desc) =>
            {
                var r = new RectangleF(552, yPos, 464, 178);
                using (var p = UniformRect(r, 26))
                {
                    using (var b = new SolidBrush(Color.FromArgb(32, 255, 247, 229))) g.FillPath(b, p);
                    using (var pen = new Pen(Color.FromArgb(110, Honey), 2f)) g.DrawPath(pen, p);
                }
                using (var fT = new Font("Segoe UI", 16f, FontStyle.Bold, GraphicsUnit.Point))
                using (var fD = new Font("Segoe UI", 15f, FontStyle.Regular, GraphicsUnit.Point))
                using (var bH = new SolidBrush(GoldLight))
                using (var bC = new SolidBrush(Cream))
                {
                    g.DrawString(title, fT, bH, 580, yPos + 24);
                    g.DrawString(desc, fD, bC, new RectangleF(580, yPos + 64, 412, 96));
                }
            };

            drawB2B(418, "✓  Padrão Artesanal Premium", "Embalagem individual lacrada, ótima apresentação e validade segura.");
            drawB2B(616, "✓  Sabores que Vendem Todo Dia", "Versões Tradicional, Zero Lactose e Café nos tamanhos 120 ml e 500 ml.");
            drawB2B(814, "✓  Parceria Comercial Direta", "Condições especiais para revenda e entregas combinadas na região.");

            DrawPill(g, "Fale conosco e solicite a tabela de revenda!", W / 2f, 1098, Honey, Color.Transparent, Chocolate, 18.5f, 42f, 14f, true);

            DrawFooter(g, W, 1215, "@sonhodepudim_", "Parcerias & Revenda", "(85) 99200-1749", Color.FromArgb(55, Cream), Cream, WarmWhite, Chocolate);
            bmp.Save(Path.Combine(outDir, "07-parceria-e-revenda.png"), ImageFormat.Png);
        }
    }

    // 08: PARA PRESENTEAR
    private void Generate08Presentear()
    {
        int W = 1080, H = 1350;
        using (var bmp = new Bitmap(W, H))
        using (var g = InitCanvas(bmp))
        using (var pFam = LoadAsset("pudim-familia.png"))
        using (var mascote = LoadAsset("mascote-padrao.png"))
        {
            using (var bg = new LinearGradientBrush(new Rectangle(0, 0, W, H), Honey, Caramel, 105f))
                g.FillRectangle(bg, 0, 0, W, H);
            DrawRadialGlow(g, W / 2f, 650, 500, Cream, 85);
            DrawHeart(g, 135, 430, 28, WarmWhite);
            DrawHeart(g, 945, 490, 24, WarmWhite);
            DrawSparkle(g, 160, 860, 24, WarmWhite);

            DrawPill(g, "GESTO DOCE E CHEIO DE AFETO", W / 2f, 82, WarmWhite, Color.Transparent, Caramel, 13.5f, 28f, 9f, true);

            DrawCenteredMixedLine(g, W / 2f, 140, "Um presente que", "", 51f, WarmWhite, Chocolate);
            DrawCenteredMixedLine(g, W / 2f, 222, "vira ", "memória boa.", 51f, WarmWhite, Chocolate);
            DrawCenteredParagraph(g, "Delicado, artesanal e irresistível — a escolha certa para surpreender quem você ama.", new RectangleF(130, 320, 820, 80), "Segoe UI", 19f, FontStyle.Regular, WarmWhite);

            // Large Cream Arch Stage
            var arch = new RectangleF(180, 418, 720, 600);
            DrawSoftShadow(g, arch, 360, 360, 46, 46, 24, 38, 90);
            using (var pA = RoundedRect(arch, 360, 360, 46, 46))
            using (var bA = new SolidBrush(WarmWhite))
                g.FillPath(bA, pA);

            DrawImageContain(g, pFam, new RectangleF(240, 485, 560, 470), true);
            DrawArtisanalSeal(g, 235, 495, 86, "Presenteie", "Feito com", "Afeto");
            DrawImageContain(g, mascote, new RectangleF(680, 790, 265, 245), true);

            DrawCenteredParagraph(g, "Embalagem caprichada  •  Individual (120 ml) ou Família (500 ml)", new RectangleF(80, 1088, 920, 50), "Segoe UI", 18.5f, FontStyle.Bold, WarmWhite);

            DrawFooter(g, W, 1215, "Sonho de Pudim", "@sonhodepudim_", "(85) 99200-1749", Color.FromArgb(80, WarmWhite), WarmWhite, WarmWhite, Chocolate);
            bmp.Save(Path.Combine(outDir, "08-para-presentear.png"), ImageFormat.Png);
        }
    }

    // 09: STORY CARDÁPIO DIGITAL (1080x1920)
    private void Generate09StoryCardapio()
    {
        int W = 1080, H = 1920;
        using (var bmp = new Bitmap(W, H))
        using (var g = InitCanvas(bmp))
        using (var logo = LoadAsset("logo.png"))
        using (var pInd = LoadAsset("pudim-individual.png"))
        using (var pFam = LoadAsset("pudim-familia.png"))
        {
            using (var bg = new LinearGradientBrush(new Rectangle(0, 0, W, H), Cream, ColorTranslator.FromHtml("#efd49a"), 95f))
                g.FillRectangle(bg, 0, 0, W, H);
            DrawCaramelTopDrips(g, W, Caramel);

            // Top Logo
            var logoRect = new RectangleF(W / 2f - 82, 96, 164, 164);
            DrawSoftShadow(g, logoRect, 82, 82, 82, 82, 12, 22, 65);
            using (var clip = UniformRect(logoRect, 82))
                DrawImageCover(g, logo, logoRect, clip);

            DrawEyebrowUpper(g, "Confeitaria Artesanal • Horizonte - CE", W / 2f, 286, 13f, Orange);
            DrawCenteredMixedLine(g, W / 2f, 326, "Nosso ", "Cardápio", 56f, Chocolate, Caramel);

            // Card 1: 120ml
            var c1 = new RectangleF(72, 460, 936, 365);
            DrawSoftShadow(g, c1, 42, 42, 42, 42, 18, 30, 65);
            using (var p1 = UniformRect(c1, 42))
            {
                using (var b = new SolidBrush(WarmWhite)) g.FillPath(b, p1);
                using (var pen = new Pen(ColorTranslator.FromHtml("#dfc496"), 3f)) g.DrawPath(pen, p1);
            }
            var c1Img = new RectangleF(108, 496, 292, 292);
            using (var pC1 = UniformRect(c1Img, 30))
                DrawImageCover(g, pInd, c1Img, pC1);

            using (var fEye = new Font("Segoe UI", 14f, FontStyle.Bold, GraphicsUnit.Point))
            using (var fTit = new Font("Georgia", 31f, FontStyle.Bold, GraphicsUnit.Point))
            using (var fSub = new Font("Segoe UI", 17f, FontStyle.Regular, GraphicsUnit.Point))
            using (var bOr = new SolidBrush(Orange))
            using (var bCh = new SolidBrush(Chocolate))
            using (var bMu = new SolidBrush(Muted))
            {
                g.DrawString("PORÇÃO INDIVIDUAL  •  120 ML", fEye, bOr, 434, 512);
                g.DrawString("Pudim Individual", fTit, bCh, 432, 550);
                g.DrawString("Para 1 pessoa • Textura cremosa e muita calda", fSub, bMu, new RectangleF(434, 622, 540, 80));
            }
            DrawPill(g, "Peça já • 120 ml", 575, 742, Caramel, Color.Transparent, Cream, 16.5f, 34f, 10f, false);

            // Card 2: 500ml
            var c2 = new RectangleF(72, 860, 936, 365);
            DrawSoftShadow(g, c2, 42, 42, 42, 42, 18, 30, 65);
            using (var p2 = UniformRect(c2, 42))
            {
                using (var b = new SolidBrush(WarmWhite)) g.FillPath(b, p2);
                using (var pen = new Pen(ColorTranslator.FromHtml("#dfc496"), 3f)) g.DrawPath(pen, p2);
            }
            var c2Img = new RectangleF(108, 896, 292, 292);
            using (var pC2 = UniformRect(c2Img, 30))
            using (var bTile = new SolidBrush(Milk))
                g.FillPath(bTile, pC2);
            DrawImageContain(g, pFam, new RectangleF(122, 916, 264, 252), true);

            using (var fEye = new Font("Segoe UI", 14f, FontStyle.Bold, GraphicsUnit.Point))
            using (var fTit = new Font("Georgia", 31f, FontStyle.Bold, GraphicsUnit.Point))
            using (var fSub = new Font("Segoe UI", 17f, FontStyle.Regular, GraphicsUnit.Point))
            using (var bOr = new SolidBrush(Orange))
            using (var bCh = new SolidBrush(Chocolate))
            using (var bMu = new SolidBrush(Muted))
            {
                g.DrawString("PARA COMPARTILHAR  •  500 ML", fEye, bOr, 434, 912);
                g.DrawString("Tamanho Família", fTit, bCh, 432, 950);
                g.DrawString("Serve até 8 fatias • Ideal para família e festas", fSub, bMu, new RectangleF(434, 1022, 540, 80));
            }
            DrawPill(g, "Peça já • 500 ml", 575, 1142, Chocolate, Color.Transparent, Cream, 16.5f, 34f, 10f, false);

            // Sabores Box
            var c3 = new RectangleF(72, 1265, 936, 330);
            DrawSoftShadow(g, c3, 42, 42, 42, 42, 18, 30, 75);
            using (var p3 = UniformRect(c3, 42))
            using (var b3 = new SolidBrush(Chocolate))
                g.FillPath(b3, p3);

            DrawEyebrowUpper(g, "Escolha a sua versão favorita", W / 2f, 1312, 14f, GoldLight);
            DrawPillRow(
                g, W / 2f, 1418,
                new[] { "Tradicional", "Zero Lactose", "Sabor Café" },
                new[] { WarmWhite, Honey, Caramel },
                new[] { Color.Transparent, Color.Transparent, GoldLight },
                new[] { Chocolate, Chocolate, Cream },
                18f, 32f, 13f, 22f
            );
            DrawCenteredParagraph(g, "Receita 100% artesanal • Sem furinhos • Calda generosa de caramelo", new RectangleF(110, 1510, 860, 55), "Segoe UI", 16f, FontStyle.Regular, Milk);

            // Bottom Story CTA
            DrawPillRow(
                g, W / 2f, 1685,
                new[] { "WhatsApp (85) 99200-1749", "Delivery no iFood" },
                new[] { WarmWhite, IFoodRed },
                new[] { Caramel, IFoodRed },
                new[] { Chocolate, Color.White },
                17f, 30f, 12f, 22f
            );

            DrawFooter(g, W, 1775, "@sonhodepudim_", "Centro • Horizonte - CE", "Encomendas", Color.FromArgb(65, Caramel), CaramelDeep, Caramel, Cream);
            bmp.Save(Path.Combine(outDir, "09-story-cardapio-sonho-de-pudim.png"), ImageFormat.Png);
        }
    }

    // 10: STORY HORÁRIOS DA LOJA (1080x1920)
    private void Generate10StoryHorarios()
    {
        int W = 1080, H = 1920;
        using (var bmp = new Bitmap(W, H))
        using (var g = InitCanvas(bmp))
        using (var logoVazada = LoadAsset("logo-vazada.png"))
        using (var mascote = LoadAsset("mascote-padrao.png"))
        {
            using (var bg = new LinearGradientBrush(new Rectangle(0, 0, W, H), Caramel, Chocolate, 95f))
                g.FillRectangle(bg, 0, 0, W, H);
            DrawRadialGlow(g, W / 2f, 380, 480, Honey, 75);
            DrawCaramelTopDrips(g, W, CaramelDeep);

            DrawImageContain(g, logoVazada, new RectangleF(W / 2f - 140, 82, 280, 260), false);
            DrawEyebrowUpper(g, "Centro • Horizonte - CE", W / 2f, 355, 13.5f, GoldLight);
            DrawCenteredMixedLine(g, W / 2f, 398, "Horários da ", "loja.", 52f, Cream, GoldLight);

            // Schedule White Card
            var card = new RectangleF(76, 525, 928, 760);
            DrawSoftShadow(g, card, 44, 44, 44, 44, 24, 38, 105);
            using (var pCard = UniformRect(card, 44))
            using (var bCard = new SolidBrush(WarmWhite))
                g.FillPath(bCard, pCard);

            string[,] rows = new string[,] {
                { "Segunda-feira", "08:00 às 17:00" },
                { "Terça-feira",   "08:00 às 17:00" },
                { "Quarta-feira",  "11:00 às 15:00" },
                { "Quinta-feira",  "08:00 às 17:00" },
                { "Sexta-feira",   "08:00 às 17:00" },
                { "Sábado",        "08:00 às 17:00" },
                { "Domingo",       "09:00 às 14:00" }
            };

            float startY = 568f;
            float rowH = 98f;
            for (int i = 0; i < 7; i++)
            {
                float ry = startY + i * rowH;
                if (i < 6)
                {
                    using (var pen = new Pen(Color.FromArgb(55, Caramel), 2f) { DashStyle = DashStyle.Dash })
                        g.DrawLine(pen, 130, ry + rowH, 948, ry + rowH);
                }
                using (var fDay = new Font("Segoe UI", 20f, FontStyle.Bold, GraphicsUnit.Point))
                using (var fTime = new Font("Segoe UI", 20f, FontStyle.Bold, GraphicsUnit.Point))
                using (var bDay = new SolidBrush(Chocolate))
                using (var bTime = new SolidBrush(Caramel))
                using (var sfR = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center })
                using (var sfL = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center })
                {
                    g.DrawString(rows[i, 0], fDay, bDay, new RectangleF(132, ry, 420, rowH), sfL);
                    g.DrawString(rows[i, 1], fTime, bTime, new RectangleF(540, ry, 408, rowH), sfR);
                }
            }

            // Bottom Mascot + Channels
            DrawImageContain(g, mascote, new RectangleF(90, 1345, 340, 320), true);

            DrawPill(g, "Delivery rápido no iFood", 695, 1435, IFoodRed, Color.Transparent, Color.White, 18.5f, 40f, 15f, true);
            DrawPill(g, "WhatsApp (85) 99200-1749", 695, 1560, WarmWhite, Honey, Chocolate, 18.5f, 36f, 15f, true);

            DrawFooter(g, W, 1765, "@sonhodepudim_", "Delivery • Eventos • Revenda", "Horizonte - CE", Color.FromArgb(65, Cream), Cream, Honey, Chocolate);
            bmp.Save(Path.Combine(outDir, "10-story-horarios-sonho-de-pudim.png"), ImageFormat.Png);
        }
    }
}
