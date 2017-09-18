namespace SimpleSqlMapper
{
    /// <summary>
    /// Supported command types
    /// </summary>
    public enum SqlCommandType
    {
        /// <summary>
        /// Command is a stored procedure
        /// </summary>
        StoredProcedure,
        /// <summary>
        /// Command is plain SQL
        /// </summary>
        QueryText
    }
}