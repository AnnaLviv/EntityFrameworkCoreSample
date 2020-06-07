namespace EntityFrameworkCoreSample.Domain
{
    /// <summary>
    /// Join entity class for many-to-many relation between Samurais and Battles.
    /// With current version of EF (3.1.4) this is the required step.
    /// In later versions many-to-many relation will be supported without such join class.    
    /// </summary>
    public class SamuraiBattle
    {
        /// <summary>
        /// Required key value.
        /// </summary>
        public int SamuraiId { get; set; }
        /// <summary>
        /// Required key value.
        /// </summary>
        public int BattleId { get; set; }
        public Samurai Samurai { get; set; }
        public Battle Battle { get; set; }
    }
}
