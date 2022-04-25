using CI.QuickSave;

public static class MemoryPoint
{
    public static void CreateFile()
    {
        var writer = QuickSaveWriter.Create("Lee's Memory", new QuickSaveSettings()
        {
            SecurityMode = SecurityMode.Aes,
            Password = "Senify/NDFT2021",
            CompressionMode = CompressionMode.Gzip
        });
        writer.Write("Memory Point", 0);
        writer.Commit();
    }

    public static void ChangeMemoryPoint(int amount)
    {
        var reader = QuickSaveReader.Create("Lee's Memory", new QuickSaveSettings()
        {
            SecurityMode = SecurityMode.Aes,
            Password = "Senify/NDFT2021",
            CompressionMode = CompressionMode.Gzip
        });
        int current = reader.Read<int>("Memory Point");
        current += amount;
        var writer = QuickSaveWriter.Create("Lee's Memory", new QuickSaveSettings()
        {
            SecurityMode = SecurityMode.Aes,
            Password = "Senify/NDFT2021",
            CompressionMode = CompressionMode.Gzip
        });
        writer.Write("Memory Point", current);
        writer.Commit();
    }

    public static int MP
    {
        get
        {
            return QuickSaveReader.Create("Lee's Memory", new QuickSaveSettings()
            {
                SecurityMode = SecurityMode.Aes,
                Password = "Senify/NDFT2021",
                CompressionMode = CompressionMode.Gzip
            }).Read<int>("Memory Point");
        }
    }
}
