"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var common_service_1 = require("./common.service");
describe('CommonService', function () {
    beforeEach(function () { return testing_1.TestBed.configureTestingModule({}); });
    it('should be created', function () {
        var service = testing_1.TestBed.get(common_service_1.CommonService);
        expect(service).toBeTruthy();
    });
});
//# sourceMappingURL=common.service.spec.js.map