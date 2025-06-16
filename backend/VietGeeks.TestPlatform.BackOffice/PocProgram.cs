using System.Text;
using System.Text.Json;

namespace VietGeeks.TestPlatform.BackOffice
{
    public static class PocProgram
    {
        public static void UsePoc(this WebApplication app)
        {
            // API endpoint to convert JSON to HTML
            app.MapPost("/convert-to-html", (List<RealEstateListing> listings) =>
            {
                var html = GenerateHtmlFromListings(listings);
                return Results.Content(html, "text/html");
            })
            .WithName("ConvertJsonToHtml")
            .WithOpenApi();

            // Alternative endpoint that accepts raw JSON string
            app.MapPost("/convert-json-string", async (HttpRequest request) =>
            {
                using var reader = new StreamReader(request.Body);
                var jsonString = await reader.ReadToEndAsync();

                try
                {
                    var listings = JsonSerializer.Deserialize<List<RealEstateListing>>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (listings == null)
                    {
                        return Results.BadRequest("Invalid JSON format");
                    }

                    var html = GenerateHtmlFromListings(listings);
                    return Results.Content(html, "text/html");
                }
                catch (JsonException ex)
                {
                    return Results.BadRequest($"JSON parsing error: {ex.Message}");
                }
            })
            .WithName("ConvertJsonStringToHtml")
            .WithOpenApi();

        }

