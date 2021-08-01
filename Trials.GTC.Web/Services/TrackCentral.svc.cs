using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Collections.Generic;
using Trials.GTC.Data;
using System.Data.Linq.SqlClient;
using System.Threading;
using System.IO;
using System.Web;
using System.Configuration;
using System.Web.Security;
using System.Net.Mail;
using System.Diagnostics;
using System.Net;
using System.Xml.Linq;

namespace Trials.GTC.Web.Services
{
    public class TracksResults
    {
        public List<Track> Tracks { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public bool IsFilter { get; set; }
    }

    public class NewTrack
    {
        public string LinkId { get; set; }
        public Guid Submitted { get; set; }
        public string Name { get; set; }
        public int TrialsVersion { get; set; }
        public int TrialsType { get; set; }
        public int Difficulty { get; set; }
        public string Creator { get; set; }
        public DateTime Created { get; set; }
        public string Hyperlink { get; set; }
        public string Description { get; set; }
        public double Rating { get; set; }
        public Guid[] Tags { get; set; }
        public byte[] Image { get; set; }

        public TimeSpan TimeUltimate { get; set; }
        public TimeSpan TimePlatinum { get; set; }
        public TimeSpan TimeGold { get; set; }
        public TimeSpan TimeSilver { get; set; }
        public double ScoreCoeffient { get; set; }
    }

    public class UserAndRole
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string[] Roles { get; set; }
    }

