namespace SimpleChat.ViewModel.User
{
    public record IsUserExistVM
    {
        public string UserName { get; set; }
        public string EMail { get; set; }
        public bool IsUserNameExist { get; set; }
        public bool IsEMailExist { get; set; }
    }
}
