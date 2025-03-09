"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var router_1 = require("@angular/router");
// Import Containers
var containers_1 = require("./containers");
var _404_component_1 = require("./views/error/404.component");
var login_component_1 = require("./views/login/login.component");
var customer_component_1 = require("./views/customer/customer.component");
var dashboard_component_1 = require("./views/dashboard/dashboard.component");
var company_component_1 = require("./views/general/company/company.component");
var bank_component_1 = require("./views/general/bank/bank.component");
var order_component_1 = require("./views/order/order.component");
var slide_component_1 = require("./views/slide/slide.component");
var function_component_1 = require("./views/system/function/function.component");
var role_component_1 = require("./views/system/role/role.component");
var user_component_1 = require("./views/user/user.component");
var branch_component_1 = require("./views/general/branch/branch.component");
var contact_component_1 = require("./views/general/contact/contact.component");
var auth_guard_1 = require("./auth.guard");
var khachhang_component_1 = require("./views/quanlykhachhang/khachhang.component");
var xe_component_1 = require("./views/quanlyxe/xe.component");
var quanlycapphoi_component_1 = require("./views/quanlycapphoi/quanlycapphoi.component");
var sanpham_component_1 = require("./views/quanlysanpham/sanpham.component");
var nhanvien_component_1 = require("./views/quanlynhanvien/nhanvien.component");
var phugia_component_1 = require("./views/quanlyphugia/phugia.component");
var duan_component_1 = require("./views/quanlyduan/duan.component");
var thongkedonhangchitiet_component_1 = require("./views/thongkedonhangchitiet/thongkedonhangchitiet.component");
var thongkedonhangtonghop_component_1 = require("./views/thongkedonhangtonghop/thongkedonhangtonghop.component");
exports.routes = [
    {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
    },
    {
        path: 'login',
        component: login_component_1.LoginComponent,
        data: {
            title: 'Đăng nhập'
        },
        canActivate: [auth_guard_1.AuthGuard]
    },
    {
        path: '',
        component: containers_1.DefaultLayoutComponent,
        data: {
            title: ''
        },
        children: [
            {
                path: 'dashboard',
                component: dashboard_component_1.DashboardComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Dashboard'
                }
            },
            {
                path: 'quanly/company',
                component: company_component_1.CompanyComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Quản lý doanh nghiệp'
                }
            },
            {
                path: 'system/function',
                component: function_component_1.FunctionComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Quản lý chức năng'
                }
            },
            {
                path: 'system/role',
                component: role_component_1.RoleComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Quản lý phân quyền'
                }
            },
            {
                path: 'system/user',
                component: user_component_1.UserComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Người dùng'
                }
            },
            {
                path: 'quanly/branch',
                component: branch_component_1.BranchComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Quản lý trạm trộn'
                }
            },
            {
                path: 'report/rpdonhang',
                component: order_component_1.OrderComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Báo cáo đơn hàng'
                }
            },
            {
                path: 'report/slide',
                component: slide_component_1.SlideComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Thống kê đơn hàng'
                }
            },
            // {
            //   path: 'report/importmanagement',
            //   component: ImportManagement,
            //   canActivate: [AuthGuard],
            //   data: {
            //     title: 'Quản lý kho'
            //   }
            // },
            {
                path: 'bank',
                component: bank_component_1.BankComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Phần mềm quản lý xe'
                }
            },
            {
                path: 'contact',
                component: contact_component_1.ContactComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Quản lý Camera'
                }
            },
            {
                path: 'customer',
                component: customer_component_1.CustomerComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Quản lý cấp phối'
                }
            },
            {
                path: 'quanly/quanlycapphoi',
                component: quanlycapphoi_component_1.QuanLyCapPhoiComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Quản lý cấp phối'
                }
            },
            {
                path: 'quanly/dathang',
                component: sanpham_component_1.SanPhamComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Quản lý hợp đồng'
                }
            },
            {
                path: 'danhmuc/quanlyxe',
                component: xe_component_1.XeComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Quản lý xe'
                }
            },
            {
                path: 'danhmuc/quanlyphugia',
                component: phugia_component_1.PhuGiaComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Quản lý phụ gia'
                }
            },
            {
                path: 'danhmuc/duan',
                component: duan_component_1.DuAnComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Quản lý dự án'
                }
            },
            {
                path: 'danhmuc/quanlykhachhang',
                component: khachhang_component_1.KhachHangComponent,
                data: {
                    title: 'Quản lý khách hàng'
                },
                canActivate: [auth_guard_1.AuthGuard]
            },
            {
                path: 'danhmuc/quanlynhanvien',
                component: nhanvien_component_1.NhanVienComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Quản lý nhân viên'
                }
            },
            {
                path: 'danhmuc',
                component: containers_1.DefaultLayoutComponent,
                data: {
                    title: ''
                }
            },
            {
                path: 'report/thongkedonhangchitiet',
                component: thongkedonhangchitiet_component_1.ThongKeDonHangChiTietComponent,
                data: {
                    title: 'Thống kê đơn hàng chi tiết'
                },
                canActivate: [auth_guard_1.AuthGuard]
            },
            {
                path: 'report/thongkedonhangtonghop',
                component: thongkedonhangtonghop_component_1.ThongKeDonHangTongHopComponent,
                data: {
                    title: 'Thống kê đơn hàng tổng hợp'
                },
                canActivate: [auth_guard_1.AuthGuard]
            },
        ]
    },
    {
        path: '**',
        component: _404_component_1.P404Component,
        pathMatch: 'full'
    }
];
var AppRoutingModule = /** @class */ (function () {
    function AppRoutingModule() {
    }
    AppRoutingModule = __decorate([
        core_1.NgModule({
            imports: [router_1.RouterModule.forRoot(exports.routes, { useHash: false })],
            exports: [router_1.RouterModule],
            providers: []
        })
    ], AppRoutingModule);
    return AppRoutingModule;
}());
exports.AppRoutingModule = AppRoutingModule;
//# sourceMappingURL=app.routing.js.map