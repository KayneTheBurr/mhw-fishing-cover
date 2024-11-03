using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager trueMusic;

    public static MusicManager instance
    {
        get
        {
            if (trueMusic == null)
            {
                trueMusic = GameObject.FindFirstObjectByType<MusicManager>();

                //Tell unity not to destroy this object when loading a new scene!
                DontDestroyOnLoad(trueMusic.gameObject);
            }

            return trueMusic;
        }
    }

    void Awake()
    {
        if (trueMusic == null)
        {
            //If I am the first instance, make me the Singleton
            trueMusic = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            //If a Singleton already exists and you find
            //another reference in scene, destroy it!
            if (this != trueMusic)
                Destroy(this.gameObject);
        }
    }



    public void Play()
    {
        //Play some audio!
    }
}