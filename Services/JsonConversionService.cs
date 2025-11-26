using System.Text;
using System.Text.Json;

namespace Json2CsvConverter.Services;

public class JsonConversionService
{
    private const int MaxJsonLength = 500_000;
    private const int MaxRowsAllowed = 10_000;

    public (bool Success, string Csv, string Error) ConvertJsonToCsv(string jsonText)
    {
        if (string.IsNullOrWhiteSpace(jsonText))
            return (false, "", "Por favor, cole um JSON válido na caixa de texto!");

        if (jsonText.Length > MaxJsonLength)
            return (false, "", $"JSON muito grande! Limite de 500 KB ({MaxJsonLength:N0} caracteres).");

        try
        {
            using var doc = JsonDocument.Parse(jsonText);
            var root = doc.RootElement;

            if (root.ValueKind != JsonValueKind.Array)
                return (false, "", "O JSON deve ser um array de objetos []. Exemplo: [ { ... } ]");

            int rowCount = root.GetArrayLength();
            if (rowCount == 0)
                return (false, "", "Array vazio. Adicione pelo menos um objeto.");

            if (rowCount > MaxRowsAllowed)
                return (false, "", $"Máximo de {MaxRowsAllowed:N0} registros permitidos.");

            var rows = new List<Dictionary<string, string>>();
            List<string>? orderedHeaders = null; 

            foreach (var item in root.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object)
                    return (false, "", "Todos os itens devem ser objetos {}.");

                bool temValorReal = false;
                var row = new Dictionary<string, string>();

                foreach (var prop in item.EnumerateObject())
                {
                    var value = prop.Value;
      
                    if (value.ValueKind == JsonValueKind.Object || value.ValueKind == JsonValueKind.Array)
                        return (false, "", $"Campo aninhado não suportado: \"{prop.Name}\"");

                    string stringValue = value.ValueKind switch
                    {
                        JsonValueKind.Null => "",
                        JsonValueKind.True => "true",
                        JsonValueKind.False => "false",
                        JsonValueKind.Number => value.ToString(),
                        _ => value.GetString() ?? ""
                    };

                    row[prop.Name] = stringValue;

                    if (!string.IsNullOrWhiteSpace(stringValue) ||
                        value.ValueKind == JsonValueKind.True ||
                        value.ValueKind == JsonValueKind.False ||
                        value.ValueKind == JsonValueKind.Number)
                        temValorReal = true;
      
                    if (orderedHeaders == null)
                    {
                        orderedHeaders = new List<string>();
                    }
                    if (!orderedHeaders.Contains(prop.Name))
                    {
                        orderedHeaders.Add(prop.Name);
                    }
                }

                if (!temValorReal)
                    return (false, "", "Objeto vazio ou com todos os campos nulos/vazios não é permitido.");

                rows.Add(row);
            }
 
            orderedHeaders ??= new List<string>();

            var sb = new StringBuilder();

            sb.AppendLine(string.Join(";", orderedHeaders.Select(EscapeCsv)));

            foreach (var row in rows)
            {
                var values = orderedHeaders.Select(h => row.TryGetValue(h, out var val) ? EscapeCsv(val) : "");
                sb.AppendLine(string.Join(";", values));
            }

            return (true, sb.ToString(), "");
        }
        catch (JsonException ex)
        {
            string linha = ex.LineNumber.HasValue ? $"linha {ex.LineNumber.Value + 1}" : "desconhecida";
            return (false, "", $"JSON inválido na {linha}: {ex.Message}");
        }
        catch (Exception ex)
        {
            return (false, "", $"Erro inesperado: {ex.Message}");
        }
    }

    private static string EscapeCsv(string value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        bool precisaEscapar = value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r');
        return precisaEscapar ? $"\"{value.Replace("\"", "\"\"")}\"" : value;
    }
}