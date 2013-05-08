using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MMContest.Models;
using www.Models.ViewModels;
using System.Web.Routing;

namespace www.Controllers
{
    public class ReportController : Controller
    {
        private CrmContext Db { get; set; }


        // Override Initialize in order to be able to read cookie and 
        // set Database context for the whole Controller upon instantiation
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            var request = requestContext.HttpContext.Request;

            if (request.Cookies["MyUserSettings"] != null)
                Db = new CrmContext(request.Cookies["MyUserSettings"]["crmContextValue"].ToString());
            else
                Db = new CrmContext();       
        }


        // GET: /Report/
        public ActionResult Index()
        {
            return View();
        }


        // GET: /Report/WelcomePartial
        public ActionResult WelcomePartial()
        {
            return PartialView();
        }


        // GET: /Report/ContestListPartial
        public ActionResult ContestListPartial()
        {
            return PartialView(Db.Contests.ToList());
        }


        // GET: /Report/ContestResultsPartial
        public ActionResult ContestResultsPartial(int id = 0)
        {
            ViewBag.ContestId = id;
            return PartialView();
        }


        // GET: /Report/CrmUsersPartial
        public ActionResult CrmUsersPartial()
        {
            var users = from u in Db.Users
                        join p in Db.Provinces on u.Province equals p.Abbreviation into JoinedProvinceUser from x in JoinedProvinceUser.DefaultIfEmpty()
                        join s in Db.Salutations on u.Salutation equals s.Id into JoinedSalutationUser from y in JoinedSalutationUser.DefaultIfEmpty()
                        select new CrmUserView
                        {
                            UserId = u.UserId,
                            Salutation = y.SalutationEn == null ? "" : y.SalutationEn,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Email = u.Email,
                            Address1 = u.Address1,
                            Address2 = u.Address2,
                            City = u.City,
                            Province = x.NameEn,
                            PostalCode = u.PostalCode,
                            Telephone = u.Telephone,
                            Language = u.Language,
                            YearOfBirth = u.YearOfBirth,
                            DateCreated = u.DateCreated,
                            DateModified = u.DateModified
                        };

            return PartialView(users);
        }


        public ActionResult CrmUsersRead([DataSourceRequest] DataSourceRequest request)
        {
            var users = from u in Db.Users
                        join p in Db.Provinces on u.Province equals p.Abbreviation into JoinedProvinceUser
                        from x in JoinedProvinceUser.DefaultIfEmpty()
                        join s in Db.Salutations on u.Salutation equals s.Id into JoinedSalutationUser
                        from y in JoinedSalutationUser.DefaultIfEmpty()
                        select new CrmUserView
                        {
                            UserId = u.UserId,
                            Salutation = y.SalutationEn == null ? "" : y.SalutationEn,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Email = u.Email,
                            Address1 = u.Address1,
                            Address2 = u.Address2,
                            City = u.City,
                            Province = x.NameEn,
                            PostalCode = u.PostalCode,
                            Telephone = u.Telephone,
                            Language = u.Language,
                            YearOfBirth = u.YearOfBirth,
                            DateCreated = u.DateCreated,
                            DateModified = u.DateModified
                        };

            var result = users.ToDataSourceResult(request, user => new CrmUserView
            {
                UserId = user.UserId,
                Salutation = user.Salutation,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Address1 = user.Address1,
                Address2 = user.Address2,
                City = user.City,
                Province = user.Province,
                PostalCode = user.PostalCode,
                Telephone = user.Telephone,
                Language = user.Language,
                YearOfBirth = user.YearOfBirth,
                DateCreated = user.DateCreated,
                DateModified = user.DateModified
            });

            return Json(result);
        }


        // GET: /Report/CrmUsersByProvincePartial
        public ActionResult CrmUsersByProvincePartial()
        {
            var usersProvinces = from user in Db.Users
                           join province in Db.Provinces
                           on user.Province equals province.Abbreviation into JoinedUsers
                           from province in JoinedUsers.DefaultIfEmpty()
                           select new
                           {
                               UserId = user.UserId,
                               ProvinceAbr = user.Province,
                               ProvinceName = province.NameEn
                           };

            var r = usersProvinces.GroupBy(p => p.ProvinceName)
                .Select(lst => new CrmUsersByProvinceView { Province = lst.Key, TotalUsers = lst.Count() } ); 

            return PartialView(r);
        }


