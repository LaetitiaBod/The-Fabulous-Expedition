using Raylib_cs;
using static Raylib_cs.Raylib;

public class AudioManager
{
	private Dictionary<string, Sound> soundList = new Dictionary<string, Sound>();

	public Sound GetSound(string name)
	{
		Sound sound = new Sound();

		soundList.TryGetValue(name, out sound);
		return sound;
	}

	public void AddSound(string name, string fileName)
	{
		soundList.Add(name, LoadSound(fileName));
	}
	public void AddAllSounds()
	{
		AddSound("menu", "resources/sounds/summer nights.ogg");
		AddSound("gameplay", "resources/sounds/Kookaburras and insects in Yallingup, Western Australia.wav");
	}

	public void PlaySound(string name)
	{
		Sound sound = GetSound(name);
		if(!IsSoundPlaying(sound))
			Raylib.PlaySound(sound);
	}

	public void StopAllSounds()
	{
		foreach (Sound sound in soundList.Values)
		{
			if (IsSoundPlaying(sound))
				StopSound(sound);
		}
	}

	public void Close()
	{
		foreach (Sound sound in soundList.Values)
		{
			UnloadSound(sound);
		}
	}
}