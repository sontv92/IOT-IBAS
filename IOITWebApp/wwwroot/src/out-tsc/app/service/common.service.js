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
var CommonService = /** @class */ (function () {
    function CommonService() {
        this.domainImage = "https://localhost:44304/";
    }
    CommonService.prototype.ConvertUrl = function (str) {
        str = str.toLowerCase();
        str = str.replace(/á|à|ả|ã|ạ|â|ấ|ầ|ẩ|ẫ|ậ|ă|ắ|ằ|ẳ|ẵ|ặ"/g, "a");
        str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
        str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
        str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
        str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
        str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
        str = str.replace(/đ/g, "d");
        str = str.replace(/!|@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\;|\'|\"|\&|\#|\[|\]|~|\$|_|`|-|{|}|\||\\/g, " ");
        str = str.replace(/[^a-zA-Z0-9 ]/g, "");
        str = str.replace(/ + /g, " ");
        str = str.trim();
        str = str.replace(/ /g, "-");
        return str;
    };
    CommonService.prototype.CheckAccessKey = function (Str, Code) {
        var Arr = [];
        Arr = Str.split('-');
        for (var i = 0; i < Arr.length; i++) {
            var ConvertArr = Arr[i].split(':');
            if (Code == ConvertArr[0]) {
                var check = ConvertArr[1].substr(0, 1);
                if (check == "1") {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        return false;
    };
    CommonService.prototype.ConvertDateTime = function (obj) {
        return obj.getFullYear() + "-" + (obj.getMonth() + 1) + "-" + obj.getDate() + " " + obj.getHours() + ":" + obj.getMinutes() + ":" + obj.getSeconds();
    };
    //Hàm đọc dữ liệu từ dạng cha con html => json truyền vào id của noda cha
    CommonService.prototype.ConvertHtmlToJson = function (Arr, CurrentNode, Selector, ParentId, lct) {
        var slt = Selector + " > ol";
        var ol = CurrentNode.getElementsByTagName("ol");
        if (ol.length > 0) {
            slt = slt + " > li";
            var li = ol[0].querySelectorAll(slt);
            if (li.length > 0) {
                for (var i = 0; i < li.length; i++) {
                    var Id = li[i].getAttribute("data-id");
                    var Name = li[i].getAttribute("data-name");
                    var PrtId = ParentId;
                    Arr.push({ CategoryId: Id, Name: Name, CategoryParentId: PrtId, Location: lct });
                    lct++;
                    this.ConvertHtmlToJson(Arr, li[i], slt, Id, lct);
                }
            }
        }
    };
    CommonService = __decorate([
        core_1.Injectable({
            providedIn: 'root'
        }),
        __metadata("design:paramtypes", [])
    ], CommonService);
    return CommonService;
}());
exports.CommonService = CommonService;
//# sourceMappingURL=common.service.js.map