        static string GenerateHtmlFromListings(List<RealEstateListing> listings)
        {
            var html = new StringBuilder();

            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang='vi'>");
            html.AppendLine("<head>");
            html.AppendLine(" <meta charset='UTF-8'>");
            html.AppendLine(" <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
            html.AppendLine(" <title>Danh sách Bất động sản</title>");
            html.AppendLine(" <style>");
            html.AppendLine(" body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5; }");
            html.AppendLine(" .container { max-width: 1200px; margin: 0 auto; }");
            html.AppendLine(" .listing { background: white; border-radius: 12px; box-shadow: 0 4px 6px rgba(0,0,0,0.1); margin-bottom: 24px; overflow: hidden; transition: transform 0.2s; }");
            html.AppendLine(" .listing:hover { transform: translateY(-2px); box-shadow: 0 8px 25px rgba(0,0,0,0.15); }");
            html.AppendLine(" .listing-header { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; }");
            html.AppendLine(" .listing-title { font-size: 1.4em; font-weight: bold; margin-bottom: 8px; line-height: 1.3; }");
            html.AppendLine(" .listing-category { background: rgba(255,255,255,0.2); padding: 4px 12px; border-radius: 15px; font-size: 0.85em; display: inline-block; }");
            html.AppendLine(" .listing-content { padding: 24px; }");
            html.AppendLine(" .price-section { background: #e8f5e8; padding: 16px; border-radius: 8px; margin-bottom: 20px; border-left: 4px solid #4caf50; }");
            html.AppendLine(" .main-price { font-size: 1.8em; font-weight: bold; color: #2e7d32; margin-bottom: 4px; }");
            html.AppendLine(" .sub-price { color: #666; font-size: 0.9em; }");
            html.AppendLine(" .details-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 16px; margin-bottom: 20px; }");
            html.AppendLine(" .detail-item { padding: 12px; background: #f8f9fa; border-radius: 6px; border-left: 3px solid #007bff; }");
            html.AppendLine(" .detail-label { font-weight: 600; color: #495057; font-size: 0.85em; margin-bottom: 4px; }");
            html.AppendLine(" .detail-value { color: #212529; }");
            html.AppendLine(" .description { background: #fff3cd; padding: 16px; border-radius: 8px; border-left: 4px solid #ffc107; margin-bottom: 20px; }");
            html.AppendLine(" .description h4 { margin-top: 0; color: #856404; }");
            html.AppendLine(" .description-text { white-space: pre-line; line-height: 1.6; color: #6c5700; }");
            html.AppendLine(" .contact-info { background: #d4edda; padding: 16px; border-radius: 8px; border-left: 4px solid #28a745; }");
            html.AppendLine(" .contact-info h4 { margin-top: 0; color: #155724; }");
            html.AppendLine(" .images-section { margin-bottom: 20px; }");
            html.AppendLine(" .image-gallery { display: flex; gap: 12px; overflow-x: auto; padding: 8px 0; }");
            html.AppendLine(" .image-item { flex: 0 0 auto; }");
            html.AppendLine(" .image-item img { width: 200px; height: 150px; object-fit: cover; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.15); }");
            html.AppendLine(" .badges { display: flex; gap: 8px; margin-bottom: 16px; }");
            html.AppendLine(" .badge { padding: 4px 12px; border-radius: 12px; font-size: 0.8em; font-weight: 500; }");
            html.AppendLine(" .badge.verified { background: #d1ecf1; color: #0c5460; }");
            html.AppendLine(" .badge.vip { background: #f8d7da; color: #721c24; }");
            html.AppendLine(" .badge.expired { background: #f1f3f4; color: #5f6368; }");
            html.AppendLine(" .metadata { font-size: 0.85em; color: #6c757d; margin-top: 16px; padding-top: 16px; border-top: 1px solid #dee2e6; }");
            html.AppendLine(" h1 { text-align: center; color: #333; margin-bottom: 32px; }");
            html.AppendLine(" </style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine(" <div class='container'>");
            html.AppendLine(" <h1>Danh sách Bất động sản</h1>");

            foreach (var listing in listings)
            {
                html.AppendLine(" <div class='listing'>");

                // Header
                html.AppendLine(" <div class='listing-header'>");
                html.AppendLine($" <div class='listing-title'>{System.Web.HttpUtility.HtmlEncode(listing.Title)}</div>");
                html.AppendLine($" <span class='listing-category'>{System.Web.HttpUtility.HtmlEncode(listing.CategoryName)}</span>");
                html.AppendLine(" </div>");

                // Content
                html.AppendLine(" <div class='listing-content'>");

                // Badges
                html.AppendLine(" <div class='badges'>");
                if (listing.IsVerified)
                    html.AppendLine(" <span class='badge verified'>✓ Đã xác minh</span>");
                if (listing.VipTypeName != "Thường")
                    html.AppendLine($" <span class='badge vip'>VIP: {System.Web.HttpUtility.HtmlEncode(listing.VipTypeName)}</span>");
                if (listing.IsExpired)
                    html.AppendLine(" <span class='badge expired'>Đã hết hạn</span>");
                html.AppendLine(" </div>");

                // Price
                html.AppendLine(" <div class='price-section'>");
                html.AppendLine($" <div class='main-price'>{System.Web.HttpUtility.HtmlEncode(listing.MainDisplayedPrice)}</div>");
                if (!string.IsNullOrEmpty(listing.SubDisplayedPrice))
                    html.AppendLine($" <div class='sub-price'>{System.Web.HttpUtility.HtmlEncode(listing.SubDisplayedPrice)}</div>");
                html.AppendLine(" </div>");

                // Images
                if (listing.Images.Any())
                {
                    html.AppendLine(" <div class='images-section'>");
                    html.AppendLine(" <h4>Hình ảnh</h4>");
                    html.AppendLine(" <div class='image-gallery'>");
                    foreach (var image in listing.Images.Take(5))
                    {
                        html.AppendLine(" <div class='image-item'>");
                        html.AppendLine($" <img src='{System.Web.HttpUtility.HtmlEncode(image.ThumbUrl)}' alt='Hình ảnh bất động sản' />");
                        html.AppendLine(" </div>");
                    }
                    html.AppendLine(" </div>");
                    html.AppendLine(" </div>");
                }

                // Details
                html.AppendLine(" <div class='details-grid'>");
                html.AppendLine(" <div class='detail-item'>");
                html.AppendLine(" <div class='detail-label'>Diện tích</div>");
                html.AppendLine($" <div class='detail-value'>{listing.Area} m²</div>");
                html.AppendLine(" </div>");
                html.AppendLine(" <div class='detail-item'>");
                html.AppendLine(" <div class='detail-label'>Địa chỉ</div>");
                html.AppendLine($" <div class='detail-value'>{System.Web.HttpUtility.HtmlEncode(listing.Address)}</div>");
                html.AppendLine(" </div>");
                if (listing.Rooms > 0)
                {
                    html.AppendLine(" <div class='detail-item'>");
                    html.AppendLine(" <div class='detail-label'>Phòng ngủ</div>");
                    html.AppendLine($" <div class='detail-value'>{listing.Rooms}</div>");
                    html.AppendLine(" </div>");
                }
                if (listing.Toilets > 0)
                {
                    html.AppendLine(" <div class='detail-item'>");
                    html.AppendLine(" <div class='detail-label'>Phòng tắm</div>");
                    html.AppendLine($" <div class='detail-value'>{listing.Toilets}</div>");
                    html.AppendLine(" </div>");
                }
                html.AppendLine(" </div>");

                // Description
                if (!string.IsNullOrEmpty(listing.Description))
                {
                    html.AppendLine(" <div class='description'>");
                    html.AppendLine(" <h4>Mô tả</h4>");
                    html.AppendLine($" <div class='description-text'>{System.Web.HttpUtility.HtmlEncode(listing.Description)}</div>");
                    html.AppendLine(" </div>");
                }

                // Contact Info
                html.AppendLine(" <div class='contact-info'>");
                html.AppendLine(" <h4>Thông tin liên hệ</h4>");
                html.AppendLine($" <p><strong>Người đăng:</strong> {System.Web.HttpUtility.HtmlEncode(listing.AuthorName)}</p>");
                html.AppendLine($" <p><strong>Điện thoại:</strong> {System.Web.HttpUtility.HtmlEncode(listing.AuthorPhone)}</p>");
                html.AppendLine(" </div>");

                // Metadata
                html.AppendLine(" <div class='metadata'>");
                html.AppendLine($" <p><strong>Mã tin:</strong> {listing.ListingId} | <strong>Cập nhật:</strong> {listing.Duration} | <strong>Số media:</strong> {listing.MediaCount}</p>");
                html.AppendLine(" </div>");

                html.AppendLine(" </div>");
                html.AppendLine(" </div>");
            }

            html.AppendLine(" </div>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
        }
    }
}



// Model classes for the real estate listing
public class RealEstateListing
{
    public long ListingId { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int VipTypeId { get; set; }
    public string VipTypeName { get; set; } = string.Empty;
    public List<ImageInfo> Images { get; set; } = new();
    public string? VideoUrl { get; set; }
    public int MediaCount { get; set; }
    public bool Has360Image { get; set; }
    public bool Has3dScan { get; set; }
    public bool HasVideo { get; set; }
    public string Title { get; set; } = string.Empty;
    public long ComputedPrice { get; set; }
    public int PriceUnit { get; set; }
    public string MainDisplayedPrice { get; set; } = string.Empty;
    public string SubDisplayedPrice { get; set; } = string.Empty;
    public decimal Area { get; set; }
    public int Rooms { get; set; }
    public int Toilets { get; set; }
    public string Address { get; set; } = string.Empty;
    public string AddressForCityLevel { get; set; } = string.Empty;
    public string AddressForDistrictLevel { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorPhone { get; set; } = string.Empty;
    public string AuthorAvatarUrl { get; set; } = string.Empty;
    public string EncryptedAuthorPhone { get; set; } = string.Empty;
    public DateTime LastUpTime { get; set; }
    public string DuplicateType { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public string Duration { get; set; } = string.Empty;
    public bool IsProAgent { get; set; }
    public bool IsExpired { get; set; }
}

public class ImageInfo
{
    public string Id { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public string CropUrl { get; set; } = string.Empty;
    public string ThumbUrl { get; set; } = string.Empty;
}