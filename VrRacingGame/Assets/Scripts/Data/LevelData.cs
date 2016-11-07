using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Data
{
    [ Serializable ]
    public class LevelData
    {
        public int LevelWidth = 0;
        public int LevelHeight = 0;
        public string LevelName = "empty";
        public int[] Level = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public int GetTile( int x, int y )
        {
            return Level[ x + LevelWidth * y ];
        }

        /// <summary>
        /// Converts the json file of the given level name to the <see cref="LevelData"/> struct so it can be used.
        /// </summary>
        /// <param name="levelName">The name of the level that needs to load</param>
        /// <returns>The <see cref="LevelData"/> struct thats filled with the data from the given level name. </returns>
        public static LevelData CreateFromJson( string levelName )
        {
            string json = File.ReadAllText( @"Assets\Data\level\" + levelName + ".json" );
            return JsonUtility.FromJson< LevelData >( json );
         }
        /// <summary>
        /// Dumps <see cref="LevelData"/> into a json file thats in the 'Assets/Data/level' folder 
        /// </summary>
        /// <param name="fileName">The Name thats the file recieves</param>
        /// <param name="levelData">The object thats needs to be converted</param>
        public static void ToJsonFile( string fileName, object levelData )
        {
            string json = JsonUtility.ToJson( levelData );
            File.WriteAllText( @"Assets\Data\level\" + fileName + ".json", json );
        }
    }
}