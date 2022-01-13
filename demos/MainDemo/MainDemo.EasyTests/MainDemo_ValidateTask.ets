#DropDB MainDemo

#Application MainDemoWin
;#Application MainDemoWeb

*FillForm
 User Name = Sam
*Action Log In

*Action Navigation(Roles)

*Action New

*Action Save and Close

*CheckValidationResult
 Message = Data Validation Error: Please review and correct the data validation error(s) listed below to proceed.
 Info = "Name" must not be empty.
*OptionalAction Close

*FillForm
 Name = TechWriter
*Action Save and Close
