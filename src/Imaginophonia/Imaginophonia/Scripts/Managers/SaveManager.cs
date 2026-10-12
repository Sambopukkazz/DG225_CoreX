using Gum.DataTypes;
using Imaginophobia;
using Penumbra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public static class SaveManager {
    private static readonly string _dir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Imaginophobia");
    private static readonly string _path = Path.Combine(_dir, "save.json");
    private static readonly JsonSerializerOptions _opts = new() { WriteIndented = true };
    private static GameData _data = LoadFromDisk();
    private static GameData LoadFromDisk() {
        try {
            if (File.Exists(_path))
                return JsonSerializer.Deserialize<GameData>(File.ReadAllText(_path)) ?? new GameData();
        }
        catch (Exception e) {
            Console.WriteLine($"Save load failed, starting fresh: {e.Message}");
        }
        return new GameData();
    }
    private static void Flush() {
        Directory.CreateDirectory(_dir);
        File.WriteAllText(_path, JsonSerializer.Serialize(_data, _opts));
    }
    // --- called when LEAVING a scene, before it unloads ---
    public static void SaveScene(string sceneName, List<Trigger> triggers, List<LightArea> lightAreas) {
        var scene = new SceneData();
        foreach (Trigger trigger in triggers) {
            scene.Triggers.Add(new TriggerData { Id = trigger.Id, Progress = trigger.Progress, Interactable = trigger.Interactable });
        }

        foreach (LightArea light in lightAreas) {
            scene.Lights.Add(new LightData { Id = light.Id, On = light.On });
        }

        _data.Scenes[sceneName] = scene;
        Flush();
    }
    // --- called right AFTER a scene is rebuilt ---
    public static void ApplyToScene(string sceneName, List<Trigger> triggers, List<LightArea> lightAreas) {
        if (!_data.Scenes.TryGetValue(sceneName, out SceneData scene)) return;
        var triggerById = new Dictionary<int, TriggerData>();
        foreach (TriggerData s in scene.Triggers) triggerById[s.Id] = s;
        foreach (Trigger t in triggers)
            if (triggerById.TryGetValue(t.Id, out TriggerData s)) {
                t.Progress = s.Progress;
                t.Interactable = s.Interactable;
            }

        var lightById = new Dictionary<int, LightData>();
        foreach (var s in scene.Lights) lightById[s.Id] = s;
        foreach (LightArea light in lightAreas)
            if (lightById.TryGetValue(light.Id, out var s)) {
                if (s.On) light.TurnOn(); else light.TurnOff();
            }
    }

    public static void TurnAllLightsOn() {
        foreach (SceneData scene in _data.Scenes.Values)
            foreach (LightData l in scene.Lights)
                l.On = true;
        Flush();
    }

    public static void ResetScene(string sceneName) {
        _data.Scenes.Remove(sceneName);   // next load of this scene finds no entry → triggers fresh
        Flush();
    }

    public static void Delete() { if (File.Exists(_path)) File.Delete(_path); _data = new GameData(); }  // handy for testing
}