        public ActionResult CrmUsersByProvinceRead([DataSourceRequest] DataSourceRequest request)
        {
            var usersProvinces = from user in Db.Users
                                 join province in Db.Provinces
                                 on user.Province equals province.Abbreviation into JoinedUsers
                                 from province in JoinedUsers.DefaultIfEmpty()
                                 select new
                                 {
                                     UserId = user.UserId,
                                     ProvinceAbr = user.Province,
                                     ProvinceName = province.NameEn
                                 };

            var r = usersProvinces.GroupBy(p => p.ProvinceName)
                .Select(lst => new CrmUsersByProvinceView { Province = lst.Key,  TotalUsers = lst.Count() });

            var result = r.ToDataSourceResult(request, u => new CrmUsersByProvinceView
            {
                Province = u.Province,
                TotalUsers = u.TotalUsers
            });

            return Json(result);
        }


        // GET: /Report/CrmUsersByGenderPartial
        public ActionResult CrmUsersByGenderPartial()
        {
            var usersSalutations = from user in Db.Users
                                   join salutation in Db.Salutations
                                   on user.Salutation equals salutation.Id into JoinedUsers
                                   from salutation in JoinedUsers.DefaultIfEmpty()
                                   select new
                                   {
                                       UserId = user.UserId,
                                       SalutationId = user.Salutation,
                                       SalutationEn = salutation.SalutationEn
                                   };

            var r = usersSalutations.GroupBy(p => p.SalutationEn)
                .Select(lst => new CrmUsersByGenderView { SalutationEn = lst.Key, TotalUsers = lst.Count() });

            return PartialView(r);
        }


        public ActionResult CrmUsersByGenderRead([DataSourceRequest] DataSourceRequest request)
        {
            var usersSalutations = from user in Db.Users
                                   join salutation in Db.Salutations
                                   on user.Salutation equals salutation.Id into JoinedUsers
                                   from salutation in JoinedUsers.DefaultIfEmpty()
                                   select new
                                   {
                                       UserId = user.UserId,
                                       SalutationId = user.Salutation,
                                       SalutationEn = salutation.SalutationEn
                                   };


            var r = usersSalutations.GroupBy(p => p.SalutationEn)
                .Select(lst => new CrmUsersByGenderView { SalutationEn = lst.Key, TotalUsers = lst.Count() });

            var result = r.ToDataSourceResult(request, u => new CrmUsersByGenderView
            {
                Salutation = u.Salutation,
                TotalUsers = u.TotalUsers
            });

            return Json(result);
        }


        // GET: /Report/CrmUsersByYearOfBirthPartial
        public ActionResult CrmUsersByYearOfBirthPartial()
        {
            var r = Db.Users.GroupBy(p => p.YearOfBirth)
                .Select(lst => new CrmUsersByYearOfBirthView { YearOfBirth = lst.Key, TotalUsers = lst.Count() }).OrderBy(o => o.YearOfBirth);

            return PartialView(r);
        }

        public ActionResult CrmUsersByYearOfBirthRead([DataSourceRequest] DataSourceRequest request)
        {
            var r = Db.Users.GroupBy(p => p.YearOfBirth)
                .Select(lst => new CrmUsersByYearOfBirthView { YearOfBirth = lst.Key, TotalUsers = lst.Count() });

            var result = r.ToDataSourceResult(request, u => new CrmUsersByYearOfBirthView
            {
                YearOfBirth = u.YearOfBirth,
                TotalUsers = u.TotalUsers
            });

            return Json(result);
        }


        // GET: /Report/CrmReportListPartial
        public ActionResult CrmReportListPartial()
        {
            return PartialView();
        }






        //******************************************************************************************************************************
        // FILE Export Methods         
        //******************************************************************************************************************************
        #region File Export Methods (CSV, Excel, PDF)

