using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;

if (args.Length < 2)
{
    Console.Error.WriteLine("Uso: dotnet run -- <entrada.md> <salida.pdf>");
    return 1;
}

var inputPath = args[0];
var outputPath = args[1];

if (!File.Exists(inputPath))
{
    Console.Error.WriteLine($"No se encontró el archivo: {inputPath}");
    return 1;
}

var lines = File.ReadAllLines(inputPath);
var blocks = MarkdownParser.Parse(lines);

const string azul = "#1D4ED8";
const string rojo = "#DC2626";
const string gris = "#4B5563";

Document.Create(doc =>
{
    doc.Page(page =>
    {
        page.Margin(40);
        page.Size(PageSizes.A4);
        page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Calibri).LineHeight(1.35f));

        page.Header().Column(headerCol =>
        {
            headerCol.Item().Text("Sistema de Gestión de Torneos Deportivos").FontSize(16).Bold().FontColor(rojo);
            headerCol.Item().PaddingTop(4).LineHorizontal(1).LineColor(azul);
        });

        page.Content().PaddingTop(12).Column(col =>
        {
            col.Spacing(6);

            var i = 0;
            while (i < blocks.Count)
            {
                var block = blocks[i];

                switch (block.Type)
                {
                    case BlockType.Heading1:
                        col.Item().PaddingTop(10).Text(block.Text).FontSize(18).Bold().FontColor(rojo);
                        break;

                    case BlockType.Heading2:
                        col.Item().PaddingTop(8).Text(block.Text).FontSize(14).Bold().FontColor(azul);
                        break;

                    case BlockType.Heading3:
                        col.Item().PaddingTop(4).Text(block.Text).FontSize(11).Bold();
                        break;

                    case BlockType.Bullet:
                        col.Item().Row(row =>
                        {
                            row.ConstantItem(14).Text("•");
                            row.RelativeItem().Text(t => RenderInline(t, block.Text));
                        });
                        break;

                    case BlockType.Quote:
                        col.Item().BorderLeft(2).BorderColor(azul).PaddingLeft(8)
                            .Text(block.Text).Italic().FontColor(gris);
                        break;

                    case BlockType.CodeLine:
                        col.Item().Background("#F3F4F6").Padding(2)
                            .Text(block.Text.Length == 0 ? " " : block.Text)
                            .FontFamily(Fonts.Consolas).FontSize(8.5f);
                        break;

                    case BlockType.HorizontalRule:
                        col.Item().PaddingVertical(4).LineHorizontal(0.5f).LineColor("#D1D5DB");
                        break;

                    case BlockType.TableRow:
                        var tableRows = new List<Block>();
                        while (i < blocks.Count && blocks[i].Type == BlockType.TableRow)
                        {
                            tableRows.Add(blocks[i]);
                            i++;
                        }
                        i--;
                        RenderTable(col, tableRows, azul);
                        break;

                    case BlockType.Paragraph:
                        col.Item().Text(t => RenderInline(t, block.Text));
                        break;
                }

                i++;
            }
        });

        page.Footer().AlignCenter().Text(t =>
        {
            t.Span("Documento vivo — actualizar con cada /planificar-sprint. Generado: ").FontColor(gris).FontSize(8);
            t.Span(DateTime.Now.ToString("yyyy-MM-dd HH:mm")).FontColor(gris).FontSize(8);
        });
    });
})
.GeneratePdf(outputPath);

Console.WriteLine($"PDF generado en: {Path.GetFullPath(outputPath)}");
return 0;

static void RenderInline(QuestPDF.Fluent.TextDescriptor t, string text)
{
    var parts = text.Split("**");
    for (var idx = 0; idx < parts.Length; idx++)
    {
        if (parts[idx].Length == 0) continue;
        var span = t.Span(parts[idx]);
        if (idx % 2 == 1) span.Bold();
    }
}

