﻿namespace NetCorePal.D3Shop.Admin.Shared.Responses;

public record RolePermissionResponse(
    string Code,
    string GroupName,
    string Remark,
    bool IsAssigned);