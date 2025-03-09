import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

// Import Containers
import { DefaultLayoutComponent } from './containers';

import { P404Component } from './views/error/404.component';
import { P500Component } from './views/error/500.component';
import { LoginComponent } from './views/login/login.component';
// import { LanguageComponent } from './views/language/language.component';
// import { OrderComponent } from './views/order/order.component';
// import { ProductComponent } from './views/product/product.component'
// import { CustomerComponent } from './views/customer/customer.component';
// import { SlideComponent } from './views/slide/slide.component';
// import { MaterialComponent } from './views/material/material.component';

import { MenuComponent } from './views/category/menu/menu.component';
import { NewsComponent } from './views/category/news/news.component';
import { PageComponent } from './views/category/page/page.component';
import { CateProductComponent } from './views/category/product/product.component';
import { ManufacturerComponent } from './views/category/manufacturer/manufacturer.component';
import { RankComponent } from './views/category/rank/rank.component';
import { TrademarkComponent } from './views/category/trademark/trademark.component';

import { ConfigThumbComponent } from './views/config/config-thumb/config-thumb.component';
import { ConfigGeneralComponent } from './views/config/config-general/config-general.component';
import { ConfigTableComponent } from './views/config/config-table/config-table.component';

import { NewsTextComponent } from './views/content/news-text/news-text.component';
import { BlockComponent } from './views/content/block/block.component';
import { CommentComponent } from './views/content/comment/comment.component';

import { CustomerComponent } from './views/customer/customer.component';

import { DashboardComponent } from './views/dashboard/dashboard.component';

import { CompanyComponent } from './views/general/company/company.component';
import { WebsiteComponent } from './views/general/website/website.component';
import { BankComponent } from './views/general/bank/bank.component';
import { DepartmentComponent } from './views/general/department/department.component';
import { PositionComponent } from './views/general/position/position.component';
import { TypeAttributeComponent } from './views/general/type-attribute/type-attribute.component';

import { LanguageComponent } from './views/language/language.component';

import { MaterialComponent } from './views/material/material.component';

import { OrderComponent } from './views/order/order.component';

import { ProductComponent } from './views/product/product.component';

import { SlideComponent } from './views/slide/slide.component';

import { FunctionComponent } from './views/system/function/function.component';
import { RoleComponent } from './views/system/role/role.component';

import { UserComponent } from './views/user/user.component';

import { PartnerComponent } from './views/category/partner/partner.component';

import { BranchComponent } from './views/general/branch/branch.component';

import { ServiceComponent } from './views/service/service.component';

import { ContactComponent } from './views/general/contact/contact.component';

import { AttribuiteComponent } from './views/general/attribuite/attribuite.component';

import { ReviewProductComponent } from './views/review-product/review-product.component';

