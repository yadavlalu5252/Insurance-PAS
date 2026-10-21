
namespace CustomerRegistration.Application.DTOs.Document;

public class CustomerDocumentDto
{
    public long document_id { get; set; }
    public long customer_id { get; set; }
    public string document_type { get; set; } = string.Empty;
    public string file_name { get; set; } = string.Empty;
    public string file_path { get; set; } = string.Empty;
    public DateTime uploaded_date { get; set; }
    public string verification_status { get; set; } = "Pending";
}
