using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.MemberShipViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IMemberShipService
    {
        Task<IEnumerable<MemberShipViewModel>> GetAllMemberShipsAsync(CancellationToken ct = default);

        Task<Result>CreateMemberShipByIdAsync(CreateMemberShipViewModel model, CancellationToken ct = default);

        Task<IEnumerable<MemberSelectViewModel>> GetMembersForDropDownList(CancellationToken ct = default);
       
        Task<IEnumerable<PlanSelectViewModel>> GetPlansForDropDownList(CancellationToken ct = default);

        Task<Result> DeleteActiveMemberShip(int memberid ,CancellationToken ct = default);
    }
}
