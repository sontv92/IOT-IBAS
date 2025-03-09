"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var call_category_function_service_1 = require("../../service/call-category-function.service");
var const_1 = require("../../data/const");
var OlComponent = /** @class */ (function () {
    function OlComponent(callCategoryFunctionService) {
        this.callCategoryFunctionService = callCategoryFunctionService;
        this.domainImage = const_1.domainImage;
        // this.subscription = this.callCategoryFunctionService.getAction().subscribe(action => {
        // 	if (action.TypeAction == 4) {
        // 		this.SaveCategorySort();
        // 	}
        // });
    }
    OlComponent.prototype.ngOnInit = function () {
    };
    OlComponent.prototype.ngOnDestroy = function () {
        // this.subscription.unsubscribe();
    };
    OlComponent.prototype.AddCate = function (CategoryId) {
        this.callCategoryFunctionService.sendAction(CategoryId, 1);
    };
    OlComponent.prototype.UpdateCate = function (CategoryId) {
        this.callCategoryFunctionService.sendAction(CategoryId, 2);
    };
    OlComponent.prototype.DeleteCate = function (CategoryId) {
        this.callCategoryFunctionService.sendAction(CategoryId, 3);
    };
    OlComponent.prototype.SaveCategorySort = function () {
        console.log(this.items);
    };
    __decorate([
        core_1.Input('data'),
        __metadata("design:type", Array)
    ], OlComponent.prototype, "items", void 0);
    __decorate([
        core_1.Input('key'),
        __metadata("design:type", String)
    ], OlComponent.prototype, "key", void 0);
    __decorate([
        core_1.Input('hasAction'),
        __metadata("design:type", Boolean)
    ], OlComponent.prototype, "hasAction", void 0);
    OlComponent = __decorate([
        core_1.Component({
            selector: 'ol',
            templateUrl: './ol.component.html',
            styleUrls: ['./ol.component.css'],
            changeDetection: core_1.ChangeDetectionStrategy.OnPush
        }),
        __metadata("design:paramtypes", [call_category_function_service_1.CallCategoryFunctionService])
    ], OlComponent);
    return OlComponent;
}());
exports.OlComponent = OlComponent;
//# sourceMappingURL=ol.component.js.map