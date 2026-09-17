using Scholar.Enums;
using Scholar.Models;
using Scholar.Models.ViewModels;

namespace Scholar.Services
{
    public interface ITestService
    {
        bool IsSuperAdmin { get; }

        Task<List<PaperCard>> GetPapersAsync(string? search);

        Task<PaperBuilderViewModel?> BuildPaperFormAsync(int[] selectedTopicIds, int subjectId, int? instituteId);

        Task<List<Question>> SearchQuestionsAsync(int[] topicIds, QuestionType type, QuestionCategory[] categories);

        Task<PaperSectionsViewModel?> RenderSectionsAsync(RenderSectionsRequest? request);

        Task<PaperDocumentViewModel?> RenderCanvasAsync(RenderCanvasRequest? request);

        Task<int?> SaveAsync(SavePaperRequest? request);

        Task<PaperDocumentViewModel?> GetPrintModelAsync(int id);
    }
}
