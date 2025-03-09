"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/router/testing");
var testing_2 = require("@angular/core/testing");
var app_component_1 = require("./app.component");
describe('AppComponent', function () {
    beforeEach(testing_2.async(function () {
        testing_2.TestBed.configureTestingModule({
            declarations: [
                app_component_1.AppComponent
            ],
            imports: [testing_1.RouterTestingModule]
        }).compileComponents();
    }));
    it('should create the app', testing_2.async(function () {
        var fixture = testing_2.TestBed.createComponent(app_component_1.AppComponent);
        var app = fixture.debugElement.componentInstance;
        expect(app).toBeTruthy();
    }));
});
//# sourceMappingURL=app.component.spec.js.map