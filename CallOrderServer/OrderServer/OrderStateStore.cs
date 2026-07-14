using System;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace OrderServer;

public static class OrderStateStore
{
    private static readonly object SaveLock = new object();
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public static string StateFilePath { get; } = Path.Combine(AppContext.BaseDirectory, "order_state.json");
    public static string StateArchiveDirectory { get; } = Path.Combine(AppContext.BaseDirectory, "state_archive");
    public static string StartupArchiveFilePath { get; private set; } = "";

    public static void ArchiveExistingStateForStartup()
    {
        StartupArchiveFilePath = "";
        if (!File.Exists(StateFilePath))
        {
            Program.Log("未找到启动前状态快照，将全新启动。");
            return;
        }

        try
        {
            Directory.CreateDirectory(StateArchiveDirectory);
            string archivePath = Path.Combine(StateArchiveDirectory, "order_state_startup_" + DateTime.Now.ToString("yyyyMMddHHmmssffffff") + ".json");
            File.Move(StateFilePath, archivePath);
            StartupArchiveFilePath = archivePath;
            Program.Log("启动前状态快照已归档：" + archivePath);
        }
        catch (Exception e)
        {
            Program.Log("启动前状态快照归档失败：" + e.Message);
        }
    }

    public static PersistedOrderState Load()
    {
        string path = GetLoadCandidatePath();
        if (!File.Exists(path))
        {
            Program.Log("未找到可加载的状态文件。");
            return null;
        }

        try
        {
            string json = File.ReadAllText(path);
            var state = JsonSerializer.Deserialize<PersistedOrderState>(json, JsonOptions);
            Program.Log("状态文件加载成功：" + path);
            return state;
        }
        catch (Exception e)
        {
            Program.Log("状态文件加载失败：" + e.Message);
            return null;
        }
    }

    public static string GetLoadCandidatePath()
    {
        if (!string.IsNullOrWhiteSpace(StartupArchiveFilePath) && File.Exists(StartupArchiveFilePath))
        {
            return StartupArchiveFilePath;
        }

        return "";
    }

    public static void SaveNow()
    {
        lock (SaveLock)
        {
            try
            {
                var state = DataClass.CreatePersistedState();
                string json = JsonSerializer.Serialize(state, JsonOptions);
                string tempPath = StateFilePath + ".tmp";
                File.WriteAllText(tempPath, json);

                if (File.Exists(StateFilePath))
                {
                    File.Replace(tempPath, StateFilePath, StateFilePath + ".bak", true);
                }
                else
                {
                    File.Move(tempPath, StateFilePath);
                }
            }
            catch (Exception e)
            {
                Program.Log("状态保存失败：" + e.Message);
            }
        }
    }

    public static void AutoSaveState()
    {
        Program.Log("管理员已为此服务器配置自动状态保存，每次保存间隔时长为15秒。");
        while (true)
        {
            SaveNow();
            Thread.Sleep(15000);
        }
    }
}
