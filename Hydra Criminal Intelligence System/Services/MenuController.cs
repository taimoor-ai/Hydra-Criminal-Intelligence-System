// ============================================================
// Services/MenuController.cs
// All console menu screens and interactions
// ============================================================
using CriminalRecordMS.Models;
using CriminalRecordMS.Utilities;

namespace CriminalRecordMS.Services
{
    public class MenuController
    {
        private readonly DataStore _store;
        private readonly AuthService _auth;
        private readonly CriminalService _criminalSvc;
        private readonly CaseService _caseSvc;
        private readonly OfficerService _officerSvc;
        private readonly EvidenceService _evidenceSvc;
        private readonly ReportService _reportSvc;

        public MenuController(DataStore store)
        {
            _store = store;
            _auth = new AuthService(store);
            _criminalSvc = new CriminalService(store);
            _caseSvc = new CaseService(store);
            _officerSvc = new OfficerService(store);
            _evidenceSvc = new EvidenceService(store);
            _reportSvc = new ReportService(store);
        }

        // ╔══════════════════════════════════════════════════════╗
        // ║                  STARTUP / LOGIN                    ║
        // ╚══════════════════════════════════════════════════════╝
        public void ShowWelcome()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            string[] logo =
           {
        @"██╗  ██╗██╗   ██╗██████╗ ██████╗  █████╗ ",
        @"██║  ██║╚██╗ ██╔╝██╔══██╗██╔══██╗██╔══██╗",
        @"███████║ ╚████╔╝ ██║  ██║██████╔╝███████║",
        @"██╔══██║  ╚██╔╝  ██║  ██║██╔══██╗██╔══██║",
        @"██║  ██║   ██║   ██████╔╝██║  ██║██║  ██║",
        @"╚═╝  ╚═╝   ╚═╝   ╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝",
        @"",
        @"      CRIMINAL INTELLIGENCE SYSTEM V1.0" };
           
            int top = (Console.WindowHeight - logo.Length) / 2;

            for (int i = 0; i < top; i++)
                Console.WriteLine();

            foreach (var line in logo)
            {
                int left = (Console.WindowWidth - line.Length) / 2;
                Console.WriteLine(new string(' ', Math.Max(0, left)) + line);
                Thread.Sleep(80);
            }

            Console.ResetColor();
            Thread.Sleep(1000);
            Console.Clear();
        }

        public bool ShowLoginScreen()
        {
            ConsoleUI.PrintHeader("SYSTEM LOGIN");
            int attempts = 0;
            while (attempts < 3)
            {
                string username = ConsoleUI.PromptInput("Username");
                string password = ConsoleUI.PromptPassword("Password");
                if (_auth.Login(username, password)) return true;
                attempts++;
                if (attempts < 3)
                    ConsoleUI.PrintWarning($"Login failed. {3 - attempts} attempt(s) remaining.");
            }
            ConsoleUI.PrintError("Too many failed attempts. Exiting.");
            return false;
        }

        // ╔══════════════════════════════════════════════════════╗
        // ║                   MAIN MENU                         ║
        // ╚══════════════════════════════════════════════════════╝
        public void ShowMainMenu()
        {
            while (true)
            {
                Console.Clear();
                ShowWelcome();
                ConsoleUI.PrintHeader($"MAIN MENU  —  {_store.CurrentUser!.Role}: {_store.CurrentUser.Username}");
                ConsoleUI.PrintMenuItem(1, "Criminal Management");
                ConsoleUI.PrintMenuItem(2, "Case Management");
                ConsoleUI.PrintMenuItem(3, "Officer Management");
                ConsoleUI.PrintMenuItem(4, "Evidence Management");
                ConsoleUI.PrintMenuItem(5, "Reports");
                ConsoleUI.PrintMenuItem(6, "Search & Filter");
                if (_auth.IsAdmin())
                    ConsoleUI.PrintMenuItem(7, "User Administration");
                ConsoleUI.PrintMenuItem(0, "Logout & Exit");
                ConsoleUI.PrintDivider();

                var choice = ConsoleUI.PromptInput("Select option");
                switch (choice)
                {
                    case "1": CriminalMenu(); break;
                    case "2": CaseMenu(); break;
                    case "3": OfficerMenu(); break;
                    case "4": EvidenceMenu(); break;
                    case "5": ReportsMenu(); break;
                    case "6": SearchMenu(); break;
                    case "7" when _auth.IsAdmin(): UserAdminMenu(); break;
                    case "0":
                        _store.SaveAll();
                        _auth.Logout();
                        return;
                    default:
                        ConsoleUI.PrintError("Invalid option. Please try again.");
                        ConsoleUI.PressAnyKey();
                        break;
                }
            }
        }