static void RenderTable(QuestPDF.Fluent.ColumnDescriptor col, List<Block> rows, string headerColor)
{
    var dataRows = rows.Where(r => !r.IsSeparator).ToList();
    if (dataRows.Count == 0) return;

    var columnCount = dataRows[0].Cells!.Count;

    col.Item().PaddingTop(4).Table(table =>
    {
        table.ColumnsDefinition(c =>
        {
            for (var k = 0; k < columnCount; k++) c.RelativeColumn();
        });

        for (var r = 0; r < dataRows.Count; r++)
        {
            var isHeader = r == 0;
            foreach (var cell in dataRows[r].Cells!)
            {
                var container = table.Cell().Border(0.5f).BorderColor("#D1D5DB").Padding(4);
                if (isHeader)
                {
                    container.Background(headerColor).Text(t =>
                    {
                        t.DefaultTextStyle(x => x.FontSize(9).FontColor("#FFFFFF").Bold());
                        RenderInline(t, cell);
                    });
                }
                else
                {
                    container.Text(t =>
                    {
                        t.DefaultTextStyle(x => x.FontSize(9));
                        RenderInline(t, cell);
                    });
                }
            }
        }
    });
}

enum BlockType { Heading1, Heading2, Heading3, Bullet, Quote, CodeLine, HorizontalRule, TableRow, Paragraph }

record Block(BlockType Type, string Text, List<string>? Cells = null, bool IsSeparator = false);

static class MarkdownParser
{
    public static List<Block> Parse(string[] lines)
    {
        var blocks = new List<Block>();
        var inCodeFence = false;

        foreach (var raw in lines)
        {
            var line = raw.TrimEnd();

            if (line.TrimStart().StartsWith("```"))
            {
                inCodeFence = !inCodeFence;
                continue;
            }

            if (inCodeFence)
            {
                blocks.Add(new Block(BlockType.CodeLine, line));
                continue;
            }

            var trimmed = line.Trim();

            if (trimmed.Length == 0) continue;

            if (IsHorizontalRule(trimmed))
            {
                blocks.Add(new Block(BlockType.HorizontalRule, ""));
                continue;
            }

            if (trimmed.StartsWith("# "))
            {
                blocks.Add(new Block(BlockType.Heading1, trimmed[2..].Trim()));
                continue;
            }

            if (trimmed.StartsWith("## "))
            {
                blocks.Add(new Block(BlockType.Heading2, trimmed[3..].Trim()));
                continue;
            }

            if (trimmed.StartsWith("### "))
            {
                blocks.Add(new Block(BlockType.Heading3, trimmed[4..].Trim()));
                continue;
            }

            if (trimmed.StartsWith("> "))
            {
                blocks.Add(new Block(BlockType.Quote, trimmed[2..].Trim()));
                continue;
            }

            if (trimmed.StartsWith("- ") || trimmed.StartsWith("* "))
            {
                blocks.Add(new Block(BlockType.Bullet, StripLeadingChecklist(trimmed[2..].Trim())));
                continue;
            }

            if (IsNumberedItem(trimmed, out var content))
            {
                blocks.Add(new Block(BlockType.Bullet, content));
                continue;
            }

            if (trimmed.StartsWith("|") && trimmed.EndsWith("|"))
            {
                var cells = trimmed.Trim('|').Split('|').Select(c => c.Trim()).ToList();
                var isSeparator = cells.All(c => c.Length > 0 && c.All(ch => ch is '-' or ':'));
                blocks.Add(new Block(BlockType.TableRow, "", cells, isSeparator));
                continue;
            }

            blocks.Add(new Block(BlockType.Paragraph, trimmed));
        }

        return blocks;
    }

    private static bool IsHorizontalRule(string trimmed) =>
        trimmed.Length >= 3 && (trimmed.All(c => c == '-') || trimmed.All(c => c == '_'));

    private static bool IsNumberedItem(string trimmed, out string content)
    {
        var dotIndex = trimmed.IndexOf(". ", StringComparison.Ordinal);
        if (dotIndex > 0 && dotIndex <= 3 && trimmed[..dotIndex].All(char.IsDigit))
        {
            content = trimmed[(dotIndex + 2)..].Trim();
            return true;
        }

        content = "";
        return false;
    }

    private static string StripLeadingChecklist(string text) =>
        text.StartsWith("[ ]") || text.StartsWith("[x]")
            ? text[3..].Trim()
            : text;
}