        public FileResult ExportCsv([DataSourceRequest]DataSourceRequest request)
        {
            IEnumerable<User> originalUsers = Db.Users.Where(u => u.OptIn);

            var users = originalUsers.Select(user => new CrmUserView
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Province = user.Province
            });

            //IEnumerable users = result.ToDataSourceResult(request).Data;

            MemoryStream output = new MemoryStream();
            StreamWriter writer = new StreamWriter(output, Encoding.UTF8);
            writer.Write("UserID,");
            writer.Write("First Name,");
            writer.Write("LastName,");
            writer.Write("Email,");
            writer.Write("Province,");
            writer.WriteLine();
            foreach (CrmUserView user in users)
            {
                writer.Write(user.UserId);
                writer.Write(",");
                writer.Write(user.FirstName);
                writer.Write(",");
                writer.Write(user.LastName);
                writer.Write(",");
                writer.Write(user.Email);
                writer.Write(",");
                writer.Write(user.Province);
                writer.WriteLine();
            }
            writer.Flush();
            output.Position = 0;

            return File(output, "text/comma-separated-values", "Products.csv");
        }


        //public FileResult ExportExcel([DataSourceRequest]DataSourceRequest request)
        //{
        //    IEnumerable<User> originalUsers = Db.Users.Where(u => u.OptIn);

        //    var users = originalUsers.Select(user => new CrmUserView
        //    {
        //        UserId = user.UserId,
        //        FirstName = user.FirstName,
        //        LastName = user.LastName,
        //        Email = user.Email,
        //        Province = user.Province
        //    });

        //    //IEnumerable users = result.ToDataSourceResult(request).Data;

        //    //Create new Excel workbook
        //    var workbook = new HSSFWorkbook();

        //    //Create new Excel sheet
        //    var sheet = workbook.CreateSheet();

        //    //(Optional) set the width of the columns
        //    sheet.SetColumnWidth(0, 10 * 256);
        //    sheet.SetColumnWidth(1, 50 * 256);
        //    sheet.SetColumnWidth(2, 50 * 256);
        //    sheet.SetColumnWidth(3, 50 * 256);
        //    sheet.SetColumnWidth(4, 20 * 256);

        //    //Create a header row
        //    var headerRow = sheet.CreateRow(0);

        //    //Set the column names in the header row
        //    headerRow.CreateCell(0).SetCellValue("UserID");
        //    headerRow.CreateCell(1).SetCellValue("First Name");
        //    headerRow.CreateCell(2).SetCellValue("Last Name");
        //    headerRow.CreateCell(1).SetCellValue("Email");
        //    headerRow.CreateCell(2).SetCellValue("Province");

        //    //(Optional) freeze the header row so it is not scrolled
        //    sheet.CreateFreezePane(0, 1, 0, 1);

        //    int rowNumber = 1;

        //    //Populate the sheet with values from the grid data
        //    foreach (CrmUserView user in users)
        //    {
        //        //Create a new row
        //        var row = sheet.CreateRow(rowNumber++);

        //        //Set values for the cells
        //        row.CreateCell(0).SetCellValue(user.UserId);
        //        row.CreateCell(1).SetCellValue(user.FirstName);
        //        row.CreateCell(2).SetCellValue(user.LastName);
        //        row.CreateCell(3).SetCellValue(user.Email);
        //        row.CreateCell(4).SetCellValue(user.Province);
        //    }

        //    //Write the workbook to a memory stream
        //    MemoryStream output = new MemoryStream();
        //    workbook.Write(output);

        //    //Return the result to the end user

        //    return File(output.ToArray(),   //The binary data of the XLS file
        //        "application/vnd.ms-excel", //MIME type of Excel files
        //        "GridExcelExport.xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user

        //}


        //public FileResult ExportPdf([DataSourceRequest]DataSourceRequest request)
        //{
        //    //var dataSource = $("#grid").data("kendoGrid").dataSource;
        //    //var filters = dataSource.filter();
        //    //var allData = dataSource.data();
        //    //var query = new kendo.data.Query(allData);
        //    //var data = query.filter(filters).data;

        //    IEnumerable<User> originalUsers = Db.Users.Where(u => u.OptIn);

        //    var users = originalUsers.Select(user => new CrmUserView
        //    {
        //        UserId = user.UserId,
        //        FirstName = user.FirstName,
        //        LastName = user.LastName,
        //        Email = user.Email,
        //        Province = user.Province
        //    });

        //    //IEnumerable users = result.ToDataSourceResult(request).Data;

        //    // step 1: creation of a document-object
        //    var document = new Document(PageSize.LETTER, 10, 10, 10, 10);

        //    //step 2: we create a memory stream that listens to the document
        //    var output = new MemoryStream();
        //    PdfWriter.GetInstance(document, output);

        //    //step 3: we open the document
        //    document.Open();

        //    //step 4: we add content to the document
        //    var numOfColumns = 5;
        //    var dataTable = new PdfPTable(numOfColumns);

        //    dataTable.DefaultCell.Padding = 3;

        //    dataTable.DefaultCell.BorderWidth = 2;
        //    dataTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

        //    // Adding headers
        //    dataTable.AddCell("UserID");
        //    dataTable.AddCell("First Name");
        //    dataTable.AddCell("Last Name");
        //    dataTable.AddCell("Email");
        //    dataTable.AddCell("Province");

        //    dataTable.HeaderRows = 1;
        //    dataTable.DefaultCell.BorderWidth = 1;

        //    foreach (CrmUserView user in users)
        //    {
        //        dataTable.AddCell(text: user.UserId.ToString(CultureInfo.InvariantCulture));
        //        dataTable.AddCell(user.FirstName);
        //        dataTable.AddCell(user.LastName);
        //        dataTable.AddCell(user.Email);
        //        dataTable.AddCell(user.Province);
        //    }

        //    // Add table to the document
        //    document.Add(dataTable);

        //    //This is important don't forget to close the document
        //    document.Close();

        //    // send the memory stream as File
        //    return File(output.ToArray(), "application/pdf", "Products.pdf");

        //}

        #endregion


        //******************************************************************************************************************************
        // CONTEST RESULTS Export Methods         
        //******************************************************************************************************************************
        #region File Export Methods (CSV, Excel, PDF)


        public FileResult ExportContestReportCsv(FormCollection form)
        {
            DateTime startDate = Convert.ToDateTime(form["start"]);
            DateTime endDate = Convert.ToDateTime(form["end"]);
            string reportType = form["report"];
            string outputFileName = "filename.csv";

            MemoryStream output = new MemoryStream();
            StreamWriter writer = new StreamWriter(output, Encoding.UTF8);


            switch (reportType)
            {
                case "AdultVotes":
                    {
                        IEnumerable<Vote> allAdultVotes = Db.Votes.Where(vote => vote.ContestId == 8 && vote.DateCreated > startDate && vote.DateCreated < endDate);

                        // Add title line to report
                        writer.Write("Adult Votes");
                        writer.WriteLine();

                        // Display Date Range
                        writer.Write("Start Date: " + startDate);
                        writer.WriteLine();
                        writer.Write("End Date: " + endDate);
                        writer.WriteLine();
                        writer.WriteLine();

                        // Display Header Row
                        writer.Write("Vote ID,");
                        writer.Write("Vote Value,");
                        writer.Write("IP Address,");
                        writer.Write("Date Vote Created,");
                        writer.Write("User ID,");
                        writer.Write("Salutation,");
                        writer.Write("First Name,");
                        writer.Write("Last Name,");
                        writer.Write("Email,");
                        writer.Write("Address 1,");
                        writer.Write("Address 2,");
                        writer.Write("City,");
                        writer.Write("Province,");
                        writer.Write("Postal Code,");
                        writer.Write("Telephone,");
                        writer.Write("Year of Birth,");
                        writer.Write("Language,");
                        writer.Write("User Date Created,");
                        writer.Write("User Date Modified,");
                        writer.WriteLine();

                        // Display Data
                        foreach (var vote in allAdultVotes)
                        {
                            writer.Write(vote.VoteId + ",");
                            writer.Write(vote.VoteValue + ",");
                            writer.Write(vote.IpAddress + ",");
                            writer.Write(vote.DateCreated + ",");
                            writer.Write(vote.User.UserId + ",");
                            writer.Write(vote.User.Salutation + ",");
                            writer.Write(vote.User.FirstName + ",");
                            writer.Write(vote.User.LastName + ",");
                            writer.Write(vote.User.Email + ",");
                            writer.Write(vote.User.Address1 + ",");
                            writer.Write(vote.User.Address2 + ",");
                            writer.Write(vote.User.City + ",");
                            writer.Write(vote.User.Province + ",");
                            writer.Write(vote.User.PostalCode + ",");
                            writer.Write(vote.User.Telephone + ",");
                            writer.Write(vote.User.YearOfBirth + ",");
                            writer.Write(vote.User.Language + ",");
                            writer.Write(vote.User.DateCreated + ",");
                            writer.Write(vote.User.DateModified);
                            writer.WriteLine();
                        }

                        // Output File
                        outputFileName = "TOK_AdultVotes_" + DateTime.Now + ".csv";
                        break;
                    }
                case "MinorVotes":
                    {
                        IEnumerable<MinorVote> allMinorVotes = Db.MinorVotes.Where(vote => vote.ContestId == 8 && vote.DateCreated > startDate && vote.DateCreated < endDate);

                        // Add title line to report
                        writer.Write("Minor Votes");
                        writer.WriteLine();

                        // Display Date Range
                        writer.Write("Start Date: " + startDate);
                        writer.WriteLine();
                        writer.Write("End Date: " + endDate);
                        writer.WriteLine();
                        writer.WriteLine();

                        // Display Header Row
                        writer.Write("Vote ID,");
                        writer.Write("Vote Value,");
                        writer.Write("IP Address,");
                        writer.Write("Date Vote Created,");
                        writer.Write("Minor ID,");
                        writer.Write("Minor First Name,");
                        writer.Write("Minor Last Name,");
                        writer.Write("Minor Email,");
                        writer.Write("Minor DoB,");
                        writer.Write("Minor Language,");
                        writer.Write("Minor Approved,");
                        writer.Write("Guardian First Name,");
                        writer.Write("Guardian Last Name,");
                        writer.Write("Guardian Email,");
                        writer.Write("Guardian Address 1,");
                        writer.Write("Guardian Address 2,");
                        writer.Write("Guardian City,");
                        writer.Write("Guardian Province,");
                        writer.Write("Guardian Postal Code,");
                        writer.Write("Guardian Telephone,");
                        writer.Write("Guardian Year of Birth,");
                        writer.Write("Guardian Accept Rules,");
                        writer.Write("Minor Date Created,");
                        writer.Write("Minor Date Modified,");
                        writer.WriteLine();

                        // Display Data
                        foreach (MinorVote vote in allMinorVotes)
                        {
                            writer.Write(vote.MinorVoteId + ",");
                            writer.Write(vote.MinorVoteValue + ",");
                            writer.Write(vote.IpAddress + ",");
                            writer.Write(vote.DateCreated + ",");
                            writer.Write(vote.MinorUser.MinorId + ",");
                            writer.Write(vote.MinorUser.MinorFirstName + ",");
                            writer.Write(vote.MinorUser.MinorLastName + ",");
                            writer.Write(vote.MinorUser.MinorEmail + ",");
                            writer.Write(vote.MinorUser.MinorDateOfBirth + ",");
                            writer.Write(vote.MinorUser.MinorLanguage + ",");
                            writer.Write(vote.MinorUser.MinorApproved + ",");
                            writer.Write(vote.MinorUser.GuardianFirstName + ",");
                            writer.Write(vote.MinorUser.GuardianLastName + ",");
                            writer.Write(vote.MinorUser.GuardianEmail + ",");
                            writer.Write(vote.MinorUser.GuardianAddress1 + ",");
                            writer.Write(vote.MinorUser.GuardianAddress2 + ",");
                            writer.Write(vote.MinorUser.GuardianCity + ",");
                            writer.Write(vote.MinorUser.GuardianProvince + ",");
                            writer.Write(vote.MinorUser.GuardianPostalCode + ",");
                            writer.Write(vote.MinorUser.GuardianTelephone + ",");
                            writer.Write(vote.MinorUser.GuardianYearOfBirth + ",");
                            writer.Write(vote.MinorUser.MinorAcceptRules + ",");
                            writer.Write(vote.MinorUser.DateCreated + ",");
                            writer.Write(vote.MinorUser.DateModified);

                            writer.WriteLine();
                        }

                        // Output File
                        outputFileName = "TOK_MinorVotes_" + DateTime.Now + ".csv";
                        break;
                    }
                case "ContestEntries":
                    {
                        IEnumerable<Ballot> allBallots = Db.Ballots.Where(ballot => ballot.ContestId == 8 && ballot.DateCreated > startDate && ballot.DateCreated < endDate);

                        // Add title line to report
                        writer.Write("Contest Entries");
                        writer.WriteLine();

                        // Display Date Range
                        writer.Write("Start Date: " + startDate);
                        writer.WriteLine();
                        writer.Write("End Date: " + endDate);
                        writer.WriteLine();
                        writer.WriteLine();

                        // Display Header Row
                        writer.Write("Ballot ID,");
                        writer.Write("IP Address,");
                        writer.Write("Ballot Source,");
                        writer.Write("Is A Bonus Ballot,");
                        writer.Write("Is A Share Bonus Ballot,");
                        writer.Write("Ballot Date Created,");
                        writer.Write("User ID,");
                        writer.Write("Salutation,");
                        writer.Write("First Name,");
                        writer.Write("Last Name,");
                        writer.Write("Email,");
                        writer.Write("Address 1,");
                        writer.Write("Address 2,");
                        writer.Write("City,");
                        writer.Write("Province,");
                        writer.Write("Postal Code,");
                        writer.Write("Telephone,");
                        writer.Write("Year of Birth,");
                        writer.Write("Language,");
                        writer.Write("User Date Created,");
                        writer.Write("User Date Modified,");
                        writer.WriteLine();

                        // Display Data
                        foreach (var ballot in allBallots)
                        {
                            writer.Write(ballot.BallotId + ",");
                            writer.Write(ballot.IpAddress + ",");
                            writer.Write(ballot.Source + ",");
                            writer.Write(ballot.IsBonusBallot + ",");
                            writer.Write(ballot.IsShareBonusBallot + ",");
                            writer.Write(ballot.DateCreated + ",");
                            writer.Write(ballot.User.UserId + ",");
                            writer.Write(ballot.User.Salutation + ",");
                            writer.Write(ballot.User.FirstName + ",");
                            writer.Write(ballot.User.LastName + ",");
                            writer.Write(ballot.User.Email + ",");
                            writer.Write(ballot.User.Address1 + ",");
                            writer.Write(ballot.User.Address2 + ",");
                            writer.Write(ballot.User.City + ",");
                            writer.Write(ballot.User.Province + ",");
                            writer.Write(ballot.User.PostalCode + ",");
                            writer.Write(ballot.User.Telephone + ",");
                            writer.Write(ballot.User.YearOfBirth + ",");
                            writer.Write(ballot.User.Language + ",");
                            writer.Write(ballot.User.DateCreated + ",");
                            writer.Write(ballot.User.DateModified);
                            writer.WriteLine();
                        }

                        // Output File
                        outputFileName = "TOK_ContestEntries_" + DateTime.Now + ".csv";
                        break;
                    }
                case "AdultVoteTallyByDateRange":
                    {
                        var qry = from v in Db.Votes
                                  where v.DateCreated > startDate
                                  where v.DateCreated < endDate
                                  group v by v.VoteValue
                                      into grp
                                      select new SchoolVotesView
                                          {
                                              SchoolName = grp.Key,
                                              TotalVotes = grp.Count()
                                          };

                        // Add title line to report
                        writer.Write("Adult Vote Tally by Date Range");
                        writer.WriteLine();

                        // Display Date Range
                        writer.Write("Start Date: " + startDate);
                        writer.WriteLine();
                        writer.Write("End Date: " + endDate);
                        writer.WriteLine();
                        writer.WriteLine();

                        // Display Header Row
                        writer.Write("School Name,");
                        writer.Write("Total Votes,");
                        writer.WriteLine();

                        // Display Data
                        foreach (var row in qry.OrderByDescending(x => x.TotalVotes))
                        {
                            writer.Write(row.SchoolName + ",");
                            writer.Write(row.TotalVotes);
                            writer.WriteLine();
                        }

                        // Output File
                        outputFileName = "TOK_AdultVoteTallyByDateRange_" + DateTime.Now + ".csv";
                        break;
                    }
                case "AdultVoteTallyCumulative":
                    {
                        var qry = from v in Db.Votes
                                  group v by v.VoteValue
                                      into grp
                                      select new SchoolVotesView
                                      {
                                          SchoolName = grp.Key,
                                          TotalVotes = grp.Count()
                                      };

                        // Add title line to report
                        writer.Write("Adult Vote Tally - Cumulative");
                        writer.WriteLine();
                        writer.WriteLine();

                        // Display Header Row
                        writer.Write("School Name,");
                        writer.Write("Total Votes,");
                        writer.WriteLine();

                        // Display Data
                        foreach (var row in qry.OrderByDescending(x => x.TotalVotes))
                        {
                            writer.Write(row.SchoolName + ",");
                            writer.Write(row.TotalVotes);
                            writer.WriteLine();
                        }

                        // Output File
                        outputFileName = "TOK_AdultVoteTallyCumulative_" + DateTime.Now + ".csv";
                        break;
                    }
                case "MinorVoteTallyByDateRange":
                    {
                        var qry = from v in Db.MinorVotes
                                  where v.DateCreated > startDate
                                  where v.DateCreated < endDate
                                  group v by v.MinorVoteValue
                                      into grp
                                      select new SchoolVotesView
                                      {
                                          SchoolName = grp.Key,
                                          TotalVotes = grp.Count()
                                      };

                        // Add title line to report
                        writer.Write("Minor Vote Tally by Date Range");
                        writer.WriteLine();

                        // Display Date Range
                        writer.Write("Start Date: " + startDate);
                        writer.WriteLine();
                        writer.Write("End Date: " + endDate);
                        writer.WriteLine();
                        writer.WriteLine();

                        // Display Header Row
                        writer.Write("School Name,");
                        writer.Write("Total Votes,");
                        writer.WriteLine();

                        // Display Data
                        foreach (var row in qry.OrderByDescending(x => x.TotalVotes))
                        {
                            writer.Write(row.SchoolName + ",");
                            writer.Write(row.TotalVotes);
                            writer.WriteLine();
                        }

                        // Output File
                        outputFileName = "TOK_MinorVoteTallyByDateRange_" + DateTime.Now + ".csv";
                        break;
                    }
                case "MinorVoteTallyCumulative":
                    {
                        var qry = from v in Db.MinorVotes
                                  group v by v.MinorVoteValue
                                      into grp
                                      select new SchoolVotesView
                                      {
                                          SchoolName = grp.Key,
                                          TotalVotes = grp.Count()
                                      };

                        // Add title line to report
                        writer.Write("Minor Vote Tally - Cumulative");
                        writer.WriteLine();
                        writer.WriteLine();

                        // Display Header Row
                        writer.Write("School Name,");
                        writer.Write("Total Votes,");
                        writer.WriteLine();

                        // Display Data
                        foreach (var row in qry.OrderByDescending(x => x.TotalVotes))
                        {
                            writer.Write(row.SchoolName + ",");
                            writer.Write(row.TotalVotes);
                            writer.WriteLine();
                        }

                        // Output File
                        outputFileName = "TOK_MinorVoteTallyCumulative_" + DateTime.Now + ".csv";
                        break;
                    }
            }


            writer.Flush();
            output.Position = 0;

            return File(output, "text/comma-separated-values", outputFileName);
        }

        #endregion
    }
}