        // ╔══════════════════════════════════════════════════════╗
        // ║               CRIMINAL MANAGEMENT                   ║
        // ╚══════════════════════════════════════════════════════╝
        private void CriminalMenu()
        {
            while (true)
            {
                Console.Clear();
                ConsoleUI.PrintHeader("CRIMINAL MANAGEMENT");
                ConsoleUI.PrintMenuItem(1, "Add New Criminal");
                ConsoleUI.PrintMenuItem(2, "View All Criminals");
                ConsoleUI.PrintMenuItem(3, "View Criminal Profile");
                ConsoleUI.PrintMenuItem(4, "Update Criminal Details");
                ConsoleUI.PrintMenuItem(5, "Mark as Arrested");
                ConsoleUI.PrintMenuItem(6, "Add Crime to Criminal");
                ConsoleUI.PrintMenuItem(7, "Delete Criminal");
                ConsoleUI.PrintMenuItem(0, "Back");
                ConsoleUI.PrintDivider();

                var choice = ConsoleUI.PromptInput("Select option");
                try
                {
                    switch (choice)
                    {
                        case "1": AddCriminalFlow(); break;
                        case "2": ViewAllCriminals(); break;
                        case "3": ViewCriminalProfile(); break;
                        case "4": UpdateCriminalFlow(); break;
                        case "5": MarkArrestedFlow(); break;
                        case "6": AddCrimeFlow(); break;
                        case "7":
                            if (_auth.IsAdmin()) DeleteCriminalFlow();
                            else ConsoleUI.PrintError("Admin access required.");
                            ConsoleUI.PressAnyKey();
                            break;
                        case "0": return;
                        default:
                            ConsoleUI.PrintError("Invalid option.");
                            ConsoleUI.PressAnyKey();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ConsoleUI.PrintError(ex.Message);
                    ConsoleUI.PressAnyKey();
                }
            }
        }

        private void AddCriminalFlow()
        {
            ConsoleUI.PrintHeader("ADD NEW CRIMINAL");
            string name = ConsoleUI.PromptInput("Full Name");
            string cnic = ConsoleUI.PromptInput("CNIC (13 digits)");
            string address = ConsoleUI.PromptInput("Address (or 'Unknown')");
            string phone = ConsoleUI.PromptInput("Phone Number");
            string dobStr = ConsoleUI.PromptInput("Date of Birth (yyyy-MM-dd)");
            string nationality = ConsoleUI.PromptInput("Nationality");
            string physicalDesc = ConsoleUI.PromptInput("Physical Description");

            if (!Validator.TryParseDate(dobStr, out DateTime dob))
                throw new Exception("Invalid date format. Use yyyy-MM-dd.");

            _criminalSvc.AddCriminal(name, cnic, address, phone, dob, nationality, physicalDesc);
            ConsoleUI.PressAnyKey();
        }

        private void ViewAllCriminals()
        {
            ConsoleUI.PrintHeader("ALL CRIMINALS");
            var list = _criminalSvc.GetAll();
            if (list.Count == 0) { ConsoleUI.PrintInfo("No criminals in database."); }
            else
            {
                Console.WriteLine($"  {"ID",-10} {"Name",-25} {"CNIC",-15} {"Danger",-8} {"Arrested",-10} {"Crimes",-6}");
                ConsoleUI.PrintDivider();
                foreach (var cr in list)
                {
                    Console.ForegroundColor = cr.IsMostWanted ? ConsoleColor.Red : ConsoleColor.White;
                    Console.WriteLine($"  {cr.Id,-10} {cr.Name,-25} {cr.Cnic,-15} {cr.DangerLevel,-8} {(cr.IsArrested ? "YES" : "No"),-10} {cr.CrimeHistory.Count,-6}");
                    Console.ResetColor();
                }
            }
            ConsoleUI.PressAnyKey();
        }

        private void ViewCriminalProfile()
        {
            ConsoleUI.PrintHeader("VIEW CRIMINAL PROFILE");
            string id = ConsoleUI.PromptInput("Enter Criminal ID");
            var cr = _criminalSvc.GetById(id);
            cr.PrintReport(); // Polymorphic call
            ConsoleUI.PressAnyKey();
        }

        private void UpdateCriminalFlow()
        {
            ConsoleUI.PrintHeader("UPDATE CRIMINAL");
            string id = ConsoleUI.PromptInput("Criminal ID to update");
            var cr = _criminalSvc.GetById(id);
            ConsoleUI.PrintInfo($"Updating: {cr.Name}  (leave blank to keep current)");

            string name = ConsoleUI.PromptInput($"Name [{cr.Name}]");
            string address = ConsoleUI.PromptInput($"Address [{cr.Address}]");
            string phone = ConsoleUI.PromptInput($"Phone [{cr.PhoneNumber}]");
            string desc = ConsoleUI.PromptInput($"Physical Description [{cr.PhysicalDescription}]");

            _criminalSvc.UpdateCriminal(id,
                string.IsNullOrWhiteSpace(name) ? null : name,
                string.IsNullOrWhiteSpace(address) ? null : address,
                string.IsNullOrWhiteSpace(phone) ? null : phone,
                string.IsNullOrWhiteSpace(desc) ? null : desc,
                null, null);
            ConsoleUI.PressAnyKey();
        }

        private void MarkArrestedFlow()
        {
            ConsoleUI.PrintHeader("MARK CRIMINAL AS ARRESTED");
            string id = ConsoleUI.PromptInput("Criminal ID");
            _criminalSvc.UpdateCriminal(id, null, null, null, null, true, DateTime.Now);
            ConsoleUI.PressAnyKey();
        }

        private void AddCrimeFlow()
        {
            ConsoleUI.PrintHeader("ADD CRIME TO CRIMINAL");
            string criminalId = ConsoleUI.PromptInput("Criminal ID");
            string caseId = ConsoleUI.PromptInput("Linked Case ID");
            string crimeType = ConsoleUI.PromptInput("Crime Type (e.g. Murder, Robbery)");
            string description = ConsoleUI.PromptInput("Description");
            string dateStr = ConsoleUI.PromptInput("Date Committed (yyyy-MM-dd)");

            Console.WriteLine("  Severity: 0=Minor, 1=Moderate, 2=Major, 3=Capital");
            string sevStr = ConsoleUI.PromptInput("Severity");

            if (!Validator.TryParseDate(dateStr, out DateTime date))
                throw new Exception("Invalid date format.");
            if (!Validator.TryParseEnum<CrimeSeverity>(sevStr, out var severity))
                throw new Exception("Invalid severity. Use Minor/Moderate/Major/Capital.");

            _criminalSvc.AddCrimeToCriminal(criminalId, crimeType, description, date, severity, caseId);
            ConsoleUI.PressAnyKey();
        }

        private void DeleteCriminalFlow()
        {
            ConsoleUI.PrintHeader("DELETE CRIMINAL");
            string id = ConsoleUI.PromptInput("Criminal ID to delete");
            var cr = _criminalSvc.GetById(id);
            if (ConsoleUI.Confirm($"Delete criminal '{cr.Name}'? This cannot be undone!"))
                _criminalSvc.DeleteCriminal(id);
            else
                ConsoleUI.PrintInfo("Deletion cancelled.");
        }

        // ╔══════════════════════════════════════════════════════╗
        // ║                  CASE MANAGEMENT                    ║
        // ╚══════════════════════════════════════════════════════╝
        private void CaseMenu()
        {
            while (true)
            {
                Console.Clear();
                ConsoleUI.PrintHeader("CASE MANAGEMENT");
                ConsoleUI.PrintMenuItem(1, "Create New Case");
                ConsoleUI.PrintMenuItem(2, "View All Cases");
                ConsoleUI.PrintMenuItem(3, "View Case Details");
                ConsoleUI.PrintMenuItem(4, "Update Case Status");
                ConsoleUI.PrintMenuItem(5, "Assign Officer to Case");
                ConsoleUI.PrintMenuItem(6, "Remove Officer from Case");
                ConsoleUI.PrintMenuItem(7, "Link Criminal to Case");
                ConsoleUI.PrintMenuItem(0, "Back");
                ConsoleUI.PrintDivider();

                var choice = ConsoleUI.PromptInput("Select option");
                try
                {
                    switch (choice)
                    {
                        case "1": CreateCaseFlow(); break;
                        case "2": ViewAllCases(); break;
                        case "3": ViewCaseDetails(); break;
                        case "4": UpdateCaseStatusFlow(); break;
                        case "5": AssignOfficerFlow(); break;
                        case "6": RemoveOfficerFlow(); break;
                        case "7": LinkCriminalFlow(); break;
                        case "0": return;
                        default:
                            ConsoleUI.PrintError("Invalid option.");
                            ConsoleUI.PressAnyKey();
                            break;
                    }
                }
                catch (Exception ex) { ConsoleUI.PrintError(ex.Message); ConsoleUI.PressAnyKey(); }
            }
        }

        private void CreateCaseFlow()
        {
            ConsoleUI.PrintHeader("CREATE NEW CASE");
            string title = ConsoleUI.PromptInput("Case Title");
            string description = ConsoleUI.PromptInput("Description");
            string location = ConsoleUI.PromptInput("Location");
            Console.WriteLine("  Severity: Minor | Moderate | Major | Capital");
            string sevStr = ConsoleUI.PromptInput("Severity");
            if (!Validator.TryParseEnum<CrimeSeverity>(sevStr, out var severity))
                throw new Exception("Invalid severity.");
            _caseSvc.CreateCase(title, description, location, severity);
            ConsoleUI.PressAnyKey();
        }

        private void ViewAllCases()
        {
            ConsoleUI.PrintHeader("ALL CASES");
            var list = _caseSvc.GetAll();
            if (list.Count == 0) { ConsoleUI.PrintInfo("No cases found."); }
            else
            {
                Console.WriteLine($"  {"ID",-10} {"Title",-28} {"Status",-22} {"Severity",-12} {"Officers",-5}");
                ConsoleUI.PrintDivider();
                foreach (var cs in list)
                {
                    Console.ForegroundColor = cs.Status == CaseStatus.Closed
                        ? ConsoleColor.DarkGray : cs.Status == CaseStatus.Open
                        ? ConsoleColor.Green : ConsoleColor.Yellow;
                    Console.WriteLine($"  {cs.CaseId,-10} {cs.Title.PadRight(28).Substring(0, 28),-28} {cs.Status,-22} {cs.Severity,-12} {cs.OfficerIds.Count,-5}");
                    Console.ResetColor();
                }
            }
            ConsoleUI.PressAnyKey();
        }

        private void ViewCaseDetails()
        {
            ConsoleUI.PrintHeader("CASE DETAILS");
            string id = ConsoleUI.PromptInput("Case ID");
            var cs = _caseSvc.GetById(id);
            cs.PrintReport(); // Polymorphic call
            ConsoleUI.PressAnyKey();
        }

        private void UpdateCaseStatusFlow()
        {
            ConsoleUI.PrintHeader("UPDATE CASE STATUS");
            string id = ConsoleUI.PromptInput("Case ID");
            Console.WriteLine("  Status: Open | UnderInvestigation | Closed");
            string statusStr = ConsoleUI.PromptInput("New Status");
            if (!Validator.TryParseEnum<CaseStatus>(statusStr, out var status))
                throw new Exception("Invalid status.");
            _caseSvc.UpdateStatus(id, status);
            ConsoleUI.PressAnyKey();
        }

        private void AssignOfficerFlow()
        {
            ConsoleUI.PrintHeader("ASSIGN OFFICER TO CASE");
            string caseId = ConsoleUI.PromptInput("Case ID");
            string officerId = ConsoleUI.PromptInput("Officer ID");
            _caseSvc.AssignOfficer(caseId, officerId);
            ConsoleUI.PressAnyKey();
        }

        private void RemoveOfficerFlow()
        {
            ConsoleUI.PrintHeader("REMOVE OFFICER FROM CASE");
            string caseId = ConsoleUI.PromptInput("Case ID");
            string officerId = ConsoleUI.PromptInput("Officer ID to remove");
            _caseSvc.RemoveOfficer(caseId, officerId);
            ConsoleUI.PressAnyKey();
        }

        private void LinkCriminalFlow()
        {
            ConsoleUI.PrintHeader("LINK CRIMINAL TO CASE");
            string caseId = ConsoleUI.PromptInput("Case ID");
            string criminalId = ConsoleUI.PromptInput("Criminal ID");
            _caseSvc.LinkCriminal(caseId, criminalId);
            ConsoleUI.PressAnyKey();
        }

        // ╔══════════════════════════════════════════════════════╗
        // ║               OFFICER MANAGEMENT                    ║
        // ╚══════════════════════════════════════════════════════╝
        private void OfficerMenu()
        {
            while (true)
            {
                Console.Clear();
                ConsoleUI.PrintHeader("OFFICER MANAGEMENT");
                ConsoleUI.PrintMenuItem(1, "Add New Officer");
                ConsoleUI.PrintMenuItem(2, "View All Officers");
                ConsoleUI.PrintMenuItem(3, "View Officer Profile");
                ConsoleUI.PrintMenuItem(4, "View Officer's Assigned Cases");
                ConsoleUI.PrintMenuItem(5, "Deactivate Officer");
                ConsoleUI.PrintMenuItem(0, "Back");
                ConsoleUI.PrintDivider();

                var choice = ConsoleUI.PromptInput("Select option");
                try
                {
                    switch (choice)
                    {
                        case "1":
                            if (_auth.IsAdmin()) AddOfficerFlow();
                            else ConsoleUI.PrintError("Admin access required.");
                            ConsoleUI.PressAnyKey();
                            break;
                        case "2": ViewAllOfficers(); break;
                        case "3": ViewOfficerProfile(); break;
                        case "4": ViewOfficerCases(); break;
                        case "5":
                            if (_auth.IsAdmin()) DeactivateOfficerFlow();
                            else ConsoleUI.PrintError("Admin access required.");
                            ConsoleUI.PressAnyKey();
                            break;
                        case "0": return;
                        default:
                            ConsoleUI.PrintError("Invalid option.");
                            ConsoleUI.PressAnyKey();
                            break;
                    }
                }
                catch (Exception ex) { ConsoleUI.PrintError(ex.Message); ConsoleUI.PressAnyKey(); }
            }
        }

        private void AddOfficerFlow()
        {
            ConsoleUI.PrintHeader("ADD NEW OFFICER");
            string name = ConsoleUI.PromptInput("Full Name");
            string cnic = ConsoleUI.PromptInput("CNIC");
            string address = ConsoleUI.PromptInput("Address");
            string phone = ConsoleUI.PromptInput("Phone");
            string dobStr = ConsoleUI.PromptInput("Date of Birth (yyyy-MM-dd)");
            string badge = ConsoleUI.PromptInput("Badge Number");
            Console.WriteLine("  Rank: Constable | SubInspector | Inspector | SeniorInspector | DSP");
            string rankStr = ConsoleUI.PromptInput("Rank");
            string dept = ConsoleUI.PromptInput("Department");
            string username = ConsoleUI.PromptInput("Login Username");
            string password = ConsoleUI.PromptPassword("Login Password");

            if (!Validator.TryParseDate(dobStr, out DateTime dob))
                throw new Exception("Invalid date format.");
            if (!Validator.TryParseEnum<OfficerRank>(rankStr, out var rank))
                throw new Exception("Invalid rank.");

            _officerSvc.AddOfficer(name, cnic, address, phone, dob, badge, rank,
                                   dept, username, password);
        }

        private void ViewAllOfficers()
        {
            ConsoleUI.PrintHeader("ALL OFFICERS");
            var list = _officerSvc.GetAll();
            if (list.Count == 0) { ConsoleUI.PrintInfo("No officers found."); }
            else
            {
                Console.WriteLine($"  {"ID",-10} {"Name",-25} {"Rank",-16} {"Badge",-10} {"Dept",-18} {"Cases",-5} {"Active"}");
                ConsoleUI.PrintDivider();
                foreach (var of in list)
                {
                    Console.ForegroundColor = of.IsActive ? ConsoleColor.White : ConsoleColor.DarkGray;
                    Console.WriteLine($"  {of.Id,-10} {of.Name,-25} {of.Rank,-16} {of.BadgeNumber,-10} {of.Department,-18} {of.CasesHandled,-5} {(of.IsActive ? "Yes" : "No")}");
                    Console.ResetColor();
                }
            }
            ConsoleUI.PressAnyKey();
        }

        private void ViewOfficerProfile()
        {
            ConsoleUI.PrintHeader("OFFICER PROFILE");
            string id = ConsoleUI.PromptInput("Officer ID");
            var of = _officerSvc.GetById(id);
            of.PrintReport(); // Polymorphic call
            ConsoleUI.PressAnyKey();
        }

        private void ViewOfficerCases()
        {
            ConsoleUI.PrintHeader("OFFICER ASSIGNED CASES");
            string id = ConsoleUI.PromptInput("Officer ID");
            var cases = _officerSvc.GetOfficerCases(id);
            if (cases.Count == 0) ConsoleUI.PrintInfo("No cases assigned.");
            else foreach (var cs in cases) Console.WriteLine("  " + cs.ToString());
            ConsoleUI.PressAnyKey();
        }

        private void DeactivateOfficerFlow()
        {
            ConsoleUI.PrintHeader("DEACTIVATE OFFICER");
            string id = ConsoleUI.PromptInput("Officer ID");
            if (ConsoleUI.Confirm("Deactivate this officer?"))
                _officerSvc.DeactivateOfficer(id);
        }

        // ╔══════════════════════════════════════════════════════╗
        // ║               EVIDENCE MANAGEMENT                   ║
        // ╚══════════════════════════════════════════════════════╝
        private void EvidenceMenu()
        {
            while (true)
            {
                Console.Clear();
                ConsoleUI.PrintHeader("EVIDENCE MANAGEMENT");
                ConsoleUI.PrintMenuItem(1, "Add Document Evidence");
                ConsoleUI.PrintMenuItem(2, "Add Weapon Evidence");
                ConsoleUI.PrintMenuItem(3, "Add Digital Evidence");
                ConsoleUI.PrintMenuItem(4, "View Evidence for a Case");
                ConsoleUI.PrintMenuItem(0, "Back");
                ConsoleUI.PrintDivider();

                var choice = ConsoleUI.PromptInput("Select option");
                try
                {
                    switch (choice)
                    {
                        case "1": AddDocEvidenceFlow(); break;
                        case "2": AddWeaponEvidenceFlow(); break;
                        case "3": AddDigitalEvidenceFlow(); break;
                        case "4": ViewCaseEvidenceFlow(); break;
                        case "0": return;
                        default:
                            ConsoleUI.PrintError("Invalid option.");
                            ConsoleUI.PressAnyKey();
                            break;
                    }
                }
                catch (Exception ex) { ConsoleUI.PrintError(ex.Message); ConsoleUI.PressAnyKey(); }
            }
        }

        private void AddDocEvidenceFlow()
        {
            ConsoleUI.PrintHeader("ADD DOCUMENT EVIDENCE");
            string caseId = ConsoleUI.PromptInput("Case ID");
            string desc = ConsoleUI.PromptInput("Description");
            string officerId = ConsoleUI.PromptInput("Collected By (Officer ID)");
            string docTitle = ConsoleUI.PromptInput("Document Title");
            string docType = ConsoleUI.PromptInput("Document Type (e.g. Contract, Letter, ID)");
            _evidenceSvc.AddDocumentEvidence(caseId, desc, officerId, docTitle, docType);
            ConsoleUI.PressAnyKey();
        }

        private void AddWeaponEvidenceFlow()
        {
            ConsoleUI.PrintHeader("ADD WEAPON EVIDENCE");
            string caseId = ConsoleUI.PromptInput("Case ID");
            string desc = ConsoleUI.PromptInput("Description");
            string officerId = ConsoleUI.PromptInput("Collected By (Officer ID)");
            string weaponType = ConsoleUI.PromptInput("Weapon Type (Firearm, Knife, Blunt, etc.)");
            string serial = ConsoleUI.PromptInput("Serial Number (or 'None')");
            bool isReg = ConsoleUI.Confirm("Is weapon registered?");
            _evidenceSvc.AddWeaponEvidence(caseId, desc, officerId, weaponType, serial, isReg);
            ConsoleUI.PressAnyKey();
        }

        private void AddDigitalEvidenceFlow()
        {
            ConsoleUI.PrintHeader("ADD DIGITAL EVIDENCE");
            string caseId = ConsoleUI.PromptInput("Case ID");
            string desc = ConsoleUI.PromptInput("Description");
            string officerId = ConsoleUI.PromptInput("Collected By (Officer ID)");
            string device = ConsoleUI.PromptInput("Device Type (Laptop, Phone, USB, etc.)");
            string hash = ConsoleUI.PromptInput("Hash Value (MD5/SHA-256)");
            string format = ConsoleUI.PromptInput("File Format (MP4, PDF, Mixed, etc.)");
            _evidenceSvc.AddDigitalEvidence(caseId, desc, officerId, device, hash, format);
            ConsoleUI.PressAnyKey();
        }

        private void ViewCaseEvidenceFlow()
        {
            ConsoleUI.PrintHeader("CASE EVIDENCE");
            string caseId = ConsoleUI.PromptInput("Case ID");
            var evidence = _evidenceSvc.GetEvidenceForCase(caseId);
            if (evidence.Count == 0) ConsoleUI.PrintInfo("No evidence recorded for this case.");
            else
                foreach (var ev in evidence)
                    ev.PrintReport(); // Polymorphic call
            ConsoleUI.PressAnyKey();
        }

        // ╔══════════════════════════════════════════════════════╗
        // ║                    REPORTS                          ║
        // ╚══════════════════════════════════════════════════════╝
        private void ReportsMenu()
        {
            while (true)
            {
                Console.Clear();
                ConsoleUI.PrintHeader("REPORTS");
                ConsoleUI.PrintMenuItem(1, "Criminal History Report");
                ConsoleUI.PrintMenuItem(2, "Case Summary Report");
                ConsoleUI.PrintMenuItem(3, "Officer Performance Report");
                ConsoleUI.PrintMenuItem(4, "System Summary Report");
                ConsoleUI.PrintMenuItem(5, "Most Wanted List");
                ConsoleUI.PrintMenuItem(0, "Back");
                ConsoleUI.PrintDivider();

                var choice = ConsoleUI.PromptInput("Select option");
                try
                {
                    switch (choice)
                    {
                        case "1":
                            ConsoleUI.PrintHeader("CRIMINAL HISTORY REPORT");
                            string crimId = ConsoleUI.PromptInput("Criminal ID");
                            _reportSvc.CriminalHistoryReport(crimId);
                            ConsoleUI.PressAnyKey();
                            break;
                        case "2":
                            ConsoleUI.PrintHeader("CASE SUMMARY REPORT");
                            string caseId = ConsoleUI.PromptInput("Case ID");
                            _reportSvc.CaseSummaryReport(caseId);
                            ConsoleUI.PressAnyKey();
                            break;
                        case "3":
                            ConsoleUI.PrintHeader("OFFICER PERFORMANCE REPORT");
                            string ofId = ConsoleUI.PromptInput("Officer ID");
                            _reportSvc.OfficerPerformanceReport(ofId);
                            ConsoleUI.PressAnyKey();
                            break;
                        case "4":
                            _reportSvc.SystemSummaryReport();
                            ConsoleUI.PressAnyKey();
                            break;
                        case "5":
                            MostWantedList();
                            break;
                        case "0": return;
                        default:
                            ConsoleUI.PrintError("Invalid option.");
                            ConsoleUI.PressAnyKey();
                            break;
                    }
                }
                catch (Exception ex) { ConsoleUI.PrintError(ex.Message); ConsoleUI.PressAnyKey(); }
            }
        }

        private void MostWantedList()
        {
            ConsoleUI.PrintHeader("MOST WANTED CRIMINALS");
            var list = _criminalSvc.GetMostWanted();
            if (list.Count == 0) ConsoleUI.PrintInfo("No most-wanted criminals.");
            else
            {
                int rank = 1;
                foreach (var cr in list)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  #{rank++,2}. [{cr.Id}] {cr.Name}");
                    Console.ResetColor();
                    Console.WriteLine($"       Danger: {cr.DangerLevel} | Crimes: {cr.CrimeHistory.Count} | Nationality: {cr.Nationality}");
                    Console.WriteLine($"       Desc  : {cr.PhysicalDescription}");
                    ConsoleUI.PrintDivider();
                }
            }
            ConsoleUI.PressAnyKey();
        }

