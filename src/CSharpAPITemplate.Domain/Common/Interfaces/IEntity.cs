namespace CSharpAPITemplate.Domain.Common.Interfaces
{
    /// <summary>
    /// Defines id property for database objects.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Unique object identifier.
        /// </summary>
        public long Id { get; set; }
    }
}