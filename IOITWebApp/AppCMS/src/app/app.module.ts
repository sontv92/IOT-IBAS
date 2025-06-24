import { BrowserModule } from '@angular/platform-browser';
import { NgModule  } from '@angular/core';
import { APP_BASE_HREF, LocationStrategy, HashLocationStrategy, DatePipe } from '@angular/common';

import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { PERFECT_SCROLLBAR_CONFIG } from 'ngx-perfect-scrollbar';
import { PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';

import { CookieService } from 'ngx-cookie-service';
import { AuthGuard } from './auth.guard';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { ModalDialogModule } from 'ngx-modal-dialog';
import { ModalModule } from 'ngx-bootstrap/modal';

 import { Ng4LoadingSpinnerModule } from 'ng4-loading-spinner';

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
    suppressScrollX: true
};

import { AppComponent } from './app.component';

// Import containers
import { DefaultLayoutComponent } from './containers';

import { P404Component } from './views/error/404.component';
import { P500Component } from './views/error/500.component';
import { LoginComponent } from './views/login/login.component';

const APP_CONTAINERS = [
    DefaultLayoutComponent
];

import {
    AppAsideModule,
    AppBreadcrumbModule,
    AppHeaderModule,
    AppFooterModule,
    AppSidebarModule,

} from '@coreui/angular';

// Import routing module
import { AppRoutingModule } from './app.routing';

import 'chartjs-plugin-labels';
// Import 3rd party components
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { ChartsModule } from 'ng2-charts';
import { ToastrModule } from 'ngx-toastr';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LoadingBarHttpClientModule } from '@ngx-loading-bar/http-client';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { CKEditorModule } from 'ng2-ckeditor';
import { NgSelectModule } from '@ng-select/ng-select';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import {TableModule} from 'primeng/table';
import {ToastModule} from 'primeng/toast';
import {CalendarModule} from 'primeng/calendar';
import {SliderModule} from 'primeng/slider';
import {MultiSelectModule} from 'primeng/multiselect';
import {ContextMenuModule} from 'primeng/contextmenu';
import {DialogModule} from 'primeng/dialog';
import {ButtonModule} from 'primeng/button';
import {DropdownModule} from 'primeng/dropdown';
import {ProgressBarModule} from 'primeng/progressbar';
import {InputTextModule} from 'primeng/inputtext';
import { PanelModule } from 'primeng/panel';




import { PreCliDirective } from './directive/preventClick/pre-cli.directive';
import { OlComponent } from './directive/ol/ol.component';

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
import { ImportManagement } from './views/importmanagement/importmanagement.component';

import { FunctionComponent } from './views/system/function/function.component';
import { RoleComponent } from './views/system/role/role.component';