        // ╔══════════════════════════════════════════════════════╗
        // ║                SEARCH & FILTER                      ║
        // ╚══════════════════════════════════════════════════════╝
        private void SearchMenu()
        {
            while (true)
            {
                Console.Clear();
                ConsoleUI.PrintHeader("SEARCH & FILTER");
                ConsoleUI.PrintMenuItem(1, "Search Criminal by Name");
                ConsoleUI.PrintMenuItem(2, "Search Criminal by CNIC");
                ConsoleUI.PrintMenuItem(3, "Filter Criminals by Crime Type");
                ConsoleUI.PrintMenuItem(4, "Filter Cases by Status");
                ConsoleUI.PrintMenuItem(0, "Back");
                ConsoleUI.PrintDivider();

                var choice = ConsoleUI.PromptInput("Select option");
                try
                {
                    switch (choice)
                    {
                        case "1":
                            string name = ConsoleUI.PromptInput("Search name (partial ok)");
                            var byName = _criminalSvc.SearchByName(name);
                            PrintCriminalList(byName);
                            ConsoleUI.PressAnyKey();
                            break;
                        case "2":
                            string cnic = ConsoleUI.PromptInput("Enter CNIC");
                            var byCnic = _criminalSvc.SearchByCnic(cnic);
                            if (byCnic != null) byCnic.PrintReport();
                            else ConsoleUI.PrintInfo("No criminal found with that CNIC.");
                            ConsoleUI.PressAnyKey();
                            break;
                        case "3":
                            string crimeType = ConsoleUI.PromptInput("Crime type keyword");
                            var filtered = _criminalSvc.FilterByCrimeType(crimeType);
                            PrintCriminalList(filtered);
                            ConsoleUI.PressAnyKey();
                            break;
                        case "4":
                            Console.WriteLine("  Status: Open | UnderInvestigation | Closed");
                            string statusStr = ConsoleUI.PromptInput("Status");
                            if (Validator.TryParseEnum<CaseStatus>(statusStr, out var status))
                            {
                                var cases = _caseSvc.GetByStatus(status);
                                foreach (var cs in cases) Console.WriteLine("  " + cs);
                            }
                            else ConsoleUI.PrintError("Invalid status.");
                            ConsoleUI.PressAnyKey();
                            break;
                        case "0": return;
                        default:
                            ConsoleUI.PrintError("Invalid option.");
                            ConsoleUI.PressAnyKey();
                            break;
                    }
                }
                catch (Exception ex) { ConsoleUI.PrintError(ex.Message); ConsoleUI.PressAnyKey(); }
            }
        }

