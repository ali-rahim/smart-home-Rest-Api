namespace smart_home_Asp.net.Configuration
{
    public class StorageOptions
    {
        public string Provider { get; set; } = string.Empty;

        public string ConnectionStrings = "Server=DESKTOP-50N87M4\\SQL2025;Database=SmartHomeDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";
      
        public bool AutoSave { get; set; }
    }
}
