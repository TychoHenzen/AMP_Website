namespace ProjectEditor;

public class ProjectInfo
{
    public bool Finished { get; set; } = false;
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string FullDescription { get; set; } = "";
    public List<MediaItem> MediaItems { get; set; } = new();
    public string SourceUrl { get; set; } = "";
    public List<string> Tags { get; set; } = new();
    public string Category { get; set; } = "Uncategorized";
    public ImageFitType ImageFit { get; set; } = ImageFitType.Cover;
}

public class MediaItem
{
    public string Url { get; set; } = "";
    public string Name { get; set; } = "";
    public MediaType Type { get; set; }
}

public enum MediaType
{
    Image,
    Video,
    Pdf
}

public enum ImageFitType
{
    Cover,
    Contain
}
