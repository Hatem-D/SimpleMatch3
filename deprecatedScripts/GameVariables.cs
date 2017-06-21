using UnityEngine;
using System.Collections;


public class GameVariables : MonoBehaviour {

    public float YSpawnPosition = 9.0f; // spawn above screen - same for every gem
        

    public enum GemType { red, blue, green, yellow, brown, undefined} // gemtypes

    public static GemType GetTypeFromInt(int i)//for random calls
    {
        switch (i) { 
            case 1 :
                return GemType.red;
        
            case 2:
                return GemType.blue;
                        
            case 3:
                return GemType.green; 
        
            case 4:
                return GemType.yellow;

            case 5:
                return GemType.brown;

            default:
                return GemType.undefined;
        }
    }
}
