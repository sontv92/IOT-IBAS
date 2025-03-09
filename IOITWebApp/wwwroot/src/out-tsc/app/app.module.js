"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __spreadArrays = (this && this.__spreadArrays) || function () {
    for (var s = 0, i = 0, il = arguments.length; i < il; i++) s += arguments[i].length;
    for (var r = Array(s), k = 0, i = 0; i < il; i++)
        for (var a = arguments[i], j = 0, jl = a.length; j < jl; j++, k++)
            r[k] = a[j];
    return r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var platform_browser_1 = require("@angular/platform-browser");
var core_1 = require("@angular/core");
var common_1 = require("@angular/common");
var ngx_perfect_scrollbar_1 = require("ngx-perfect-scrollbar");
var ngx_cookie_service_1 = require("ngx-cookie-service");
var auth_guard_1 = require("./auth.guard");
var forms_1 = require("@angular/forms");
var http_1 = require("@angular/common/http");
var ngx_modal_dialog_1 = require("ngx-modal-dialog");
var modal_1 = require("ngx-bootstrap/modal");
var ng4_loading_spinner_1 = require("ng4-loading-spinner");
var DEFAULT_PERFECT_SCROLLBAR_CONFIG = {
    suppressScrollX: true
};
var app_component_1 = require("./app.component");
// Import containers
var containers_1 = require("./containers");
var _404_component_1 = require("./views/error/404.component");
var _500_component_1 = require("./views/error/500.component");
var login_component_1 = require("./views/login/login.component");
var APP_CONTAINERS = [
    containers_1.DefaultLayoutComponent
];
var angular_1 = require("@coreui/angular");
// Import routing module
var app_routing_1 = require("./app.routing");
// Import 3rd party components
var dropdown_1 = require("ngx-bootstrap/dropdown");
var tabs_1 = require("ngx-bootstrap/tabs");
var ng2_charts_1 = require("ng2-charts");
var ngx_toastr_1 = require("ngx-toastr");
var animations_1 = require("@angular/platform-browser/animations");
var http_client_1 = require("@ngx-loading-bar/http-client");
var pagination_1 = require("ngx-bootstrap/pagination");
var ng2_ckeditor_1 = require("ng2-ckeditor");
var ng_select_1 = require("@ng-select/ng-select");
var tooltip_1 = require("ngx-bootstrap/tooltip");
var buttons_1 = require("ngx-bootstrap/buttons");
var pre_cli_directive_1 = require("./directive/preventClick/pre-cli.directive");
var ol_component_1 = require("./directive/ol/ol.component");
var menu_component_1 = require("./views/category/menu/menu.component");
var news_component_1 = require("./views/category/news/news.component");
var page_component_1 = require("./views/category/page/page.component");
var product_component_1 = require("./views/category/product/product.component");
var manufacturer_component_1 = require("./views/category/manufacturer/manufacturer.component");
var rank_component_1 = require("./views/category/rank/rank.component");
var trademark_component_1 = require("./views/category/trademark/trademark.component");
var config_thumb_component_1 = require("./views/config/config-thumb/config-thumb.component");
var config_general_component_1 = require("./views/config/config-general/config-general.component");
var config_table_component_1 = require("./views/config/config-table/config-table.component");
var news_text_component_1 = require("./views/content/news-text/news-text.component");
var block_component_1 = require("./views/content/block/block.component");
var comment_component_1 = require("./views/content/comment/comment.component");
var customer_component_1 = require("./views/customer/customer.component");
var dashboard_component_1 = require("./views/dashboard/dashboard.component");
var company_component_1 = require("./views/general/company/company.component");
var website_component_1 = require("./views/general/website/website.component");
var bank_component_1 = require("./views/general/bank/bank.component");
var department_component_1 = require("./views/general/department/department.component");
var position_component_1 = require("./views/general/position/position.component");
var type_attribute_component_1 = require("./views/general/type-attribute/type-attribute.component");
var language_component_1 = require("./views/language/language.component");
var material_component_1 = require("./views/material/material.component");
var order_component_1 = require("./views/order/order.component");
var product_component_2 = require("./views/product/product.component");
var slide_component_1 = require("./views/slide/slide.component");
var importmanagement_component_1 = require("./views/importmanagement/importmanagement.component");
var function_component_1 = require("./views/system/function/function.component");
var role_component_1 = require("./views/system/role/role.component");
var user_component_1 = require("./views/user/user.component");
var ng_pick_datetime_1 = require("ng-pick-datetime");
var ng2_currency_mask_1 = require("ng2-currency-mask");
var partner_component_1 = require("./views/category/partner/partner.component");
var branch_component_1 = require("./views/general/branch/branch.component");
var service_component_1 = require("./views/service/service.component");
var contact_component_1 = require("./views/general/contact/contact.component");
var ngx_sortable_1 = require("ngx-sortable");
var ol_category_component_1 = require("./directive/ol-category/ol-category.component");
var check_box_component_1 = require("./directive/check-box/check-box.component");
var attribuite_component_1 = require("./views/general/attribuite/attribuite.component");
var truncate_pipe_1 = require("./pipe/truncate.pipe");
var review_product_component_1 = require("./views/review-product/review-product.component");
var khachhang_component_1 = require("./views/quanlykhachhang/khachhang.component");
var xe_component_1 = require("./views/quanlyxe/xe.component");
var quanlycapphoi_component_1 = require("./views/quanlycapphoi/quanlycapphoi.component");
var sanpham_component_1 = require("./views/quanlysanpham/sanpham.component");
var nhanvien_component_1 = require("./views/quanlynhanvien/nhanvien.component");
var phugia_component_1 = require("./views/quanlyphugia/phugia.component");
var duan_component_1 = require("./views/quanlyduan/duan.component");
var thongkedonhangchitiet_component_1 = require("./views/thongkedonhangchitiet/thongkedonhangchitiet.component");
var thongkedonhangtonghop_component_1 = require("./views/thongkedonhangtonghop/thongkedonhangtonghop.component");
var employeedetails_component_1 = require("./employeedetails/employeedetails.component");
var AppModule = /** @class */ (function () {
    function AppModule() {
    }
    AppModule = __decorate([
        core_1.NgModule({
            imports: [
                platform_browser_1.BrowserModule,
                app_routing_1.AppRoutingModule,
                angular_1.AppAsideModule,
                angular_1.AppBreadcrumbModule.forRoot(),
                angular_1.AppFooterModule,
                angular_1.AppHeaderModule,
                angular_1.AppSidebarModule,
                ngx_perfect_scrollbar_1.PerfectScrollbarModule,
                dropdown_1.BsDropdownModule.forRoot(),
                tabs_1.TabsModule.forRoot(),
                ng2_charts_1.ChartsModule,
                forms_1.FormsModule,
                forms_1.ReactiveFormsModule,
                http_1.HttpClientModule,
                animations_1.BrowserAnimationsModule,
                ngx_toastr_1.ToastrModule.forRoot(),
                http_client_1.LoadingBarHttpClientModule,
                ngx_modal_dialog_1.ModalDialogModule.forRoot(),
                modal_1.ModalModule.forRoot(),
                pagination_1.PaginationModule.forRoot(),
                ng_select_1.NgSelectModule,
                tooltip_1.TooltipModule.forRoot(),
                ng2_ckeditor_1.CKEditorModule,
                buttons_1.ButtonsModule,
                ng_pick_datetime_1.OwlDateTimeModule,
                ng_pick_datetime_1.OwlNativeDateTimeModule,
                ng2_currency_mask_1.CurrencyMaskModule,
                ngx_sortable_1.NgxSortableModule,
                ng4_loading_spinner_1.Ng4LoadingSpinnerModule.forRoot()
            ],
            declarations: __spreadArrays([
                app_component_1.AppComponent
            ], APP_CONTAINERS, [
                _404_component_1.P404Component,
                _500_component_1.P500Component,
                login_component_1.LoginComponent,
                menu_component_1.MenuComponent,
                news_component_1.NewsComponent,
                page_component_1.PageComponent,
                product_component_1.CateProductComponent,
                manufacturer_component_1.ManufacturerComponent,
                rank_component_1.RankComponent,
                trademark_component_1.TrademarkComponent,
                config_thumb_component_1.ConfigThumbComponent,
                config_general_component_1.ConfigGeneralComponent,
                config_table_component_1.ConfigTableComponent,
                news_text_component_1.NewsTextComponent,
                block_component_1.BlockComponent,
                comment_component_1.CommentComponent,
                customer_component_1.CustomerComponent,
                dashboard_component_1.DashboardComponent,
                company_component_1.CompanyComponent,
                website_component_1.WebsiteComponent,
                bank_component_1.BankComponent,
                department_component_1.DepartmentComponent,
                position_component_1.PositionComponent,
                type_attribute_component_1.TypeAttributeComponent,
                language_component_1.LanguageComponent,
                material_component_1.MaterialComponent,
                order_component_1.OrderComponent,
                product_component_2.ProductComponent,
                slide_component_1.SlideComponent,
                importmanagement_component_1.ImportManagement,
                function_component_1.FunctionComponent,
                role_component_1.RoleComponent,
                user_component_1.UserComponent,
                ol_component_1.OlComponent,
                pre_cli_directive_1.PreCliDirective,
                partner_component_1.PartnerComponent,
                branch_component_1.BranchComponent,
                service_component_1.ServiceComponent,
                contact_component_1.ContactComponent,
                ol_category_component_1.OlCategoryComponent,
                check_box_component_1.CheckBoxComponent,
                attribuite_component_1.AttribuiteComponent,
                truncate_pipe_1.TruncatePipe,
                review_product_component_1.ReviewProductComponent,
                khachhang_component_1.KhachHangComponent,
                xe_component_1.XeComponent,
                quanlycapphoi_component_1.QuanLyCapPhoiComponent,
                sanpham_component_1.SanPhamComponent,
                nhanvien_component_1.NhanVienComponent,
                phugia_component_1.PhuGiaComponent,
                duan_component_1.DuAnComponent,
                thongkedonhangchitiet_component_1.ThongKeDonHangChiTietComponent,
                employeedetails_component_1.EmployeedetailsComponent,
                thongkedonhangtonghop_component_1.ThongKeDonHangTongHopComponent
            ]),
            exports: [pre_cli_directive_1.PreCliDirective],
            providers: [auth_guard_1.AuthGuard, ngx_cookie_service_1.CookieService, common_1.DatePipe, { provide: common_1.APP_BASE_HREF, useValue: '' }],
            bootstrap: [app_component_1.AppComponent]
        })
    ], AppModule);
    return AppModule;
}());
exports.AppModule = AppModule;
//# sourceMappingURL=app.module.js.map