import { UserComponent } from './views/user/user.component';
import { OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime';
import { CurrencyMaskModule } from "ng2-currency-mask";
import { PartnerComponent } from './views/category/partner/partner.component';
import { BranchComponent } from './views/general/branch/branch.component';
import { ServiceComponent } from './views/service/service.component';
import { ContactComponent } from './views/general/contact/contact.component';
import { NgxSortableModule } from 'ngx-sortable';
import { OlCategoryComponent } from './directive/ol-category/ol-category.component';
import { CheckBoxComponent } from './directive/check-box/check-box.component';
import { AttribuiteComponent } from './views/general/attribuite/attribuite.component';
import { TruncatePipe } from './pipe/truncate.pipe';
import { ReviewProductComponent } from './views/review-product/review-product.component';

import { KhachHangComponent } from './views/quanlykhachhang/khachhang.component';
import { XeComponent } from './views/quanlyxe/xe.component';
import { QuanLyCapPhoiComponent } from './views/quanlycapphoi/quanlycapphoi.component';
import { SanPhamComponent } from './views/quanlysanpham/sanpham.component';
import { NhanVienComponent } from './views/quanlynhanvien/nhanvien.component';
import { PhuGiaComponent } from './views/quanlyphugia/phugia.component';
import { DuAnComponent } from './views/quanlyduan/duan.component';
import { ThongKeDonHangChiTietComponent } from './views/thongkedonhangchitiet/thongkedonhangchitiet.component';
import { ThongKeDonHangTongHopComponent } from './views/thongkedonhangtonghop/thongkedonhangtonghop.component';
import { EmployeedetailsComponent } from './employeedetails/employeedetails.component';
import { ThongKeTongVatTuComponent } from './views/thongketongvattu/thongketongvattu.component';
import { ThongKeChiTietChuyenXeComponent } from './views/thongkechitietchuyenxe/thongkechitietchuyenxe.component';
import { ThongKeChiTietKhoiLuongBeTongComponent } from './views/thongkechitietkhoiluongbetong/thongkechitietkhoiluongbetong.component';
import { ThongKeTongKhoiLuongBeTongComponent } from './views/thongketongkhoiluongbetong/thongketongkhoiluongbetong.component';
import { ThongKeChiTietVatTuComponent } from './views/thongkechitietvattu/thongkechitietvattu.component';
import { BaoCaoXuatKhoComponent } from './views/baocaoxuatkho/baocaoxuatkho.component';
import { ThongKeChiTietVatTuTheoXeTronComponent } from './views/thongkechitietvattutheoxetron/thongkechitietvattutheoxetron.component';
import { TC_BaoCaoTramCanComponent } from './views/tc_baocaotramcan/tc_baocaotramcan.component';
import { CanvasJSChart } from './canvasjs.angular.component';
@NgModule({
    imports: [
        BrowserModule,
        AppRoutingModule,
        AppAsideModule,
        AppBreadcrumbModule.forRoot(),
        AppFooterModule,
        AppHeaderModule,
        AppSidebarModule,
        PerfectScrollbarModule,
        BsDropdownModule.forRoot(),
        TabsModule.forRoot(),
        ChartsModule,
        FormsModule,
        ReactiveFormsModule,
        HttpClientModule,
        BrowserAnimationsModule,
        ToastrModule.forRoot(),
        LoadingBarHttpClientModule,
        ModalDialogModule.forRoot(),
        ModalModule.forRoot(),
        PaginationModule.forRoot(),
        NgSelectModule,
        TooltipModule.forRoot(),
        CKEditorModule,
        ButtonsModule,
        OwlDateTimeModule,
        OwlNativeDateTimeModule,
        CurrencyMaskModule,
        NgxSortableModule,
        Ng4LoadingSpinnerModule.forRoot(),
        TableModule,
        CalendarModule,
		SliderModule,
		DialogModule,
		MultiSelectModule,
		ContextMenuModule,
		DropdownModule,
		ButtonModule,
		ToastModule,
        InputTextModule,
        PanelModule,
        ProgressBarModule
    ],
    declarations: [
        AppComponent,
        ...APP_CONTAINERS,
        P404Component,
        P500Component,
        LoginComponent,
        MenuComponent,
        NewsComponent,
        PageComponent,
        CateProductComponent,
        ManufacturerComponent,
        RankComponent,
        TrademarkComponent,
        ConfigThumbComponent,
        ConfigGeneralComponent,
        ConfigTableComponent,
        NewsTextComponent,
        BlockComponent,
        CommentComponent,
        CustomerComponent,
        DashboardComponent,
        CompanyComponent,
        WebsiteComponent,
        BankComponent,
        DepartmentComponent,
        PositionComponent,
        TypeAttributeComponent,
        LanguageComponent,
        MaterialComponent,
        OrderComponent,
        ProductComponent,
        SlideComponent,
        ImportManagement,
        FunctionComponent,
        RoleComponent,
        UserComponent,
        OlComponent,
        PreCliDirective,
        PartnerComponent,
        BranchComponent,
        ServiceComponent,
        ContactComponent,
        OlCategoryComponent,
        CheckBoxComponent,
        AttribuiteComponent,
        TruncatePipe,
        ReviewProductComponent,
        KhachHangComponent,
        XeComponent,
        QuanLyCapPhoiComponent,
        SanPhamComponent,
        NhanVienComponent,
        PhuGiaComponent,
        DuAnComponent,
        ThongKeDonHangChiTietComponent,
        EmployeedetailsComponent,
        ThongKeDonHangTongHopComponent,
        ThongKeTongVatTuComponent,
        ThongKeChiTietChuyenXeComponent,
        ThongKeChiTietKhoiLuongBeTongComponent,
        ThongKeTongKhoiLuongBeTongComponent,
        ThongKeChiTietVatTuComponent,
        BaoCaoXuatKhoComponent,
        ThongKeChiTietVatTuTheoXeTronComponent,
        TC_BaoCaoTramCanComponent,
        CanvasJSChart
    ],
    exports: [PreCliDirective],
    providers: [AuthGuard, CookieService, DatePipe, { provide: APP_BASE_HREF, useValue: '' }],
    bootstrap: [AppComponent]
})
export class AppModule { }
