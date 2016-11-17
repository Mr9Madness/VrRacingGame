using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Data
{
    [ Serializable ]
    public class Level
    {
        public int LevelWidth = 16;
        public int LevelHeight = 16;
        public string LevelName = "level1";
        public int[] LevelData = { 
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 
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
                return JsonUtility.FromJson< Level >( loaded.text );
            }
            string loadedString = File.ReadAllText( Application.dataPath + @"\Data\level\" + levelName + ".json" );

            if( loadedString != "" )
            {                          
                Debug.Log( loadedString );

                return JsonUtility.FromJson< Level >( loadedString );
            }
            return null;
         }

        /// <summary>
        /// Converts a given <see cref="Level"/>object to a json file in the 'Assets/Data/level' folder.
        /// </summary>
        /// <param name="levelData">The object thats needs to be converted</param>
        public static void ToJsonFile( Level levelData )
        {                       
            Directory.CreateDirectory( Application.dataPath + @"\Data\level" );

            string json = JsonUtility.ToJson( levelData );

            string legalLevelName = levelData.LevelName.Trim( ' ' );
            string regexSearch = new String( Path.GetInvalidFileNameChars() );
            Regex r = new Regex( string.Format( "[{0}]", Regex.Escape( regexSearch ) ) );
            legalLevelName = r.Replace( legalLevelName, "" );

            File.WriteAllText( Application.dataPath + @"\Data\level\" + legalLevelName + ".json", json );
        }

        /// <summary>
        /// Converts a given json string to a json file in the 'Assets/Data/level' folder.
        /// </summary>
        /// <param name="json">A string that contains the json data.</param>
        public static void ToJsonFile( string json )
        {
            Directory.CreateDirectory( Application.dataPath + @"\Data\level" );

            Level levelData = JsonUtility.FromJson< Level >( json );

            string legalLevelName = levelData.LevelName.Trim( ' ' );
            string regexSearch = new string( Path.GetInvalidFileNameChars() );
            Regex r = new Regex( string.Format( "[{0}]", Regex.Escape( regexSearch ) ) );
            legalLevelName = r.Replace( legalLevelName, "" );

            File.WriteAllText( Application.dataPath + @"\Data\level\" + legalLevelName + ".json", json );
        }
    }
}