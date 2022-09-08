namespace kzmpCloudAPI.Interfaces
{
    public interface IFilesCompressingService
    {

        /// <summary>
        /// Method that compress all the files inside a folder (non-recursive) into a zip file.
        /// </summary>
        /// <param name="DirectoryPath"></param>
        /// <param name="OutputFilePath"></param>
        /// <param name="CompressionLevel"></param>
        public void compressDirectory(string DirectoryPath, string OutputFilePath, int CompressionLevel = 9);

        /// <summary>
        /// Extracts the content from a .zip file inside an specific folder.
        /// </summary>
        /// <param name="FileZipPath"></param>
        /// <param name="password"></param>
        /// <param name="OutputFolder"></param>
        public void ExtractZipContent(string FileZipPath, string password, string OutputFolder);
    }
}
