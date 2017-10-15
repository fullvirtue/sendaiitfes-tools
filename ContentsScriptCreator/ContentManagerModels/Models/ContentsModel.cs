using System;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ContentManagerModels.Entities.Database;

namespace ContentManagerModels.Models
{
    public class ContentsModel
    {
        #region Session.mdの生成を行います。
        /// <summary>
        /// Session.mdの生成を行います。
        /// </summary>
        /// <param name="sessionsDirectory"></param>
        /// <returns></returns>
        public async Task<bool> CreateSessionsAsync(string sessionsDirectory)
        {
            if (Directory.Exists(sessionsDirectory))
            {
                Directory.Delete(sessionsDirectory, true);
            }
            Directory.CreateDirectory(sessionsDirectory);

            
            using (var dbx = new ContentsDbEntities())
            {
                var sessions = await GetSessionsAsync(dbx);
                foreach (var session in sessions)
                {
                    await CreateSessionFileAsync(sessionsDirectory, session);
                }
            }
            return true;
        }

        /// <summary>
        /// セッションファイル名の取得を行います。
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        private string GetSessionFilename(Session session) => $"{session.SessionStart:yyyy-MM-dd}-session{session.SessionNo}.html.md";

        /// <summary>
        /// セッション情報の取得を行います。
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        private string GetSeesionInfo(Session session)
        {
            var title = session.Title;
            var description = string.IsNullOrWhiteSpace(session.Abstract) ? "Comming Soon!!" : session.Abstract;
            var sessionText = new StringBuilder()
                .AppendLine("---")
                .AppendLine($"title: {title}")
                .AppendLine($"description: \"{title}\"")
                .AppendLine($"date: {session.SessionStart:yyyy-MM-dd HH:mm}")
                .AppendLine($"sessionlevel: {session.SessionLevel}");

            var authorCount = 0;
            foreach (var author in session.Author.OrderBy(a => a.Order))
            {
                var header = authorCount == 0 ? "author"
                           : authorCount == 1 ? "co_author"
                           : $"co_author{authorCount}";
                sessionText.AppendLine($"{header}: {author.Speaker.SpeakerName}");
                authorCount++;
            }
            sessionText
                .AppendLine("category: sessions")
                .AppendLine("---")
                .AppendLine($"{description}");

            return sessionText.ToString();
        }

        private async Task CreateSessionFileAsync(string sessionsDirectory, Session session)
        {
            var path = Path.Combine(sessionsDirectory, GetSessionFilename(session));
            using (var fs = new FileStream(path, FileMode.Create))
            using (var sw = new StreamWriter(fs, Encoding.UTF8))
            {
                await sw.WriteAsync(GetSeesionInfo(session));
            }
        }

        private async Task<Session[]> GetSessionsAsync(ContentsDbEntities dbx)
        {
            return await dbx.Session.Where(s => string.IsNullOrEmpty(s.SessonNo3) == false).Include("Author.Speaker").Include("SessionGroup").ToArrayAsync();
        }
        #endregion

        #region Speaker情報の出力を行います。
        private async Task<Speaker[]> GetSpeakersAsync()
        {
            using (var dbx = new ContentsDbEntities())
            {
                return await dbx.Speaker.Where(s => s.SpeakerId < 500).OrderBy(s => s.SpeakerId).ToArrayAsync();
            }
        }

