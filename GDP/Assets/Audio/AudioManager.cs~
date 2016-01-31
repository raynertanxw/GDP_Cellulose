using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

	private AudioSource BackgroundAudioSource;
	private List<AudioSource> SFXAudioSources;
	public AudioClip[] BackgroundTracks;
	
	private float BackgroundClipLength;
	private string CurrentSceneName;
	
	private static List<AudioSource> MenuTracks;
	private static List<AudioSource> PlayerTracks;
	private static List<AudioSource> EnemyTracks;
	private static List<AudioSource> SquadTracks;
	
	private SceneType CurrentSceneType;
	private enum SceneType{Menu,Gameplay,Null};

	// Use this for initialization
	void Start () 
	{
		DontDestroyOnLoad(gameObject);
	
		BackgroundAudioSource = GetComponent<AudioSource>();
		SFXAudioSources = new List<AudioSource>();
		
		MenuTracks = new List<AudioSource>();
		PlayerTracks = new List<AudioSource>();
		EnemyTracks = new List<AudioSource>();
		SquadTracks = new List<AudioSource>();
		
		CurrentSceneName = Application.loadedLevelName;
		
		LoadTracksToLists();
		LoadRandomBackgroundTrack();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//if the scene had been change, reload a random background track
		if(CurrentSceneName != Application.loadedLevelName)
		{
			LoadRandomBackgroundTrack();
			CurrentSceneName = Application.loadedLevelName;
			BackgroundAudioSource.volume = 0f;
			StartCoroutine(FadeInBackgroundMusic());
		}
	
		//Fading in/Fading out of BGM
		if(BackgroundAudioSource.time <= 3.5f)
		{
			StartCoroutine(FadeInBackgroundMusic());
		}
		if(BackgroundAudioSource.time >= BackgroundClipLength - 3.5f)
		{
			StartCoroutine(FadeOutBackgroundMusic());
		}
	}
	
	private void LoadTracksToLists()
	{
		for(int i = 0; i < transform.childCount; i++)
		{
			GameObject Child = transform.GetChild(i).gameObject;
			
			if(Child.name.Contains("Menu"))
			{
				MenuTracks.Add(Child.GetComponent<AudioSource>());
			}
			else if(Child.name.Contains("Player"))
			{
				PlayerTracks.Add(Child.GetComponent<AudioSource>());
			}
			else if(Child.name.Contains("Enemy"))
			{
				EnemyTracks.Add(Child.GetComponent<AudioSource>());
			}
			else if(Child.name.Contains("Squad"))
			{
				SquadTracks.Add(Child.GetComponent<AudioSource>());
			}
		}
	}
	
	private SceneType DetermineCurrentSceneType()
	{
		if(Application.loadedLevelName.Contains("Menu"))
		{
			return SceneType.Menu;
		}
		else if(Application.loadedLevelName.Contains("Level"))
		{
			return SceneType.Gameplay;
		}
		return SceneType.Null;
	}
	
	public void LoadRandomBackgroundTrack()
	{
		CurrentSceneType = DetermineCurrentSceneType();
		
		if(CurrentSceneType == SceneType.Null){return;}
		
		int RandomTrackIndex = 0;
		
		if(CurrentSceneType == SceneType.Menu){RandomTrackIndex = Random.Range(0,3);}
		if(CurrentSceneType == SceneType.Gameplay){RandomTrackIndex = Random.Range(3,7);}
		
		LoadAndPlayBackgroundTrack(BackgroundTracks[RandomTrackIndex]);
	
		if(RandomTrackIndex == 1 || RandomTrackIndex == 2){BackgroundClipLength = BackgroundTracks[RandomTrackIndex].length * 6f; return;}
		BackgroundClipLength = BackgroundTracks[RandomTrackIndex].length;
	}
	
	public void LoadAndPlayBackgroundTrack(AudioClip _Clip)
	{
		BackgroundAudioSource.Stop();
		BackgroundAudioSource.clip = _Clip;
		BackgroundAudioSource.Play();
	}
	
	private IEnumerator FadeInBackgroundMusic()
	{
		while(BackgroundAudioSource.volume < 1.0f)
		{
			BackgroundAudioSource.volume += 0.0075f;
			yield return new WaitForSeconds(0.75f);
		}
	}
	
	private IEnumerator FadeOutBackgroundMusic()
	{
		while(BackgroundAudioSource.volume > 0.0f)
		{
			BackgroundAudioSource.volume -= 0.0075f;
			yield return new WaitForSeconds(0.75f);
		}
		LoadRandomBackgroundTrack();
	}
	
	public static void PlayMenuSoundEffect(MenuSFX _sfx)
	{
		MenuTracks[(int) _sfx].Stop();
		MenuTracks[(int) _sfx].Play();
	}
	
	public static void PlayPlayerSoundEffect(PlayerSFX _sfx)
	{
		PlayerTracks[(int) _sfx].Stop();
		PlayerTracks[(int) _sfx].Play();
	}
	
	public static void PlayEnemySoundEffect(EnemySFX _sfx)
	{
		EnemyTracks[(int) _sfx].Stop();
		EnemyTracks[(int) _sfx].Play();
	}
}
