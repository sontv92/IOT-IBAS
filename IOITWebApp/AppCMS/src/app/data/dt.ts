export class UserInfo {
    userId: number;
    access_token: string;
}

export class UserChangePass {
    UserId: number;
    PasswordOld: string;
    PasswordOldE: string;
    PasswordNew: string;
    PasswordNewE: string;
    UserName: string;
    Avatar: string;
    FullName: string;
    ConfirmPassword: string;
}

export class Paging {
    page: number;
    page_size: number;
    query: string;
    order_by: string;
    item_count: number;
    select: string;
}
export class tablesilde {
    name: string;
    nametable: string;
    nameget: string;
}

export class QueryFilter {
    txtSearch: string;
    // ConferenceId: number;
    Type: number;
    // TypeDeputy: number;
    Title: number;
    // CountryId: number;
    TypeAttributeId: number;
    // SessionId: number;
    // StatusConference: number;
    TypeFormId: number;
    // TypeStatusMail: number;
    UserId: number;
    ManufacturerId: number;
    TrademarkId: number;
    TypeOrderStatus: number;
    TypeContactId: any;
    TypePaymentOrderStatus: number;
    CompanyId: number;
    BranchId: number;
    Branchlist: [];
    fromdate: string;
    todate: string;
    typeTram: number;
}
