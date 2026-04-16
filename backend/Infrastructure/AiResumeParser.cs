using System.Text;
using System.Text.Json;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace Letiao.ATS.Api.Infrastructure;

public class AiResumeParser(HttpClient httpClient)
{
    private readonly string _apiKey = Environment.GetEnvironmentVariable("VOLCENGINE_API_KEY") 
                                      ?? throw new Exception("MISSING VOLCENGINE_API_KEY in .env");
    // 火山方舟需要填写「推理接入点 ID」(ep-xxx) 或带版本后缀的模型 ID
    private readonly string _modelId = Environment.GetEnvironmentVariable("VOLCENGINE_MODEL_ID") 
                                      ?? "doubao-seed-2-0-pro-260215";
    
    public async Task<string> ParsePdfAndExtractInfoAsync(Stream pdfStream)
    {
        // 1. Extract Text from PDF
        string pdfText;
        try
        {
            using var pdfReader = new PdfReader(pdfStream);
            using var pdfDoc = new PdfDocument(pdfReader);
            var textBuilder = new StringBuilder();
            
            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                var page = pdfDoc.GetPage(i);
                var strategy = new SimpleTextExtractionStrategy();
                string currentText = PdfTextExtractor.GetTextFromPage(page, strategy);
                textBuilder.Append(currentText);
            }
            
            pdfText = textBuilder.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception($"PDF Parse Error: {ex.Message}");
        }

        if (string.IsNullOrWhiteSpace(pdfText))
        {
            return "{}"; // No text found
        }

        // 2. Call Doubao AI
        var systemPrompt = @"你是一个精准的简历实体提取AI。请从以下简历文本中提取出结构化信息。
必须返回严格的JSON格式数据，不要包含任何额外的多余文字，也不要包裹 markdown code block，仅仅只返回 JSON。
JSON 字段结构如下:
{
  ""name"": ""姓名"",
  ""phone"": ""手机号（必须是数字或包含拨号符）"",
  ""email"": ""邮箱地址"",
  ""gender"": 1, // 1男，2女，0未知
  ""highestDegree"": ""最高学历"",
  ""workYears"": 工作年限（提取为数字格式，例如3。若无经验或应届则为0）
}";

        var requestBody = new
        {
            model = _modelId,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = pdfText }
            },
            temperature = 0.1
        };

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://ark.cn-beijing.volces.com/api/v3/chat/completions");
        requestMessage.Headers.Add("Authorization", $"Bearer {_apiKey}");
        requestMessage.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(requestMessage);
        var responseString = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"AI 接口调用失败: {response.StatusCode} - {responseString}");
        }

        using var jsonDoc = JsonDocument.Parse(responseString);
        var contentString = jsonDoc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        
        // Simple cleanup in case the AI added markdown block
        var cleanedString = contentString?.Trim() ?? "{}";
        if (cleanedString.StartsWith("```json"))
        {
            cleanedString = cleanedString.Substring(7);
            if (cleanedString.EndsWith("```")) cleanedString = cleanedString.Substring(0, cleanedString.Length - 3);
        }

        return cleanedString.Trim();
    }
}