import { AuthGuard } from './auth.guard';
import { ImportManagement } from './views/importmanagement/importmanagement.component';
import { KhachHangComponent } from './views/quanlykhachhang/khachhang.component';
import { XeComponent } from './views/quanlyxe/xe.component';
import { QuanLyCapPhoiComponent } from './views/quanlycapphoi/quanlycapphoi.component';
import { SanPhamComponent } from './views/quanlysanpham/sanpham.component';
import { NhanVienComponent } from './views/quanlynhanvien/nhanvien.component';
import { PhuGiaComponent } from './views/quanlyphugia/phugia.component';
import { DuAnComponent } from './views/quanlyduan/duan.component';
import { ThongKeDonHangChiTietComponent } from './views/thongkedonhangchitiet/thongkedonhangchitiet.component';
import { ThongKeDonHangTongHopComponent } from './views/thongkedonhangtonghop/thongkedonhangtonghop.component';
import { ThongKeTongVatTuComponent } from './views/thongketongvattu/thongketongvattu.component';
import { ThongKeChiTietChuyenXeComponent } from './views/thongkechitietchuyenxe/thongkechitietchuyenxe.component';
import { ThongKeChiTietKhoiLuongBeTongComponent } from './views/thongkechitietkhoiluongbetong/thongkechitietkhoiluongbetong.component';
import { ThongKeTongKhoiLuongBeTongComponent } from './views/thongketongkhoiluongbetong/thongketongkhoiluongbetong.component';
import { ThongKeChiTietVatTuComponent } from './views/thongkechitietvattu/thongkechitietvattu.component';
import { BaoCaoXuatKhoComponent } from './views/baocaoxuatkho/baocaoxuatkho.component';
import { ThongKeChiTietVatTuTheoXeTronComponent } from './views/thongkechitietvattutheoxetron/thongkechitietvattutheoxetron.component';
export const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },
  {
    path: 'login',
    component: LoginComponent,
    data: {
      title: 'Đăng nhập'
    },
    canActivate: [AuthGuard]
  },
  {
    path: '',
    component: DefaultLayoutComponent,
    data: {
      title: ''
    },
    children: [
      {
        path: 'dashboard',
        component: DashboardComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Dashboard'
        }
      },
      {
        path: 'quanly/company',
        component: CompanyComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Quản lý doanh nghiệp'
        }
      },
      {
        path: 'system/function',
        component: FunctionComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Quản lý chức năng'
        }
      },
      {
        path: 'system/role',
        component: RoleComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Quản lý phân quyền'
        }
      },
      {
        path: 'system/user',
        component: UserComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Người dùng'
        }
      },
      {
        path: 'quanly/branch',
        component: BranchComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Quản lý trạm trộn'
        }
      },
      {
        path: 'report/rpdonhang',
        component: OrderComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Báo cáo đơn hàng'
        }
      },
      {
        path: 'report/slide',
        component: SlideComponent,
        canActivate: [AuthGuard],
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
        component: BankComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Phần mềm quản lý xe'
        }
      },
      {
        path: 'contact',
        component: ContactComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Quản lý Camera'
        }
      },
      {
        path: 'customer',
        component: CustomerComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Quản lý cấp phối'
        }
      },

      {
        path: 'quanly/quanlycapphoi',
        component: QuanLyCapPhoiComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Quản lý cấp phối'
        }
      },
      {
        path: 'quanly/dathang',
        component: SanPhamComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Quản lý hợp đồng'
        }
      },
      {
        path: 'danhmuc/quanlyxe',
        component: XeComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Quản lý xe'
        }
      },
      {
        path: 'danhmuc/quanlyphugia',
        component: PhuGiaComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Quản lý vật liệu'
        }
      },
      {
        path: 'danhmuc/duan',
        component: DuAnComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Quản lý dự án'
        }
      },
      {
        path: 'danhmuc/quanlykhachhang',
        component: KhachHangComponent,
        data: {
          title: 'Quản lý khách hàng'
        },
        canActivate: [AuthGuard]
      },
      {
        path: 'danhmuc/quanlynhanvien',
        component: NhanVienComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Quản lý nhân viên'
        }
      },

      {
        path: 'danhmuc',
        component: DefaultLayoutComponent,
        data: {
          title: ''
        }
      },

      {
        path: 'report/thongkedonhangchitiet',
        component: ThongKeDonHangChiTietComponent,
        data: {
          title: 'Thống kê chi tiết hợp đồng'
        },
        canActivate: [AuthGuard]
      },
      {
        path: 'report/thongkedonhangtonghop',
        component: ThongKeDonHangTongHopComponent,
        data: {
          title: 'Thống kê tổng hợp đồng'
        },
        canActivate: [AuthGuard]
      },
      {
        path: 'report/thongketongvattu',
        component: ThongKeTongVatTuComponent,
        data: {
          title: 'Thống kê tổng vật tư'
        },
        canActivate: [AuthGuard]
      },
      {
        path: 'report/thongkechitietchuyenxe',
        component: ThongKeChiTietChuyenXeComponent,
        data: {
          title: 'Thống kê chi tiết chuyển xe'
        },
        canActivate: [AuthGuard]
      },
      {
        path: 'report/thongkechitietkhoiluongbetong',
        component: ThongKeChiTietKhoiLuongBeTongComponent,
        data: {
          title: 'Thống kê chi tiết khối lượng bê tông'
        },
        canActivate: [AuthGuard]
      },
      {
        path: 'report/thongketongkhoiluongbetong',
        component: ThongKeTongKhoiLuongBeTongComponent,
        data: {
          title: 'Thống kê tổng khối lượng bê tông'
        },
        canActivate: [AuthGuard]
      },
      {
        path: 'report/chitietvattu',
        component: ThongKeChiTietVatTuComponent,
        data: {
          title: 'Thống kê chi tiết vật tư'
        },
        canActivate: [AuthGuard]
      },
      {
        path: 'report/baocaoxuatkho',
        component: BaoCaoXuatKhoComponent,
        data: {
          title: 'Báo cáo xuất kho'
        },
        canActivate: [AuthGuard]
      },
      {
        path: 'report/chitietvattutheoxetron',
        component: ThongKeChiTietVatTuTheoXeTronComponent,
        data: {
          title: 'Thống kê chi tiết vật tư theo xe trộn'
        },
        canActivate: [AuthGuard]
      },
    ]
  },
  {
    path: '**',
    component: P404Component,
    pathMatch: 'full'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { useHash: false })],
  exports: [RouterModule],
  providers: []
})
export class AppRoutingModule { }
