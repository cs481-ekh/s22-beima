namespace BEIMA.Backend.MongoService
{
    /// <summary>
    /// Helps to store the instance of the Mongo client being used.
    /// </summary>
    /// <remarks>
    /// The purpose of this class is to allow us to switch out the default
    /// Mongo connector with a mock Mongo connector for testing.
    /// </remarks>
    public static class MongoDefinition
    {
        /// <summary>
        /// Gets and sets the currently defined instance of the Mongo connector
        /// </summary>
        public static IMongoConnector MongoInstance { get; set; } = MongoConnector.Instance;
    }
}
