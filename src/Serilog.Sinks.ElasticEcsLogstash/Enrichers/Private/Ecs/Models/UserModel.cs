namespace Serilog.Enrichers.Private.Ecs.Models
{
    /// <summary>
    /// The user fields describe information about the user that is relevant to the event.
    /// Fields can have one entry or multiple entries. If a user has more than one id, provide an array that includes all of them.
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// User email address.
        /// type: keyword
        /// </summary>
        public string Email { get; set; }       

        /// <summary>
        /// User’s full name, if available.
        /// type: keyword
        /// example: Ehsan Maleki Zoeram
        /// </summary>
        public string FullName { get; set; }       

        /// <summary>
        /// Unique user hash to correlate information for a user in anonymize form.
        /// Useful if user.id or user.name contain confidential information and cannot be used.
        /// type: keyword
        /// </summary>
        public string Hash { get; set; }       

        /// <summary>
        /// One or multiple unique identifiers of the user.
        /// type: keyword
        /// </summary>
        public string Id { get; set; }       

        /// <summary>
        /// Short name or login of the user.
        /// type: keyword
        /// example: ehsan
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// User’s group relevant to the event.
        /// </summary>
        public GroupModel Group { get; set; }
    }
}