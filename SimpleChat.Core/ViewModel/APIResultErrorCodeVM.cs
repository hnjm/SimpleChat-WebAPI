namespace SimpleChat.Core.ViewModel
{
    public record APIResultErrorCodeVM
    {
        public string Field { get; set; }
        public string ErrorCode { get; set; }
    }
}