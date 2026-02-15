using System;
using SkillExtractor.Api.Models;

namespace SkillExtractor.Api.DTOs
{
    public static class Mapping
    {
        public static SkillDto ToDto(this Skill s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            return new SkillDto
            {
                Id = s.Id,
                Name = s.Name,
                Aliases = s.Aliases
            };
        }

        public static CvDocumentDto ToDto(this CVDocument cv)
        {
            if (cv == null) throw new ArgumentNullException(nameof(cv));

            var preview = cv.RawText ?? string.Empty;
            if (preview.Length > 200)
                preview = preview.Substring(0, 200);

            return new CvDocumentDto
            {
                Id = cv.Id,
                CreatedAt = cv.CreatedAt,
                Preview = preview
            };
        }
    }
}
