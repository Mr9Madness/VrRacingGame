using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Data
{
    [ Serializable ]
    public class Level
    {
        public int LevelWidth = 16;
        public int LevelHeight = 16;
        public string LevelName = "empty";
        public int[] LevelData = { 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 
        };

        public int GetTile( int x, int y )
        {
            return LevelData[ x + LevelWidth * y ];
        }

        public void SetTile( int x, int y, int value )
        {
            Debug.Log( x + " | " + y + " | " + LevelWidth );
            Debug.Log( x + ( LevelWidth * y ) );
            LevelData[ x + ( LevelWidth * y ) ] = value;
        }

        /// <summary>
        /// Converts the json file of the given level name to the <see cref="LevelData"/> struct so it can be used.
        /// </summary>
        /// <param name="levelName">The name of the level that needs to load</param>
        /// <returns>The <see cref="LevelData"/> struct thats filled with the data from the given level name. </returns>
        public static Level CreateFromJson( string levelName )
        {
            //string json = File.ReadAllText( @"Assets\Data\level\" + levelName + ".json" );
            TextAsset loaded = Resources.Load( levelName ) as TextAsset;
            if( loaded != null )
            {
                Debug.Log( loaded );
                return JsonUtility.FromJson< Level >( loaded.text );
            }
            string loadedString = File.ReadAllText( Application.dataPath + @"\Data\level\" + levelName + ".json" );

            if( loadedString != null )
            {                          
                Debug.Log( loadedString );

                return JsonUtility.FromJson< Level >( loadedString );
            }
            return null;
         }

        /// <summary>
        /// Converts a given <see cref="Level"/>object to a json file in the 'Assets/Data/level' folder.
        /// </summary>
        /// <param name="fileName">The Name thats the file recieves</param>
        /// <param name="levelData">The object thats needs to be converted</param>
        public static void ToJsonFile( string fileName, object levelData )
        {
            string json = JsonUtility.ToJson( levelData );
            Directory.CreateDirectory( Application.dataPath + @"\Data\level" );

            File.WriteAllText( Application.dataPath + @"\Data\level\" + fileName + ".json", json );
        }

        /// <summary>
        /// Converts a given json string to a json file in the 'Assets/Data/level' folder.
        /// </summary>
        /// <param name="jsonData">A string that contains the json data.</param>
        /// <param name="fileName">Name of the file it gets stored in.</param>
        public static void ToJsonFile( string fileName, string jsonData )
        {
            Directory.CreateDirectory( Application.dataPath + @"\Data\level" );

            File.WriteAllText( Application.dataPath + @"\Data\level\" + fileName + ".json", jsonData );
        }
    }
}