    [ServiceContract(Namespace = "")]
    [SilverlightFaultBehavior]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class TrackCentral
    {
        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion

        [OperationContract]
        public string ResetUser(string username, string emailAddress)
        {
            var dbUserName = Membership.GetUserNameByEmail(emailAddress);
            if (dbUserName == null)
            {
                throw new Exception("User not found");
            }
            else
            {
                var user = Membership.GetUser(dbUserName);
                var password = user.ResetPassword();

                var mailMessage = new MailMessage("reset@***.com", emailAddress);
                mailMessage.Subject = "Global Track Central - Password reset";
                mailMessage.Body = @"Hi, your password is: " + password;

                var smtp = new SmtpClient();
                smtp.Send(mailMessage);

                return password;
            }
        }

        [OperationContract]
        public UserAndRole RegisterUser(string username, string password, string emailAddress)
        {
            // Attempt to register the user
            MembershipCreateStatus createStatus;
            Membership.CreateUser(username, password, emailAddress, null, null, true, null, out createStatus);

            if (createStatus == MembershipCreateStatus.Success)
            {
                FormsAuthentication.SetAuthCookie(username, false /* createPersistentCookie */);
                var user = Membership.GetUser(username);
                var roles = Roles.GetRolesForUser(username);

                var result = new UserAndRole()
                {
                    Id = (Guid)user.ProviderUserKey,
                    UserName = user.UserName,
                    EmailAddress = user.Email,
                    Roles = roles
                };

                try
                {
                    var mailMessage = new MailMessage("e.new@***.com", emailAddress);
                    mailMessage.Subject = "Global Track Central - New Account";
                    mailMessage.Body = @"Hi " + username + @". Welcome to Global Track Central. Thanks for registering. ";

                    var smtp = new SmtpClient();
                    smtp.Send(mailMessage);
                }
                catch
                {

                }

                return result;
            }

            throw new Exception("Unable to register account: " + createStatus);
        }

        [OperationContract]
        public UserAndRole LoginUser(string username, string password)
        {
            var validate = Membership.ValidateUser(username, password);
            if (validate)
            {
                var user = Membership.GetUser(username);
                var roles = Roles.GetRolesForUser(username);

                return new UserAndRole()
                {
                    Id = (Guid)user.ProviderUserKey,
                    UserName = user.UserName,
                    EmailAddress = user.Email,
                    Roles = roles
                };
            }

            throw new Exception("Invalid credential");
        }

        [OperationContract]
        public TracksResults GetTracks(int pageNumber, int resultsPerPage, int? trials, int? type, int? difficulty, string creator, string keyword, string sortName, bool sortDirection, Guid[] tags)
        {
            Thread.Sleep(250);

            var dc = new GTCDataContext();
            var tracks = dc.Tracks.AsQueryable();
            bool isFilter = false;

            // Trials Version filter
            if (trials != null && trials >= 0)
            {
                tracks = tracks.Where(t => t.TrialsVersion.Equals(trials));
                isFilter = true;
            }

            // Trials Type filter
            if (type != null && type >= 0)
            {
                tracks = tracks.Where(t => t.TrialsType.Equals(type));
                isFilter = true;
            }

            // Trials Version filter
            if (difficulty != null && difficulty >= 0)
            {
                tracks = tracks.Where(t => t.Difficulty.Equals(difficulty));
                isFilter = true;
            }
            // Creator filter
            if (!string.IsNullOrEmpty(creator))
            {
                tracks = tracks.Where(t => SqlMethods.Like(t.Creator, "%" + creator + "%"));
                isFilter = true;
            }

            // Keyword filter
            if (!string.IsNullOrEmpty(keyword))
            {
                tracks = tracks.Where(t => SqlMethods.Like(t.Name, "%" + keyword + "%")
                    || SqlMethods.Like(t.Description, "%" + keyword + "%"));
                isFilter = true;
            }
            // Tags
            if (tags != null && tags.Length > 0)
            {
                tracks = tracks.SelectMany(t =>
                    dc.TrackTags.Where(g => (g.TrackId == t.Id && tags.Contains(g.TagId))), (x, y) => x).Distinct();

                isFilter = true;
            }

            int pages = 1;
            int items = tracks.Count();
            int skip = (pageNumber - 1) * resultsPerPage;

            //if (skip > 0)
            //    tracks = dc.Tracks.Skip(skip);

            if (items > 0)
                pages = (int)Math.Ceiling((double)items / (double)resultsPerPage);

            #region Sorting
            if (sortName == "Name")
            {
                if (!sortDirection)
                    tracks = tracks.OrderByDescending(t => t.Name);
                else
                    tracks = tracks.OrderBy(t => t.Name);
            }

            if (sortName == "Creator")
            {
                if (!sortDirection)
                    tracks = tracks.OrderByDescending(t => t.Creator);
                else
                    tracks = tracks.OrderBy(t => t.Creator);
            }

            if (sortName == "Created")
            {
                if (!sortDirection)
                    tracks = tracks.OrderByDescending(t => t.Created);
                else
                    tracks = tracks.OrderBy(t => t.Created);
            }


            if (sortName == "Difficulty")
            {
                if (!sortDirection)
                    tracks = tracks.OrderByDescending(t => t.Difficulty);
                else
                    tracks = tracks.OrderBy(t => t.Difficulty);
            }


            if (sortName == "Type")
            {
                if (!sortDirection)
                    tracks = tracks.OrderByDescending(t => t.TrialsType);
                else
                    tracks = tracks.OrderBy(t => t.TrialsType);
            }

            if (sortName == "Version")
            {
                if (!sortDirection)
                    tracks = tracks.OrderByDescending(t => t.TrialsVersion);
                else
                    tracks = tracks.OrderBy(t => t.TrialsVersion);
            }


            if (sortName == "Submitted")
            {
                if (!sortDirection)
                    tracks = tracks.OrderByDescending(t => t.SubmittedOn);
                else
                    tracks = tracks.OrderBy(t => t.SubmittedOn);
            }

            if (sortName == "SubmittedBy")
            {
                if (!sortDirection)
                    tracks = tracks.OrderByDescending(t => t.Submitted);
                else
                    tracks = tracks.OrderBy(t => t.Submitted);
            }

            #endregion

            var list = tracks
                        .Skip(skip)
                        .Take(resultsPerPage);

            var result = new TracksResults()
                {
                    Tracks = list.ToList(),

                    TotalPages = pages,
                    TotalItems = items,
                    IsFilter = isFilter
                };

            return result;
        }

        [OperationContract]
        public Track GetTrack(string linkId)
        {
            var dc = new GTCDataContext();
            var track = (from t in dc.Tracks
                         where t.LinkId == linkId
                         select t).FirstOrDefault();

            return track;
        }

        [OperationContract]
        public List<Tag> GetTags()
        {
            var dc = new GTCDataContext();
            return (from s in dc.Tags
                    orderby s.Name
                    select s).ToList();
        }

        [OperationContract]
        public List<Time> GetTimes(Guid trackId)
        {
            var dc = new GTCDataContext();

            var track = dc.Tracks.FirstOrDefault(f => f.Id == trackId);
            if (track.TrialsType == 0)
            {
                var times = new List<Time>();

                using (var webclient = new WebClient())
                {
                    try
                    {
                        var output = webclient.DownloadString("http://trials2se.redlynx.com/HighscoresSearch.action?xml&amount=10&trackId=" + track.LinkId);
                        var doc = XDocument.Parse(output);

                        var positiondata = doc.Descendants("positiondata");

                        foreach (var pd in positiondata)
                        {
                            var pos = pd.Element("position").Value;
                            var name = pd.Element("name").Value;
                            var faults = pd.Element("faults").Value;
                            var record = pd.Element("record").Value;
                            var tt = record.Split(':');

                            times.Add(new Time()
                                {
                                    Rider = name,
                                    Time1 = new TimeSpan(0, 0, int.Parse(tt[0]), int.Parse(tt[1]), int.Parse(tt[2])),
                                    Faults = int.Parse(faults)
                                });
                        }
                    }
                    catch (Exception ex)
                    {
                        return (from t in dc.Times
                                where t.TrackId == trackId
                                select t).ToList();
                    }
                }

                return times;
            }
            else
                return (from t in dc.Times
                    where t.TrackId == trackId
                    select t).ToList();
        }

        [OperationContract]
        public List<GetMessagesResult> GetMessages(Guid trackId)
        {
            var dc = new GTCDataContext();
            var result = dc.GetMessages(trackId).ToList();
            return result;
        }

        [OperationContract]
        public List<EventSummaryResult> GetEventSummary(Guid tagId)
        {
            var dc = new GTCDataContext();
            var x = dc.EventSummary(tagId).ToList();
            return x;
        }

        [OperationContract]
        public bool NewTrack(NewTrack n)
        {
            var dc = new GTCDataContext();
            Track track = null;

            var item = dc.Tracks.FirstOrDefault(f => f.LinkId == n.LinkId);
            if (item == null)
            {
                track = new Track()
                {
                    Name = n.Name,
                    Submitted = n.Submitted,
                    LinkId = n.LinkId,
                    Description = n.Description,
                    TrialsVersion = n.TrialsVersion,
                    TrialsType = n.TrialsType,
                    Difficulty = n.Difficulty,
                    Creator = n.Creator,
                    Created = n.Created,
                    Hyperlink = n.Hyperlink,
                    Rating = n.Rating,
                    TimeUltimate = n.TimeUltimate,
                    TimePlatinum = n.TimePlatinum,
                    TimeGold = n.TimeGold,
                    TimeSilver = n.TimeSilver,
                    ScoreCoefficient = n.ScoreCoeffient
                };

                dc.Tracks.InsertOnSubmit(track);
                dc.SubmitChanges();
            }
            else
            {
                track = item;

                track.Name = n.Name;
                track.Description = n.Description;
                track.TrialsVersion = n.TrialsVersion;
                track.TrialsType = n.TrialsType;
                track.Difficulty = n.Difficulty;
                track.Creator = n.Creator;
                track.Created = n.Created;
                track.Hyperlink = n.Hyperlink;
                track.Rating = n.Rating;

                track.TimeUltimate = n.TimeUltimate;
                track.TimePlatinum = n.TimePlatinum;
                track.TimeGold = n.TimeGold;
                track.TimeSilver = n.TimeSilver;

                if (n.ScoreCoeffient > 0)
                    track.ScoreCoefficient = n.ScoreCoeffient;

                dc.SubmitChanges();

                var trackTags = from tt in dc.TrackTags
                                where tt.TrackId == track.Id
                                select tt;

                if (trackTags.Count() > 0)
                {
                    foreach (var tracktag in trackTags)
                        dc.TrackTags.DeleteOnSubmit(tracktag);

                    dc.SubmitChanges();
                }
            }

            if (n.Tags != null && n.Tags.Length > 0)
            {

                foreach (var t in n.Tags)
                {
                    var trackTag = new TrackTag();
                    trackTag.TrackId = track.Id;
                    trackTag.TagId = t;
                    dc.TrackTags.InsertOnSubmit(trackTag);
                }

                dc.SubmitChanges();
            }

            if (n.Image != null)
            {
                FileStream fileStream = null;
                BinaryWriter writer = null;
                string filePath;

                try
                {
                    filePath = HttpContext.Current.Server.MapPath("./Thumbnails/") + track.LinkId + ".png";
                    fileStream = File.Open(filePath, FileMode.Create);
                    writer = new BinaryWriter(fileStream);
                    writer.Write(n.Image);

                    track.HasThumb = true;
                    dc.SubmitChanges();
                }
                finally
                {
                    if (fileStream != null)
                        fileStream.Close();

                    if (writer != null)
                        writer.Close();
                }
            }

            return true;
        }

        [OperationContract]
        public bool DeleteTrack(Guid id)
        {
            var dc = new GTCDataContext();
            var track = dc.Tracks.FirstOrDefault(f => f.Id == id);

            if (track != null)
            {
                var trackTags = from tt in dc.TrackTags
                                where tt.TrackId == id
                                select tt;

                if (trackTags.Count() > 0)
                {
                    foreach (var tracktag in trackTags)
                        dc.TrackTags.DeleteOnSubmit(tracktag);

                    dc.SubmitChanges();
                }


                dc.Tracks.DeleteOnSubmit(track);
                dc.SubmitChanges();
            }

            return true;
        }

        [OperationContract]
        public bool SubmitTime(Guid userId, Guid trackId, string linkId, string rider, TimeSpan time, int faults, string link)
        {
            var dc = new GTCDataContext();
            var currenttime = dc.Times.FirstOrDefault(f => f.TrackId == trackId && f.Rider == rider);

            if (currenttime == null)
            {
                currenttime = new Time();
                dc.Times.InsertOnSubmit(currenttime);
            }

            currenttime.TrackId = trackId;
            currenttime.LinkId = linkId;
            currenttime.Rider = rider;
            currenttime.Time1 = time;
            currenttime.Faults = faults;

            if (!string.IsNullOrEmpty(link))
                currenttime.Link = link;

            dc.SubmitChanges();
            return true;
        }

        [OperationContract]
        public bool NewMessage(Guid userId, Guid trackId, string body)
        {
            var dc = new GTCDataContext();
            var message = new Message()
            {
                UserId = userId,
                TrackId = trackId,
                Body = body,
                DateTime = DateTime.UtcNow
            };

            dc.Messages.InsertOnSubmit(message);
            dc.SubmitChanges();

            return true;
        }
    }
}