        private void PrintCriminalList(List<Criminal> list)
        {
            if (list.Count == 0) { ConsoleUI.PrintInfo("No results found."); return; }
            Console.WriteLine($"  Found {list.Count} result(s):");
            ConsoleUI.PrintDivider();
            foreach (var cr in list)
                Console.WriteLine($"  {cr.Id,-10} {cr.Name,-25} CNIC: {cr.Cnic} | Danger: {cr.DangerLevel}");
        }

        // ╔══════════════════════════════════════════════════════╗
        // ║               USER ADMINISTRATION                   ║
        // ╚══════════════════════════════════════════════════════╝
        private void UserAdminMenu()
        {
            while (true)
            {
                Console.Clear();
                ConsoleUI.PrintHeader("USER ADMINISTRATION  (Admin Only)");
                ConsoleUI.PrintMenuItem(1, "View All Users");
                ConsoleUI.PrintMenuItem(2, "Deactivate User");
                ConsoleUI.PrintMenuItem(3, "Change My Password");
                ConsoleUI.PrintMenuItem(4, "Seed Sample Data (if empty)");
                ConsoleUI.PrintMenuItem(0, "Back");
                ConsoleUI.PrintDivider();

                var choice = ConsoleUI.PromptInput("Select option");
                try
                {
                    switch (choice)
                    {
                        case "1":
                            ConsoleUI.PrintHeader("ALL USERS");
                            foreach (var u in _store.Users.Values)
                                Console.WriteLine($"  {u.UserId,-10} {u.Username,-20} Role: {u.Role,-10} Active: {u.IsActive}");
                            ConsoleUI.PressAnyKey();
                            break;
                        case "2":
                            string uid = ConsoleUI.PromptInput("User ID to deactivate");
                            if (_store.Users.TryGetValue(uid, out var user))
                            {
                                user.IsActive = false;
                                _store.SaveAll();
                                ConsoleUI.PrintSuccess("User deactivated.");
                            }
                            else ConsoleUI.PrintError("User not found.");
                            ConsoleUI.PressAnyKey();
                            break;
                        case "3":
                            string oldPwd = ConsoleUI.PromptPassword("Old Password");
                            string newPwd = ConsoleUI.PromptPassword("New Password");
                            _auth.ChangePassword(oldPwd, newPwd);
                            ConsoleUI.PressAnyKey();
                            break;
                        case "4":
                            _store.SeedSampleData();
                            ConsoleUI.PressAnyKey();
                            break;
                        case "0": return;
                        default:
                            ConsoleUI.PrintError("Invalid option.");
                            ConsoleUI.PressAnyKey();
                            break;
                    }
                }
                catch (Exception ex) { ConsoleUI.PrintError(ex.Message); ConsoleUI.PressAnyKey(); }
            }
        }
    }
}