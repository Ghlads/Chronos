using System;
using System.IO;
using UnityEngine;

namespace Framework.Core.Editor
{
    public static class FileUtils
    {
    
        public static bool CreateTextFileAtPath( string directoryPath, string fileName, string content, string extension )
        {
            try
            {
                Directory.CreateDirectory( directoryPath );
            }
            catch ( Exception e)
            {
                Debug.LogError( $"[CreateTextFileAtPath] error while creating directory : {e}" );
                return false;
            }

            try
            {
                string safeExtension = extension.StartsWith( "." ) ? extension : "."+extension;
                string safeDirectory = directoryPath.EndsWith( "/" ) ? directoryPath : directoryPath + "/";
                File.WriteAllText( $"{safeDirectory}{fileName}{safeExtension}", content );
            }
            catch ( Exception e )
            {
                Debug.LogError( $"[CreateTextFileAtPath] error while writing file : {e}" );
                return false;
            }

            Debug.Log( $"Successfuly created {fileName} at path {directoryPath}" );
            return true;
        }

    }
}
