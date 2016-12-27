Entity to PlayerPrefs
---------------------
Plugin for Unity which helps to save and load entity to PlayerPrefs with single line of code.

Usage
-----
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

Supported types.
----------------

Plugin extends PlayerPrefs types:
- string
- int
- float

by following types:
- bool
- DateTime
