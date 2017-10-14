using System;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.IO;
using System.Linq;
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

        private string GetSessionFilename(Session session)
        {
            var sessionDate = session.SessionStart.Date;
            var sessionDay = sessionDate.Day;
            return $"{session:yyyy-MM-dd}-session{sessionDay:D2}{session.SessionNo}.html.md";
        }

        private string GetSeesionInfo(Session session)
        {
            var title = session.Title;
            var description = string.IsNullOrWhiteSpace(session.Abstract) ? "Comming Soon!" : session.Abstract;
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
                        : "co_author{authorCount}";
                sessionText.AppendLine($"{header}: {author.Speaker}");
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
            return await dbx.Session.Include("Author").Include("SessionGroup").ToArrayAsync();
        }
        #endregion

        #region Speaker情報の出力を行います。
        private async Task<Speaker[]> GetSpeakersAsync()
        {
            using (var dbx = new ContentsDbEntities())
            {
                return await dbx.Speaker.OrderBy(s => s.SpeakerId).ToArrayAsync();
            }
        }

        /// <summary>
        /// Speaker情報の出力を行います。
        /// </summary>
        /// <param name="path">出力先スピーカー情報ファイルパス(speakers.yml)</param>
        /// <returns></returns>
        public async Task<bool> CreateSpekaersAsync(string path)
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
                    await WriteSpeakerInfoAsync(sw, "imageUrl", string.IsNullOrWhiteSpace(speaker.ImageUrl) ? @"/assets/images/speakers/blank_user.png" : speaker.ImageUrl);
                    await WriteSpeakerInfoAsync(sw, "profile", speaker.Profile);
                    await WriteSpeakerInfoAsync(sw, "twitter", speaker.Twitter);
                    await WriteSpeakerInfoAsync(sw, "facebook", speaker.Facebook);
                    await WriteSpeakerInfoAsync(sw, "github", speaker.Github);
                    await WriteSpeakerInfoAsync(sw, "link", speaker.Link);
                    await WriteSpeakerInfoAsync(sw, "email", speaker.email);
                    await WriteSpeakerInfoAsync(sw, "microsoftmvpcategoly", speaker.MSMVPExpertise);
                }
            }
        }
        #endregion
    }
}
