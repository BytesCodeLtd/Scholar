namespace Scholar.Models.ViewModels
{
    // SuperAdmin management screen: the upload form plus the existing library.
    public class ManagePastPapersViewModel
    {
        public IEnumerable<Board> Boards { get; set; } = new List<Board>();

        public UploadPastPaperViewModel Upload { get; set; } = new();
    }
}
