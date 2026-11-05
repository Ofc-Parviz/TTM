using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamTaskManagement.Domain.Enums;

namespace TeamTaskManagement.Domain.Shared {
    public static class DomainConstants {

        /// <summary>
        /// وضعیت های وظیفه که بعنوان بسته بودن آن وظیفه شناخته میشوند
        /// </summary>
        public static readonly HashSet<TeamTaskStatus> TeamTaskClosedStatuses = new() {

            TeamTaskStatus.ClosedAsFailure,
            TeamTaskStatus.ClosedAsDeadlineFailure,
            TeamTaskStatus.ClosedAsComplete
        };

        /// <summary>
        /// وضعیت های وظیفه که بعنوان باز بودن آن وظیفه شناخته میشوند
        /// </summary>
        public static readonly HashSet<TeamTaskStatus> TeamTaskOpenStatuses = new() {

            TeamTaskStatus.Pending,
            TeamTaskStatus.OnGoing,
            TeamTaskStatus.ClaimedAsComplete,
            TeamTaskStatus.ClaimedAsFailure,
            TeamTaskStatus.OrderedToTryAgain,
            TeamTaskStatus.AcceptedToTryAgain,
            TeamTaskStatus.RejectedToTryAgain,
        };
    }
}
