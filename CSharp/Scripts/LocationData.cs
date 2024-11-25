using System.Collections.Generic;
using UnityEngine;

public class LocationData : ScriptableObject
{
    public string locationName;
    public LocationType locationType;
    public Sprite locationImage;
    public Vector2 position;

    [Header("Sounds")]
    public AudioClip locationAmbience;
    public List<AudioClip> randomAmbiences;
}

public enum LocationType { Town, Wilderness, Dungeon }
