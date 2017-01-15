Entity to PlayerPrefs
---------------------
Plugin for Unity game engine. It helps to store entities in PlayerPrefs.

Take the latest unitypackage from [releases](//github.com/nubick/entity-to-playerprefs/releases).

Usage
-----
###[PlayerPrefsField] attribute

Instead of using 'Set' methods and string keys for saving

	PlayerPrefs.SetFloat("SoundsVolumeKey", SoundsVolume);

and 'Get' methods and string keys for loading

	SoundsVolume = PlayerPrefs.GetFloat("SoundsVolumeKey");

Mark field by [PlayerPrefsField] attribute:

	[PlayerPrefsField] public float SoundsVolume;

When call 'PlayerPrefsMapper.Save' all fields which are marked by [PlayerPrefsField] attribute will be saved to PlayerPrefs.

	PlayerPrefsMapper.Save(entity);
	
When call 'PlayerPrefsMapper.Load' all fields which are marked by [PlayerPrefsField] attribute will be loaded from PlayerPrefs.

	PlayerPrefsMapper.Load(entity);
	
[[Code Example]](https://github.com/nubick/entity-to-playerprefs/blob/development-v-0.2-features/EntityToPlayerPrefs/Assets/Tests/Scripts/GameState.cs)

###[PlayerPrefsEntityId] attribute
Add [PlayerPrefsEntityId] attribute if you want to save several different instances of one class to PlayerPrefs. Field with [PlayerPrefsEntityId] attribute must have string type. Value of this field must be unique between all instances of this class.

Example: 
When we want to save state of several levels we can use following class:

	public class Level : MonoBehaviour
	{
		public int Number;
		[PlayerPrefsEntityId] public string Id { get { return Number.ToString(); } }
		[PlayerPrefsField] public bool IsCompleted;
		[PlayerPrefsField] public int Stars;
	}

We can save level state when level is completed:

	public void Complete(int stars)
	{
		Stars = stars;
		IsCompleted = true;
		PlayerPrefsMapper.Save(this);
	}

We can load all levels state on game start:

 	public Level[] Levels;
	public void Awake()
	{
		PlayerPrefsMapper.Load(Levels);
	}

Editor
------

Plugin contains editor for game state management during development. It allows to edit any field or delete any field from PlayerPrefs. Editor groups records by tabs based on entity type. This allows quickly find required entity. You can use editor by open it from menu 'Your Game' -> 'Entity To PlayerPrefs' in the Unity.

![My image](http://nubick.github.com/readme/entity-to-playerprefs-editor.png)

![My image](http://nubick.github.com/readme/entity-to-playerprefs-editor-2.png)



Supported types.
----------------

Plugin extends PlayerPrefs types:
- string
- int
- float

by following types:
- bool
- DateTime
- Enumerators
