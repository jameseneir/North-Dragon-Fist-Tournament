using CI.QuickSave;
using System.Collections.Generic;

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
        writer.Write("Unlocked Skills", new List<int>());
        writer.Commit();
    }

    public static void AddMemoryPoint(int amount)
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

    public static void UnlockSkill(int index, int memoryPointCost)
    {
        var reader = QuickSaveReader.Create("Lee's Memory", new QuickSaveSettings()
        {
            SecurityMode = SecurityMode.Aes,
            Password = "Senify/NDFT2021",
            CompressionMode = CompressionMode.Gzip
        });

        List<int> current = reader.Read<List<int>>("Unlocked Skills");
        current.Add(index);

        int mp = reader.Read<int>("Memory Point");
        mp -= memoryPointCost;
        var writer = QuickSaveWriter.Create("Lee's Memory", new QuickSaveSettings()
        {
            SecurityMode = SecurityMode.Aes,
            Password = "Senify/NDFT2021",
            CompressionMode = CompressionMode.Gzip
        });
        writer.Write("Unlocked Skills", current);
        writer.Write("Memory Point", mp);
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