        /// <summary>
        /// Speaker情報の出力を行います。
        /// </summary>
        /// <param name="path">出力先スピーカー情報ファイルパス(speakers.yml)</param>
        /// <returns></returns>
        public async Task<bool> CreateSpeakersAsync(string path)
        {
            try
            {
                if (File.Exists(path) == true)
                {
                    File.Delete(path);
                }
                var speakers = await GetSpeakersAsync();
                await WriteSpeakersAsync(path, speakers);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        private async Task WriteSpeakerInfoAsync(StreamWriter sw, string header, string body)
        {
            if (string.IsNullOrWhiteSpace(body))
            {
                return;
            }
            await sw.WriteLineAsync($"    {header}: {body}");
        }

        private async Task WriteSpeakersAsync(string path, Speaker[] speakers)
        {
            using (var fs = new FileStream(path, FileMode.Create))
            using (var sw = new StreamWriter(fs, Encoding.UTF8))
            {
                await sw.WriteLineAsync("normals:");
                foreach (var speaker in speakers)
                {
                    await sw.WriteLineAsync($"  - name: {speaker.SpeakerName}");
                    await WriteSpeakerInfoAsync(sw, "organization", speaker.Organization);
                    await WriteSpeakerInfoAsync(sw, "organization2", speaker.Organization2);
                    await WriteSpeakerInfoAsync(sw, "title", speaker.Title);
                    await WriteSpeakerInfoAsync(sw, "title2", speaker.Title2);
                    await WriteSpeakerInfoAsync(sw, "profile_image_url", string.IsNullOrWhiteSpace(speaker.ImageUrl) ? @"/assets/images/speakers/blank_user.png" : speaker.ImageUrl);
                    await WriteSpeakerInfoAsync(sw, "profile",$"\"{speaker.Profile}\"" );
                    await WriteSpeakerInfoAsync(sw, "twitter", speaker.Twitter);
                    await WriteSpeakerInfoAsync(sw, "facebook", speaker.Facebook);
                    await WriteSpeakerInfoAsync(sw, "github", speaker.Github);
                    await WriteSpeakerInfoAsync(sw, "link", speaker.Link);
                    await WriteSpeakerInfoAsync(sw, "microsoftmvpcategoly", speaker.MSMVPExpertise);
                }
            }
        }
        #endregion

        private async Task<SessionGroup[]> GetSessionInfos()
        {
            using (var dbx = new ContentsDbEntities())
            {
                return await dbx.SessionGroup
                    .Include("Session.Author.Speaker")
                    .OrderBy(s => s.SessionGroupId)
                    .ToArrayAsync();
            }
        }

        public async Task<bool> CreateTimetableAsync(string timetablePath, string commonHeaderFilePath,
            string timetableHeaderFilePath)
        {
            var sessionGroups = await GetSessionInfos();
            var commonHeader = File.ReadAllText(commonHeaderFilePath, Encoding.UTF8);
            var timeTableHeader = File.ReadAllText(timetableHeaderFilePath);

            using (var fs = new FileStream(timetablePath, FileMode.Create))
            using (var sw = new StreamWriter(fs, Encoding.UTF8))
            {
                // ヘッダーの出力
                await sw.WriteLineAsync(commonHeader);
                foreach (var sessionGroup in sessionGroups.GroupBy(s => s.SessionGroup1))
                {
                    await WriteTimetableHeaderAsync(sw, sessionGroup);
                }

                // Timetableの出力
                await sw.WriteLineAsync(timeTableHeader);
                foreach (var sessionGroup in sessionGroups.OrderBy(sg => sg.SessionGroupId))
                {
                    await WriteTimetableBodyAsync(sw, sessionGroup);
                }
                await sw.FlushAsync();
            }
            
            return true;
        }

        private string GetSpeakerName(Speaker speaker)
        {
            var speakerName = new StringBuilder();

            speakerName.Append(string.IsNullOrEmpty(speaker.Organization)  ? string.Empty : speaker.Organization);
            speakerName.Append(string.IsNullOrEmpty(speaker.Organization2) ? string.Empty : speakerName.Length > 0 ? $" / {speaker.Organization2}" : speaker.Organization2);
            speakerName.Append(string.IsNullOrEmpty(speaker.Title)         ? string.Empty : speakerName.Length > 0 ? $" / {speaker.Title}"         : speaker.Title);
            speakerName.Append(string.IsNullOrEmpty(speaker.Title2)        ? string.Empty : speakerName.Length > 0 ? $" / {speaker.Title2}"        : speaker.Title2);

            return speakerName.ToString();
        }



        private async Task WriteTimetableBodyAsync(StreamWriter sw, SessionGroup sessionGroup)
        {
            await sw.WriteLineAsync($"  .schedule");
            await sw.WriteLineAsync($"    h2.schedule_title#{sessionGroup.SesisonGroupFlagments} {sessionGroup.SessionGroup1} - {sessionGroup.SessionGroupName}");
            await sw.WriteLineAsync($"    .tab-content");
            await sw.WriteLineAsync($"      .tab-pane.active");
            await sw.WriteLineAsync($"        .scheduleTable");
            foreach (var sg in sessionGroup.Session)
            {
                var sessionTime = sg.SessionEnd - sg.SessionStart;
                foreach (var author in sg.Author)
                {
                    var speaker = author.Speaker;
                    var speakerName = GetSpeakerName(speaker);
                    if (author.Order == 1)
                    {
                        await sw.WriteLineAsync($"          .scheduleTable_line");
                        await sw.WriteLineAsync($"            .scheduleTable_line_time ");
                        await sw.WriteLineAsync($"              | {sg.SessionStart.Date:yyyy-MM-dd}");
                        await sw.WriteLineAsync($"              br/");
                        await sw.WriteLineAsync($"              | {sg.SessionStart:HH:mm} - {sg.SessionEnd:HH:mm}");
                        await sw.WriteLineAsync($"              .scheduleTable_line_time_min {sessionTime.Minutes}min");
                        await sw.WriteLineAsync($"            .scheduleTable_line_session");
                        await sw.WriteLineAsync($"              .scheduleTable_line_speakerIcon ");
                        await sw.WriteLineAsync($"                img src=\"..{speaker.ImageUrl}\" width=\"100\" height=\"100\" alt=\"{speakerName}\"");
                        await sw.WriteLineAsync($"              .scheduleTable_line_descriptions");
                        await sw.WriteLineAsync($"                .scheduleTable_line_title {sg.Title}");
                        await sw.WriteLineAsync($"                .scheduleTable_line_speaker {speakerName}");
                    }
                    else
                    {
                        await sw.WriteLineAsync($"          .scheduleTable_line");
                        await sw.WriteLineAsync($"            .scheduleTable_line_time ");
                        await sw.WriteLineAsync($"            .scheduleTable_line_session");
                        await sw.WriteLineAsync($"              .scheduleTable_line_speakerIcon ");
                        await sw.WriteLineAsync($"                img src=\"..{speaker.ImageUrl}\" width=\"100\" height=\"100\" alt=\"{speakerName}\"");
                        await sw.WriteLineAsync($"              .scheduleTable_line_descriptions");
                        await sw.WriteLineAsync($"                .scheduleTable_line_title {sg.Title}");
                        await sw.WriteLineAsync($"                .scheduleTable_line_speaker {speakerName}");
                    }
                }

            }

        }

        private async　Task WriteTimetableHeaderAsync(StreamWriter sw, IGrouping<string, SessionGroup> sessionGroup)
        {
            await sw.WriteLineAsync($"    h2 {sessionGroup.Key}");
            await sw.WriteLineAsync($"    ul");
            foreach (var s in sessionGroup.OrderBy(sg => sg.SessionGroupId))
            {
                await sw.WriteLineAsync($"      li ");
                await sw.WriteLineAsync($"        a href=\"#{s.SesisonGroupFlagments}\" {s.SessionGroupName}");
            }
        }
    }
}
