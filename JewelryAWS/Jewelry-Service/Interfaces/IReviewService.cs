using Jewelry_Model.Paginate;
using Jewelry_Model.Payload;
using Jewelry_Model.Payload.Request.Review;
using Jewelry_Model.Payload.Response;
using Jewelry_Model.Payload.Response.Review;

namespace Jewelry_Service.Interfaces;

public interface IReviewService
{
    Task<BaseResponse<CreateReviewResponse>> CreateReview(Guid id, CreateReviewRequest createReviewRequest);
    
    Task<BaseResponse<IPaginate<GetReviewResponse>>> GetAllReviews(Guid id, int page, int size);
    
    Task<BaseResponse<bool>> DeleteReview(Guid id);
}