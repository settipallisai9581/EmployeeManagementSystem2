namespace EmployeeManagementSystem2.Server.DTOs;

public record EmployeeReportDto(
    int TotalEmployees,
    int ActiveEmployees,
    int InactiveEmployees,
    List<DepartmentSummary> DepartmentSummaries,
    List<RoleSummary> RoleSummaries,
    decimal AverageSalary,
    decimal TotalSalary
);

public record DepartmentSummary(
    string DepartmentName,
    int EmployeeCount
);

public record RoleSummary(
    string RoleName,
    int EmployeeCount
);
