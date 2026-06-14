using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; 
using Services.Abstraction.Contracts;
using Domain.Entities.SystemModule;
using Shared.Dtos.History;

namespace Services.Implementations
{
    public class HistoryService : IHistoryService
    {
        private readonly IApplicationDbContext _context;

        public HistoryService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HistoryListItemDto>> GetUserHistoryAsync(string userEmail, string type)
        {
            var query = _context.Histories.Where(h => h.AttachedUserEmail == userEmail);

            if (!string.Equals(type, "All", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(h => h.ContentType.Equals(type, StringComparison.OrdinalIgnoreCase));
            }

            var records = await query.OrderByDescending(h => h.ActionDate).ToListAsync();

            return records.Select(h => new HistoryListItemDto
            {
                Id = h.Id,
                Url = h.ContentUrl,
                ContentType = h.ContentType,
                Status = h.ContentType.Equals("Video", StringComparison.OrdinalIgnoreCase) ? "Flagged" : "Safe",
                DetectionDate = h.ActionDate,
                TimeAgo = CalculateTimeAgo(h.ActionDate)
            });
        }

        public async Task<HistoryDetailsDto?> GetDetailsAsync(string id)
        {
            var record = await _context.Histories.FirstOrDefaultAsync(h => h.Id == id);
            if (record == null) return null;

            var isVideo = record.ContentType.Equals("Video", StringComparison.OrdinalIgnoreCase);
            var summary = new List<string>();

            if (isVideo)
            {
                summary.Add("Identified high-impact physical actions in the video.");
                summary.Add("Detected rapid and forceful movements consistent with aggression.");
                summary.Add("Presence of aggressive postures and gestures between individuals.");
            }
            else
            {
                summary.Add("Aggressive tone detected in middle paragraph.");
                summary.Add("Harmful intent identified against specific groups.");
                summary.Add("Threatening language found in closing statements.");
            }

            return new HistoryDetailsDto
            {
                Id = record.Id,
                Url = record.ContentUrl,
                ContentType = isVideo ? "Video Stream (MP4)" : "Text",
                FormattedDate = record.ActionDate.ToString("MMMM d, yyyy"),
                FormattedTime = record.ActionDate.ToString("h:mm tt"),
                CurrentStatus = isVideo ? "Violent Content" : "Non-Violent Content",
                ConfidenceText = isVideo ? "82% MATCH" : "15% MATCH",
                AnalysisSummary = summary
            };
        }

        public async Task<bool> DeleteRecordAsync(string id)
        {
            var record = await _context.Histories.FirstOrDefaultAsync(h => h.Id == id);
            if (record == null) return false;

            _context.Histories.Remove(record);
            await _context.SaveChangesAsync();
            return true;
        }

        private static string CalculateTimeAgo(DateTime dateTime)
        {
            var diff = DateTime.UtcNow - dateTime;
            if (diff.TotalMinutes < 60) return $"{Math.Max(1, (int)diff.TotalMinutes)} minutes ago";
            if (diff.TotalHours < 24) return $"{(int)diff.TotalHours} hours ago";
            if (diff.TotalDays < 2) return "Yesterday";
            return dateTime.ToString("dd/MM/yyyy");
        }
    }
}