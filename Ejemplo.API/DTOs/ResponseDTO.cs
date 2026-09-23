public class ResponseDTO <T>
{
    
    public bool Success { get; set; }


    public string? Message { get; set; }

    public int Code { get; set; }

    public T? Payload { get; set; }
}