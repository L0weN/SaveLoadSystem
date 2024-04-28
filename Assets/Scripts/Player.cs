using Mert.SaveLoadSystem;
using System;
using UnityEngine;

public class Player : MonoBehaviour, IBind<PlayerData>
{
    [SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
    [SerializeField] PlayerData data;

    public void Bind(PlayerData data)
    {
        this.data = data;
        this.data.Id = Id;
        transform.position = data.position;
        transform.rotation = data.rotation;
    }

    private void Update()
    {
        data.position = transform.position;
        data.rotation = transform.rotation;
    }
}

[Serializable]
public class PlayerData : ISaveable
{
    [SerializeField] public SerializableGuid Id { get; set; }
    public Vector3 position;
    public Quaternion rotation;
}
