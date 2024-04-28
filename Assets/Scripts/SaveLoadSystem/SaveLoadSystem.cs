using Mert.SaveLoadSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mert.SaveLoadSystem
{
    [Serializable]
    public class GameData
    {
        public string Name;
        public string CurrentLevelName;
        public PlayerData playerData;
    }

    public interface ISaveable
    {
        SerializableGuid Id { get; set; }
    }

    public interface IBind<TData> where TData : ISaveable
    {
        SerializableGuid Id { get; }
        void Bind(TData data);
    }

    public class SaveLoadSystem : PersistentSingleton<SaveLoadSystem>
    {
        [SerializeField] public GameData gameData;

        IDataService dataService;

        protected override void Awake()
        {
            base.Awake();
            dataService = new FileDataService(new JsonSerializer());
        }

        private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
        private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

        void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
        {
            if (scene.name == "Menu") return;

            Bind<Player, PlayerData>(gameData.playerData);
        }

        void Bind<T, TData>(TData data) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            var entity = FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();
            if (entity != null)
            {
                if (data == null)
                {
                    data = new TData { Id = entity.Id };
                }
                entity.Bind(data);
            }
        }

        void Bind<T, TData>(List<TData> datas) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            var entities = FindObjectsByType<T>(FindObjectsSortMode.None);

            foreach (var entity in entities)
            {
                var data = datas.FirstOrDefault(d => d.Id == entity.Id);
                if (data == null)
                {
                    data = new TData { Id = entity.Id };
                    datas.Add(data);
                }
                entity.Bind(data);
            }
        }

        public void NewGame()
        {
            gameData = new GameData
            {
                Name = "New Game",
                CurrentLevelName = "Demo"
            };
            SceneManager.LoadScene(gameData.CurrentLevelName);
        }

        public void SaveGame() => dataService.Save(gameData);

        public void LoadGame(string gameName)
        {
            gameData = dataService.Load(gameName);

            if (String.IsNullOrWhiteSpace(gameData.CurrentLevelName))
            {
                gameData.CurrentLevelName = "Demo";
            }

            SceneManager.LoadScene(gameData.CurrentLevelName);
        }

        public void ReloadGame() => LoadGame(gameData.Name);

        public void DeleteGame(string gameName) => dataService.Delete(gameName);
    }
}
