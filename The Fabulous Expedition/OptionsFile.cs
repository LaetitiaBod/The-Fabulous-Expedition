using System.Text.Json;

public class OptionsFile
{
    const string FILENAME = "options.json";

    private string AppName = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);

    private string fullPath;

    protected Dictionary<string, string> options;

    public OptionsFile()
    {
        options = new Dictionary<string, string>();
        fullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName, FILENAME);
    }

    public void Clear()
    {
        options.Clear();
    }

    public void AddOption(string key, string value)
    {
        options[key] = value;
    }

    public void AddOption(string key, float value)
    {
        options[key] = value.ToString();
    }

    public void AddOption(string key, int value)
    {
        options[key] = value.ToString();
    }

    public void AddOption(string key, object value)
    {
        string json = JsonSerializer.Serialize(value);
        options[key] = json;
    }

    public bool IsOptionExists(string key)
    {
        return options.ContainsKey(key);
    }

    public string GetOptionString(string key)
    {
        if (options.ContainsKey(key))
        {
            return options[key];
        }
        else
            return "";
    }

    public int GetOptionInt(string key)
    {
        if (options.ContainsKey(key))
        {
            try
            {
                return int.Parse(GetOptionString(key));
            }
            catch
            {
                return 0;
            }
        }
        else
            return 0;
    }

    public float GetOptionFloat(string key)
    {
        if (options.ContainsKey(key))
        {
            try
            {
                return float.Parse(GetOptionString(key));
            }
            catch
            {
                return 0f;
            }
        }
        else
            return 0f;
    }

    public bool GetOptionBool(string key)
    {
        if (options.ContainsKey(key))
        {
            try
            {
                return bool.Parse(GetOptionString(key));
            }
            catch
            {
                return false;
            }
        }
        else
            return false;
    }

    public void Load()
    {
        if (File.Exists(fullPath))
        {
            var jsonString = File.ReadAllText(fullPath);
            options = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString)!;
            if (options == null)
            {
                throw new JsonException($"Invalid options file. Path : {fullPath}");
            }
        }
        else
        {
            options.Clear();
        }
    }

    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        var jsonString = JsonSerializer.Serialize(options);

        File.WriteAllText(fullPath, jsonString);
    }
}