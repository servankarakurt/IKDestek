using HRSupport.Domain.Entites;
using HRSupport.Domain.Enum;
using System;

namespace HRSupport.Application.Services
{
    public class LeaveRequestService
    {
        public bool ProcessLeaveRequest(LeaveRequest request, EmployeeLeaveBalance currentBalance)
        {
            bool isExtremeCase = request.Type == LeaveType.Hastalık ||
                                 request.Type == LeaveType.Doğum ||
                                 request.Type == LeaveType.Ölüm;

            if (isExtremeCase)
            { 
                if (currentBalance.RemainingAnnualLeaveDays <= 0)
                {
                    request.IsUrgentWithoutBalance = true; 
                }
                return true;
            }
            else
            {
                if (currentBalance.RemainingAnnualLeaveDays >= request.RequestedDays)
                {
                    currentBalance.RemainingAnnualLeaveDays -= request.RequestedDays;
                    return true; 
                }
                else
                {
                    throw new Exception("Yeterli yıllık izin bakiyeniz bulunmamaktadır.");
                }
            }
        }
    }
}