// This file can be replaced during build by using the `fileReplacements` array.
// `ng build ---prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  clientId: "E88138FE9E9B490EA862352D0C501FFA",
  clientSecret: "6CYTL4K5zcLfP4RAAyUucptY9oY2G-bBu71DpQpCb_KAWTmS",
  callbackUri: "https://localhost:44315/authorization",
  scope: "openid profile email offline_access payroll.timesheets.read payroll.timesheets",
  state: "12345678",
  xeroTokenURL: "https://identity.xero.com/connect/token",
  apiURL: "https://localhost:44315/api"
};

/*
 * In development mode, to ignore zone related error stack frames such as
 * `zone.run`, `zoneDelegate.invokeTask` for easier debugging, you can
 * import the following file, but please comment it out in production mode
 * because it will have performance impact when throw error
 *
 *
 * Scope: "openid profile email files accounting.transactions accounting.transactions.read accounting.reports.read accounting.journals.read accounting.settings accounting.settings.read accounting.contacts accounting.contacts.read accounting.attachments accounting.attachments.read offline_access payroll.employees payroll.employees.read payroll.payruns payroll.payruns.read payroll.payslip payroll.payslip.read payroll.settings payroll.timesheets.read payroll.timesheets payroll.settings.read"
 * 
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
