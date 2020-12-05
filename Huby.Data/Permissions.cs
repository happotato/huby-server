namespace Huby.Data
{
    public interface IPermissions
    {
        public bool CanDeletePosts { get; }
        public bool CanEdit { get; }
    }

    public sealed class Permissions : IPermissions
    {
        public bool CanDeletePosts { get; set; }
        public bool CanEdit { get; set; }

        public static Permissions Owner = new Permissions
        {
            CanDeletePosts = true,
            CanEdit = true,
        };
    }
}
