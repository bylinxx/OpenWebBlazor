namespace OpenWebBlazor.ViewModels
{
    public class WebMenuTree
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int Sort { get; set; }
        public int ParentId { get; set; }
        public List<ChildItem> Items { get; set; }
        public class ChildItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Path { get; set; }
            public int Sort { get; set; }
        }
    }
}
